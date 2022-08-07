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
		public int posInGreed;

		[field: SerializeField] public PieceData PieceData { get; set; }

		[Header("ScaleSetting")]
		public Vector3 scaleOnBoard;
		public Vector3 scaleInContainer;

		private Vector3 _screenPoint;
		private Vector3 _offset;
		private bool _inContainer = true;
		private bool _onInitialPos = false;


        public void OnBeginDrag(PointerEventData eventData)
		{
			_inContainer = false;
			_screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			_offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x,
				eventData.position.y, _screenPoint.z));
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector3 cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
			Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
			transform.position = cursorPosition;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			Debug.Log(transform.position.x);
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
				transform.position = startPos;
			else
				Debug.Log(Mathf.Abs(transform.position.x - startPos.x) + " " +
					Mathf.Abs(transform.position.x - startPos.x));
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

