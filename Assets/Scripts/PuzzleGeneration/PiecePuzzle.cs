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
		[field: Header("Container")]
		public RectTransform ElementForScroll { get; set; }
		public Vector3 StartPos { get; set; }
		public Vector2Int PosInGreed { get; set; }

		public bool OnBoard { get; set; }

		[field: SerializeField] public PieceData PieceData { set; get; }

		[field: Header("ScaleSetting")]
		public Vector3 ScaleOnBoard { get; set; }
		public Vector3 ScaleInContainer { get; set; }
		public int GroupNumber { get; set; }

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

