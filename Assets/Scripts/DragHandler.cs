using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField] private PuzzleGeneration.PiecePazzle piecePazzle;

	private Vector3 _screenPoint;
	private Vector3 _offset;
	private bool _inContainer = true;


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
		if (piecePazzle.onInitialPos)
		{
			var scale = transform.localScale;
			foreach (var piece in piecePazzle.nearPieces)
			{
				piece.transform.position = cursorPosition +
					new Vector3(3 * piece.posInGreed.x * scale.x, -3 *
					piece.posInGreed.y * scale.y, 0f);
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (piecePazzle.onInitialPos)
		{
			foreach (var piece in piecePazzle.nearPieces)
			{
				piece.transform.position = piece.startPos;
			}
		}
		if (piecePazzle.onInitialPos)
		{
			transform.position = piecePazzle.startPos;
			return;
		}

		if (transform.position.x > 21.5f)
			_inContainer = true;
		else
				piecePazzle.ComparePos();
		transform.localScale = (_inContainer) ? piecePazzle.scaleInContainer : piecePazzle.scaleOnBoard;

	}

	private void Update()
	{
		if (_inContainer && piecePazzle.elementForScroll)
			transform.position = piecePazzle.elementForScroll.position;
	}
}
