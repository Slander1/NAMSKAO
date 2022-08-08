using UnityEngine;
using PuzzleGeneration;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameLogicController : MonoBehaviour
{

    [Header("Generated pazzle")]
    public PiecePazzle[,] generatedPazzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator _puzzleGenerator;

    [Header("UI")]
    [SerializeField] private UI.PuzzleScrollContainer _puszzleScrollContainer;
    [SerializeField] private UI.PuzzleGridGenerator _puzzleGridGenerator;
    [SerializeField] private Image Winscreen;

    private List<PiecePazzle> _puzzleOnInitPos = new List<PiecePazzle>();
    private int _rowsCount;
    private int _columnsCount;


    void Start()
    {
        StartGame();
    }

    private void OnEnable()
    {
        PiecePazzle.PiecePazzleOnInitialPos += CheckStatePuzzle;
    }

    private void OnDisable()
    {
        PiecePazzle.PiecePazzleOnInitialPos += CheckStatePuzzle;
    }

    private void StartGame()
    {
        _rowsCount = _puzzleGenerator.rowsCount;
        _columnsCount = _puzzleGenerator.columnsCount;

        var scaleOnBoard = _puzzleGenerator.CalculateScale();

        generatedPazzle = _puzzleGenerator.GenerateGridPuzles();

        UV.UVGenerator.GetVertexFromPazzle(generatedPazzle, texture2D);

        _puzzleGridGenerator.GenerateImagesForGridPuzzles(generatedPazzle,
           scaleOnBoard);

        var count = _rowsCount * _columnsCount;
        

        var listGeneratedPuzzles = new List<PiecePazzle>();

        for (int y = 0; y < _rowsCount; y++)
        {
            for (int x = 0; x < _columnsCount; x++)
            {
                listGeneratedPuzzles.Add(generatedPazzle[y, x]);
            }
        }
        //var i = 0;
        foreach (var UIImageForScroll in _puszzleScrollContainer.GenerateImagesToScroll(count))
        {
            var i = Random.Range(0, listGeneratedPuzzles.Count);
            listGeneratedPuzzles[i].elementForScroll = UIImageForScroll;

            UIImageForScroll.transform.position = Vector3.zero;

            listGeneratedPuzzles[i].transform.localScale = listGeneratedPuzzles[i].scaleInContainer =
               new Vector3(0.8f, 0.8f, 1f);
            listGeneratedPuzzles[i].scaleOnBoard = scaleOnBoard;

            listGeneratedPuzzles.RemoveAt(i);
            //var y = i / _columnsCount;
            //var x = i % _columnsCount;

            //generatedPazzle[y, x].elementForScroll = UIImageForScroll;

            //UIImageForScroll.transform.position = Vector3.zero;

            //generatedPazzle[y, x].transform.localScale = generatedPazzle[y, x].scaleInContainer =
            //    new Vector3(0.8f, 0.8f, 1f);
            //generatedPazzle[y, x].scaleOnBoard = scaleOnBoard;
            //i++;
        }
    }

    private void CheckStatePuzzle(PiecePazzle piecePazzle)
    {
        if (_puzzleOnInitPos.Contains(piecePazzle))
            return;

        _puzzleOnInitPos.Add(piecePazzle);

        if (_puzzleOnInitPos.Count == _rowsCount * _columnsCount)
            Winscreen.gameObject.SetActive(true);
    }
}
