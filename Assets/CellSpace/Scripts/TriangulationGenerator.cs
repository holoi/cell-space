// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using VoroGen;

namespace CellSpace
{
    public class TriangulationGenerator : MonoBehaviour 
    {
        private GameObject[] points;
        public Material pointMaterial;
        public Material wireframeMaterial;
        public GameObject[] sites;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            //Find all objects with PlayerPoseSynchronizer in the scenes
            CreateTriangulation();
        }

        private void CreateTriangulation()
        {
            int n = sites.Length;
            if (sites.Any(p => p == null)) {
                return;
            }

            var weightedPoints = sites.Select(p => new WeightedPoint { x = p.transform.position.x,
                    y = p.transform.position.y,
                    z = p.transform.position.z,
                    w = p.transform.localScale.x} ).ToArray();
            
            var (vertices, indices) = VoronoiGeneratorAPI.ComputeDelaunay(weightedPoints);

            string triangulationName = $"Triangulation";
            GameObject triangulationGameObject = transform.Find(triangulationName)?.gameObject;
            if (triangulationGameObject == null) {
                triangulationGameObject = new GameObject(triangulationName);
                triangulationGameObject.transform.parent = transform;
                triangulationGameObject.AddComponent<MeshFilter>().mesh = new Mesh();
                triangulationGameObject.AddComponent<MeshRenderer>().material = wireframeMaterial;
                triangulationGameObject.AddComponent<TubeRenderer>();
            } 
            var mesh = triangulationGameObject.GetComponent<MeshFilter>().mesh;
            mesh.name = $"Triangulation";
            if (vertices.Length > 0) {
                mesh.SetVertices(vertices);
                mesh.SetIndices(indices, MeshTopology.Lines, 0);
               // mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            } else {
                mesh.Clear();
            }
        }
    }
}