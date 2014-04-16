using System;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Chess {
	namespace Utils {

		public static class FSharpInteropExtensions {

			public static FSharpList<TItemType> ToFSharplist<TItemType>(this IEnumerable<TItemType> myList) {
				return Microsoft.FSharp.Collections.ListModule.OfSeq<TItemType>(myList);
			}

			public static IEnumerable<TItemType> ToEnumerable<TItemType>(this FSharpList<TItemType> fList) {
				return Microsoft.FSharp.Collections.SeqModule.OfList<TItemType>(fList);
			}
		}

		public static class Utils {

			public static Fs.Move CreateMoveTo(Fs.Position p) {
				System.Func<Unit, Fs.Move> f = Unit => Fs.Move.End;
				var ff = FSharpFunc<Unit, Fs.Move>.FromConverter(
					new System.Converter<Unit, Fs.Move>(f));
				return Fs.Move.Move.NewMove(p, ff);
			}

			public static Fs.Pawn Create(Fs.Color c, Fs.PawnType t, int row, int col) {
				return new Fs.Pawn(
					new Fs.Position(row, col),
					new Fs.Piece(t, c));
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
		}
	}
}
