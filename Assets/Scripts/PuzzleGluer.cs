using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleGeneration;
using UnityEngine;

public class PuzzleGluer: MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    
    private PiecePuzzle[] _piecePuzzles;

    private RectTransform _containerTransform;
    private Transform _puzzleGeneratorTransform;

    private IEnumerable<PiecePuzzle> PiecesOnBoard => _piecePuzzles.Where(piece=> piece.OnBoard);
    private Vector2 _pieceDistance;

    public event Action<int> OnPieceMovedToPosition;

    private void OnDestroy()
    {
        if (_piecePuzzles == null) return;
        foreach (var piece in _piecePuzzles)
        {
            piece.PiecePuzzleOnInitialPos -= CheckStatePuzzle;
            var dragHandler = piece.GetComponent<DragHandler>();
            // возможно TryGetComponent
            dragHandler.OnDragEnd -= OnEndDrag;
            dragHandler.OnDragging -= OnDragging;
        }
    }

    public void Init(PiecePuzzle[] piecePuzzles, Vector2 pieceDistance, RectTransform containerTransform,
        Transform puzzleGeneratorTransform)
    {
        _piecePuzzles = piecePuzzles;
        _pieceDistance = pieceDistance;
        _containerTransform = containerTransform;
        _puzzleGeneratorTransform = puzzleGeneratorTransform;

        foreach (var item in _piecePuzzles)
        {
            var dragHandler = item.GetComponent<DragHandler>();
            item.PiecePuzzleOnInitialPos += CheckStatePuzzle;
            dragHandler.OnDragEnd += OnEndDrag;
            dragHandler.OnDragging += OnDragging;
        }
    }
    
    private void OnDragging(DragHandler drag, Vector3 dragPosition)
    {
        if (!drag.PiecePuzzle.OnBoard) return;
        
        var scale = transform.localScale;
        foreach (var piece in GetGroup(drag.PiecePuzzle.GroupNumber))
        {
            piece.transform.position = dragPosition +
                                       new Vector3(piece.PosInGreed.x * scale.x,
                                           piece.PosInGreed.y * -scale.y, 0f);
        }
    }

    private void OnEndDrag(DragHandler drag)
    {
        var piece = drag.PiecePuzzle;
        piece.ElementForScroll.gameObject.SetActive(!drag.PiecePuzzle.OnBoard);
        //if (drag.PiecePuzzle.OnBoard)
        //{
        //    foreach (var item in GetGroup(piece.GroupNumber))
        //    {
        //        item.transform.position = item.StartPos;
        //    }
        //    transform.position = piece.StartPos;
        //    return;
        //}

        var isMouseOnBoard = !IsMouseInsideContainer();
        if (isMouseOnBoard)
            TryGluePuzzle(piece);
        drag.PiecePuzzle.OnBoard = isMouseOnBoard;
        drag.transform.SetParent(drag.PiecePuzzle.OnBoard ? _puzzleGeneratorTransform : _containerTransform);
    }
    
    private bool IsMouseInsideContainer()
    {
        var mousePosition = Input.mousePosition;
        var canvasRect = ((RectTransform)canvas.transform).rect;
        var normalizedMousePosition = new Vector2(mousePosition.x / Screen.width * canvasRect.width, mousePosition.y / Screen.height * canvasRect.height);
        var (squareMin, squareMax) = GetRectTransformSquare(_containerTransform);
        Debug.Log(normalizedMousePosition + " " + _containerTransform.offsetMin + " " + _containerTransform.offsetMax);
        
        return normalizedMousePosition.x > squareMin.x &&
               normalizedMousePosition.x < squareMax.x &&
               normalizedMousePosition.y > squareMin.y &&
               normalizedMousePosition.y < squareMax.y;
    }

    private (Vector2 from, Vector2 to) GetRectTransformSquare(RectTransform rectTransform)
    {
        var from = rectTransform.anchorMin * ((RectTransform)canvas.transform).rect.size;
        var to = rectTransform.anchorMax * ((RectTransform)canvas.transform).rect.size;

        if (rectTransform.rect.x < 0)
            from += Vector2.right * rectTransform.rect.x;
        else
            to += Vector2.right * rectTransform.rect.x;
        
        if (rectTransform.rect.y < 0)
            from += Vector2.up * rectTransform.rect.y;
        else
            to += Vector2.up * rectTransform.rect.y;

        return (from, to);
    }
    
    private void TryGluePuzzle(PiecePuzzle piece)
    {
        var offsets = new List<(int groupNumber, Vector2 offset)>();
        var piecePos = piece.transform.position;
        
        piece.OnBoard = true;
        var findedGroups = new HashSet<int>();
        foreach (var other in PiecesOnBoard)
        {
            var otherPos = other.transform.position;
            
            if (findedGroups.Contains(other.GroupNumber)) continue;
            if (!PiecesIsNear(piecePos, otherPos, out var offset, out var isXEqual)) continue;
            if (!IsPiecesNeighbours(piece, other, isXEqual)) continue;
            
            findedGroups.Add(other.GroupNumber);
            offsets.Add((other.GroupNumber, offset));
        }
            
        foreach (var item in _piecePuzzles)
        {
            if (!findedGroups.Contains(item.GroupNumber)) continue;
            
            item.GroupNumber = piece.GroupNumber;
            item.transform.position += (Vector3)offsets[item.GroupNumber].offset;
        }
        
    }

    private bool IsPiecesNeighbours(PiecePuzzle piece, PiecePuzzle other, bool isXEqual)
    {
        Vector2Int firstPieceStartPosition;
        Vector2Int secondPieceStartPosition;
        if (isXEqual && (piece.transform.position.x < other.transform.position.x)
            || (!isXEqual && piece.transform.position.y > other.transform.position.y))
        {
            firstPieceStartPosition = piece.PosInGreed;
            secondPieceStartPosition = other.PosInGreed;
        }
        else
        {
            secondPieceStartPosition = piece.PosInGreed;
            firstPieceStartPosition = other.PosInGreed;
        }

        return (firstPieceStartPosition.x == secondPieceStartPosition.x &&
                firstPieceStartPosition.y + 1 == secondPieceStartPosition.y)
               || (firstPieceStartPosition.y == secondPieceStartPosition.y &&
                   firstPieceStartPosition.x + 1 == secondPieceStartPosition.x);
    }

    private bool PiecesIsNear(Vector2 piecePos, Vector2 otherPos, out Vector2 offset, out bool isXEqual)
    {
        var comparisonsPositions = new Vector2[] { new(_pieceDistance.x * 3, 0), new(0, _pieceDistance.y * 3) };
        foreach (var comparison in comparisonsPositions)
        {
            if (IsEqualDistance(otherPos.x, piecePos.x, comparison.x, 0.5f)
                && IsEqualDistance(otherPos.y, piecePos.y, comparison.y, 0.5f))
            {
                offset.x = DeltaDistance(otherPos.x, piecePos.x, comparison.x);
                offset.y = DeltaDistance(otherPos.y, piecePos.y, comparison.y);
                isXEqual = comparison.x == 0; 
                return true;
            }
        }
        offset = Vector2.zero;
        isXEqual = false;
        return false;
    }

    private IEnumerable<PiecePuzzle> GetGroup(int group)
    {
        return _piecePuzzles.Where(item => item.GroupNumber == group);
    }


    private void CheckStatePuzzle(PiecePuzzle piecePuzzle)
    {
        OnPieceMovedToPosition?.Invoke(PiecesOnBoard.Count());
    }

    private bool IsEqualDistance(float positionOne, float positionTwo, float requiredDistance, float equalRange)
    {
        return DeltaDistance(positionOne, positionTwo, requiredDistance) <= equalRange;
    }

    private float DeltaDistance(float positionOne, float positionTwo, float requiredDistance)
    {
        var distance = Mathf.Abs(positionOne - positionTwo);
        return Mathf.Abs(distance - requiredDistance);
    }
}
