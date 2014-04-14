using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

//////////////////
// C# to F# links:
//		http://stackoverflow.com/questions/661095/retrieving-items-from-an-f-list-passed-to-c-sharp
//		http://stackoverflow.com/questions/1952114/call-a-higher-order-f-function-from-c-sharp
//		http://www.voyce.com/index.php/2011/05/09/mixing-it-up-when-f-meets-c/
//////////////////

namespace ChessUI.model {

	public static class FSharpInteropExtensions {

		public static FSharpList<TItemType> ToFSharplist<TItemType>(this IEnumerable<TItemType> myList) {
			return Microsoft.FSharp.Collections.ListModule.OfSeq<TItemType>(myList);
		}

		public static IEnumerable<TItemType> ToEnumerable<TItemType>(this FSharpList<TItemType> fList) {
			return Microsoft.FSharp.Collections.SeqModule.OfList<TItemType>(fList);
		}
	}

	class Board {

		private readonly List<Chess.Fs.Pawn> _pieces;

		public IReadOnlyCollection<Chess.Fs.Pawn> Pieces {
			get { return _pieces.AsReadOnly(); }
		}

		public Board() {
			_pieces = new List<Chess.Fs.Pawn>();

			// white
			for (int i = 0; i < 8; i++)
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 1, i));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 3, 0));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 0, 7));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 1));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 6));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 2));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 5));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.QUEEN, 0, 3));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KING, 0, 4));

			// black
			//for (int i = 0; i < 8; i++)
				//_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 6, i));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 0));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 7));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 1));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 6));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 2));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 5));
			//_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KING, 7, 4));
			//_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.QUEEN, 7, 3));
			//_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 1, 4));
			//_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KING, 0, 4));

			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KING, 4, 5));

			/*
			// pawn tests
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 6, 1));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 6, 4));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 5, 4));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 4, 1));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 3, 0));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 3, 2));
			
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 1, 6));
			 */
		}



		private static Chess.Fs.Pawn Create(Chess.Fs.Color c, Chess.Fs.PawnType t, int row, int col) {
			return new Chess.Fs.Pawn(
				new Chess.Fs.Position(row, col),
				new Chess.Fs.Piece(t, c));
		}

		public List<Chess.Fs.Position> getMoves(Chess.Fs.Pawn pawn) {
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			var a = Chess.Fs.unfoldMoves_indirect(board, pawn);
			return new List<Chess.Fs.Position>(a);
			//return __debugTestMovesOfBlackPawns();
		}

		public Chess.Fs.Pawn atPosition(Chess.Fs.Position p) {
			Chess.Fs.Pawn pw = null;
			foreach (Chess.Fs.Pawn pn in _pieces) {
				if (pn.p.col == p.col && pn.p.row == p.row) pw = pn;
			}
			return pw;
		}

		public void move(Chess.Fs.Pawn pawn, Chess.Fs.Position p){
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			System.Func<Unit,Chess.Fs.Move> f= Unit => Chess.Fs.Move.End;

			// get move results
			var ff = FSharpFunc<Unit, Chess.Fs.Move>.FromConverter(
				new System.Converter<Unit, Chess.Fs.Move>( f ) );
			var m = Chess.Fs.Move.Move.NewMove(p, ff);
			var res = Chess.Fs.applyMove(board, pawn, m);

			// update board
			var newBoard = res.Item1;
			_pieces.Clear();
			_pieces.AddRange(newBoard);

			// print move notation
			Chess.Fs.MoveResultType moveType = res.Item2;
			System.Console.WriteLine(Chess.Fs.moveNotation(moveType));
		}

		public bool isCheck() {
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			var a= Chess.Fs.isCheck(board, Chess.Fs.Color.BLACK);
			System.Console.WriteLine("\tcheck? - "+a);
			/*
			var king = Chess.Fs.getKing( board);
			//System.Console.WriteLine("\tking? - " + b.p.row+" "+b.p.col);
			var c = Chess.Fs.buildAllPositionsForColor(board,Chess.Fs.Color.WHITE);
			foreach (Chess.Fs.Position p in c) {
				var bbb = p == king.p;
				if (Chess.Fs.posEqual(king.p,p))
					System.Console.WriteLine("\thand q!");
				//System.Console.WriteLine(string.Format( "\t{0} -> eq: {1} ( r: {2} c: {3})", Chess.Fs.positionNotation(p), bbb ,(p.row == b.p.row) , p.col == b.p.col));
			}

			var d = Chess.Fs.positionExistsInSet(king.p, c);
			if(d)
				System.Console.WriteLine("\tauto q!");

			var eee = new List<Chess.Fs.Position>();
			eee.Add(king.p);
			var ee = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Position>(eee);
			var e = Chess.Fs.arePositionsInRangeOfPawnsOfColor(board,Chess.Fs.Color.WHITE,ee);
			System.Console.WriteLine("\tauto2 qq ->"+e);
			*/
			return a;
		}

		private List<Chess.Fs.Position> __debugTestMovesOfBlackPawns() {
			var r = new List<Chess.Fs.Position>();
			foreach (Chess.Fs.Pawn p in _pieces) {
				var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
				var a = Chess.Fs.unfoldMoves_indirect(board, p);
				if(p.data.player==Chess.Fs.Color.BLACK)
					r.AddRange(a);
			}
			return r;
		}
	}
}
