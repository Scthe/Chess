using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Chess;
using Chess.Utils;

namespace ChessUnitTests {

	//System.Diagnostics.Trace.WriteLine("Hello World");

	[TestClass]
	public class ChessFsTest {

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
			var p1 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KING, 0, 0);
			bb.Add(p1);
			ExpectMoves(bb, p1, 3);
			// corner, block by same color
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 0));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 1));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 1, 1));
			ExpectMoves(bb, p1, 0);
		}

		[TestMethod]
		public void TestApplyMove() {
			var cmp = new Utils.PositionComparer();
			List<Fs.Pawn> bb = new List<Fs.Pawn>();

			// just move
			var p0 = Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 4);
			bb.Add(p0);
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 0, 0));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 2, 0));
			bb.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.PAWN, 4, 0));
			var target = new Fs.Position(3, 4);

			var res = ExecuteMove(bb, p0, target);
			var newBoard = new List<Fs.Pawn>(res.Item1);
			var move = res.Item2;
			Assert.AreEqual(bb.Count, newBoard.Count);
			Assert.AreEqual(0, cmp.Compare(newBoard[0].p, target));
			Assert.IsTrue(res.Item2.IsSimpleMove);
			Chess.Fs.MoveResultType.SimpleMove sm = res.Item2 as Chess.Fs.MoveResultType.SimpleMove;
			Assert.AreEqual(sm.Item1, p0);

			// capture
			var p1 = Utils.Create(Fs.Color.WHITE, Fs.PawnType.PAWN, 3, 4);
			bb.Add(p1);

			res = ExecuteMove(bb, p0, target);
			newBoard = new List<Fs.Pawn>(res.Item1);
			move = res.Item2;
			Assert.AreEqual(bb.Count - 1, newBoard.Count); // deleted one !
			Assert.IsTrue(newBoard.Exists((Fs.Pawn p) => { return cmp.Compare(p.p, target) == 0; }));
			Assert.IsTrue(res.Item2.IsCapture);
			Chess.Fs.MoveResultType.Capture cap = res.Item2 as Chess.Fs.MoveResultType.Capture;
			Assert.AreEqual(cap.Item1, p0);
			Assert.AreEqual(cap.Item3, p1);
			// impossible move
			var targetImpossible = new Fs.Position(7, 7);
			res = ExecuteMove(bb, p0, targetImpossible);
			Assert.IsTrue(res.Item2.IsImpossible);
			// TODO promotions etc.
		}

		[TestMethod]
		public void TestIsCheck() {
			List<Fs.Pawn> baseBoard = new List<Fs.Pawn>();
			baseBoard.Add(Utils.Create(Fs.Color.BLACK, Fs.PawnType.KING, 4, 4));

			List<Fs.PawnType> ll = new List<Fs.PawnType>();
			ll.Add(Fs.PawnType.ROOK);
			ll.Add(Fs.PawnType.BISHOP);
			ll.Add(Fs.PawnType.KNIGHT);
			ll.Add(Fs.PawnType.QUEEN);
			Dictionary<Fs.PawnType, int> dict = new Dictionary<Fs.PawnType, int>();
			foreach (Fs.PawnType pt in ll) dict.Add(pt, 0);

			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					foreach (Fs.PawnType pt in ll) {
						var p = new Fs.Pawn(new Fs.Position(i, j), new Fs.Piece(pt, Fs.Color.WHITE));
						List<Fs.Pawn> bb = new List<Fs.Pawn>(baseBoard);
						bb.Add(p);
						var b = Fs.isCheck(FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(bb), Fs.Color.BLACK);
						if (b) {
							dict[pt] = dict[pt] + 1;
						}
					}
				}
			}
			Assert.AreEqual(14, dict[Fs.PawnType.ROOK]); // there are total of 14 positions where rook can reach the king
			Assert.AreEqual(13, dict[Fs.PawnType.BISHOP]);
			Assert.AreEqual(8, dict[Fs.PawnType.KNIGHT]);
			Assert.AreEqual(27, dict[Fs.PawnType.QUEEN]);
			//foreach (KeyValuePair<Fs.PawnType, int> pair in dict) {
			//Console.WriteLine(pair.Key.ToString() + " -> " + pair.Value);
			//}
		}

		[TestMethod]
		public void TestCheckmateDetection() {
			// king in the corner, cover position of only solution with the knight, place the queen in that position
			List<Fs.Pawn> baseBoard = new List<Fs.Pawn>();
			var king = Utils.Create(Fs.Color.BLACK, Fs.PawnType.KING, 0, 0);
			var knight = Utils.Create(Fs.Color.WHITE, Fs.PawnType.KNIGHT, 3, 2);
			var q = new Fs.Piece(Fs.PawnType.QUEEN, Fs.Color.WHITE);
			baseBoard.Add(king);
			baseBoard.Add(knight);

			int cnt = 0;
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					//if (i == king.p.row && j == king.p.col) continue;
					var p = new Fs.Pawn(new Fs.Position(i, j), q);
					List<Fs.Pawn> bb = new List<Fs.Pawn>(baseBoard);
					bb.Add(p);
					var b = Fs.isCheckmated(FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(bb), Fs.Color.BLACK);
					if (b) {
						Assert.IsTrue(Fs.isCheck(FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(bb), Fs.Color.BLACK));
						Console.WriteLine(i + "x" + j);
						cnt += 1;
					}
				}
			}
			Console.WriteLine("(" + cnt + ")");
			Assert.AreEqual(1, cnt);



			// sub test - king moves detection
			/*
			var res = Fs.kk(FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(board), Fs.Color.BLACK);
			Console.WriteLine("("+res.Length+")"); 
			foreach (Fs.Position p in res) {
				Console.WriteLine(p.row + " x " + p.col);
			}*/
			// straight up solution sub-test
			/*
			baseBoard.Add(Utils.Create(Fs.Color.WHITE, Fs.PawnType.QUEEN, 1, 1));

			Console.WriteLine( "knight can move to (1,1): "+Fs.canMoveTo(true,
				FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(baseBoard),
				knight.data, new Fs.Position(1,1) ) );

			var lll = Fs.buildAllPositionsForColor(
				FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(baseBoard), Fs.Color.WHITE );
			foreach (IEnumerable< Fs.Position> ps in lll) {
				Console.WriteLine("("+ps.ToFSharplist().Length+"):");
				foreach (Fs.Position p in ps) {
					Console.WriteLine("\t"+p.row + " x " + p.col);
				}
				//break;
				Console.WriteLine("--next pawn--");
			}

			Console.WriteLine("Around king state:");

			var positionsAroundTheKingIncludingPresent = Fs.kk(FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(baseBoard), Fs.Color.BLACK);
			foreach (Fs.Position p in positionsAroundTheKingIncludingPresent) {
				var ll = new List<Fs.Position>();
				ll.Add(p);
				var r =Fs.arePositionsInRangeOfPawnsOfColor(
					FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(baseBoard),
					Fs.Color.WHITE, 
					FSharpInteropExtensions.ToFSharplist<Fs.Position>(ll)
					);
				Console.WriteLine("\t"+p.row + " x " + p.col+" --inrange--> "+r);
			}
			*/
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
			var res1 = Fs.getAvailableMoves(false, b, pawn);
			var res2 = Fs.unfoldMoves(res1);
			var res = new List<Fs.Position>(res2);
			Console.WriteLine(res.Count);
			Assert.AreEqual(cnt, res.Count);
		}
		private static void ExpectMoves(List<Fs.Pawn> board, Fs.Pawn pawn, List<Fs.Position> positions) {
			var b = FSharpInteropExtensions.ToFSharplist<Fs.Pawn>(board);
			var res1 = Fs.getAvailableMoves(false, b, pawn);
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

		private static Tuple<Microsoft.FSharp.Collections.FSharpList<Fs.Pawn>, Fs.MoveResultType> ExecuteMove(List<Fs.Pawn> board, Fs.Pawn p, Fs.Position target) {
			var bb = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(board);
			var move = Utils.CreateMoveTo(target);
			return Chess.Fs.applyMove(bb, p, move);
			//return new List<Fs.Pawn>(res.Item1);
		}
	}

}
