using System;
using System.Collections.Generic;
using System.Linq;
using PieceData;
using PuzzleControllers;
using UnityEngine;

public class PuzzleGluer: MonoBehaviour
{
    private PiecePuzzle[] _piecePuzzles;

    private IEnumerable<PiecePuzzle> PiecesOnBoard => _piecePuzzles.Where(piece=> piece.OnBoard);
    private Vector2 _pieceDistance;

    private DragHandler[] _dragHandlers;

    private PieceStateChanger _pieceStateChanger;

    public event Action<int> OnPieceGlued;

    public void Init(PiecePuzzle[] piecePuzzles, DragHandler[] dragHandlers
        , Vector2 pieceDistance, PieceStateChanger pieceStateChanger)
    {
        _piecePuzzles = piecePuzzles;
        _pieceDistance = pieceDistance;
        _dragHandlers = dragHandlers;
        _pieceStateChanger = pieceStateChanger;

        _pieceStateChanger.OnChangeMouseOnBoard += TryGluePuzzle; 

        foreach (var dragHandler in _dragHandlers)
        {
            dragHandler.OnDragging += OnDragging;
        }
    }

    private void OnDestroy()
    {
        _pieceStateChanger.OnChangeMouseOnBoard -= TryGluePuzzle;

        if (_dragHandlers == null) return;
        
        foreach (var dragHandler in _dragHandlers)
        {
            dragHandler.OnDragging -= OnDragging;
        }
    }

    private void OnDragging(DragHandler drag, Vector3 dragPosition)
    {
        foreach (var piece in GetGroup(drag.PiecePuzzle.GroupNumber))
            piece.transform.position += dragPosition;
    }
    
    private void TryGluePuzzle(PiecePuzzle piece)
    {
        var offsets = new Dictionary<int, Vector2>();
        
        var foundGroups = new HashSet<int>();

        foreach (var item in PiecesOnBoard)
        {
            if (!item.OnBoard) continue;
            var itemPos = item.transform.localPosition;
            if (item.GroupNumber != piece.GroupNumber) continue;

            foreach (var other in PiecesOnBoard)
            {
                var otherPos = other.transform.localPosition;

                if (other.GroupNumber == piece.GroupNumber) continue;
                if (foundGroups.Contains(other.GroupNumber)) continue;
                if (!PiecesIsNear(itemPos, otherPos, out var offset, out var isXEqual)) continue;
                if (!IsPiecesNeighbours(item, other, isXEqual)) continue;
                if (!other.OnBoard) continue;

                item.IsGlued = true;
                other.IsGlued = true;
                foundGroups.Add(other.GroupNumber);
                offsets.Add(other.GroupNumber, offset);
                
            }
        }
            
        foreach (var item in _piecePuzzles)
        {
            if (!foundGroups.Contains(item.GroupNumber)) continue;
            
            item.transform.localPosition += (Vector3)offsets[item.GroupNumber];
            item.GroupNumber = piece.GroupNumber;
            var count = PiecesOnBoard.Count(piece => piece.GroupNumber == item.GroupNumber);
            OnPieceGlued?.Invoke(count);
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

        if (isXEqual)
            return firstPieceStartPosition.x == secondPieceStartPosition.x &&
                   firstPieceStartPosition.y + 1 == secondPieceStartPosition.y;
        else
            return firstPieceStartPosition.y == secondPieceStartPosition.y &&
                   firstPieceStartPosition.x + 1 == secondPieceStartPosition.x;
}

    private bool PiecesIsNear(Vector2 piecePos, Vector2 otherPos, out Vector2 offset, out bool isXEqual)
    {
        var comparisonsPositions = new Vector2[] { new(_pieceDistance.x, 0), new(0, _pieceDistance.y) };
        foreach (var comparison in comparisonsPositions)
        {
            if (!IsEqualDistance(otherPos.x, piecePos.x, comparison.x, 0.4f) ||
                !IsEqualDistance(otherPos.y, piecePos.y, comparison.y, 0.4f)) continue;
            
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
            isXEqual = comparison.x == 0; 
            return true;
        }
        offset = Vector2.zero;
        isXEqual = false;
        return false;
    }

    private IEnumerable<PiecePuzzle> GetGroup(int group)
    {
        return _piecePuzzles.Where(item => item.GroupNumber == group);
    }

    private static bool IsEqualDistance(float positionOne, float positionTwo, float requiredDistance, float equalRange)
    {
        return DeltaDistance(positionOne, positionTwo, requiredDistance) <= equalRange;
    }

    private static float DeltaDistance(float positionOne, float positionTwo, float requiredDistance)
    {
        var distance = Mathf.Abs(positionOne - positionTwo);
        return Mathf.Abs(distance - requiredDistance);
    }
}
