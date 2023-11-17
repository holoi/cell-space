// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using System.Text;
using System.Runtime.CompilerServices;

namespace VoroGen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WeightedPoint
    {
        public float x, y, z, w;
    }

    public class VoronoiGenerator
    {
        public static (Mesh, Mesh)[] GenerateVoronoi(WeightedPoint[] weightedPoints, Bounds bounds, float offset = 0.0f)
        {
            int n = weightedPoints.Length;
            IntPtr verticesPtr, trianglesPtr, linesPtr;
            IntPtr numVerticesPtr, numTrianglesPtr, numLinesPtr;

            (Mesh, Mesh)[] meshes = new (Mesh, Mesh)[n];

            VoroGen_ComputeVoronoi(
                weightedPoints,
                n,
                bounds.min.x, bounds.max.x,
                bounds.min.y, bounds.max.y,
                bounds.min.z, bounds.max.z,
                offset,
                out verticesPtr, out numVerticesPtr,
                out trianglesPtr, out numTrianglesPtr,
                out linesPtr, out numLinesPtr);
            
            int[] numVertices = new int[n];
            int[] numTriangles = new int[n];
            int[] numLines = new int[n];

            Marshal.Copy(numVerticesPtr, numVertices, 0, n);
            Marshal.Copy(numTrianglesPtr, numTriangles, 0, n);
            Marshal.Copy(numLinesPtr, numLines, 0, n);

            int totalVerticesOut = 0;
            int totalTrianglesOut = 0;
            int totalLinesOut = 0;
            for (int i = 0; i < weightedPoints.Length; i++) {
                totalVerticesOut += numVertices[i];
                totalTrianglesOut += numTriangles[i];
                totalLinesOut += numLines[i];
            }

            float[] vertices = new float[totalVerticesOut];
            int[] triangles = new int[totalTrianglesOut];
            int[] lines = new int[totalLinesOut];
            
            Marshal.Copy(verticesPtr, vertices, 0, totalVerticesOut);
            Marshal.Copy(trianglesPtr, triangles, 0, totalTrianglesOut);
            Marshal.Copy(linesPtr, lines, 0, totalLinesOut);

            VoroGen_FreeMemory(verticesPtr);
            VoroGen_FreeMemory(trianglesPtr);
            VoroGen_FreeMemory(linesPtr);
            VoroGen_FreeMemory(numVerticesPtr);
            VoroGen_FreeMemory(numTrianglesPtr);
            VoroGen_FreeMemory(numLinesPtr);

            totalVerticesOut = 0;
            totalTrianglesOut = 0;
            totalLinesOut = 0;

            for (int i = 0; i < weightedPoints.Length; i++) {
                // Now you need to marshal the data from the pointers to managed arrays
                // Use vertices and triangles to create a mesh
                Vector3[] cellVertices = new Vector3[numVertices[i]];
                for (int j = 0; j < numVertices[i] / 3; j++) {
                    cellVertices[j] = new Vector3(vertices[totalVerticesOut + j * 3], vertices[totalVerticesOut + j * 3 + 1], vertices[totalVerticesOut + j * 3 + 2]);
                }

                int[] cellTriangles = new int[numTriangles[i]];
                for (int j = 0; j < numTriangles[i]; j++) {
                    cellTriangles[j] = triangles[totalTrianglesOut + j];
                }

                int[] cellLines = new int [numLines[i]];
                for (int j = 0; j < numLines[i]; j++) {
                    cellLines[j] = lines[totalLinesOut + j];
                }

                Mesh mesh = new Mesh();
                mesh.SetVertices(cellVertices);
                mesh.SetIndices(cellTriangles, MeshTopology.Triangles, 0);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                Mesh wireframe = new Mesh();
                wireframe.SetVertices(cellVertices);
                wireframe.SetIndices(cellLines, MeshTopology.Lines, 0);
                wireframe.RecalculateBounds();
                meshes[i] = (mesh, wireframe);

                totalVerticesOut += numVertices[i];
                totalTrianglesOut += numTriangles[i];
                totalLinesOut += numLines[i];
            }

            return meshes;
        }
        
        #if !UNITY_EDITOR && UNITY_IOS
            const string DllName = "__Internal";
        #else
            const string DllName = "VoroGen";
        #endif

        [DllImport(DllName)]
        private static extern void VoroGen_FreeMemory(
            IntPtr ptr
        );

        [DllImport(DllName)]
        private static extern void VoroGen_ComputeVoronoi(
            WeightedPoint[] weightedPoints,
            int numPoints,
            float minX, float maxX,
            float minY, float maxY,
            float minZ, float maxZ,
            float offset, 
            out IntPtr vertices, out IntPtr numVertices,
            out IntPtr triangles, out IntPtr numTriangles,
            out IntPtr lines, out IntPtr numLines);
    }

}