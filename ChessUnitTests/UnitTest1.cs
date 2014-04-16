using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Chess;
using Chess.Utils;

namespace ChessUnitTests {

	//System.Diagnostics.Trace.WriteLine("Hello World");

	[TestClass]
	public class ChessFsTest {

		public ChessFsTest() {		}

		[TestMethod]
		public void TestMoveNotation() {
			List<Fs.Pawn> _pieces = new List<Fs.Pawn>();
			_pieces.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 1));
			_pieces.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 0, 2));
			_pieces.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 0, 3));
			_pieces.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.BISHOP, 0, 4));
			_pieces.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.QUEEN, 0, 5));
			_pieces.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.KING, 0, 6));
			
			int[,] destinations = { { 4, 5 }, { 6, 1 }, { 0, 0 }, { 7, 7 }, { 5, 5 }, { 6, 6 } };
			string[] expected = { "f5", "Rb7", "Na1", "Bh8", "Qf6", "Kg7" };
			for (int i = 0; i < _pieces.Count; i++) {
				var p = _pieces[i];
				var m = Utils.CreateMoveTo(new Fs.Position(destinations[i, 0], destinations[i, 1]));
				var mm = Fs.MoveResultType.SimpleMove.NewSimpleMove(p, m);
				var res = Fs.moveNotation(mm);
				Console.WriteLine(res);
				Assert.AreEqual<string>(expected[i], res);
			}
		}

		[TestMethod]
		public void TestAvailableMoves_Pawn() {
			List<Fs.Pawn> _pawnBoard = new List<Fs.Pawn>();
			// no moves - edge
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 0);
			var p1 = Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 7, 0);
			_pawnBoard.Add(p0);
			_pawnBoard.Add(p1);
			// go forward
			var p2 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 1);
			var p3 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 3, 1);
			var p4 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 7, 1);
			_pawnBoard.Add(p2);
			_pawnBoard.Add(p3);
			_pawnBoard.Add(p4);
			// capture diagonals, block in front
			var p5 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 3);
			_pawnBoard.Add(p5);
			_pawnBoard.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 3, 3));
			_pawnBoard.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 3, 2));
			_pawnBoard.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 3, 4));
			// test
			ExpectMoves(_pawnBoard, p0, 0);
			ExpectMoves(_pawnBoard, p1, 0);
			ExpectMoves(_pawnBoard, p2, 1);
			ExpectMoves(_pawnBoard, p3, 1);
			ExpectMoves(_pawnBoard, p4, 1);
			ExpectMoves(_pawnBoard, p5, 2);
		}

		[TestMethod]
		public void TestAvailableMovesRook() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// no moves - edge
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 0, 0);
			bb.Add(p0);
			// block by own color
			bb.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 1, 2));
			bb.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 1, 4));
			// capture
			bb.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 2, 5));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 2, 7));
			// test
			ExpectMoves(bb, p0, 14);
			ExpectMoves(bb, bb[1], 7 + 3);
			ExpectMoves(bb, bb[2], 7 + 4);
			ExpectMoves(bb, bb[3], 7 + 7);
			ExpectMoves(bb, bb[4], 7 + 2);
		}

		[TestMethod]
		public void TestAvailableMovesKnight() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// corner
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 0, 0);
			bb.Add(p0);
			int[,] e1 = { { 1, 2 }, { 2, 1 } };
			// center
			var p1 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 4, 4);
			bb.Add(p1);
			int[,] e2 = {
						{ 5, 6 }, { 5, 2 }, { 3, 6 }, { 3, 2 },
						{ 6, 5 }, { 6, 3 }, { 2, 5 }, { 2, 3 } };
			// test
			ExpectMoves(bb, p0, PositionListFromArray(e1));
			ExpectMoves(bb, p1, PositionListFromArray(e2));
		}

		[TestMethod]
		public void TestAvailableMovesBishop() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// long diagonal
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.BISHOP, 0, 0);
			bb.Add(p0);
			ExpectMoves(bb, p0, 7);
			// long diagonal, obstacle
			var p1 = Utils.Create(Fs.Color.WHITE, Fs.PawnType.BISHOP, 4, 4);
			bb.Add(p1);
			ExpectMoves(bb, p0, 4);
			// center
			bb.Clear();
			bb.Add(p1);
			ExpectMoves(bb, p1, 13);
		}

		[TestMethod]
		public void TestAvailableMovesQueen() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// center
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.QUEEN, 4, 4);
			bb.Add(p0);
			ExpectMoves(bb, p0, 27);
			// block capture
			var p1 = Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 5, 5);
			var p2 = Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 4, 5);
			bb.Add(p1);
			bb.Add(p2);
			ExpectMoves(bb, p0, 23);
			// block by same color
			bb.Clear();
			bb.Add(p0);
			var p3 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 5, 5);
			var p4 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 5);
			bb.Add(p3);
			bb.Add(p4);
			ExpectMoves(bb, p0, 21);
		}

		[TestMethod]
		public void TestAvailableMovesKing() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// center
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KING, 4, 4);
			bb.Add(p0);
			ExpectMoves(bb, p0, 8);
			// corner
			bb.Clear();
			var p1 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KING, 0,0 );
			bb.Add(p1);
			ExpectMoves(bb, p1, 3);
			// corner, block by same color
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 0));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 1));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 1));
			ExpectMoves(bb, p1, 0);
		}


		//
		// utils
		//
		private static List<Fs.Position> PositionListFromArray(int[,] arr) {
			int len = arr.Length / 2;
			var r = new List<Fs.Position>(len);
			for (int i = 0; i < len; i++) {
				r.Add(new Fs.Position(arr[i, 0], arr[i, 1]));
			}
			return r;
		}

		private static void ExpectMoves(List<Fs.Pawn> board, Fs.Pawn pawn, int cnt) {
			var b = FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(board);
			var res1 = Fs.getAvailableMoves(b, pawn);
			var res2 = Fs.unfoldMoves(res1);
			var res = new List<Fs.Position>(res2);
			Console.WriteLine(res.Count);
			Assert.AreEqual(cnt, res.Count);
		}
		private static void ExpectMoves(List<Fs.Pawn> board, Fs.Pawn pawn, List<Fs.Position> positions) {
			var b = FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(board);
			var res1 = Fs.getAvailableMoves(b, pawn);
			var res2 = Fs.unfoldMoves(res1);
			var res = new List<Fs.Position>(res2);
			/*
			foreach (var p in res) {
				Console.WriteLine(p.row + " " + p.col + " ; ");
			}
			Console.WriteLine("------------");
			foreach (var p in positions) {
				Console.WriteLine(p.row + " " + p.col + " ; ");
			}*/

			CollectionAssert.AreEquivalent(res, positions);
		}

	}

	
}
