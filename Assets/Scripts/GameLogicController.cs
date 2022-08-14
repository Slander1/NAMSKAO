using UnityEngine;
using PuzzleGeneration;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Utils;

public class GameLogicController : MonoBehaviour
{
    [Header("Generated pazzle")]
    public PiecePuzzle[] generatedPuzzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator puzzleGenerator;
    [SerializeField] private PuzzleGluer puzzleGluer;

    [Header("UI")]
    [SerializeField] private UI.PuzzleScrollContainer puzzleScrollContainer;
    [SerializeField] private UI.PuzzleGridGenerator puzzleGridGenerator;
    [SerializeField] private Image winscreen;
    [SerializeField] private RectTransform container;

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
        puzzleGluer.Init(generatedPuzzle, puzzleGenerator.CalculateScale(), container, puzzleGenerator.transform);
        puzzleGluer.OnPieceMovedToPosition += CheckToWin;

        var count = _rowsCount * _columnsCount;

        var listGeneratedPuzzles = generatedPuzzle.Shuffle().ToArray();
        var i = 0;
        foreach (var uiImageForScroll in puzzleScrollContainer.GenerateImagesToScroll(count, container))
        {
            listGeneratedPuzzles[i].elementForScroll = uiImageForScroll;
            listGeneratedPuzzles[i].transform.SetParent(uiImageForScroll.transform, false);

            listGeneratedPuzzles[i].transform.localScale = listGeneratedPuzzles[i].scaleInContainer =
               new Vector3(50, 50, 1);
            i++;
        }
    }

    private void CheckToWin(int pieceCurrentCount)
    {
        if (pieceCurrentCount == _rowsCount * _columnsCount)
            winscreen.gameObject.SetActive(true);
    }
}
