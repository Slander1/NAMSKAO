using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UV
{
    public static class UVGenerator
    {

        public static void GetVertexFromPazzle(PuzzleGeneration.PiecePazzle[,] generatedPuzzle, Texture2D picture)
        {
            var meshes = new List<MeshFilter>();

            foreach (var piece in generatedPuzzle)
            {
                var meshFilter = piece.GetComponentInChildren<MeshFilter>();
                meshes.Add(meshFilter);
            }

            var verticles = meshes.SelectMany((keyVal) => keyVal.mesh.vertices
                                  .Select(vert => keyVal.transform.TransformPoint(vert)));

            var minx = verticles.Min(vertex => vertex.x);
            var maxx = verticles.Max(vertex => vertex.x);

            var miny = verticles.Min(vertex => vertex.y);
            var maxy = verticles.Max(vertex => vertex.y);

            Vector2 size = new Vector2(Mathf.Abs(maxx - minx), Mathf.Abs(maxy - miny));

            foreach (var meshFilrer in meshes)
            {
                var uv = new Vector2[meshFilrer.mesh.vertices.Length];

                for (int i = 0; i < meshFilrer.mesh.vertices.Length; i++)
                {
                    var vertex = meshFilrer.mesh.vertices[i];
                    var globalPosition = meshFilrer.transform.TransformPoint(vertex);
                    uv[i] = new Vector2(Mathf.InverseLerp(minx, maxx, globalPosition.x),
                        Mathf.InverseLerp(miny, maxy, globalPosition.y));
                }

                meshFilrer.mesh.uv = uv;
            }
            SetPictureToPuzzle(generatedPuzzle, picture);
        }

        public static void SetPictureToPuzzle(PuzzleGeneration.PiecePazzle[,] generatedPuzzle, Texture2D picture)
        {
            var meshRenderers = new List<MeshRenderer>();
            foreach (var piece in generatedPuzzle)
            {
                var meshRenderer = piece.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material.mainTexture = picture;
            }
        }
    }
}

