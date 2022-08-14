using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class RandomExtensions
    {
        public static IEnumerable<T> Shuffle<T> (this IEnumerable<T> array)
        {
            var copyArray = array.ToArray();
            var n = copyArray.Length;
            for (var i = 0; i < copyArray.Length - 1; i++)
            {
                var randomId = Random.Range(i + 1, n);
                (copyArray[i], copyArray[randomId]) = (copyArray[randomId], copyArray[i]);
            }

            return copyArray;
        }
    }
}