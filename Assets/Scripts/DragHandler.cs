using System;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PiecePuzzle))]
public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public PiecePuzzle PiecePuzzle { get; private set; }
	public bool InContainer { get; set; } = true;
	private Vector3 _screenPoint;
	private Vector3 _offset;

	public event Action<DragHandler> OnDragEnd;
	public event Action<DragHandler, Vector3> OnDragging;

	private void Awake()
    {
		PiecePuzzle = GetComponent<PiecePuzzle>();
	}

    public void OnBeginDrag(PointerEventData eventData)
	{
		InContainer = false;
		_screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		_offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x,
			eventData.position.y, _screenPoint.z));
	}

	public void OnDrag(PointerEventData eventData)
	{
		var cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
		var cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
		transform.position = cursorPosition;
		OnDragging?.Invoke(this, cursorPosition);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		OnDragEnd?.Invoke(this);
		transform.localScale = (InContainer) ? PiecePuzzle.scaleInContainer : PiecePuzzle.scaleOnBoard;
	}

	private void Update()
	{
		if (InContainer && PiecePuzzle.elementForScroll)
			transform.position = PiecePuzzle.elementForScroll.position;
	}
}
