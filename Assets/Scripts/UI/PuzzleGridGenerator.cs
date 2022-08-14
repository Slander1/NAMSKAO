using System.Collections.Generic;
using PuzzleGeneration;
using UnityEngine;

namespace UI
{
    public class PuzzleGridGenerator : MonoBehaviour
    {
        [SerializeField] private RectTransform greed;
        [SerializeField] private RectTransform cellGridPrefab;

        public void GenerateImagesForGridPuzzles(IEnumerable<PiecePuzzle> generatedPuzzle
            , Vector3 scaleOnBoard)
        {
            foreach (var puzzle in generatedPuzzle)
            {
                var cell = Instantiate(cellGridPrefab, greed);
                cell.transform.position = puzzle.transform.position;
            }
        }
    }
}
