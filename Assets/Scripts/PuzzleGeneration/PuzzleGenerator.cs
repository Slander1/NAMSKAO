using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration
{
    public enum TipsVariant
    {
        CAVITY = 0,
        CONVEX = 1,
        STRAIGHT = 2
    }

    public class PuzzleGenerator : MonoBehaviour
    {
        [Header("Tiles Settings")]
        public int seed;
        public int columnsCount = 4;
        public int rowsCount = 4;

        [Header("Puzzle prefabs Settings")]
        [SerializeField] private PiecePuzzle[] puzzlePrefabs;

        private PiecesCollection _piecesCollections;

        private void Awake()
        {
            _piecesCollections = new PiecesCollection(puzzlePrefabs);
        }

        public PiecePuzzle[] GenerateGridPuzles()
        {
            var generatedPuzzle = new PiecePuzzle[rowsCount * columnsCount];
            var randomTipsHorizontal = new bool[rowsCount, columnsCount - 1];
            var randomTipsVertical = new bool[rowsCount - 1, columnsCount];

            var scale = CalculateScale();
            Random.InitState(seed);

            for (int y = 0; y < rowsCount; y++)
            {
                for (var x = 0; x < columnsCount; x++)
                {
                    if (x != columnsCount - 1)
                        randomTipsHorizontal[y, x] = Random.Range(0, 2) == 0;

                    if (y != rowsCount - 1)
                        randomTipsVertical[y, x] = Random.Range(0, 2) == 0;
                }
            }

            for (int y = 0; y < rowsCount; y++)
            {
                for (int x = 0; x < columnsCount; x++)
                {
                    var pos = new Vector2Int(x, y);
                    var namePos = DefineNamePos(pos);
                    var tips = CheckSides(pos, randomTipsHorizontal, randomTipsVertical);

                    var piecePuzzlePrefab = _piecesCollections.FindSuitablePazzle(new PieceData(namePos, tips));
                    var pieceObject = Instantiate(piecePuzzlePrefab, Vector3.zero,
                        Quaternion.Euler(new Vector3(0, 180, 0)), transform);
                    pieceObject.GroupNumber = y * columnsCount + x;
                    PieceRotation.RotateTips(pieceObject, pos, rowsCount, columnsCount);
                    
                    pieceObject.transform.position  = pieceObject.StartPos =
                        new Vector3( -x * scale.x, -y * scale.y, 0);

                    pieceObject.PosInGreed = new Vector2Int(x, y);
                    pieceObject.ScaleOnBoard = scale;
                    pieceObject.transform.localScale = scale;
                    
                    generatedPuzzle[y * columnsCount +  x] = pieceObject;
                }
            }
            return generatedPuzzle;
        }

        private NamePos DefineNamePos(Vector2Int currPos)
        {

            if ((currPos.x == 0 || currPos.x == columnsCount - 1) &&
                (currPos.y == 0 || currPos.y == rowsCount - 1))
                return NamePos.CORNER;

            if (currPos.x == 0 || currPos.y == 0 ||
                currPos.x == columnsCount - 1 || currPos.y == rowsCount - 1)
                return NamePos.EDGE;

            return NamePos.CENTER;
        }

        private TipsVariant[] CheckSides(Vector2Int curPos, bool[,] randomEdgesHorizontal, bool[,] randomEdgesVertical)
        {
            var tips = new TipsVariant[4];

            if (curPos.x != 0)
                tips[0] = randomEdgesHorizontal[curPos.y, curPos.x - 1] ? TipsVariant.CONVEX : TipsVariant.CAVITY;
            else tips[0] = TipsVariant.STRAIGHT;

            if (curPos.y != 0)
                tips[1] = randomEdgesVertical[curPos.y - 1, curPos.x] ? TipsVariant.CONVEX : TipsVariant.CAVITY;
            else tips[1] = TipsVariant.STRAIGHT;

            if (curPos.x != columnsCount - 1)
                tips[2] = randomEdgesHorizontal[curPos.y, curPos.x] ? TipsVariant.CAVITY : TipsVariant.CONVEX;
            else tips[2] = TipsVariant.STRAIGHT;

            if (curPos.y != rowsCount - 1)
                tips[3] = randomEdgesVertical[curPos.y, curPos.x] ? TipsVariant.CAVITY : TipsVariant.CONVEX;
            else tips[3] = TipsVariant.STRAIGHT;

            return tips;
        }

        public Vector3 CalculateScale()
        {
            var maxCount = Mathf.Max(rowsCount * 2, columnsCount);
            return new Vector3(19.2f / maxCount, 19.2f / maxCount, 1f);
        }
    }
}
