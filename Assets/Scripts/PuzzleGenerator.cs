using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// подумать над правильностью
public enum PossibleTips
{
    CAVITY = 0,
    CONVEX = 1,
    STRAIGHT = 2,
    indefinitely = 3
}


// проверить названия
// columns

public class PuzzleGenerator : MonoBehaviour
{
    public static PuzzleGenerator Instanse { get; private set; }

    [Header("Tiles Settings")]
    public int seed;
    public int ColumnsCount = 4;
    public int RowsCount = 4;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Puzzle prefabs Settings")]
    [SerializeField] private GameObject[] puzzlePrefabs;

    private PiecesCollection _piecesCollections;
    private int [,][] _matrixForGenerate;
    private GameObject[,] _GeneretedPieces;

    private List<Vector2Int> _steps => new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1)
    }; // попробовать убрать

    private void Awake()
    {
        if (Instanse != null && Instanse != this)
            Destroy(this);
        else
            Instanse = this;

        _piecesCollections = new PiecesCollection(puzzlePrefabs);

        GenerateGridPuzles();
    }

    private void GenerateGridPuzles()
    {
        PieceRotation.Init(RowsCount, ColumnsCount);
        _matrixForGenerate = new int[RowsCount, ColumnsCount][];
        _GeneretedPieces = new GameObject[RowsCount, ColumnsCount];

        InivializeArray();

        for (int y = 0; y < RowsCount; y++)
        {
            for (int x = 0; x < ColumnsCount; x++)
            {
                CheckSides(new Vector2Int(x, y));

                DebLog(_matrixForGenerate[y, x], "after"); // forCheck;

                var piecePuzzle = _piecesCollections.FindSuitablePazzle(_matrixForGenerate[y, x],
                    new Vector2Int(x, y));

                if (piecePuzzle != null)
                    _GeneretedPieces[y, x] = piecePuzzle.gameObject;

                DebLog(_matrixForGenerate[y, x], "before");// forCheck;
            }
        }

        InstatiatePuzzles();
    }

    private void DebLog(int[] arr, string z)// forCheck;
    {
        string test = "";
        for (int i = 0; i < arr.Length; i++)
        {
            test += arr[i] + " ";
        }
        Debug.Log(z + test);
    }

    private void InivializeArray()
    {
        for (var i = 0; i < RowsCount; i++)
            for (var j = 0; j < ColumnsCount; j++)
                    _matrixForGenerate[i, j] = new int[4];
    }

    private void InstatiatePuzzles()
    {
        for (var i = 0; i < RowsCount; i++)
        { 
            for (var j = 0; j < ColumnsCount; j++)
            {
                if (_GeneretedPieces[i, j] != null)
                { 
                    var currPiece = Instantiate(_GeneretedPieces[i,j].gameObject);
                    // ????????? PieceRotation.RotatePiece(currPiece, new Vector2Int(j, i));
                    currPiece.transform.position = new Vector3((j * -3f), (i * -3f), 0); //  очень рядом
                }
            }
        }
    }


    

    private void CheckSides(Vector2Int currPos)
    {
        var sideStartGenereation_x = -1; // -1 left, 1 right
        var sideStartGenereation_y = -1; // -1 left, 1 right
        var checkPrevElemTips_x = (sideStartGenereation_x == 1) ? 0 : 2;
        var checkPrevElemTips_y = (sideStartGenereation_y == 1) ? 3 : 1; // delete this

        for (var i = 0; i < _steps.Count; i++)
        {
            /// refactor
            if (currPos.x + _steps[i].x < 0 || currPos.x + _steps[i].x >= ColumnsCount ||
                currPos.y + _steps[i].y < 0 || currPos.y + _steps[i].y >= RowsCount)
                _matrixForGenerate[currPos.y, currPos.x][i] = (int)PossibleTips.STRAIGHT;

            else if (_steps[i].x == sideStartGenereation_x)
                _matrixForGenerate[currPos.y, currPos.x][i] =
                    (_matrixForGenerate[currPos.y, currPos.x + _steps[i].x] [checkPrevElemTips_x] ==
                    (int)PossibleTips.CAVITY) ? (int)PossibleTips.CONVEX : (int)PossibleTips.CAVITY;

            else if (_steps[i].y == sideStartGenereation_y)
                _matrixForGenerate[currPos.y, currPos.x] [i] =
                    (_matrixForGenerate[currPos.y + _steps[i].y, currPos.x] [checkPrevElemTips_y] ==
                    (int)PossibleTips.CAVITY) ? (int)PossibleTips.CONVEX : (int)PossibleTips.CAVITY;

            else
                _matrixForGenerate[currPos.y, currPos.x][i] =
                    (int)PossibleTips.indefinitely;
        }
    }
}
