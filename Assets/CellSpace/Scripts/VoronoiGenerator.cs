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
    public class VoronoiGenerator : MonoBehaviour 
    {
        // Game objects used to visualize the results
        private GameObject[] cells;
        private GameObject[] points;
        private GameObject[] wireframes;
        public Color cellColor;
        public Color wireframeColor;
        public Color nonPlayerCellColor;
        public Color nonPlayerWireframeColor;

        public Material cellMaterial;
        public Material pointMaterial;
        public Material wireframeMaterial;
        public Bounds bounds; 

        [Range(0.0f, 0.1f)]
        public float offset;
        public GameObject[] sites;
        Color[] colors;
        Material[] wireframeMaterials;
        Material[] cellMaterials;


        [Range(0.01f, 0.2f)]
        public float playerTubeWidth;
        [Range(0.01f, 0.2f)]
        public float nonPlayerTubeWidth;


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

            if (colors == null || colors.Length != n) {
                colors = new Color[n];
                wireframeMaterials = new Material[n];
                cellMaterials = new Material[n];
                for (int i = 0; i < n; i++) {
                    colors[i] = UnityEngine.Random.ColorHSV(0.05f, 0.3f, 0.85f, 0.95f, 0.85f, 0.95f);
                    wireframeMaterials[i] = new Material(wireframeMaterial);
                    cellMaterials[i] = new Material(cellMaterial);
                    var cellRole = sites[i].GetComponent<CellRole>();

                    cellMaterials[i].SetColor("_Wire_Color", cellRole.isPlayer ? cellColor : nonPlayerCellColor);
                    wireframeMaterials[i].SetColor("_Wire_Color", cellRole.isPlayer ? wireframeColor: nonPlayerWireframeColor);
                }
            }

            var weightedPoints = sites.Select(p => new WeightedPoint { x = p.transform.position.x,
                    y = p.transform.position.y,
                    z = p.transform.position.z,
                    w = p.transform.localScale.x} ).ToArray();
            
            var offsetBounds = new Bounds(new Vector3(bounds.center.x + transform.position.x,
                bounds.center.y + transform.position.y,
                bounds.center.z + transform.position.z), bounds.size);

            var meshes = VoronoiGeneratorAPI.GenerateVoronoi(weightedPoints, offsetBounds, offset);

            for (int i = 0; i < meshes.Length; i++) {
                var (cellVertices, cellTriangles, cellLines) = meshes[i];
                {
                    string cellName = $"Cell Volume {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = new Mesh();
                        cell.AddComponent<MeshRenderer>().material = cellMaterials[i];
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
                        cell.AddComponent<MeshRenderer>().material = wireframeMaterials[i];
                        cell.AddComponent<TubeRenderer>();
                    } 

                    var tube = cell.GetComponent<TubeRenderer>();
                    tube.SetPositions(cellVertices);
                    tube.SetIndices(cellLines);
                    tube.tubularSegments = 1;
                    tube.radialSegments = 8;
                    var cellRole = sites[i].GetComponent<CellRole>();
                    tube.radius = cellRole.isPlayer ? playerTubeWidth : nonPlayerTubeWidth;
                }
            }
        }
    }
}