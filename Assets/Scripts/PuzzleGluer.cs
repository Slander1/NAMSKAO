using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleGeneration;
using UnityEngine;

public class PuzzleGluer: MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    
    private PiecePuzzle[] _piecePuzzles;

    private IEnumerable<PiecePuzzle> PiecesOnBoard => _piecePuzzles.Where(piece=> piece.OnBoard);
    private Vector2 _pieceDistance;

    private DragHandler[] _dragHandlers;
    private RectTransform _containerTransform;

    public event Action<int> OnPieceMovedToPosition;

    public event Action<bool, PiecePuzzle> OnChangeMauseOnBoard;


    public void Init(PiecePuzzle[] piecePuzzles, DragHandler[] dragHandlers
        , Vector2 pieceDistance, RectTransform rectTransform)
    {
        _piecePuzzles = piecePuzzles;
        _pieceDistance = pieceDistance;
        _containerTransform = rectTransform;
        _dragHandlers = dragHandlers;

        foreach (var item in _piecePuzzles)
            item.PiecePuzzleOnInitialPos += CheckStatePuzzle;


        foreach (var dragHandler in _dragHandlers)
        {
            dragHandler.OnDragEnd += OnEndDrag;
            dragHandler.OnDragging += OnDragging;
        }
    }

    private void OnDestroy()
    {
        if (_piecePuzzles != null)
            foreach (var item in _piecePuzzles)
                item.PiecePuzzleOnInitialPos += CheckStatePuzzle;

        if (_dragHandlers != null)
        { 
            foreach (var dragHandler in _dragHandlers)
            {
                dragHandler.OnDragEnd += OnEndDrag;
                dragHandler.OnDragging += OnDragging;
            }
        }
    }

    private void OnDragging(DragHandler drag, Vector3 dragPosition)
    {
        foreach (var piece in GetGroup(drag.PiecePuzzle.GroupNumber))
            piece.transform.position += dragPosition;
    }

    private void OnEndDrag(DragHandler drag)
    {
        var piece = drag.PiecePuzzle;
        piece.ElementForScroll.gameObject.SetActive(!drag.PiecePuzzle.OnBoard);

        var isMouseOnBoard = !IsMouseInsideContainer();

        if (isMouseOnBoard)
            TryGluePuzzle(piece);

        drag.PiecePuzzle.OnBoard = isMouseOnBoard;
        OnChangeMauseOnBoard?.Invoke(isMouseOnBoard, drag.PiecePuzzle);
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
        var offsets = new Dictionary<int, Vector2>();
        
        var findedGroups = new HashSet<int>();

        foreach (var item in PiecesOnBoard)
        {
            var itemPos = item.transform.localPosition;
            if(item.GroupNumber != piece.GroupNumber) continue;
            
            foreach (var other in PiecesOnBoard)
            {
                var otherPos = other.transform.localPosition;

                if(other.GroupNumber == piece.GroupNumber) continue;
                if (findedGroups.Contains(other.GroupNumber)) continue;
                if (!PiecesIsNear(itemPos, otherPos, out var offset, out var isXEqual)) continue;
                if (!IsPiecesNeighbours(item, other, isXEqual)) continue;

                findedGroups.Add(other.GroupNumber);
                offsets.Add(other.GroupNumber, offset);
            }
        }

        piece.OnBoard = true;   
            
        foreach (var item in _piecePuzzles)
        {
            if (!findedGroups.Contains(item.GroupNumber)) continue;
            
            item.transform.localPosition += (Vector3)offsets[item.GroupNumber];
            item.GroupNumber = piece.GroupNumber;
        }
        
    }

    private bool IsPiecesNeighbours(PiecePuzzle piece, PiecePuzzle other, bool isXEqual)
    {
        Vector2Int firstPieceStartPosition;
        Vector2Int secondPieceStartPosition;
        if ((!isXEqual && (piece.transform.position.x > other.transform.position.x))
            || (isXEqual && piece.transform.position.y > other.transform.position.y))
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
        var comparisonsPositions = new Vector2[] { new(_pieceDistance.x, 0), new(0, _pieceDistance.y) };
        foreach (var comparison in comparisonsPositions)
        {
            if (IsEqualDistance(otherPos.x, piecePos.x, comparison.x, 0.9f)
                && IsEqualDistance(otherPos.y, piecePos.y, comparison.y, 0.9f))
            {
                var distanceX = DeltaDistance(otherPos.x, piecePos.x, comparison.x);
                var distanceY = DeltaDistance(otherPos.y, piecePos.y, comparison.y);
                int signX;
                int signY;
                if (comparison.x == 0f)
                {
                    signX = (Mathf.Abs(otherPos.x - piecePos.x) > comparison.x) == (otherPos.x < piecePos.x) ? 1 : -1;
                    signY = (Mathf.Abs(otherPos.y - piecePos.y) > comparison.y) == (otherPos.y < piecePos.y) ? 1 : -1;
                }
                else
                {
                    signX = (Mathf.Abs(otherPos.x - piecePos.x) > comparison.x) == (otherPos.x < piecePos.x) ? 1 : -1;
                    signY = (Mathf.Abs(otherPos.y - piecePos.y) > comparison.y) == (otherPos.y < piecePos.y) ? 1 : -1;
                }

                offset.x = distanceX * signX;
                offset.y = distanceY * signY;
               //offset.x = comparison.x - (otherPos.x - piecePos.x);
               //offset.y = comparison.y - (otherPos.y - piecePos.y);
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
