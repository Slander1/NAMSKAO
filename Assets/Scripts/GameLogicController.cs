using UnityEngine;
using PuzzleGeneration;
using System;
using UnityEngine.EventSystems;

public class GameLogicController : MonoBehaviour
{

    [Header("Generated pazzle")]
    public PiecePazzle[,] generatedPazzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator _puzzleGenerator;

    [Header("UI")]
    [SerializeField] private PuszzleScrollContainer _puszzleScrollContainer;




    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        generatedPazzle = _puzzleGenerator.GenerateGridPuzles();
        UV.UVGenerator.GetVertexFromPazzle(generatedPazzle, texture2D);
        _puzzleGenerator.ScalePazzlesTosqreen();

        int count = _puzzleGenerator.RowsCount * _puzzleGenerator.ColumnsCount;
        var i = 0;
        foreach(var UIImageForScroll in _puszzleScrollContainer.GenerateImagesToScroll(count))
        {
            var y = i / _puzzleGenerator.ColumnsCount;
            var x = i % _puzzleGenerator.ColumnsCount;
            generatedPazzle[y, x].elementForScroll = UIImageForScroll;
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
