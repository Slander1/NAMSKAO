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

	public class PiecePazzle : MonoBehaviour
	{
		[Header("Container")]
		public RectTransform elementForScroll;

		[Header("Starts location piece")]
		public Vector3 startPos;
		public Vector2Int posInGreed;

		[Header("")]
		public bool onInitialPos = false;
		public List<PiecePazzle> nearPieces = new List<PiecePazzle>();

		[field: SerializeField] public PieceData PieceData { get; set; }

		[Header("ScaleSetting")]
		public Vector3 scaleOnBoard;
		public Vector3 scaleInContainer;


		public static Action<PiecePazzle> PiecePazzleOnInitialPos;

		private void OnEnable()
		{
			PiecePazzleOnInitialPos += NearPiecePazzle;
		}

		private void OnDisable()
		{
			PiecePazzleOnInitialPos += NearPiecePazzle;
		}

		private bool CheckPosInGreed(Vector2Int startPos, Vector2Int checkPos)
		{
			if (Math.Abs(startPos.x - checkPos.x) == 1 && startPos.y == checkPos.y)
				return true;
			if (Math.Abs(startPos.y - checkPos.y) == 1 && startPos.x == checkPos.x)
				return true;
			
			return false;
		}

		public void GetListAtNearPieces(List<PiecePazzle> otherList, PiecePazzle newInNear)
        {
            foreach (var piece in nearPieces)
            {
				piece.AddNewPieceInChain(newInNear);
				if (!otherList.Contains(piece))
				{
					otherList.Add(piece);
				}
			}
        }

		public void AddNewPieceInChain(PiecePazzle newInNear)
        {
			if (!nearPieces.Contains(newInNear))
				nearPieces.Add(newInNear);
        }

		public void ComparePos()
        {
			if (Mathf.Abs(transform.position.x - startPos.x) < 2
				&& Mathf.Abs(transform.position.y - startPos.y) < 2)
			{
				transform.position = startPos;
				onInitialPos = true;
				PiecePazzleOnInitialPos?.Invoke(this);
				var i = 0;
				while(i < nearPieces.Count)
				{
					nearPieces[i].GetListAtNearPieces(nearPieces, this);
					i++;
				}
				GiveNewInfoToNear();
				Destroy(elementForScroll.gameObject);
			}
		}

		private void GiveNewInfoToNear()
        {
            foreach (var piece in nearPieces)
            {
				piece.GiveListToNear(nearPieces);
            }
					
		}

		public void GiveListToNear(List<PiecePazzle> otherList)
        {
            foreach (var piece in otherList)
            {
				AddNewPieceInChain(piece);
			}
        }

		private void NearPiecePazzle(PiecePazzle otherPiecPazzle)
        {
			if (CheckPosInGreed(posInGreed, otherPiecPazzle.posInGreed) &&
				otherPiecPazzle!=this && (!nearPieces.Contains(otherPiecPazzle)))
			{ 
				nearPieces.Add(otherPiecPazzle);
			}
        }
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

