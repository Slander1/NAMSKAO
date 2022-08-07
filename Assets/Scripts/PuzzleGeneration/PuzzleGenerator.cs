using UnityEngine;

namespace PuzzleGeneration
{
    public enum TipsVariant
    {
        CAVITY = 0,
        CONVEX = 1,
        STRAIGHT = 2,
        UNCERTAIN = 3
    }

    public class PuzzleGenerator : MonoBehaviour
    {
        [Header("Tiles Settings")]
        public int seed;
        public int columnsCount = 4;
        public int rowsCount = 4;

        [Header("Puzzle prefabs Settings")]
        [SerializeField] private PiecePazzle[] puzzlePrefabs;

        private PiecesCollection _piecesCollections;

        private void Awake()
        {
            _piecesCollections = new PiecesCollection(puzzlePrefabs);
        }

        public PiecePazzle[,] GenerateGridPuzles()
        {
            var generatedPazzle = new PiecePazzle[rowsCount, columnsCount];
            var randomTipsHorizontal = new bool[rowsCount, columnsCount - 1];
            var randomTipsVertical = new bool[rowsCount - 1, columnsCount];
            var scale = CalculateScale();
            Random.InitState(seed);

            for (int y = 0; y < rowsCount; y++)
            {
                for (int x = 0; x < columnsCount; x++)
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

                    var piecePuzzle = _piecesCollections.FindSuitablePazzle(new PieceData(namePos, tips), pos);
                    var piecePazzle = Instantiate(piecePuzzle, transform);
                    PieceRotation.RotateTips(piecePazzle, pos, rowsCount, columnsCount);
                    piecePazzle.transform.position  = piecePazzle.startPos =
                        new Vector3(3 * x * scale.x +2, -3 * y * scale.y +3, 0);
                    piecePazzle.posInGreed = new Vector2Int(x, y);
                    piecePazzle.scaleOnBoard = scale;
                    piecePazzle.transform.localScale = scale;
                    generatedPazzle[y, x] = piecePazzle;
                }
            }
            return generatedPazzle;
        }

        private NamePos DefineNamePos(Vector2Int currPos)
        {

            if ((currPos.x == 0 || currPos.x == columnsCount - 1) &&
                (currPos.y == 0 || currPos.y == rowsCount - 1))
                return NamePos.CORNER;

            if (currPos.x == 0 || currPos.y == 0 ||
                currPos.x == columnsCount - 1 || currPos.y == rowsCount - 1)
                return NamePos.EDGE;

            else
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
            return new Vector3(5.4f / (float)rowsCount, 5.4f / (float)columnsCount, 1f);
        }
    }
}