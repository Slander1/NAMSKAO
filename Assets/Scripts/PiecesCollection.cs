using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PiecesCollection
{
	private List<PiecePazzle> _cornerCollection = new List<PiecePazzle>();
	private List<PiecePazzle> _edgeCollection = new List<PiecePazzle>();
	private List<PiecePazzle> _centerCollection = new List<PiecePazzle>();

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
	}


	public PiecePazzle FindSuitablePazzle(int[] tips)
	{
		UnityEngine.Random.InitState(PuzzleGenerator.Instanse.seed);
		// подумать где лучше проиницилизовать, возможно в конструкторе
		var randomizeTips = RandomizeTips(tips);
		List<PiecePazzle> collection = null;

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
				tips[i] = UnityEngine.Random.Range((int)PossibleTips.CONVEX, (int)PossibleTips.CAVITY);
			if (tips[i]!= (int) PossibleTips.STRAIGHT)
				RandomTips.Add(tips[i]);
		}
			
		return RandomTips;
	}
}

