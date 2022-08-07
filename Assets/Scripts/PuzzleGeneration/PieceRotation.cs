using UnityEngine;

namespace PuzzleGeneration
{
    public static class PieceRotation
    {

        public static void RotateTips(PiecePazzle piece, Vector2Int pos,
            int rowsCount, int columnsCount)
        {
            var coefficientShift = SearchCoefficient(pos, rowsCount, columnsCount);
            piece.PieceData.tipsPiece = ShiftArray(piece.PieceData.tipsPiece, coefficientShift);
            piece.transform.Rotate(0, 0, coefficientShift * 90);
        }

        public static T[] ShiftArray<T>(T[] array, int shiftCount)
        {
            if (shiftCount == 0)
                return array;

            var cloneArray = (T[])array.Clone();

            for (int i = 0; i < cloneArray.Length; i++)
            {
                var nextId = (i + shiftCount) % cloneArray.Length;
                array[i] = cloneArray[nextId];
            }

            return array;
        }


        private static int SearchCoefficient(Vector2Int pos, int rowsCount, int columnsCount)
        {
            if (pos.x == 0 && pos.y != 0)
                return 3;

            if (pos.x != 0 && pos.y == rowsCount - 1)
                return 2;

            if (pos.x == columnsCount - 1 && pos.y != rowsCount - 1)
                return 1;

            return 0;
        }
    }
}

