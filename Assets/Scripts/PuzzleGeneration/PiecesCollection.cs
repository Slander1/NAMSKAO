using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PuzzleGeneration
{
    public class PiecesCollection
	{
		class DictionaryComparer : IEqualityComparer<TipsVariant[]>
		{
			bool IEqualityComparer<TipsVariant[]>.Equals(TipsVariant[] x, TipsVariant[] y)
			{
				return (Enumerable.SequenceEqual(x, y));

			}

			int IEqualityComparer<TipsVariant[]>.GetHashCode(TipsVariant[] objects)
			{
				if (objects != null)
				{
						int hash = 17;

						foreach (var item in objects)
						{
							hash = hash * 23 + item.GetHashCode();
						}

						return hash;
				}

				return 0;
			}
		}
		private Dictionary<NamePos, Dictionary<TipsVariant[], PiecePuzzle>> pices { get; }

		public PiecesCollection(PiecePuzzle[] pieces)
		{
			pices = new Dictionary<NamePos, Dictionary<TipsVariant[], PiecePuzzle>> {
			{ NamePos.CENTER, new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) },
			{ NamePos.CORNER, new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) },
			{ NamePos.EDGE,   new Dictionary<TipsVariant[],PiecePuzzle>(new DictionaryComparer()) } };
			foreach (var piece in pieces)
			{
				if (piece.PieceData.namePos == NamePos.CENTER)
					pices[piece.PieceData.namePos].Add(piece.PieceData.tipsPiece, piece);
				else
				{
					for (int i = 0; i < 4; i++)
					{
						var tips = PieceRotation.ShiftArray(piece.PieceData.tipsPiece.ToArray(), i);
						pices[piece.PieceData.namePos].Add(tips, piece);
					}
				}
			}
		}


		public PiecePuzzle FindSuitablePazzle(PieceData pieceData)
		{
			return pices[pieceData.namePos][pieceData.tipsPiece];
		}
    }
}

