// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

#ifndef VoroGen_h
#define VoroGen_h

extern "C" {
    void VoroGen_Hello();

    void VoroGen_FreeMemory(void** ptr);

    void VoroGen_ComputeDelaunay(float* weighted_points, int numPoints,
                        float minX, float maxX,
                        float minY, float maxY,
                        float minZ, float maxZ,
                        int** edgesOut, int** numEdgesOut);

    void VoroGen_ComputeVoronoi(float* weighted_points, int numPoints,
                            float minX, float maxX,
                            float minY, float maxY,
                            float minZ, float maxZ,
                            float offset,
                            float** verticesOut, int** numVerticesOut,
                            int** trianglesOut, int** numTrianglesOut,
                            int** linesOut, int** numLinesOut,
                            int** edgesOut, int** numEdgesOut);
   

}
#endif /* VoroGen_h */
