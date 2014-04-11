using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Collections;

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

		//private readonly List<Pawn> _pieces;
		private readonly List<Chess.Fs.Pawn> _pieces;

		public IReadOnlyCollection<Chess.Fs.Pawn> Pieces {
			get { return _pieces.AsReadOnly(); }
		}

		public Board() {
			_pieces = new List<Chess.Fs.Pawn>();

			// white
			//for (int i = 0; i < 8; i++)
				//_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 1, i));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 0, 0));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 0, 7));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 1));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 6));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 2));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 5));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.QUEEN, 0, 3));
			_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KING, 0, 4));

			// black
			for (int i = 0; i < 8; i++)
				_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 6, i));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 0));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 7));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 1));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 6));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 2));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 5));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.QUEEN, 7, 4));
			_pieces.Add(Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KING, 7, 3));

			//_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 1, 4));
			//_pieces.Add(Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KING, 0, 4));
		}



		private static Chess.Fs.Pawn Create(Chess.Fs.Color c, Chess.Fs.PawnType t, int row, int col) {
			return new Chess.Fs.Pawn(
				new Chess.Fs.Position(row, col),
				new Chess.Fs.Piece(t, c));
		}

		public List<Chess.Fs.Position> getMoves(Chess.Fs.Pawn pawn) {
			var toFs = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			var a = Chess.Fs.unfoldMoves_indirect( toFs, pawn);
			return new List<Chess.Fs.Position>(a);

			//var a = Chess.Fs.getAvailableMoves(toFs, pawn);
			//return new List<Chess.Fs.Position>();
		}

		public Chess.Fs.Pawn atPosition(Chess.Fs.Position p) {
			Chess.Fs.Pawn pw = null;
			foreach (Chess.Fs.Pawn pn in _pieces) {
				if (pn.p.col == p.col && pn.p.row == p.row) pw = pn;
			}
			return pw;
		}
	}
}
