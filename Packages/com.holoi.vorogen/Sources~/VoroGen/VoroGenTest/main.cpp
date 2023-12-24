#include "VoroGen.h"

#include <chrono>
#include <iostream>
#include <random>

//void test() {
//    std::vector<Weighted_point_3> points;
//    
//    points.push_back(Weighted_point_3(Point_3(16.6455, 13.7577, 1.68993), 1));
//    points.push_back(Weighted_point_3(Point_3(23.3496, 18.7361, 10.54947), 1));
//    points.push_back(Weighted_point_3(Point_3(21.3651, 13.872, 30.59779), 1));
//    points.push_back(Weighted_point_3(Point_3(14.404, 21.8245, 1.58279), 1));
//    
//
////    points.push_back(Weighted_point_3(Point_3(19.6542, 10.9324, 40.53077), 1));
////
////    points.push_back(Weighted_point_3(Point_3(8.34219, 16.9751, 0.55933), 1));
////    points.push_back(Weighted_point_3(Point_3(7.05614, 8.69331, 1.55608), 1));
////    points.push_back(Weighted_point_3(Point_3(18.71, 10.7257, 1.56997), 16));
////
////    points.push_back(Weighted_point_3(Point_3(14.5137, 7.4029, 1.50375), 9));
////    points.push_back(Weighted_point_3(Point_3(15.7983, 16.2696, 1.52043), 4));
////    points.push_back(Weighted_point_3(Point_3(23.81, 10.0232, 1.57585), 1));
////
////    points.push_back(Weighted_point_3(Point_3(23.543, 7.93398, 1.91253), 1));
////    points.push_back(Weighted_point_3(Point_3(9.0225, 15.0061, 1.1218), 1));
////    points.push_back(Weighted_point_3(Point_3(13.5859, 14.6321, 1.6586), 1));
//    
//    Iso_cuboid_3 bbox(Point_3(0, 0, 0), Point_3(50, 50, 50));
//
//    Voronoi_mesh_3 voronoi = compute_voronoi_mesh(points, bbox, 0.1);
//}

void test() {
    std::uniform_real_distribution<float> real_dist(0, 50.0);
    std::random_device myRandomDevice;
    unsigned seed = myRandomDevice();
    std::default_random_engine random_engine(seed);

    int numPoints = 14;
    float* weighted_points = new float[numPoints * 4];
    for (int i = 0; i < numPoints; i++) {
        weighted_points[i * 4] = real_dist(random_engine);
        weighted_points[i * 4 + 1] = real_dist(random_engine);
        weighted_points[i * 4 + 2] = real_dist(random_engine);
        weighted_points[i * 4 + 3] = 1;
    }

    // float* weighted_points = new float[] {
    //     19.6542, 10.9324, 40.53077, 1,
    //     23.3496, 18.7361, 10.54947, 1,
    //     21.3651, 13.872, 30.59779, 1, 
    //     14.404, 21.8245, 1.58279, 1,
    //     19.6542, 10.9324, 40.53077, 1,
    //     8.34219, 16.9751, 0.55933, 1,
    //     7.05614, 8.69331, 1.55608, 1, 
    //     18.71, 10.7257, 1.56997, 1,
    //     14.5137, 7.4029, 1.50375, 1, 
    //     15.7983, 16.2696, 1.52043, 1,
    //     23.81, 10.0232, 1.57585, 1,
    //     23.543, 7.93398, 1.91253, 1,
    //     9.0225, 15.0061, 1.1218, 1, 
    //     13.5859, 14.6321, 1.6586, 1
    // };
    int* edgesOut;
    int* numEdgesOut;

    VoroGen_ComputeDelaunay(weighted_points, numPoints,
                        0, 50, 0, 50, 0, 50,
                        &edgesOut, &numEdgesOut);
    
    std::cout << "Delaunay" << std::endl;
    int totalEdges = 0;
    for (int i = 0; i < numPoints; i++) {
       for (int j = 0; j < numEdgesOut[i]; j++) {
           std::cout << edgesOut[totalEdges + j] << " ";
       }
       totalEdges += numEdgesOut[i];
       std::cout << std::endl;
    }
    VoroGen_FreeMemory((void**) edgesOut);
    VoroGen_FreeMemory((void**) numEdgesOut);

    float* verticesOut;
    int* numVerticesOut;
    int* trianglesOut;
    int* numTrianglesOut;
    int* linesOut;
    int* numLinesOut;

    VoroGen_ComputeVoronoi(weighted_points, numPoints,
                        0, 50, 0, 50, 0, 50,
                        0.0,
                        &verticesOut, &numVerticesOut,
                        &trianglesOut, &numTrianglesOut,
                        &linesOut, &numLinesOut);

    int totalVertices = 0;
    int totalTriangles = 0;
    int totalLines = 0;
    for (int i = 0; i < numPoints; i++) {
//        for (int j = 0; j < numVerticesOut[i]; j++) {
//            std::cout << verticesOut[totalVertices + j] << " ";
//        }
//        std::cout << std::endl;
//        for (int j = 0; j < numTrianglesOut[i]; j++) {
//            std::cout << trianglesOut[totalTriangles + j] << " ";
//        }
//        std::cout << std::endl;
//        for (int j = 0; j < numLinesOut[i]; j++) {
//            std::cout << linesOut[totalLines + j] << " ";
//        }
//        std::cout << std::endl;

        totalVertices += numVerticesOut[i];
        totalTriangles += numTrianglesOut[i];
        totalLines += numLinesOut[i];
    }
    delete [] weighted_points;
    VoroGen_FreeMemory((void**) verticesOut);
    VoroGen_FreeMemory((void**) numVerticesOut);
    VoroGen_FreeMemory((void**) trianglesOut);
    VoroGen_FreeMemory((void**) numTrianglesOut);
    VoroGen_FreeMemory((void**) linesOut);
    VoroGen_FreeMemory((void**) numLinesOut);
}



int main() {
    auto m_StartTime = std::chrono::system_clock::now();
    
    for (int i = 0; i < 10000; i++) {
        test();
    }
    auto m_EndTime = std::chrono::system_clock::now();

    auto counter = std::chrono::duration_cast<std::chrono::milliseconds>(m_EndTime - m_StartTime).count() / 1000.0;
    
    std::cout << counter << std::endl;
}

