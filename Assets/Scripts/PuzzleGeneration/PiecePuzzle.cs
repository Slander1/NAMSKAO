using System;
using UnityEngine;

namespace PuzzleGeneration
{
    public enum NamePos
	{
		CORNER = 0,
		EDGE = 1,
		CENTER = 2
	}

	public class PiecePuzzle : MonoBehaviour
	{
		[Header("Container")]
		[NonSerialized] public RectTransform elementForScroll;

		[Header("Starts location piece")]
		public Vector3 startPos;
		public Vector2Int posInGreed;

		[Header("")]
		public bool onBoard;

		[field: SerializeField] public PieceData PieceData { set; get; }

		[Header("ScaleSetting")]
		public Vector3 scaleOnBoard;
		public Vector3 scaleInContainer;

		public int groupNumber;

		public event Action<PiecePuzzle> PiecePuzzleOnInitialPos;

		public PiecePuzzle(PieceData pieceData)
		{
			PieceData = pieceData;
		}
	}

	[Serializable]
	public class PieceData
	{
		public NamePos namePos;
		public TipsVariant[] tipsPiece;

		public PieceData(NamePos namePos, TipsVariant[] tipsPiece)
		{
			this.namePos = namePos;
			this.tipsPiece = tipsPiece;
		}
	}
}

