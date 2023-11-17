// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;

namespace CellSpace {
    public class CellSpaceVoronoiGenerator : MonoBehaviour 
    {
        public Color cellColor;
        public Color pointColor;
        public Color wireframeColor;
        // Game objects used to visualize the results
        private GameObject geometryObject;
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
            //CreateTriangulation();
        }


        // Update is called once per frame
        void Update()
        {
            pointMaterial.SetColor("_Color", pointColor);
            wireframeMaterial.SetColor("_Color", wireframeColor);
            cellMaterial.SetColor("_Color", cellColor);

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

            var meshes = VoronoiGenerator.GenerateVoronoi(weightedPoints, bounds, offset);
            
            for (int i = 0; i < meshes.Length; i++) {
                {
                    string cellName = $"Cell Volume {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = meshes[i].Item1;
                        cell.AddComponent<MeshRenderer>().material = cellMaterial;
                    } else {
                        cell.GetComponent<MeshFilter>().mesh = meshes[i].Item1;
                    }
                }

                {
                    string cellName = $"Cell Wireframe {i}";
                    GameObject cell = transform.Find(cellName)?.gameObject;
                    if (cell == null) {
                        cell = new GameObject(cellName);
                        cell.transform.parent = transform;
                        cell.AddComponent<MeshFilter>().mesh = meshes[i].Item2;
                        cell.AddComponent<MeshRenderer>().material = wireframeMaterial;
                    } else {
                        cell.GetComponent<MeshFilter>().mesh = meshes[i].Item2;
                    }
                }
            }
        }
    }
}
