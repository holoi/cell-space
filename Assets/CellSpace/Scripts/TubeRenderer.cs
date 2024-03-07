// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using UnityEngine;

namespace VoroGen
{
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class TubeRenderer : MonoBehaviour
    {
        [Min(1)]
        public int tubularSegments = 10;
        [Min(0)]
        public int radialSegments = 8;
        Vector3[] positions = new Vector3[0];
        int[] indices = new int[0];

        [Min(0)]
        public float radius = 0.01f;
        public bool showNodesInEditor = false;
        public Vector2 uvScale = Vector2.one;
        public bool inside = false;
        private MeshFilter meshFilter;
        private Mesh mesh = null;
        private float theta = 0f;
        private int lastUpdate = 0;

        public void SetPositions(Vector3[] positions)
        {
            this.positions = positions;
        }

        public void SetIndices(int[] indices)
        {
            this.indices = indices;
        }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (mesh == null) mesh = new Mesh();
            meshFilter.mesh = CreateMesh();
            lastUpdate = PropHashCode();
        }

        private Mesh CreateMesh()
        {
            mesh.Clear();
            if (positions == null || positions.Length == 0) return mesh;
            if (indices == null || indices.Length == 0) return mesh;

            theta = (Mathf.PI * 2) / radialSegments;

            Vector3[] verts = new Vector3[indices.Length / 2 * (tubularSegments + 1) * radialSegments];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];
            int[] tris = new int[2 * 3 * radialSegments * tubularSegments * indices.Length / 2];
            int triIndex = 0;
            for (int k = 0; k < indices.Length / 2; k++) {
                Vector3[] interpolatedPositions = Enumerable.Range(0, tubularSegments + 1)
                    .Select(j => Vector3.Lerp(
                            positions[indices[k * 2]], 
                            positions[indices[k * 2 + 1]],
                            ((float) j) / ((float) tubularSegments))).ToArray();
               
                Vector3 localForward = (positions[indices[k * 2 + 1]] - positions[indices[k * 2]]).normalized;
                Vector3 localUp = Vector3.Cross(localForward, Vector3.up);
                if (localUp.sqrMagnitude < 0.00000001f) {
                    localUp = Vector3.Cross(localForward, Vector3.right);
                }
                localUp = localUp.normalized;
                Vector3 localRight = Vector3.Cross(localForward, localUp);

                for (int i = 0; i <= tubularSegments; i++)
                {
                    float dia = radius;

                    for (int j = 0; j < radialSegments; ++j)
                    {
                        float t = theta * j;
                        Vector3 vert = interpolatedPositions[i] + (Mathf.Sin(t) * localUp * dia) + (Mathf.Cos(t) * localRight * dia);
                        int x = k * radialSegments * (tubularSegments + 1) + i * radialSegments + j;
                        verts[x] = vert;
                        uvs[x] = uvScale * new Vector2(t / (Mathf.PI * 2), ((float)i) / ((float)tubularSegments));
                        normals[x] = Mathf.Sin(t) * localUp + Mathf.Cos(t) * localRight; //(vert - interpolatedPositions[i]).normalized;
                        
                        if (inside) normals[x] = -normals[x];

                        if (i >= tubularSegments) 
                            continue;

                        if (!inside)
                        {
                            tris[triIndex++] = x;
                            tris[triIndex++] = x + radialSegments;
                            tris[triIndex++] = x - j + (j + 1 + radialSegments) % radialSegments;

                            tris[triIndex++] = x;
                            tris[triIndex++] = x - j + (j - 1 + radialSegments) % radialSegments + radialSegments;
                            tris[triIndex++] = x + radialSegments;
                        }
                        else
                        {
                            tris[triIndex++] = x - j + (j + 1 + radialSegments) % radialSegments;
                            tris[triIndex++] = x + radialSegments;
                            tris[triIndex++] = x;

                            tris[triIndex++] = x + radialSegments;
                            tris[triIndex++] = x - j + (j - 1 + radialSegments) % radialSegments + radialSegments;
                            tris[triIndex++] = x;
                        }
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
        private void OnDrawGizmos()
        {
            if (showNodesInEditor)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < positions.Length; ++i)
                {
                    Gizmos.DrawSphere(transform.position + positions[i], radius);
                }
            }
        }

        private int PropHashCode()
        {
            return positions.Aggregate(0, (total, it) => total ^ it.GetHashCode()) 
                ^ indices.Aggregate(0, (total, it) => total ^ it.GetHashCode()) 
                ^ positions.GetHashCode() ^ positions.GetHashCode() 
                ^ tubularSegments.GetHashCode() ^ radialSegments.GetHashCode() 
                ^ radius.GetHashCode();
        }

        private void LateUpdate()
        {
            if (lastUpdate != PropHashCode())
            {
                meshFilter.mesh = CreateMesh();
            }
        }
    }
}