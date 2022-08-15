using System;
using PieceData;
using UnityEngine;
using Utils;

namespace PuzzleControllers
{
    public class PieceStateChanger
    {
        private RectTransform _containerTransform;
        private Transform _puzzleGeneratorTransform;
        private Canvas _canvas;

        private DragHandler[] _dragHandlers;

        public event Action<PiecePuzzle> OnChangeMouseOnBoard;

        public PieceStateChanger(RectTransform containerTransform, Transform puzzleGenerator,
            DragHandler[] dragHandlers, Canvas canvas)
        {
            _containerTransform = containerTransform;
            _puzzleGeneratorTransform = puzzleGenerator;
            _dragHandlers = dragHandlers;
            _canvas = canvas;
            SubscribeOnEvents();
        }

        public void OnDestroyedGameLogicController()
        {
            Unsubscribe();
        }

        private void SubscribeOnEvents()
        {
            foreach (var handler in _dragHandlers)
            {
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
            var piece = handler.PiecePuzzle;

            var onBoard = piece.IsGlued || !MouseChecker.IsMouseInsideContainer(_canvas,_containerTransform);

            piece.OnBoard = onBoard;

            piece.ElementForScroll.gameObject.SetActive(!onBoard);

            piece.transform.SetParent(onBoard ? _puzzleGeneratorTransform : piece.ElementForScroll);

            handler.transform.localScale = onBoard ? piece.ScaleOnBoard : piece.ScaleInContainer;

            if (onBoard)
                OnChangeMouseOnBoard?.Invoke(piece);
            else
                piece.transform.position = piece.ElementForScroll.position;
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
}
