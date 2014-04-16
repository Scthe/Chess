module Chess.Fs

open System

type Color = WHITE|BLACK

type PawnType = PAWN|ROOK|KNIGHT|BISHOP|QUEEN|KING
type Position = { row:int; col:int}
type Piece = { type_:PawnType; player:Color}
type Pawn = { p:Position; data:Piece}

type Move = End | Move of Position * ( unit->Move)
type MoveResultType =               // should be enough to construct move notation description
    SimpleMove of Pawn * Move       // who, where
    | Capture of Pawn * Move * Pawn // who, where, whom
    | Impossible                    // used to denote error move evaluation. Should not be used..
    // TODO | Promotion of Pawn * Move * PawnType  // who, where, to what < implement as post-move check ?>
    // TODO | PromotionCapture of Pawn * Move * Pawn * PawnType  // who, where, whom, to what
    // TODO | Castle of ?
    // TODO | EnPassant of Pawn * Move * Pawn // who, where, whom

exception IllegalMove of string
//type Board = List of Pawn


let private opposite = function WHITE -> BLACK | _ -> WHITE

let private pieceTypeNotation = function PAWN-> "" | ROOK -> "R" | KNIGHT -> "N" | BISHOP -> "B" | QUEEN -> "Q" | KING -> "K"

let private positionNotation p = string ( char (int 'a' + p.col)) + string (p.row+1)

/// <summary> allow for much nicer syntax -> Pawn := PawnData 'at' position </summary>
let private (@) (a:Piece) (b:Position) :Pawn = {p=b; data=a}

//#region move
/// <summary> if the position is still within the bounds of the board </summary>
let private inBounds position = position.row >= 0 && position.row < 8 && position.col >= 0 && position.col < 8

/// <summary> returns pawn on the postion 'p' [ actual return type is Pawn option] </summary>
let rec private getPawnOnBoard board (p:Position) = 
 match board with
    | h::t -> if p.col=h.p.col && p.row=h.p.row then Some h else getPawnOnBoard t p
    | [] -> None

let private isEnemyAt board (piece:Piece) (p:Position) =
    let e = getPawnOnBoard board p
    e.IsSome && e.Value.data.player = opposite piece.player

/// <summary> if piece can be put on this very position </summary>
let private canMoveTo board (piece:Piece) (p:Position) =
    match (getPawnOnBoard board p), inBounds p with
        | a,_ when a.IsSome && a.Value.data.player = piece.player -> false // cannot move to position occupied by friendly piece
        | _ ,t-> t

/// <summary> move type: 'teleport' from one position to another ignoring obstacles </summary>
let private jump board pawn dir =
    let newPos = { row = fst(dir)+pawn.p.row; col = snd(dir)+pawn.p.col}
    //Console.WriteLine( "\> {0} {1}",fst(dir),snd(dir))
    if (canMoveTo board pawn.data newPos) then Move( newPos, function ()->End) else End

/// <summary> move type: along the line, till board ends </summary>
let rec private trace board pawn dir = 
    let rec f (n:int) =
        let newPos = { row = fst(dir)*n+pawn.p.row; col = snd(dir)*n+pawn.p.col}
        //Console.WriteLine( "::> depth:{0:D} dir:{1} moveTo: [{2} {3}]", n, fst(dir)*2+snd(dir), newPos.row, newPos.col)
        //Console.WriteLine( "::> \tstats canMoveTo: {0} isEnemyAt: {1}", (canMoveTo board pawn.data newPos), (isEnemyAt board pawn.data newPos))
        match (canMoveTo board pawn.data newPos), (isEnemyAt board pawn.data newPos) with
            false,_-> End
            | _, true -> Move( newPos, function ()->End) // obstacle !
            | _-> Move( newPos, function ()->f (n+1)) // move along
    f 1
//#endregion

/// <summary> get the Algebraic notation of the move </summary>
let moveNotation = function // TODO add '+' to denote check, '#' for checkmate
    SimpleMove(p, Move(pos,_)) -> (pieceTypeNotation p.data.type_) + (positionNotation pos)
    | Capture( p, Move(pos,_), vic) ->(pieceTypeNotation p.data.type_) + "x" + (positionNotation pos)
    | _ -> raise (IllegalMove("asked to denote an illegal move"))
    // TODO denote other types of moves

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
        | KNIGHT -> List.map (jump board pawn)  knight_dirs
        | BISHOP -> List.map (trace board pawn) bishop_dirs
        | QUEEN  -> List.map (trace board pawn) queen_dirs
        | KING   -> List.map (jump board pawn)  king_dirs
    f pawn.data.type_

/// <summary> return list of all future positions </summary>
let unfoldMoves moveList = 
    let rec unfoldMove acc move=
        //Console.WriteLine( "::> unfolding ! {0}",(match move with End ->"END" | _->"Go on !") )
        match move with
            Move( p, next) -> unfoldMove (p::acc) (next() )
            | End -> acc
    moveList |> List.fold unfoldMove []

/// <summary> apply move and return new board representing post-move state.
/// Rememeber to always check if returned MoveResultType is not Impossible ! </summary>
let applyMove board (pawn:Pawn) (move:Move)=
    let possibleMoves = unfoldMoves ( getAvailableMoves board pawn)
    match move with
        Move( p, _) when not ( possibleMoves |> List.exists (fun e -> p.row=e.row && p.col=e.col)) -> board, Impossible
        |Move( p, _) when isEnemyAt board pawn.data p ->
            let enemy = getPawnOnBoard board p
            board
                |> List.filter ( fun e-> e <> enemy.Value)
                |> List.map ( fun e -> if e=pawn then (pawn.data@p) else e), Capture( pawn, move, enemy.Value)
        | Move( p, _) ->
            board |> List.map ( fun e -> if e=pawn then (pawn.data@p) else e), SimpleMove(pawn, move)
        | End -> board, Impossible


/// <summary> checks if all positions on the list can be reached by at least one pawn of the specified color </summary>
let private arePositionsInRangeOfPawnsOfColor board color (ps:Position List) =
    let buildAllPositionsForColor board color= 
        board
            |> List.filter ( fun e-> e.data.player = color)
            |> List.map ( fun p-> unfoldMoves ( getAvailableMoves board p))
            |> List.concat
            |> Set.ofList
    let r = buildAllPositionsForColor board color
    let posEqual p1 p2 = p1.row = p2.row && p1.col = p2.col
    let positionExistsInSet e = Set.exists (posEqual e) r
    ps |> List.fold  ( fun acc e-> acc && (positionExistsInSet e) ) true

/// <summary> if the king of provided color is under direct danger ( if the game state is 'Check') </summary>
let isCheck board color = 
    let king = List.head( board |> List.filter ( fun e-> e.data.type_ = KING && e.data.player = color ))
    [king.p] |> arePositionsInRangeOfPawnsOfColor board (opposite color)




// Experimental
type GameStatus = CONTINUE|DRAW| WIN of Color

// Experimental
let getGameStatus board =
    let isCheckmated color board =
        let king = List.head( board |> List.filter ( fun e-> e.data.type_ = KING && e.data.player = color ))
        // calculate status of fields around the king
        // if EACH one is either occupied by friendly or in range of enemy pawn, then checkmate ( literally)
        let kingDir = [(0,0);(1,0);(0,1);(-1,0);(0,-1);(1,1);(-1,1);(1,-1);(-1,-1)]
        kingDir
            |> List.map ( fun e -> { row = king.p.row + fst(e); col = king.p.col + snd(e)})
            |> List.filter ( fun e -> // get positions on board where there is none or enemy
                match inBounds e, getPawnOnBoard board e with
                    false, _ -> false
                    | _, Some p when p.data.player = color -> false // cannot move where friendly is at
                    | _ -> true)
            |> arePositionsInRangeOfPawnsOfColor board (opposite color)
    if isCheckmated BLACK board || isCheckmated WHITE board then WIN(BLACK) else CONTINUE // TODO this is not OK !
//    let isDraw = // TODO implement draw situation



// TODO <post-move check> check win conditions
// TODO minimax
// TODO after move should not leave at the state 'check' - wrap applyMove function in: let newBoard=...; if isCheck newBoard color then [] else newBoard
// TODO if pawn still at starting postion ( check dir!), then check if can go row+2


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