using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Tiles Settings")]
    [SerializeField] private int Columns = 4;
    [SerializeField] private int Rows = 4;
    [SerializeField] private int Seed;

    [Header("Texture2D Settings")]
    [SerializeField] private Texture2D texture2D;

    [Header("Puzzle prefabs Settings")]
    [SerializeField] private GameObject[] puzzlePrefabs;



    private void Awake()
    {
        
    }


}
