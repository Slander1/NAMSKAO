using System;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

public class PuzzleParent
{
    private RectTransform _containerTransform;
    private Transform _puzzleGeneratorTransform;
    private Canvas _canvas;

    private DragHandler[] _dragHandlers;

    public event Action<PiecePuzzle> OnChangeMauseOnBoard;

    public PuzzleParent(RectTransform containerTransform, Transform puzzleGenerator,
        DragHandler[] dragHandlers, Canvas canvas)
    {
        _containerTransform = containerTransform;
        _puzzleGeneratorTransform = puzzleGenerator;
        _dragHandlers = dragHandlers;
        _canvas = canvas;
        SubscribeOnEvents();
    }

    public void OnDestroyGameLogicContoller()
    {
        Unsubscribe();
    }

    private void SubscribeOnEvents()
    {
        foreach (var handler in _dragHandlers)
        {
            handler.PiecePuzzle.OnBoard = true;
            handler.OnBeginDragging += OnBeginDrag;

            handler.OnDragEnd += OnEndDrag;
        }
    }

    private void OnBeginDrag(DragHandler handler)
    {
        var transform = handler.transform;
        transform.SetParent(_puzzleGeneratorTransform);
        transform.localScale = handler.PiecePuzzle.ScaleOnBoard;
    }

    private void OnEndDrag(DragHandler handler)
    {
        var onBoard = !MousePosOnBoard.IsMouseInsideContainer(_canvas,
            _containerTransform);

        var piece = handler.PiecePuzzle;
        handler.transform.localScale = piece.OnBoard ?
        piece.ScaleOnBoard : piece.ScaleInContainer;

        piece.ElementForScroll.gameObject.SetActive(!piece.OnBoard);
        piece.OnBoard = onBoard;

        if (onBoard)
            OnChangeMauseOnBoard?.Invoke(piece);

        piece.transform.SetParent(onBoard ? _puzzleGeneratorTransform : _containerTransform);
    }

    private void Unsubscribe()
    {
        foreach (var handler in _dragHandlers)
        {
            handler.OnBeginDragging -= OnBeginDrag;
            handler.OnDragEnd -= OnEndDrag;
        }
    }
}
