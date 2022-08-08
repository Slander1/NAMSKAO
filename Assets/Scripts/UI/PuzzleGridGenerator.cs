using UnityEngine;

namespace UI
{
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
}
