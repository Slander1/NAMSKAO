using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PossibleTips
{
    CONVEX = 0,
    CAVITY = 1,
    STRAIGHT = 2,
    indefinitely = 3
}

public class PuzzleGenerator : MonoBehaviour
{
    public static PuzzleGenerator Instanse { get; private set; } // подумать по поводу
    //instance

    [Header("Tiles Settings")]
    public int seed;
    [SerializeField] private int Columns = 4;
    [SerializeField] private int Rows = 4;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Puzzle prefabs Settings")]
    [SerializeField] private GameObject[] puzzlePrefabs;

    private const int possibleSides = 4; // подумать над названиаем;

    private PiecesCollection _piecesCollections;
    private int[,,] _matrixForGenerate;
    private GameObject[,] _GeneretedPieces;

    private List<Vector2Int> _steps => new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1)
    };

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
        _matrixForGenerate = new int[Rows, Columns, possibleSides];
        _GeneretedPieces = new GameObject[Rows, Columns];
        InivializeArray(); // потом заменить на нормальную инициализацю.

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                CheckSides(new Vector2Int(i, j));
                var tempary = new List<int>(); // заменить структуру
                string deblog = "";
                for (int k = 0; k < possibleSides; k++)
                {
                    tempary.Add(_matrixForGenerate[i, j, k]);
                    deblog += _matrixForGenerate[i, j, k].ToString() + " ";
                }
                
                var piecePuzzle = _piecesCollections.FindSuitablePazzle(tempary); //FIX THIS
                if (piecePuzzle != null)
                    _GeneretedPieces[i, j] = piecePuzzle.gameObject;
            }
        }
        InstatiatePuzzles();
    }

    private void InivializeArray()
    {
        for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                for (var k = 0; k < possibleSides; k++)
                    _matrixForGenerate[i, j, k] = -1;
    }

    private void InstatiatePuzzles()
    {
        for (var i = 0; i < Rows; i++)
        { 
            for (var j = 0; j < Columns; j++)
            {
                if (_GeneretedPieces[i, j] != null)
                { 
                    var currPiece = Instantiate(_GeneretedPieces[i,j].gameObject);
                    currPiece.transform.position = new Vector3((i * 10), (j * 10), 0);
                }
            }
        }
    }


    

    private void CheckSides(Vector2Int currPos)
    {
        var sideStartGenereation_x = -1; // -1 left, 1 right
        var sideStartGenereation_y = -1; // -1 left, 1 right
        var checkPrevElemTips_x = (sideStartGenereation_x == 1) ? 0 : 2;
        var checkPrevElemTips_y = (sideStartGenereation_y == 1) ? 3 : 1;

        for (var i = 0; i<_steps.Count; i++)
        {
            if (currPos.x + _steps[i].x < 0 || currPos.x + _steps[i].x >= Columns ||
                currPos.y + _steps[i].y < 0 || currPos.y + _steps[i].y >= Rows)
                _matrixForGenerate[currPos.x, currPos.y, i] = (int)PossibleTips.STRAIGHT;

            else if (_steps[i].x == sideStartGenereation_x)
                _matrixForGenerate[currPos.x, currPos.y, i] =
                    (_matrixForGenerate[currPos.x + _steps[i].x, currPos.y, checkPrevElemTips_x] ==
                    (int)PossibleTips.CAVITY) ? (int)PossibleTips.CONVEX : (int)PossibleTips.CAVITY;

            else if (_steps[i].y == sideStartGenereation_y)
                _matrixForGenerate[currPos.x, currPos.y, i] =
                    (_matrixForGenerate[currPos.x, currPos.y + _steps[i].y, checkPrevElemTips_y] ==
                    (int)PossibleTips.CAVITY) ? (int)PossibleTips.CONVEX : (int)PossibleTips.CAVITY;

            else
                _matrixForGenerate[currPos.x, currPos.y, i] =
                    (int)PossibleTips.indefinitely;
        }
    }
}
