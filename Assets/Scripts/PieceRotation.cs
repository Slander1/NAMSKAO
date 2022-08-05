using System;
using System.Collections.Generic;
using UnityEngine;


public static class PieceRotation
{

    private static int[,] _coefMatrix;


    public static void Init(int RowsCount, int ColumsCount)
    {
        _coefMatrix = new int[RowsCount, ColumsCount];
    }

    public static void RotatePiece(GameObject piece, Vector2Int pos)
    {
        piece.transform.Rotate(0, 0, _coefMatrix[pos.y, pos.x]);
    }

    public static void ShiftTips(Vector2Int pos, int[] tips, out int coefficientShift)
    {
        coefficientShift = SearchCoefficient(pos);
        if (coefficientShift == 0)
            return;
        var tipsCopy = (int[])tips.Clone();
        for (int i = 0; i < tips.Length; i++)
        {
            tips[(i + coefficientShift) % tips.Length] =tipsCopy [i];
        }
    }

    private static int SearchCoefficient(Vector2Int pos)
    {
        var coef = 0;
        var rotation = 0;

        if (pos.y == 0)
            rotation = (pos.x == PuzzleGenerator.Instanse.Columns - 1) ? 90 : 0;

        else if (pos.y == PuzzleGenerator.Instanse.Rows - 1)
        {
            coef = (pos.x == 0) ? 1 : 0;
            rotation = (pos.x == 0) ? 270 : 180;
        }

        else if (pos.x == PuzzleGenerator.Instanse.Columns - 1)
            rotation = 90;



        else if (pos.x == 0)
        {
            rotation = 270;
            coef = 2;
        }


        _coefMatrix[pos.y, pos.x] = rotation;
        return coef;
    }
}

