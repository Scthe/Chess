using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Chess;

namespace ChessUnitTests {

	//System.Diagnostics.Trace.WriteLine("Hello World");

	[TestClass]
	public class ChessFsTest {

		private readonly List<Fs.Pawn> _pieces;

		public ChessFsTest() {
			_pieces = new List<Fs.Pawn>();


			_pieces.Add(Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 1));
			_pieces.Add(Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 0, 2));
			_pieces.Add(Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 0, 3));
			_pieces.Add(Create(Fs.Color.BLACK, Fs.PawnType.BISHOP, 0, 4));
			_pieces.Add(Create(Fs.Color.WHITE, Fs.PawnType.QUEEN, 0, 5));
			_pieces.Add(Create(Fs.Color.WHITE, Fs.PawnType.KING, 0, 6));
		}



		[TestMethod]
		public void TestMoveNotation() {
			int[,] destinations = { { 4, 5 }, { 6, 1 }, { 0, 0 }, { 7, 7 }, { 5, 5 }, { 6, 6 } };
			string[] expected = { "f5", "Rb7", "Na1", "Bh8", "Qf6", "Kg7" };
			for (int i = 0; i < _pieces.Count; i++) {
				var p = _pieces[i];
				var m = createMoveTo(new Fs.Position(destinations[i, 0], destinations[i, 1]));
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
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 0);
			var p1 = Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 7, 0);
			_pawnBoard.Add(p0);
			_pawnBoard.Add(p1);
			// go forward
			var p2 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 1);
			var p3 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 3, 1);
			var p4 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 7, 1);
			_pawnBoard.Add(p2);
			_pawnBoard.Add(p3);
			_pawnBoard.Add(p4);
			// capture diagonals, block in front
			var p5 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 3);
			_pawnBoard.Add(p5);
			_pawnBoard.Add(Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 3, 3));
			_pawnBoard.Add(Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 3, 2));
			_pawnBoard.Add(Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 3, 4));
			// test
			expectMoves(_pawnBoard, p0, 0);
			expectMoves(_pawnBoard, p1, 0);
			expectMoves(_pawnBoard, p2, 1);
			expectMoves(_pawnBoard, p3, 1);
			expectMoves(_pawnBoard, p4, 1);
			expectMoves(_pawnBoard, p5, 2);
		}

		[TestMethod]
		public void TestAvailableMovesRook() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// no moves - edge
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 0, 0);
			bb.Add(p0);
			// block by own color
			bb.Add(Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 1, 2));
			bb.Add(Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 1, 4));
			// capture
			bb.Add(Create(Fs.Color.WHITE, Fs.PawnType.ROOK, 2, 5));
			bb.Add(Create(Fs.Color.BLACK, Fs.PawnType.ROOK, 2, 7));
			// test
			expectMoves(bb, p0, 14);
			expectMoves(bb, bb[1], 7 + 3);
			expectMoves(bb, bb[2], 7 + 4);
			expectMoves(bb, bb[3], 7 + 7);
			expectMoves(bb, bb[4], 7 + 2);
		}




		[TestMethod]
		public void TestAvailableMovesKnight() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// corner
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 0, 0);
			bb.Add(p0);
			int[,] e1 = { { 1, 2 }, { 2, 1 } };
			// center
			var p1 = Create(Fs.Color.BLACK, Fs.PawnType.KNIGHT, 4, 4);
			bb.Add(p1);
			int[,] e2 = {
						{ 5, 6 }, { 5, 2 }, { 3, 6 }, { 3, 2 },
						{ 6, 5 }, { 6, 3 }, { 2, 5 }, { 2, 3 } };
			// test
			expectMoves(bb, p0, positionArray(e1));
			expectMoves(bb, p1, positionArray(e2));
		}

		[TestMethod]
		public void TestAvailableMovesBishop() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// long diagonal
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.BISHOP, 0, 0);
			bb.Add(p0);
			expectMoves(bb, p0, 7);
			// long diagonal, obstacle
			var p1 = Create(Fs.Color.WHITE, Fs.PawnType.BISHOP, 4, 4);
			bb.Add(p1);
			expectMoves(bb, p0, 4);
			// center
			bb.Clear();
			bb.Add(p1);
			expectMoves(bb, p1, 13);
		}

		[TestMethod]
		public void TestAvailableMovesQueen() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// center
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.QUEEN, 4, 4);
			bb.Add(p0);
			expectMoves(bb, p0, 27);
			// block capture
			var p1 = Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 5, 5);
			var p2 = Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 4, 5);
			bb.Add(p1);
			bb.Add(p2);
			expectMoves(bb, p0, 23);
			// block by same color
			bb.Clear();
			bb.Add(p0);
			var p3 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 5, 5);
			var p4 = Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 5);
			bb.Add(p3);
			bb.Add(p4);
			expectMoves(bb, p0, 21);
		}

		[TestMethod]
		public void TestAvailableMovesKing() {
			List<Fs.Pawn> bb = new List<Fs.Pawn>();
			// center
			var p0 = Create(Fs.Color.BLACK, Fs.PawnType.KING, 4, 4);
			bb.Add(p0);
			expectMoves(bb, p0, 8);
			// corner
			bb.Clear();
			var p1 = Create(Fs.Color.BLACK, Fs.PawnType.KING, 0,0 );
			bb.Add(p1);
			expectMoves(bb, p1, 3);
			// corner, block by same color
			bb.Add(Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 0));
			bb.Add(Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 1));
			bb.Add(Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 1));
			expectMoves(bb, p1, 0);
		}

		//
		// TODO create separate utils class
		//
		private static Fs.Move createMoveTo(Fs.Position p) {
			System.Func<Unit, Fs.Move> f = Unit => Fs.Move.End;
			var ff = FSharpFunc<Unit, Fs.Move>.FromConverter(
				new System.Converter<Unit, Fs.Move>(f));
			return Fs.Move.Move.NewMove(p, ff);
		}

		private static Fs.Pawn Create(Fs.Color c, Fs.PawnType t, int row, int col) {
			return new Fs.Pawn(
				new Fs.Position(row, col),
				new Fs.Piece(t, c));
		}

		private static List<Fs.Position> positionArray(int[,] arr) {
			int len = arr.Length / 2;
			var r = new List<Fs.Position>(len);
			for (int i = 0; i < len; i++) {
				r.Add(new Fs.Position(arr[i, 0], arr[i, 1]));
			}
			return r;
		}
		public class PositionComparer : System.Collections.IComparer {
			public int Compare(object x, object y) {
				var lhs = x as Fs.Position;
				var rhs = y as Fs.Position;
				if (lhs == null || rhs == null) throw new InvalidOperationException();
				return Compare(lhs, rhs);
			}

			public int Compare(Fs.Position e1, Fs.Position e2) {
				//Console.WriteLine("\t" + e1.row + " " + e1.col + " ; ");
				//Console.WriteLine("\t" + e2.row + " " + e2.col + " ; ");
				bool eq = e1.row == e2.row && e1.col == e2.col;
				return eq ? 0 : e1.row < e2.row ? -1 : 1;
			}
		}

		private static void expectMoves(List<Fs.Pawn> board, Fs.Pawn pawn, int cnt) {
			var b = FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(board);
			var res1 = Fs.getAvailableMoves(b, pawn);
			var res2 = Fs.unfoldMoves(res1);
			var res = new List<Fs.Position>(res2);
			Console.WriteLine(res.Count);
			Assert.AreEqual(cnt, res.Count);
		}
		private static void expectMoves(List<Fs.Pawn> board, Fs.Pawn pawn, List<Fs.Position> positions) {
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

			System.Collections.IComparer cmp = new PositionComparer();
			CollectionAssert.AreEquivalent(res, positions);
		}

	}

	public static class FSharpInteropExtensions {

		public static FSharpList<TItemType> ToFSharplist<TItemType>(this IEnumerable<TItemType> myList) {
			return Microsoft.FSharp.Collections.ListModule.OfSeq<TItemType>(myList);
		}

		public static IEnumerable<TItemType> ToEnumerable<TItemType>(this FSharpList<TItemType> fList) {
			return Microsoft.FSharp.Collections.SeqModule.OfList<TItemType>(fList);
		}
	}
}
