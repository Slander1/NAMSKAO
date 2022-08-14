using System;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PiecePuzzle))]
public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public PiecePuzzle PiecePuzzle { get; private set; }
	private Vector3 _screenPoint;
	private Vector3 _offset;
	private Camera _camera;

	//public event Action<DragHandler> OnBeginDrag;
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
		//transform.localScale = PiecePuzzle.ScaleOnBoard;
	}

	public void OnDrag(PointerEventData eventData)
	{
		var cursorPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
		var cursorPosition = _camera.ScreenToWorldPoint(cursorPoint) + _offset;
		var deltaPosition = cursorPosition - transform.position; 
		//transform.position = cursorPosition;
		OnDragging?.Invoke(this, deltaPosition);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		OnDragEnd?.Invoke(this);
		transform.localScale = PiecePuzzle.OnBoard ? PiecePuzzle.ScaleOnBoard : PiecePuzzle.ScaleInContainer;
		// ^ Ответсвенность другого класса
		//transform.SetParent(PiecePuzzle.onBoard ? );
		if (!PiecePuzzle.OnBoard)
			transform.position = PiecePuzzle.ElementForScroll.position;
	}
}
