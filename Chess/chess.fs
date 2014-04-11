module Chess.Fs

open System

type Color = WHITE|BLACK
type PawnType = PAWN|ROOK|KNIGHT|BISHOP|QUEEN|KING
type Position = { row:int; col:int}
type Piece = { type_:PawnType; player:Color}
type Pawn = { p:Position; data:Piece}
type Move = End | Move of Position * ( unit->Move)
//type Board = List of Pawn

let opposite = function WHITE->BLACK | _ -> WHITE

/// <summary> allow for much nicer syntax -> Pawn := PawnData 'at' position </summary>
let (@) (a:Piece) (b:Position) :Pawn = {p=b; data=a}

/// <summary> returns pawn on the postion 'p' [ actual return type is Pawn option] </summary>
let rec getPawnOnBoard board (p:Position) = 
 match board with
    | h::t -> if p.col=h.p.col && p.row=h.p.row then Some h else getPawnOnBoard t p
    | [] -> None

/// <summary> if the position is still within the bounds of the board </summary>
let inBounds position = position.row >= 0 && position.row < 8 && position.col >= 0 && position.col < 8

let isEnemyAt board (piece:Piece) (p:Position) =
    let e = getPawnOnBoard board p
    e.IsSome && e.Value.data.player = opposite piece.player

/// <summary> if piece can be put on this very position </summary>
let canMoveTo board (piece:Piece) (p:Position) =
    match (getPawnOnBoard board p), inBounds p with
    _,false -> false
    | a,_ when a.IsSome && a.Value.data.player = piece.player -> false // cannot move to position occupied by friendly piece
    | _ -> true

/// <summary> move type: 'teleport' from one position to another ignoring obstacles </summary>
let jump board pawn dir =
    let newPos = { row = fst(dir)+pawn.p.row; col = snd(dir)+pawn.p.col}
    //Console.WriteLine( "\> {0} {1}",fst(dir),snd(dir))
    if (canMoveTo board pawn.data newPos) then Move( newPos, function ()->End) else End

/// <summary> move type: along the line, till board ends </summary>
let rec trace board pawn dir = 
    let rec f (n:int) =
        let newPos = { row = fst(dir)*n+pawn.p.row; col = snd(dir)*n+pawn.p.col}
        //Console.WriteLine( "::> depth:{0:D} dir:{1} moveTo: [{2} {3}]", n, fst(dir)*2+snd(dir), newPos.row, newPos.col)
        //Console.WriteLine( "::> \tstats canMoveTo: {0} isEnemyAt: {1}", (canMoveTo board pawn.data newPos), (isEnemyAt board pawn.data newPos))
        match (canMoveTo board pawn.data newPos), (isEnemyAt board pawn.data newPos) with
            false,_-> End
            | _, true -> Move( newPos, function ()->End) // obstacle !
            | _-> Move( newPos, function ()->f (n+1)) // move along
    f 1


/// <summary> return list of lazy lists ( list of Moves) representing all possible future positions </summary>
let getAvailableMoves board (pawn:Pawn)=
    let rook_dirs = [(1,0);(0,1);(-1,0);(0,-1)]
    let knight_dirs = [(1,2);(2,1);(-1,2);(-2,1);(1,-2);(2,-1);(-1,-2);(-2,-1)]
    let bishop_dirs = [(1,1);(-1,1);(1,-1);(-1,-1)]
    let queen_dirs = List.concat [rook_dirs; bishop_dirs]
    let king_dirs = queen_dirs
    let f = function
        PAWN -> 
            let dir = if pawn.data.player=BLACK then -1 else 1
            let inFront = {row=pawn.p.row+dir; col=pawn.p.col}
            let a1 = { inFront with col=inFront.col-1}
            let a2 = { inFront with col=inFront.col+1}
            let f a = if isEnemyAt board pawn.data a then Move(a, fun ()-> End) else End
            (if canMoveTo board pawn.data inFront then Move(inFront, fun ()-> End) else End) :: ( f a1) :: ( f a2) :: []
        | ROOK   -> List.map (trace board pawn) rook_dirs
        | KNIGHT -> List.map (jump board pawn) knight_dirs
        | BISHOP -> List.map (trace board pawn) bishop_dirs
        | QUEEN  -> List.map (trace board pawn) queen_dirs
        | KING   -> List.map (jump board pawn) king_dirs
    f pawn.data.type_

/// <summary> return list of all future positions </summary>
let unfoldMoves moveList = 
    let rec unfoldMove acc move=
        //Console.WriteLine( "::> unfolding ! {0}",(match move with End ->"END" | _->"Go on !") )
        match move with
            Move( p, next) -> unfoldMove (p::acc) (next() )
            | End -> acc
    let rec f acc l=
        //Console.WriteLine( "::> --" )
        match l with
            h::t -> f (unfoldMove acc h) t
            | _ -> acc
    f [] moveList

let unfoldMoves_indirect board (pawn:Pawn) = unfoldMoves ( getAvailableMoves board pawn) // yeah, just a shortcut..

/// <summary> apply move and return new board representing post-move state </summary>
let applyMove board (pawn:Pawn) (move:Move)=
    match move with
        Move( p, _) when not (canMoveTo board pawn.data p) -> board // TODO what now ? return bool 'if move executed correctly ?'
        |Move( p, _) when isEnemyAt board pawn.data p ->
            //Console.WriteLine( "::> KILL" )
            let enemy = getPawnOnBoard board p
            board
                |> List.filter ( fun e-> e <> enemy.Value)
                |> List.map ( fun e -> if e=pawn then (pawn.data@p) else e)
        | Move( p, _) ->
            //Console.WriteLine( "::> MOVE" )
            board |> List.map ( fun e -> if e=pawn then (pawn.data@p) else e)
        | End -> board  // TODO what now ? return bool 'if move executed correctly ?'

(* applyMove ad hoc testing
let p1 = ({player=WHITE; type_=PAWN}@{row=0;col=0})
let p2 = ({player=BLACK; type_=PAWN}@{row=1;col=0})
let p3 = ({player=BLACK; type_=PAWN}@{row=4;col=0})
let r1 = applyMove [p1;p2] p1 ( Move( {row=1;col=0},fun ()->End))
let r2 = applyMove [p1;p3] p1 ( Move( {row=1;col=0},fun ()->End))
*)

// TODO <post-move check> check win conditions
// TODO <post-move check> promotions
// TODO minimax


(*
let getOfColor color board = board |> List.filter ( fun e-> not (isEnemy color e))

let eval best pretend = 5

let minmax (color:Color) board eval =
    // get all my pawns
    let my = getOfColor color board
    // iterate over all pawns and try all moves - returns product of eval
    let rec permutate myPawns eval acc= 
        match myPawns with
        pawn::t ->
            // for each pawn get it's moves
            let moves = getAvailableMoves board pawn
            // iterate over moves
            let rec f moveList acc_= //List.fold (fun s b->)
                match moveList with
                move::t -> f t ( eval acc_ move)
                | [] -> acc_
            f (unfoldMoves moves) 
        | [] -> acc
        

    //let rec max depth board_ =
    //   match 


    ()
*)


(* UTILS *)
(*
let compare_ints a b=
        match a with
        | x when x < b -> -1
        | x when x > b -> 1
        | x -> 0
let compare a b=
        match a with
        | {col=c; row=r} when c < b.col -> -1
        | {col=c; row=r} when c > b.col -> 1
        | x -> compare_ints a.row b.row


unfoldMoves( getAvailableMoves [] { p={col=7;row=5}; data={type_=KING; player=BLACK}})
    |> List.sortWith compare
    |> List.map (function p-> Console.WriteLine( "{0} {1}",p.col,p.row))
*)

//p |> List.map (function e-> match e with Move(p,_)-> Console.WriteLine( "{0} {1}",p.col,p.row) | _->())