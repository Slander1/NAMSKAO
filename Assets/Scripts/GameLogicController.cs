using UnityEngine;
using PuzzleGeneration;

public class GameLogicController : MonoBehaviour
{

    [Header("Generated pazzle")]
    public PiecePazzle[,] generatedPazzle;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Scripts")]
    [SerializeField] private PuzzleGenerator _puzzleGenerator;

    void Start()
    {
        generatedPazzle = _puzzleGenerator.GenerateGridPuzles();
        UV.UVGenerator.GetVertexFromPazzle(generatedPazzle, texture2D);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
