using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// подумать над правильностью
public enum PossibleTips
{
    CAVITY = 0,
    CONVEX = 1,
    STRAIGHT = 2,
    UNCERTAIN = 3
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
    [SerializeField] private PiecePazzle[] puzzlePrefabs;

    private PiecesCollection _piecesCollections;
    
    //private PiecePazzle[,] _GeneretedPieces; // переделать в массив PieceCollection

    private List<Vector2Int> _steps => new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 0),
        new Vector2Int(0, 0)
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
        var randomEdgesHorizaontal = new bool[RowsCount, ColumnsCount - 1]; //  переиминовать
        var randomEdgesVertical = new bool[RowsCount - 1, ColumnsCount];

        //_GeneretedPieces = new PiecePazzle[RowsCount, ColumnsCount];

        for (int y = 0; y < RowsCount; y++)
        {
            for (int x = 0; x < ColumnsCount; x++)
            {
                if (x != ColumnsCount-1)
                    randomEdgesHorizaontal[y, x] = Random.Range(0, 2) == 0;
                if (y != RowsCount - 1)
                    randomEdgesVertical[y, x] = Random.Range(0, 2) == 0;
            }
        }

        for (int y = 0; y < RowsCount; y++)
        {
            for (int x = 0; x < ColumnsCount; x++)
            {
                var pos = new Vector2Int(x, y);
                //var piecePazzle = new PiecePazzle();
                var namePos = DefineNamePos(pos);
                var tips = CheckSides(pos, randomEdgesHorizaontal, randomEdgesVertical);

                var piecePuzzle = _piecesCollections.FindSuitablePazzle(new PieceData(namePos, tips), pos);
                var test = Instantiate(piecePuzzle);
                PieceRotation.RotateTips(test, pos);
                test.transform.position = new Vector3(-3 * x, -3 * y, 0);
            }
        }
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

    private NamePos DefineNamePos(Vector2Int currPos)
    {

        if ((currPos.x == 0 || currPos.x == ColumnsCount - 1) &&
            (currPos.y == 0 || currPos.y == RowsCount - 1))
            return  NamePos.CORNER;

        if (currPos.x == 0 || currPos.y == 0 ||
            currPos.x == ColumnsCount - 1 || currPos.y == RowsCount - 1)
            return NamePos.EDGE;

        else
            return NamePos.CENTER;
    }

    private PossibleTips[] CheckSides(Vector2Int curPos, bool [,] randomEdgesHorizontal, bool[,] randomEdgesVertical)
    {
        var tips= new PossibleTips[4];

        if (curPos.x != 0)
            tips[0] = randomEdgesHorizontal[curPos.y , curPos.x - 1] ? PossibleTips.CONVEX : PossibleTips.CAVITY;
        else tips[0] = PossibleTips.STRAIGHT;

        if (curPos.y != 0)
            tips[1] = randomEdgesVertical[curPos.y - 1, curPos.x] ? PossibleTips.CONVEX : PossibleTips.CAVITY;
        else tips[1] = PossibleTips.STRAIGHT;

        if (curPos.x != ColumnsCount -1)
            tips[2] = randomEdgesHorizontal[curPos.y, curPos.x] ? PossibleTips.CAVITY : PossibleTips.CONVEX;
        else tips[2] = PossibleTips.STRAIGHT;

        if (curPos.y != RowsCount - 1)
            tips[3] = randomEdgesVertical[curPos.y, curPos.x] ? PossibleTips.CAVITY : PossibleTips.CONVEX;
        else tips[3] = PossibleTips.STRAIGHT;

        return tips;
    }
}
