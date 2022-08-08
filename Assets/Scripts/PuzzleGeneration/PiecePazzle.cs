using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PuzzleGeneration
{
	public enum NamePos
	{
		CORNER = 0,
		EDGE = 1,
		CENTER = 2
	}

	public class PiecePazzle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		public RectTransform elementForScroll;
		public Vector3 startPos;
		public Vector2Int posInGreed;
		public bool onInitialPos = false;

		[field: SerializeField] public PieceData PieceData { get; set; }

		[Header("ScaleSetting")]
		public Vector3 scaleOnBoard;
		public Vector3 scaleInContainer;


		public static Action<PiecePazzle> PiecePazzleOnInitialPos;

		private Vector3 _screenPoint;
		private Vector3 _offset;
		private bool _inContainer = true;

		private List<PiecePazzle> _nearPieces = new List<PiecePazzle>();
		private List<PiecePazzle> _allPuzzles = new List<PiecePazzle>();


		private void OnEnable()
		{
			PiecePazzleOnInitialPos += NearPiecePazzle;
		}

		private void OnDisable()
		{
			PiecePazzleOnInitialPos += NearPiecePazzle;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_inContainer = false;
			_screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			_offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x,
				eventData.position.y, _screenPoint.z));
		}

		public void OnDrag(PointerEventData eventData)
		{
			var cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
			var cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
			transform.position = cursorPosition;
			if (onInitialPos)
			{
				var scale = transform.localScale;
				foreach (var piece in _nearPieces)
				{
					piece.transform.position = cursorPosition +
						new Vector3(3 * piece.posInGreed.x * scale.x, -3 *
						piece.posInGreed.y * scale.y, 0f);
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (onInitialPos)
			{
				foreach (var piece in _nearPieces)
				{
					piece.transform.position = piece.startPos;
				}
			}
			if (onInitialPos)
			{ 				
				transform.position = startPos;
				return;
			}
			
			if (transform.position.x > 21.5f)
				_inContainer = true;
			else
				ComparePos();
			transform.localScale = (_inContainer) ? scaleInContainer : scaleOnBoard;
			
		}


        private void Update()
        {
			if (_inContainer && elementForScroll)
				transform.position = elementForScroll.position;
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
            foreach (var piece in _nearPieces)
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
			if (!_nearPieces.Contains(newInNear))
				_nearPieces.Add(newInNear);
        }

		private void ComparePos()
        {
			if (Mathf.Abs(transform.position.x - startPos.x) < 2
				&& Mathf.Abs(transform.position.y - startPos.y) < 2)
			{
				transform.position = startPos;
				onInitialPos = true;
				PiecePazzleOnInitialPos?.Invoke(this);
				var i = 0;
				while(i < _nearPieces.Count)
				{
					_nearPieces[i].GetListAtNearPieces(_nearPieces, this);
					i++;
				}
				GiveNewInfoToNear();
				Destroy(elementForScroll.gameObject);
			}

		}

		private void GiveNewInfoToNear()
        {
			var i = 0;
			while (i < _nearPieces.Count)
			{
				if (CheckPosInGreed(posInGreed, _nearPieces[i].posInGreed))
					_nearPieces[i].GiveListToNear(_nearPieces);
				i++;
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
				otherPiecPazzle!=this && (!_nearPieces.Contains(otherPiecPazzle)))
			{ 
				_nearPieces.Add(otherPiecPazzle);
			}
			else
				_allPuzzles.Add(otherPiecPazzle);
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

