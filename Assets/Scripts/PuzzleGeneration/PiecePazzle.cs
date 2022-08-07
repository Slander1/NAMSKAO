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

		private List<PiecePazzle> _thisOnInitialPosNearPuzzles;
		private List<PiecePazzle> _thisNotOnInitialPosNearPuzzles = new List<PiecePazzle>();


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
				foreach (var piece in _thisOnInitialPosNearPuzzles)
				{
					piece.transform.position = cursorPosition +
						new Vector3(3 * piece.posInGreed.x * scale.x + 2, -3 * piece.posInGreed.y * scale.y + 3, 0);
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (onInitialPos)
			{
				foreach (var piece in _thisOnInitialPosNearPuzzles)
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


		private void ComparePos()
        {
			if (Mathf.Abs(transform.position.x - startPos.x) < 2
				&& Mathf.Abs(transform.position.y - startPos.y) < 2)
			{
				transform.position = startPos;
				onInitialPos = true;
				_thisOnInitialPosNearPuzzles = _thisNotOnInitialPosNearPuzzles;
				PiecePazzleOnInitialPos?.Invoke(this);
				Destroy(elementForScroll.gameObject);
			}

		}

		private void NearPiecePazzle(PiecePazzle otherPiecPazzle)
        {
			if (onInitialPos)
				_thisOnInitialPosNearPuzzles.Add(otherPiecPazzle);
			else
				_thisNotOnInitialPosNearPuzzles.Add(otherPiecPazzle);

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

