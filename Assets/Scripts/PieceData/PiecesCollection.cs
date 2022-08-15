using System.Collections.Generic;
using System.Linq;
using PuzzleControllers;

namespace PieceData
{
    public class PiecesCollection
	{
		private class DictionaryComparer : IEqualityComparer<TipsVariant[]>
		{
			bool IEqualityComparer<TipsVariant[]>.Equals(TipsVariant[] x, TipsVariant[] y)
			{
				return x.SequenceEqual(y);
			}

			int IEqualityComparer<TipsVariant[]>.GetHashCode(TipsVariant[] objects)
			{
				int hash = 17;

				foreach (var item in objects)
				{
					hash = hash * 23 + item.GetHashCode();
				}

				return hash;
			}
		}

		private readonly Dictionary<NamePos, Dictionary<TipsVariant[], PiecePuzzle>> _pieces;

		public PiecesCollection(IEnumerable<PiecePuzzle> pieces)
		{
			_pieces = new Dictionary<NamePos, Dictionary<TipsVariant[], PiecePuzzle>> {
			{ NamePos.CENTER, new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) },
			{ NamePos.CORNER, new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) },
			{ NamePos.EDGE,   new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) } };
			foreach (var piece in pieces)
			{
				if (piece.PieceData.namePos == NamePos.CENTER)
					_pieces[piece.PieceData.namePos].Add(piece.PieceData.tipsPiece, piece);
				else
				{
					for (int i = 0; i < 4; i++)
					{
						var tips = PieceRotationChanger.ShiftArray(piece.PieceData.tipsPiece.ToArray(), i);
						_pieces[piece.PieceData.namePos].Add(tips, piece);
					}
				}
			}
		}

		public PiecePuzzle FindSuitablePuzzle(global::PieceData.PieceData pieceData)
		{
			return _pieces[pieceData.namePos][pieceData.tipsPiece];
		}
    }
}

