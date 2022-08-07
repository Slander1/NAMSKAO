using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGridGenerator : MonoBehaviour
{
    [SerializeField] private RectTransform greed;
    [SerializeField] private RectTransform cellGridPrefab;

    public void GenerateImagesForGridPuzzles(PuzzleGeneration.PiecePazzle[,] generatedPazzle,
        Vector3 scaleOnBoard)
    {
        foreach (var puzzle in generatedPazzle)
        {
            var cell = Instantiate(cellGridPrefab, greed);
            cell.transform.position = puzzle.transform.position;
            cell.localScale = scaleOnBoard;
        }
    }
}
