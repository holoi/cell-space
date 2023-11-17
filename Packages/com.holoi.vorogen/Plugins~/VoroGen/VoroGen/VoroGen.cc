// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

#include "voro++.hh"
#include "VoroGen.h"

extern "C" {

    void VoroGen_FreeMemory(void** ptr) {
        delete [] ptr;
    }

    void VoroGen_ComputeVoronoi(float* weighted_points, int numPoints,
                                float minX, float maxX,
                                float minY, float maxY,
                                float minZ, float maxZ,
                                float offset,
                                float** verticesOut, int** numVerticesOut,
                                int** trianglesOut, int** numTrianglesOut,
                                int** linesOut, int** numLinesOut) {
        
        // Generate Voroinoi Cells
        int n_x = 6, n_y = 6, n_z = 6;
        voro::container_poly con(minX, maxX, minY, maxY, minZ, maxZ, n_x, n_y, n_z, false, false, false, 8);
        
        for (int i = 0; i < numPoints; i++) {
            con.put(i, weighted_points[i * 4], weighted_points[i * 4 + 1], weighted_points[i * 4 + 2], weighted_points[i * 4 + 3]);
        }
        
        auto vertices = std::vector<std::vector<float>>(numPoints, std::vector<float>());
        auto triangles = std::vector<std::vector<int>>(numPoints, std::vector<int>());
        auto lines = std::vector<std::vector<int>>(numPoints, std::vector<int>());

        voro::c_loop_all vl(con);
        int vi = 0;
        if (vl.start()) {
            do {
                voro::voronoicell cell;
                if (!con.compute_cell(cell, vl)) {
                    break;
                }
                double* site_ptr = con.p[vl.ijk] + con.ps * vl.q;
                                
                if (cell.p > 0) {
                    double* cellpts_ptr = cell.pts;
                    
                    // Add vertices
                    for(int i = 0; i < cell.p; i++, cellpts_ptr += 3) {
                        for (int d = 0; d < 3; d++) {
                            vertices[vi].push_back(site_ptr[d] + cellpts_ptr[d] * (0.5 - offset));
                        }
                    }
                    
                    // Add WireFrame
                    for(int i = 1; i < cell.p; i++){
                        for(int j = 0; j < cell.nu[i]; j++) {
                            int k = cell.ed[i][j];
                            
                            if( k >= 0 ) {
                                lines[vi].push_back(i);
                                lines[vi].push_back(k);
                            }
                        }
                    }

                    // Add Triangles
                    for(int i = 1; i < cell.p; i++) {
                        for(int j = 0; j < cell.nu[i]; j++) {
                            
                            int k = cell.ed[i][j];
                            if (k >= 0) {
                                cell.ed[i][j] = -1 - k;
                                int l = cell.cycle_up( cell.ed[i][cell.nu[i] + j], k);
                                int m = cell.ed[k][l];
                                cell.ed[k][l] = -1 - m;
                                
                                while (m != i) {
                                    int n = cell.cycle_up(cell.ed[k][cell.nu[k]+l], m);
                                    triangles[vi].push_back(i);
                                    triangles[vi].push_back(k);
                                    triangles[vi].push_back(m);
                                    
                                    k = m;
                                    l = n;
                                    m = cell.ed[k][l];
                                    cell.ed[k][l] = -1 - m;
                                }
                            }
                        }
                    }
                }
                vi++;
            } while (vl.inc());
        }
        
        // Calculate total size
        int totalCell = vi;
        int totalVerticesOut = 0;
        int totalTrianglesOut = 0;
        int totalLinesOut = 0;
        for (int vi = 0; vi < totalCell; vi++) {
            totalVerticesOut += vertices[vi].size();
            totalTrianglesOut += triangles[vi].size();
            totalLinesOut += lines[vi].size();
        }
        
        // Allocate memory
        *numVerticesOut = new int[numPoints];
        *numTrianglesOut = new int[numPoints];
        *numLinesOut = new int[numPoints];

        *verticesOut = new float[totalVerticesOut];
        *trianglesOut = new int[totalTrianglesOut];
        *linesOut = new int[totalLinesOut];

        //Output Mesh
        totalVerticesOut = 0;
        totalTrianglesOut = 0;
        totalLinesOut = 0;
        for (int vi = 0; vi < totalCell; vi++) {

            for (auto verticeContent: vertices[vi]) {
                (*verticesOut)[totalVerticesOut++] = verticeContent;
            }
            (*numVerticesOut)[vi] = (int) vertices[vi].size();
            
            for (auto triangleIndice: triangles[vi]) {
                (*trianglesOut)[totalTrianglesOut++] = triangleIndice;
            }
            (*numTrianglesOut)[vi] = (int) triangles[vi].size();

            for (auto lineIndice: lines[vi]) {
                (*linesOut)[totalLinesOut++] = lineIndice;
            }
            (*numLinesOut)[vi] = (int) lines[vi].size();
        }
    }

}
