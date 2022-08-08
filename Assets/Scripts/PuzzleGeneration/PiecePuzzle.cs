using System;
using System.Collections.Generic;
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
		public RectTransform elementForScroll;

		[Header("Starts location piece")]
		public Vector3 startPos;
		public Vector2Int posInGreed;

		[Header("")]
		public bool onInitialPos = false;

		[field: SerializeField] public PieceData PieceData { get; set; }

		[Header("ScaleSetting")]
		public Vector3 scaleOnBoard;
		public Vector3 scaleInContainer;

		public int groupNumber;

		public Action<PiecePuzzle> PiecePazzleOnInitialPos;

	}

	[System.Serializable]
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

