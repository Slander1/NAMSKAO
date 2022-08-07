﻿using UnityEngine;
using System.Collections.Generic;

public class PuszzleScrollContainer : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform imagePrefab;


    public IEnumerable<RectTransform> GenerateImagesToScroll(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return Instantiate(imagePrefab, container);
        }
    }
}

