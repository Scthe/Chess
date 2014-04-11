namespace UnitTestProject1

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Chess


[<TestClass>]
type UnitTest() = 

    member x.getBoard = 
        [
            { p={col=1;row=1}; data={type_=PAWN; player=BLACK}} ;
            { p={col=2;row=5}; data={type_=PAWN; player=BLACK}} ;
            { p={col=3;row=4}; data={type_=PAWN; player=BLACK}} ;
        ]

    member x.listsSame l1 l2= 
        let rec f l =
            match l with
                h::t -> if List.exists ((=)h) l2 then f t else false
                | _ -> true
        //f l1
        (List.sort l1) = (List.sort l2)
    
    [<TestMethod>]
    member x.oppositeTest () = 
        Assert.AreEqual( Chess.opposite BLACK, WHITE)
        Assert.AreEqual( Chess.opposite Chess.BLACK, WHITE)
    
   

    [<TestMethod>]
    member x.getPawn () =
        let board = x.getBoard
        Assert.IsFalse( isAvailable board {col=1;row=1})
        Assert.IsFalse( isAvailable board {col=2;row=5})
        Assert.IsFalse( isAvailable board {col=3;row=4})

    [<TestMethod>]
    member x.inBounds () =
        Assert.IsTrue( inBounds {col=1;row=1})
        Assert.IsTrue( inBounds {col=0;row=5})
        Assert.IsTrue( inBounds {col=3;row=0})
        Assert.IsFalse( inBounds {col=3;row=8})
        Assert.IsFalse( inBounds {col=3;row=(-1) })
      
    [<TestMethod>]
    member x.movePawn () = ()
    
    [<TestMethod>]
    member x.moveRook () = ()
    
    [<TestMethod>]
    member x.moveKnight () = ()
    
    [<TestMethod>]
    member x.moveBishop () = ()

    [<TestMethod>]
    member x.moveQueen () = ()

    [<TestMethod>]
    member x.moveKing () = 
        let aa = getAvailableMoves [] ({type_=KING; player=BLACK}@{col=7;row=5}) |> unfoldMoves //|> List.map (fun p-> p.col,p.row)
        let a = List.map (fun e->e.col,e.row) aa
        //let aaa = getAvailableMoves [] (data@{col=7;row=5})
        //let aa = unfoldMoves aaa
        //let a  = List.map (fun p-> p.col,p.row) aa
        // data
        //   |> List.sortWith compare
        //   |> List.map (function p-> Console.WriteLine( "{0} {1}",p.col,p.row))        

        //Assert.IsTrue(x.listsSame [1,2] [1,2])
        //Assert.IsTrue(x.listsSame [(1,2);(3,3)] [(3,3);(1,2)])
        Assert.IsTrue(x.listsSame a [6,4; 6,5; 6,6; 7,6; 7,4])

        ()
