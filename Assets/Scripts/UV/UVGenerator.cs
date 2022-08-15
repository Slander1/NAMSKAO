using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PieceData;

namespace UV
{
    public static class UVGenerator
    {
        public static void GetVertexFromPuzzle(PiecePuzzle[] generatedPuzzle, Texture2D picture)
        {
            var meshes = generatedPuzzle.Select(piece => piece.GetComponentInChildren<MeshFilter>()).ToList();

            var vertices = meshes.SelectMany((keyVal) => keyVal.mesh.vertices
                                  .Select(vert => keyVal.transform.TransformPoint(vert))).ToArray();

            var minX = vertices.Min(vertex => vertex.x);
            var maxX = vertices.Max(vertex => vertex.x);

            var minY = vertices.Min(vertex => vertex.y);
            var maxY = vertices.Max(vertex => vertex.y);

            foreach (var meshFilter in meshes)
            {
                var uv = new Vector2[meshFilter.mesh.vertices.Length];

                for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                {
                    var vertex = meshFilter.mesh.vertices[i];
                    var globalPosition = meshFilter.transform.TransformPoint(vertex);
                    uv[i] = new Vector2(Mathf.InverseLerp(maxX, minX, globalPosition.x),
                        Mathf.InverseLerp(minY, maxY, globalPosition.y));
                }

                meshFilter.mesh.uv = uv;
            }
            SetPictureToPuzzle(generatedPuzzle, picture);
        }

        private static void SetPictureToPuzzle(IEnumerable<PiecePuzzle> generatedPuzzle, Texture picture)
        {
            foreach (var piece in generatedPuzzle)
            {
                var meshRenderer = piece.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material.mainTexture = picture;
            }
        }
    }
}

