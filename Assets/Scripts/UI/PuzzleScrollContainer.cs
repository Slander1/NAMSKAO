using UnityEngine;
using System.Collections.Generic;
using System;

namespace UI
{
    public class PuzzleScrollContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform imagePrefab;

        public IEnumerable<RectTransform> GenerateImagesToScroll(int count, RectTransform container)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Instantiate(imagePrefab, container);
            }
        }
    }
}

