using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;


//  возможно стоит сделать статическим
public class PiecesCollection
{
    class A : IEqualityComparer<TipsVariant[]>
    {
        bool IEqualityComparer<TipsVariant[]>.Equals(TipsVariant[] x, TipsVariant[] y)
        {
			return (Enumerable.SequenceEqual(x, y));

		}

        int IEqualityComparer<TipsVariant[]>.GetHashCode(TipsVariant[] objects)
        {
			return 1; // избавиться 
			int hash = objects.GetHashCode();
			foreach(var obj in objects)
            {
				hash ^= obj.GetHashCode();
            }
			return hash;
		}
    }
    private Dictionary<NamePos, Dictionary<TipsVariant[], PiecePazzle>> _pices { get; }

	public PiecesCollection(PiecePazzle[] pieces)
	{
		_pices = new Dictionary<NamePos, Dictionary<TipsVariant[],PiecePazzle>> {
			{ NamePos.CENTER, new Dictionary<TipsVariant[],PiecePazzle>(new A()) },
			{ NamePos.CORNER, new Dictionary<TipsVariant[],PiecePazzle>(new A()) },
			{ NamePos.EDGE,   new Dictionary<TipsVariant[],PiecePazzle>(new A()) } };
		foreach (var piece in pieces)
		{
			if (piece.PieceData.namePos == NamePos.CENTER)
				_pices[piece.PieceData.namePos].Add(piece.PieceData.tipsPiece, piece);
			else
            {
                for (int i = 0; i < 4; i++)
                {
					var tips =  PieceRotation.ShiftArray(piece.PieceData.tipsPiece.ToArray(), i);
					_pices[piece.PieceData.namePos].Add(tips, piece);
				}
            }				
		}
		// не работает InitState;
		Random.InitState(PuzzleGenerator.Instanse.seed);
	}


	public PiecePazzle FindSuitablePazzle(PieceData pieceData, Vector2Int pos)
	{
		return _pices[pieceData.namePos][pieceData.tipsPiece];
		//PieceRotation.ShiftTips(pos, tips); //Проверять на центр

		//var randomizeTips = RandomizeTips(tips);

		List<PiecePazzle> collection = null;
		// переработать на систему по позициям
		//switch (randomizeTips.Count)
		//{
		//	case 2:
		//		collection = _cornerCollection;
		//		break;

		//	case 3:
		//		collection = _edgeCollection;
		//		break;

		//	case 4:
		//		collection = _centerCollection;
		//		break;

		//	default:
		//		break;
		//}
		
		foreach (var piece in collection)
		{
			//if (Enumerable.SequenceEqual(randomizeTips, piece.tipsPiece))
				return piece;
		}

		return null;
	}

	private List<TipsVariant> RandomizeTips(TipsVariant[] tips)
    {
		var RandomTips = new List<TipsVariant>();

        for (int i = 0; i < tips.Length; i++)
        {
			if (tips[i] == TipsVariant.UNCERTAIN)
				tips[i] = (TipsVariant)Random.Range(0, 2);

			if (tips[i]!=  TipsVariant.STRAIGHT)
				RandomTips.Add(tips[i]);
		}
			
		return RandomTips;
	}
}

