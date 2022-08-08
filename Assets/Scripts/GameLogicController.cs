using UnityEngine;
using PuzzleGeneration;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameLogicController : MonoBehaviour
{

    [Header("Generated pazzle")]
    public PiecePuzzle[,] generatedPuzzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator puzzleGenerator;
    [SerializeField] private PuzzleGluer puzzleGluer;

    [Header("UI")]
    [SerializeField] private UI.PuzzleScrollContainer puszzleScrollContainer;
    [SerializeField] private UI.PuzzleGridGenerator puzzleGridGenerator;
    [SerializeField] private Image winscreen;

    private int _rowsCount;
    private int _columnsCount;


    private void Start()
    {
        StartGame();
    }
    private void OnDestroy()
    {
        puzzleGluer.OnPieceMovedToPosition -= CheckToWin;
    }

    private void StartGame()
    {
        _rowsCount = puzzleGenerator.rowsCount;
        _columnsCount = puzzleGenerator.columnsCount;

        var scaleOnBoard = puzzleGenerator.CalculateScale();

        generatedPuzzle = puzzleGenerator.GenerateGridPuzles();

        UV.UVGenerator.GetVertexFromPazzle(generatedPuzzle, texture2D);

        puzzleGridGenerator.GenerateImagesForGridPuzzles(generatedPuzzle, scaleOnBoard);
        puzzleGluer.Init( generatedPuzzle);
        puzzleGluer.OnPieceMovedToPosition += CheckToWin;


        var count = _rowsCount * _columnsCount;

        var listGeneratedPuzzles = new List<PiecePuzzle>();

        for (int y = 0; y < _rowsCount; y++)
        {
            for (int x = 0; x < _columnsCount; x++)
            {
                listGeneratedPuzzles.Add(generatedPuzzle[y, x]);
            }
        }

        foreach (var UIImageForScroll in puszzleScrollContainer.GenerateImagesToScroll(count))
        {
            var i = Random.Range(0, listGeneratedPuzzles.Count);
            listGeneratedPuzzles[i].elementForScroll = UIImageForScroll;

            UIImageForScroll.transform.position = Vector3.zero;

            listGeneratedPuzzles[i].transform.localScale = listGeneratedPuzzles[i].scaleInContainer =
               new Vector3(0.8f, 0.8f, 1f);
            listGeneratedPuzzles[i].scaleOnBoard = scaleOnBoard;

            listGeneratedPuzzles.RemoveAt(i);
        }
    }

    private void CheckToWin(int pieceCurrentCount)
    {
        if (pieceCurrentCount == _rowsCount * _columnsCount)
            winscreen.gameObject.SetActive(true);
    }




}
