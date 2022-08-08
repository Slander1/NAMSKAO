using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleGeneration;
using UnityEngine;

public class PuzzleGluer: MonoBehaviour
{
    private PiecePuzzle[,] _piecePuzzles;


    private List<PiecePuzzle> _puzzleOnInitPos = new List<PiecePuzzle>();

    public event Action<int> OnPieceMovedToPosition;

    private void OnDestroy()
    {
        foreach (var piece in _piecePuzzles)
        {
            piece.PiecePazzleOnInitialPos -= CheckStatePuzzle;
            var dragHandler = piece.GetComponent<DragHandler>();
            dragHandler.OnDragEnd -= OnEndDrag;
            dragHandler.OnDragging -= OnDraggig;
        }
    }

    public void Init(PiecePuzzle[,] piecePuzzles)
    {
        _piecePuzzles = piecePuzzles;
        foreach (var item in _piecePuzzles)
        {
            var dragHandler = item.GetComponent<DragHandler>();
            item.PiecePazzleOnInitialPos += CheckStatePuzzle;
            dragHandler.OnDragEnd += OnEndDrag;
            dragHandler.OnDragging += OnDraggig;
        }
    }
    private void OnDraggig(DragHandler drag, Vector3 dragPosition)
    {
        if (drag.PiecePuzzle.onInitialPos)
        {
            var scale = transform.localScale;
            foreach (var piece in GetGroup(drag.PiecePuzzle.groupNumber))
            {
                piece.transform.position = dragPosition +
                    new Vector3(piece.posInGreed.x * scale.x,
                    piece.posInGreed.y * -scale.y, 0f);
            }
        }
    }

    private void OnEndDrag(DragHandler drag)
    {
        var piece = drag.PiecePuzzle;
        if (piece.onInitialPos)
        {
            foreach (var item in GetGroup(piece.groupNumber))
            {
                item.transform.position = item.startPos;
            }
            transform.position = piece.startPos;
            return;
        }

        if (transform.position.x > 21.5f)
            drag.InContainer = true;
        else
            ComparePos(piece);
    }


    public void ComparePos(PiecePuzzle piece)
    {
        if (Mathf.Abs(piece.transform.position.x - piece.startPos.x) < 2
            && Mathf.Abs(piece.transform.position.y - piece.startPos.y) < 2)
        {
            piece.transform.position = piece.startPos;
            piece.onInitialPos = true;
            var findedGroups = new HashSet<int>();
            foreach (var other in _puzzleOnInitPos)
            {
                if ((Mathf.Abs(other.posInGreed.x - piece.posInGreed.x) == 1
                    && other.posInGreed.y == piece.posInGreed.y)
                    || (Mathf.Abs(other.posInGreed.y - piece.posInGreed.y) == 1
                    && other.posInGreed.x == piece.posInGreed.x))
                {
                    findedGroups.Add(other.groupNumber);
                }

            }
            foreach (var item in _piecePuzzles)
            {
                if (findedGroups.Contains(item.groupNumber))
                    item.groupNumber = piece.groupNumber;
            }
            Destroy(piece.elementForScroll.gameObject);
            piece.PiecePazzleOnInitialPos?.Invoke(piece);
        }
    }

    public IEnumerable<PiecePuzzle> GetGroup(int group)
    {
        foreach (var item in _piecePuzzles)
        {
            if (item.groupNumber == group)
                yield return item;
        }
    }


    private void CheckStatePuzzle(PiecePuzzle piecePuzzle)
    {
        _puzzleOnInitPos.Add(piecePuzzle);
        OnPieceMovedToPosition.Invoke(_puzzleOnInitPos.Count);

    }
}
