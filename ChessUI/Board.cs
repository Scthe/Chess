using System.Collections.Generic;
using Chess.Utils;

//////////////////
// C# to F# links:
//		http://stackoverflow.com/questions/661095/retrieving-items-from-an-f-list-passed-to-c-sharp
//		http://stackoverflow.com/questions/1952114/call-a-higher-order-f-function-from-c-sharp
//		http://www.voyce.com/index.php/2011/05/09/mixing-it-up-when-f-meets-c/
//////////////////

namespace ChessUI.model {

	class Board {

		private readonly List<Chess.Fs.Pawn> _pieces;

		public IReadOnlyCollection<Chess.Fs.Pawn> Pieces {
			get { return _pieces.AsReadOnly(); }
		}

		public Board() {
			_pieces = new List<Chess.Fs.Pawn>();
			
			// white
			for (int i = 0; i < 8; i++)
				_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.PAWN, 1, i));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 3, 0));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.ROOK, 0, 7));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 1));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KNIGHT, 0, 6));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 2));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.BISHOP, 0, 5));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.QUEEN, 0, 3));
			_pieces.Add(Utils.Create(Chess.Fs.Color.WHITE, Chess.Fs.PawnType.KING, 0, 4));

			// black
			for (int i = 0; i < 8; i++)
				_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.PAWN, 6, i));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 0));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.ROOK, 7, 7));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 1));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KNIGHT, 7, 6));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 2));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.BISHOP, 7, 5));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.KING, 7, 4));
			_pieces.Add(Utils.Create(Chess.Fs.Color.BLACK, Chess.Fs.PawnType.QUEEN, 7, 3));
		}

		
		public List<Chess.Fs.Position> GetMoves(Chess.Fs.Pawn pawn) {
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			var aa = Chess.Fs.getAvailableMoves(board, pawn);
			var a = Chess.Fs.unfoldMoves(aa);
			return new List<Chess.Fs.Position>(a);
		}

		public Chess.Fs.Pawn AtPosition(Chess.Fs.Position p) {
			Chess.Fs.Pawn pw = null;
			foreach (Chess.Fs.Pawn pn in _pieces) {
				if (pn.p.col == p.col && pn.p.row == p.row) pw = pn;
			}
			return pw;
		}

		public void Move(Chess.Fs.Pawn pawn, Chess.Fs.Position p){
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);

			// get move results
			var res = Chess.Fs.applyMove(board, pawn, Utils.CreateMoveTo(p));

			// update board
			var newBoard = res.Item1;
			_pieces.Clear();
			_pieces.AddRange(newBoard);

			// print move notation
			Chess.Fs.MoveResultType moveType = res.Item2;
			System.Console.WriteLine(Chess.Fs.moveNotation(moveType));
		}

		private bool IsCheck() {
			var board = FSharpInteropExtensions.ToFSharplist<Chess.Fs.Pawn>(_pieces);
			var a= Chess.Fs.isCheck(board, Chess.Fs.Color.BLACK);
			System.Console.WriteLine("\tcheck? - "+a);
			return a;
		}
	}
}
