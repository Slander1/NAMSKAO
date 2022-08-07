using UnityEngine;
using PuzzleGeneration;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameLogicController : MonoBehaviour
{

    [Header("Generated pazzle")]
    public PiecePazzle[,] generatedPazzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator _puzzleGenerator;

    [Header("UI")]
    [SerializeField] private PuzzleScrollContainer _puszzleScrollContainer;
    [SerializeField] private PuzzleGridGenerator _puzzleGridGenerator;




    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        var rows = _puzzleGenerator.rowsCount;
        var columns = _puzzleGenerator.columnsCount;
        var scaleOnBoard = _puzzleGenerator.CalculateScale();
        generatedPazzle = _puzzleGenerator.GenerateGridPuzles();
        UV.UVGenerator.GetVertexFromPazzle(generatedPazzle, texture2D);
        _puzzleGridGenerator.GenerateImagesForGridPuzzles(generatedPazzle,
           scaleOnBoard);
       // var scale = _puzzleGenerator.ScalePazzlesTosqreen();

        int count = rows * columns;
        var i = 0;
        foreach (var UIImageForScroll in _puszzleScrollContainer.GenerateImagesToScroll(count))
        {
            var y = i / columns;
            var x = i % columns;
            generatedPazzle[y, x].elementForScroll = UIImageForScroll;
            UIImageForScroll.transform.position = Vector3.zero;
            generatedPazzle[y, x].transform.localScale = generatedPazzle[y, x].scaleInContainer =
                new Vector3(0.8f, 0.8f, 1f);
            generatedPazzle[y, x].scaleOnBoard = scaleOnBoard;
            i++;
        }
    }

    



    //private void Update()
    //{ 
    //    if (Input.GetMouseButtonDown(0) && selectedObject != null)
    //    {
    //        var ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out var info, 100, LayerMask.GetMask(Tiles.TILE, Tiles.HOVER, Tiles.HIGLIGHT)))
    //    }
    //}

    //private RaycastHit CastRay()
    //{
    //    var screenMousePosFar = new Vector3(Input.mousePosition.x,
    //        Input.);
    //}
}
