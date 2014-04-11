module Chess.Fs

open System

type Color = WHITE|BLACK
type PawnType = PAWN|ROOK|KNIGHT|BISHOP|QUEEN|KING
type Position = { row:int; col:int}
type Piece = { type_:PawnType; player:Color}
type Pawn = { p:Position; data:Piece}
type Move = End | Move of Position * ( unit->Move)
//type Board = List of Pawn

// allow for much nicer syntax -> Pawn := PawnData 'at' position
let (@) (a:Piece) (b:Position) :Pawn = {p=b; data=a}

let opposite = function WHITE->BLACK | _ -> WHITE

let rec getPawnOnBoard board (p:Position) = 
 match board with
    | h::t -> if p.col=h.p.col && p.row=h.p.row then Some h else getPawnOnBoard t p
    | [] -> None

let rec isAvailable board (p:Position) = getPawnOnBoard board p = None   

let inBounds position = position.row >= 0 && position.row < 8 && position.col >= 0 && position.col < 8

let isValidPosition board position= inBounds position && isAvailable board position

(*
let rook_dirs = [(1,0);(0,1);(-1,0);(0,-1)]
let knight_dirs = [(1,2);(2,1);(-1,2);(-2,1);(1,-2);(2,-1);(-1,-2);(-2,-1)]
let bishop_dirs = [(1,1);(-1,1);(1,-1);(-1,-1)]
let queen_dirs = List.concat [rook_dirs; bishop_dirs]
let king_dirs = queen_dirs
*)

let jump board pawn dir = // move type: 'teleport' from one position to another ignoring obstacles
    let newPos = { row = fst(dir)+pawn.p.row; col = snd(dir)+pawn.p.col}
    //Console.WriteLine( "\> {0} {1}",fst(dir),snd(dir))
    if isValidPosition board newPos then Move( newPos, function ()->End) else End

let rec trace board pawn dir = // move type: along the line, till board ends
    let rec f n =
        let newPos = { row = fst(dir)*n+pawn.p.row; col = snd(dir)*n+pawn.p.col}
        if isValidPosition board newPos then Move( newPos, function ()->f (n+1)) else End
    f 0

let getAvailableMoves board (pawn:Pawn)=
    let rook_dirs = [(1,0);(0,1);(-1,0);(0,-1)]
    let knight_dirs = [(1,2);(2,1);(-1,2);(-2,1);(1,-2);(2,-1);(-1,-2);(-2,-1)]
    let bishop_dirs = [(1,1);(-1,1);(1,-1);(-1,-1)]
    let queen_dirs = List.concat [rook_dirs; bishop_dirs]
    let king_dirs = queen_dirs
    let f = function
        PAWN -> [ Move( {col=0;row=0}, function ()->End)] // fill, especialy when when attacking can move on diagonal
        | ROOK   -> List.map (trace board pawn) rook_dirs
        | KNIGHT -> List.map (jump board pawn) knight_dirs
        | BISHOP -> List.map (trace board pawn) bishop_dirs
        | QUEEN  -> List.map (trace board pawn) queen_dirs
        | KING   -> List.map (jump board pawn) king_dirs
    f pawn.data.type_

let unfoldMoves moveList = 
    let rec unfoldMove acc move=
        match move with
        Move( p, next) -> unfoldMove (p::acc) (next() )
        | End -> acc
    let rec f acc l=
        match l with
        h::t -> f (List.concat [unfoldMove [] h; acc]) t
        | _ -> acc
    f [] moveList

let unfoldMoves_indirect board (pawn:Pawn) = unfoldMoves ( getAvailableMoves board pawn) // yeah, just a shortcut..

let isEnemy myColor otherPawn = myColor=otherPawn.data.player

let getOfColor color board = board |> List.filter ( fun e-> not (isEnemy color e))

let applyMove board (pawn:Pawn) (move:Move)=
    match move with
    Move( p, _) ->
        let f e = if e=pawn then (pawn.data@p) else e
        List.map f board
    | End -> []


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



(* UTILS *)
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


//p |> List.map (function e-> match e with Move(p,_)-> Console.WriteLine( "{0} {1}",p.col,p.row) | _->())