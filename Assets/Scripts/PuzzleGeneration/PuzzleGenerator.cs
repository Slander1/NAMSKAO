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
        public static PuzzleGenerator Instanse { get; private set; } // избавиться от Instance


        [Header("Tiles Settings")]
        public int seed;
        public int ColumnsCount = 4;
        public int RowsCount = 4;

        [Header("Puzzle prefabs Settings")]
        [SerializeField] private PiecePazzle[] puzzlePrefabs;

        

        private PiecesCollection _piecesCollections;


        private void Awake()
        {
            if (Instanse != null && Instanse != this)
                Destroy(this);
            else
                Instanse = this;

            _piecesCollections = new PiecesCollection(puzzlePrefabs);
        }

        public PiecePazzle[,] GenerateGridPuzles()
        {
            var generatedPazzle = new PiecePazzle[RowsCount, ColumnsCount];
            var randomTipsHorizontal = new bool[RowsCount, ColumnsCount - 1];
            var randomTipsVertical = new bool[RowsCount - 1, ColumnsCount];

            Random.InitState(seed);

            for (int y = 0; y < RowsCount; y++)
            {
                for (int x = 0; x < ColumnsCount; x++)
                {
                    if (x != ColumnsCount - 1)
                        randomTipsHorizontal[y, x] = Random.Range(0, 2) == 0;
                    if (y != RowsCount - 1)
                        randomTipsVertical[y, x] = Random.Range(0, 2) == 0;
                }
            }

            for (int y = 0; y < RowsCount; y++)
            {
                for (int x = 0; x < ColumnsCount; x++)
                {
                    var pos = new Vector2Int(x, y);
                    var namePos = DefineNamePos(pos);
                    var tips = CheckSides(pos, randomTipsHorizontal, randomTipsVertical);

                    var piecePuzzle = _piecesCollections.FindSuitablePazzle(new PieceData(namePos, tips), pos);
                    var piecePazzle = Instantiate(piecePuzzle, transform);
                    PieceRotation.RotateTips(piecePazzle, pos);
                    piecePazzle.transform.position = new Vector3(3 * x, -3 * y, 0);
                    generatedPazzle[y, x] = piecePazzle;
                }
            }
            return generatedPazzle;
        }

        private NamePos DefineNamePos(Vector2Int currPos)
        {

            if ((currPos.x == 0 || currPos.x == ColumnsCount - 1) &&
                (currPos.y == 0 || currPos.y == RowsCount - 1))
                return NamePos.CORNER;

            if (currPos.x == 0 || currPos.y == 0 ||
                currPos.x == ColumnsCount - 1 || currPos.y == RowsCount - 1)
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

            if (curPos.x != ColumnsCount - 1)
                tips[2] = randomEdgesHorizontal[curPos.y, curPos.x] ? TipsVariant.CAVITY : TipsVariant.CONVEX;
            else tips[2] = TipsVariant.STRAIGHT;

            if (curPos.y != RowsCount - 1)
                tips[3] = randomEdgesVertical[curPos.y, curPos.x] ? TipsVariant.CAVITY : TipsVariant.CONVEX;
            else tips[3] = TipsVariant.STRAIGHT;

            return tips;
        }
    }
}
