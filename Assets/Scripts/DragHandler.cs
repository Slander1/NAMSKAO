using System;
using PieceData;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PiecePuzzle))]
public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public PiecePuzzle PiecePuzzle { get; private set; }
	private Vector3 _screenPoint;
	private Vector3 _offset;
	private Camera _camera;

	public event Action<DragHandler> OnBeginDragging;
	public event Action<DragHandler> OnDragEnd;
	public event Action<DragHandler, Vector3> OnDragging;


	private void Awake()
    {
		PiecePuzzle = GetComponent<PiecePuzzle>();
		_camera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
	{
		_screenPoint = _camera.WorldToScreenPoint(transform.position);
		_offset = transform.position - _camera.ScreenToWorldPoint(new Vector3(eventData.position.x,
			eventData.position.y, _screenPoint.z));
		OnBeginDragging?.Invoke(this);
		
	}

	public void OnDrag(PointerEventData eventData)
	{
		var cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
		var cursorPosition = _camera.ScreenToWorldPoint(cursorPoint) + _offset;
		var deltaPosition = cursorPosition - transform.position; ;
		OnDragging?.Invoke(this, deltaPosition);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		OnDragEnd?.Invoke(this);
	}
}
