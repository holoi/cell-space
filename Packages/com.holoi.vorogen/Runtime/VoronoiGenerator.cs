// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace VoroGen
{
    public class VoronoiGenerator : MonoBehaviour 
    {
        // Game objects used to visualize the results
        private GameObject[] cells;
        private GameObject[] points;
        private GameObject[] wireframes;
        public Material cellMaterial;
        public Material pointMaterial;
        public Material wireframeMaterial;
        public Bounds bounds; 

        [Range(0.0f, 0.1f)]
        public float offset;
        public GameObject[] sites;

        // Start is called before the first frame update
        void Start()
        {
        }


        // Update is called once per frame
        void Update()
        {
            //Find all objects with PlayerPoseSynchronizer in the scenes
            CreateVoronoi();
        }

        private void CreateVoronoi()
        {
            int n = sites.Length;
            if (sites.Any(p => p == null)) {
                return;
            }

            var weightedPoints = sites.Select(p => new WeightedPoint { x = p.transform.position.x,
                    y = p.transform.position.y,
                    z = p.transform.position.z,
                    w = p.transform.localScale.x} ).ToArray();

            var meshes = VoronoiGeneratorAPI.GenerateVoronoi(weightedPoints, bounds, offset);
            
            for (int i = 0; i < meshes.Length; i++) {
                var (cellVertices, cellTriangles, cellLines) = meshes[i];
                {
                    string cellName = $"Cell Volume {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = new Mesh();
                        cell.AddComponent<MeshRenderer>().material = cellMaterial;
                    }

                    var mesh = cell.GetComponent<MeshFilter>().mesh;
                    mesh.name = $"Cell Volume {i}";
                    if (cellVertices.Length > 0) {
                        mesh.SetVertices(cellVertices);
                        mesh.SetIndices(cellTriangles, MeshTopology.Triangles, 0);
                        mesh.RecalculateNormals();
                        mesh.RecalculateBounds();
                    } else {
                        mesh.Clear();
                    }
                }

                {
                    string cellName = $"Cell Wireframe {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = new Mesh();
                        cell.AddComponent<MeshRenderer>().material = wireframeMaterial;
                    } 

                    var mesh = cell.GetComponent<MeshFilter>().mesh;
                    mesh.name = $"Cell Wireframe {i}";
                    if (cellVertices.Length > 0) {
                        mesh.SetVertices(cellVertices);
                        mesh.SetIndices(cellLines, MeshTopology.Lines, 0);
                        mesh.RecalculateBounds();
                    } else {
                        mesh.Clear(); 
                    }
                }
            }
        }
    }
}