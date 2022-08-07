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
		public Vector3 startPosition;

		[field: SerializeField] public PieceData PieceData { get; set; }

		private Vector3 _screenPoint;
		private Vector3 _offset;
		private bool _inContainer;


		public void OnBeginDrag(PointerEventData eventData)
		{
			_inContainer = true;
			_screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			_offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z));
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector3 cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
			Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
			transform.position = cursorPosition;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_inContainer = false;
		}

        private void Update()
        {
			if (!_inContainer && elementForScroll)
				transform.position = elementForScroll.position;
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

