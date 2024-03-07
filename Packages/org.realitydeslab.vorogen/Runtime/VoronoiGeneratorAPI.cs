// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace VoroGen
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WeightedPoint
    {
        public float x, y, z, w;
    }

    public class VoronoiGeneratorAPI
    {
        public static (Vector3[], int[]) ComputeDelaunay(WeightedPoint[] weightedPoints) {
            int n = weightedPoints.Length;
            IntPtr edgesPtr;
            IntPtr numEdgesPtr;

            if (n == 0) {
                return (new Vector3[0], new int[0]);
            }

            const float SafeMargin = 1.0f;
            
            VoroGen_ComputeDelaunay(
                weightedPoints,
                n,
                weightedPoints.Min(p => p.x) - SafeMargin, weightedPoints.Max(p => p.x) + SafeMargin,
                weightedPoints.Min(p => p.y) - SafeMargin, weightedPoints.Max(p => p.y) + SafeMargin,
                weightedPoints.Min(p => p.z) - SafeMargin, weightedPoints.Max(p => p.z) + SafeMargin,
                out edgesPtr, out numEdgesPtr
            );
            
            int[] numEdges = new int[n];

            Marshal.Copy(numEdgesPtr, numEdges, 0, n);
            int totalEdgesOut = 0;
            for (int i = 0; i < weightedPoints.Length; i++) {
                totalEdgesOut += numEdges[i];
            }
            
            int[] edges = new int[totalEdgesOut];
            Marshal.Copy(edgesPtr, edges, 0, totalEdgesOut);

            VoroGen_FreeMemory(edgesPtr);
            VoroGen_FreeMemory(numEdgesPtr);

            Vector3[] vertices = new Vector3[n];
            int[] indices = new int[totalEdgesOut * 2];
    
            int totalIndicesOut = 0;
            totalEdgesOut = 0;
            for (int i = 0; i < weightedPoints.Length; i++) {
                int[] cellEdges = new int[numEdges[i] * 2];
                for (int j = 0; j < numEdges[i]; j++) {
                    indices[totalIndicesOut++] = i;
                    indices[totalIndicesOut++] = edges[totalEdgesOut + j];
                }
                vertices[i] = new Vector3(weightedPoints[i].x, weightedPoints[i].y, weightedPoints[i].z);
                totalEdgesOut += numEdges[i];
            }

            return (vertices, indices);
        }

        // This is the function you need to call to generate the voronoi diagram. 
        // array of weighted points, bounds of the voronoi diagram, and offset are the inputs
        // the output is an array of meshes, each mesh is a cell of the voronoi diagram
        // each mesh is represented by an array of vertices, an array of triangles, and an array of lines
        // cellEdges is the dual triangulation's edges.
        // cellVertices, cellTriangles, cellLines, cellEdges
        public static (Vector3[], int[], int[], int[])[] GenerateVoronoi(WeightedPoint[] weightedPoints, Bounds bounds, float offset = 0.0f)
        {
            int n = weightedPoints.Length;
            IntPtr verticesPtr, trianglesPtr, linesPtr, edgesPtr;
            IntPtr numVerticesPtr, numTrianglesPtr, numLinesPtr, numEdgesPtr;

            (Vector3[], int[], int[], int[])[] meshes = new (Vector3[], int[], int[], int[])[n];

            if (n == 0) {
                return meshes;
            }

            VoroGen_ComputeVoronoi(
                weightedPoints,
                n,
                bounds.min.x, bounds.max.x,
                bounds.min.y, bounds.max.y,
                bounds.min.z, bounds.max.z,
                offset,
                out verticesPtr, out numVerticesPtr,
                out trianglesPtr, out numTrianglesPtr,
                out linesPtr, out numLinesPtr,
                out edgesPtr, out numEdgesPtr);
            
            int[] numVertices = new int[n];
            int[] numTriangles = new int[n];
            int[] numLines = new int[n];
            int[] numEdges = new int[n];

            Marshal.Copy(numVerticesPtr, numVertices, 0, n);
            Marshal.Copy(numTrianglesPtr, numTriangles, 0, n);
            Marshal.Copy(numLinesPtr, numLines, 0, n);
            Marshal.Copy(numEdgesPtr, numEdges, 0, n);

            int totalVerticesOut = 0;
            int totalTrianglesOut = 0;
            int totalLinesOut = 0;
            int totalEdgesOut = 0;

            for (int i = 0; i < weightedPoints.Length; i++) {
                totalVerticesOut += numVertices[i];
                totalTrianglesOut += numTriangles[i];
                totalLinesOut += numLines[i];
                totalEdgesOut += numEdges[i];
            }

            float[] vertices = new float[totalVerticesOut];
            int[] triangles = new int[totalTrianglesOut];
            int[] lines = new int[totalLinesOut];
            int[] edges = new int[totalEdgesOut];

            Marshal.Copy(verticesPtr, vertices, 0, totalVerticesOut);
            Marshal.Copy(trianglesPtr, triangles, 0, totalTrianglesOut);
            Marshal.Copy(linesPtr, lines, 0, totalLinesOut);
            Marshal.Copy(edgesPtr, edges, 0, totalEdgesOut);

            VoroGen_FreeMemory(verticesPtr);
            VoroGen_FreeMemory(trianglesPtr);
            VoroGen_FreeMemory(linesPtr);
            VoroGen_FreeMemory(edgesPtr);

            VoroGen_FreeMemory(numVerticesPtr);
            VoroGen_FreeMemory(numTrianglesPtr);
            VoroGen_FreeMemory(numLinesPtr);
            VoroGen_FreeMemory(numEdgesPtr);

            totalVerticesOut = 0;
            totalTrianglesOut = 0;
            totalLinesOut = 0;
            totalEdgesOut = 0;

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

                int[] cellEdges = new int[numEdges[i]];
                for (int j = 0; j < numEdges[i]; j++)
                {
                    cellEdges[j] = edges[totalEdgesOut + j];
                }

                meshes[i] = (cellVertices, cellTriangles, cellLines, cellEdges);

                totalVerticesOut += numVertices[i];
                totalTrianglesOut += numTriangles[i];
                totalLinesOut += numLines[i];
                totalEdgesOut += numEdges[i];
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
        private static extern void VoroGen_ComputeDelaunay(
            WeightedPoint[] weightedPoints,
            int numPoints,
            float minX, float maxX,
            float minY, float maxY,
            float minZ, float maxZ,
            out IntPtr edges, out IntPtr numEdges);

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
            out IntPtr lines, out IntPtr numLines,
            out IntPtr edges, out IntPtr numEdges);
    }

}