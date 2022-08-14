using UnityEngine;
using PuzzleGeneration;
using UnityEngine.UI;
using System.Linq;
using Utils;
using System.Collections.Generic;

public class GameLogicController : MonoBehaviour
{
    

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator puzzleGenerator;
    [SerializeField] private PuzzleGluer puzzleGluer;

    [Header("UI")]
    [SerializeField] private UI.PuzzleScrollContainer puzzleScrollContainer;
    [SerializeField] private Image winscreen;
    [SerializeField] private RectTransform container;

    private int _rowsCount;
    private int _columnsCount;

    private PuzzleParent _puzzleParent;

    public PiecePuzzle[] _generatedPuzzle;
    private DragHandler[] _dragHandlers;



    private void Start()
    {
        StartGame();
    }
    private void OnDestroy()
    {
        //_puzzleParent.OnDestroyGameLogicContoller();
        puzzleGluer.OnPieceMovedToPosition -= CheckToWin;
    }

    private void StartGame()
    {
        _rowsCount = puzzleGenerator.rowsCount;
        _columnsCount = puzzleGenerator.columnsCount;

        var scaleOnBoard = puzzleGenerator.CalculateScale();

        _generatedPuzzle = puzzleGenerator.GenerateGridPuzles();

        _dragHandlers = GetDragHandlers();

        UV.UVGenerator.GetVertexFromPazzle(_generatedPuzzle, texture2D);

        _puzzleParent = new PuzzleParent((RectTransform)puzzleScrollContainer.transform,
            puzzleGenerator.transform, puzzleGluer, _dragHandlers);

        puzzleGluer.Init(_generatedPuzzle, _dragHandlers, puzzleGenerator.CalculateScale(),
            (RectTransform)puzzleScrollContainer.transform);
        puzzleGluer.OnPieceMovedToPosition += CheckToWin;

        var count = _rowsCount * _columnsCount;

        var listGeneratedPuzzles = _generatedPuzzle.Shuffle().ToArray();
        var i = 0;
        foreach (var uiImageForScroll in puzzleScrollContainer.GenerateImagesToScroll(count, container))
        {
            listGeneratedPuzzles[i].ElementForScroll = uiImageForScroll;
            listGeneratedPuzzles[i].transform.SetParent(uiImageForScroll.transform, false);
            
            listGeneratedPuzzles[i].transform.localScale = listGeneratedPuzzles[i].ScaleInContainer =
               new Vector3(50, 50, 1);
            i++;
        }
    }

    private DragHandler[] GetDragHandlers()
    {
        var dragHandlers = new List<DragHandler>();
        foreach (var item in _generatedPuzzle)
        {
            item.TryGetComponent<DragHandler>(out var dragHandler);
            dragHandlers.Add(dragHandler);
        }
        return dragHandlers.ToArray();

    }

    private void CheckToWin(int pieceCurrentCount)
    {
        if (pieceCurrentCount == _rowsCount * _columnsCount)
            winscreen.gameObject.SetActive(true);
    }
}
