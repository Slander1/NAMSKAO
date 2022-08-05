using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;


//  возможно стоит сделать статическим
public class PiecesCollection
{
	private readonly List<PiecePazzle> _cornerCollection = new();
	private readonly List<PiecePazzle> _edgeCollection = new();
	private readonly List<PiecePazzle> _centerCollection = new();

	public PiecesCollection(GameObject[] piecesPrefabs)
	{
		foreach (var piecePrefab in piecesPrefabs)
		{
			piecePrefab.TryGetComponent<PiecePazzle>(out var pieceData);

			if (pieceData == null)
				continue;

            switch (pieceData.namePos)
            {
				case NamePos.CORNER:
					_cornerCollection.Add(pieceData);
					break;

				case NamePos.EDGE:
					_edgeCollection.Add(pieceData);
					break;

				case NamePos.CENTER:
					_centerCollection.Add(pieceData);
					break;

				default:
                    break;
            }
		}
		// не работает InitState;
		Random.InitState(PuzzleGenerator.Instanse.seed);
	}


	public PiecePazzle FindSuitablePazzle(int[] tips, Vector2Int pos)
	{
		PieceRotation.ShiftTips(pos, tips); //Проверять на центр

		var randomizeTips = RandomizeTips(tips);

		List<PiecePazzle> collection = null;
		// переработать на систему по позициям
		switch (randomizeTips.Count)
		{
			case 2:
				collection = _cornerCollection;
				break;

			case 3:
				collection = _edgeCollection;
				break;

			case 4:
				collection = _centerCollection;
				break;

			default:
				break;
		}
		
		foreach (var piece in collection)
		{
			if (Enumerable.SequenceEqual(randomizeTips, piece.tipsPiece))
				return piece;
		}

		return null;
	}

	private List<int> RandomizeTips(int[] tips)
    {
		var RandomTips = new List<int>();

        for (int i = 0; i < tips.Length; i++)
        {
			if (tips[i] == (int)PossibleTips.indefinitely)
				tips[i] = Random.Range((int)PossibleTips.CAVITY, (int)PossibleTips.CONVEX + 1);

			if (tips[i]!= (int) PossibleTips.STRAIGHT)
				RandomTips.Add(tips[i]);
		}
			
		return RandomTips;
	}
}

