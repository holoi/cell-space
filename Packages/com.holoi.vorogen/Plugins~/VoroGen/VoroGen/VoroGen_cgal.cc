// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Botao Amber Hu <botao@holoi.com>
// SPDX-License-Identifier: MIT

#include "VoroGen.h"

#include <vector>
#include <map>
#include <optional>

#include <CGAL/Exact_predicates_exact_constructions_kernel.h>
#include <CGAL/Regular_triangulation_3.h>
#include <CGAL/Polyhedron_3.h>
#include <CGAL/Polygon_mesh_processing/clip.h>
#include <CGAL/Polygon_mesh_processing/triangulate_faces.h>
#include <CGAL/convex_hull_3.h>

typedef CGAL::Exact_predicates_exact_constructions_kernel K;
typedef K::Point_3 Point_3;
typedef K::Iso_cuboid_3 Iso_cuboid_3;
typedef K::Segment_3 Segment_3;
typedef K::Ray_3 Ray_3;
typedef K::Line_3 Line_3;
typedef K::Vector_3 Vector_3;
typedef K::Plane_3 Plane_3;
typedef K::Triangle_3 Triangle_3;
typedef K::Weighted_point_3 Weighted_point_3;
typedef CGAL::Regular_triangulation_3<K> Regular_triangulation;
typedef CGAL::Polyhedron_3<K> Polyhedron;
typedef K::Intersect_3 Intersect_3;
typedef Regular_triangulation::Cell_handle Cell_handle;
typedef Regular_triangulation::Vertex_handle Vertex_handle;
typedef Regular_triangulation::Edge Edge;
typedef Regular_triangulation::Facet Facet;
typedef Regular_triangulation::Cell Cell;
typedef Regular_triangulation::Vertex Vertex;
typedef std::vector<Polyhedron> Voronoi_mesh_3;

std::optional<Point_3> intersection_with_bbox(const Ray_3& ray, const Polyhedron& bbox_polyhedron) {
    for (const auto& fh: bbox_polyhedron.facet_handles()) {
        //        CGAL::intersection(ray, bbox_polyhedron);
        auto tri = Triangle_3(fh->halfedge()->vertex()->point(),
                            fh->halfedge()->next()->vertex()->point(),
                            fh->halfedge()->next()->next()->vertex()->point());
        auto result = CGAL::intersection(ray, tri);
        
        if (result) {
            if (const Point_3* p = boost::get<Point_3>(&*result)) {
                return *p;
            }
        }
    }
    return std::nullopt;
}


Voronoi_mesh_3 compute_voronoi_mesh(const std::vector<Weighted_point_3>& wp, const Iso_cuboid_3& bbox, const float offset) {

    // Generate polyhedron of bounding box
    std::vector<Point_3> bbox_vertices;
    for (int i = 0; i < 8; i++) {
        bbox_vertices.push_back(bbox[i]);
    }
    Polyhedron bbox_polyhedron;
    CGAL::convex_hull_3(bbox_vertices.begin(), bbox_vertices.end(), bbox_polyhedron);
    
    // Triangulation 3D
    Regular_triangulation rt(wp.begin(), wp.end());
    
    // Results of Voronoi 3D
    Voronoi_mesh_3 voronoi_cells;
    
    // Iterate over all the vertex in Triangulation, which is the dual of polyhedron of Voronoi.
    // For each vertex, we generate a polyhedron
    
    for (int vi = 0; vi < wp.size(); vi++) {
        
        Vertex_handle vh;
        CGAL_assertion(rt.is_vertex(wp[vi], vh));
        
        std::vector<Cell_handle> incident_cells;
        rt.incident_cells(vh, std::back_inserter(incident_cells));
        std::vector<Point_3> cell_vertices;
        
        bool has_infinite_cell = false;
        for (const auto& ch : incident_cells) {
            if (!rt.is_infinite(ch)) {
                cell_vertices.push_back(ch->weighted_circumcenter());
            } else {
                has_infinite_cell = true;
            }
        }
        Polyhedron cell_polyhedron;
        
        //CGAL::convex_hull_3(cell_vertices.begin(), cell_vertices.end(), cell_polyhedron);
        std::vector<Edge> incident_edges;
        rt.incident_edges(vh, std::back_inserter(incident_edges));
        
//        for (const auto& eh : incident_edges) {
//            
//            // Store all dual segments on the facets of Triangulation.
//            // Store all dual segments on the facets of Triangulation.
////            std::vector<Point_3> polylines;
//            
//            // Iterator over all incident facets of the edges
//            auto fh = rt.incident_facets(eh);
//            auto done = fh;
//            
//            CGAL_For_all(fh, done) {
//                if (!rt.is_infinite(fh->first, fh->second)) {
//                    auto line = rt.dual(fh->first, fh->second);
//                    if (const Segment_3* segment = CGAL::object_cast<Segment_3>(&line)) {
//                       // cell_vertices.push_back(segment->start());
//                       // cell_vertices.push_back(segment->target());
//                    } else if (const Ray_3* ray = CGAL::object_cast<Ray_3>(&line)) {
//                        // cell_vertices.push_back(ray->start());
//                        
////                        auto intersection = intersection_with_bbox(*ray, bbox_polyhedron);
////                        if (intersection.has_value()) {
////                            cell_vertices.push_back(intersection.value());
////                        }
//                    }
//                }
//            }
//        }
                    
    }
    return voronoi_cells;
}


Voronoi_mesh_3 compute_voronoi_mesh_3(const std::vector<Weighted_point_3>& wp, const Iso_cuboid_3& bbox, const float offset) {

    // Generate polyhedron of bounding box
    std::vector<Point_3> bbox_vertices;
    for (int i = 0; i < 8; i++) {
        bbox_vertices.push_back(bbox[i]);
    }
    Polyhedron bbox_polyhedron;
    CGAL::convex_hull_3(bbox_vertices.begin(), bbox_vertices.end(), bbox_polyhedron);
    
    // Triangulation 3D
    Regular_triangulation rt(wp.begin(), wp.end());
    
    // Results of Voronoi 3D
    Voronoi_mesh_3 voronoi_cells;
    
    // Iterate over all the vertex in Triangulation, which is the dual of polyhedron of Voronoi.
    // For each vertex, we generate a polyhedron
    
    for (int vi = 0; vi < wp.size(); vi++) {
        
        Vertex_handle vh;
        CGAL_assertion(rt.is_vertex(wp[vi], vh));
        
        std::vector<Cell_handle> incident_cells;
        rt.incident_cells(vh, std::back_inserter(incident_cells));

        std::vector<Point_3> cell_vertices;
        bool has_infinite_cell = false;
        for (const auto& ch : incident_cells) {
            if (!rt.is_infinite(ch)) {
                cell_vertices.push_back(ch->weighted_circumcenter());
            } else {
                has_infinite_cell = true;
            }
        }
        
        if (!has_infinite_cell) {
            
            Polyhedron cell_polyhedron;
            CGAL::convex_hull_3(cell_vertices.begin(), cell_vertices.end(), cell_polyhedron);

            CGAL::Polygon_mesh_processing::clip(cell_polyhedron, bbox_polyhedron, CGAL::parameters::clip_volume(true), CGAL::parameters::clip_volume(true));
            voronoi_cells.push_back(cell_polyhedron);
            continue;
        }
        
        // Iterate over all incident edges of the vertex in Triangulation, which is dual of facets of Voronoi
        std::vector<Edge> incident_edges;
        rt.incident_edges(vh, std::back_inserter(incident_edges));
        
        for (const auto& eh : incident_edges) {
            
            // Store all dual segments on the facets of Triangulation.
//            std::vector<Point_3> polylines;
            
            // Iterator over all incident facets of the edges
            auto fh = rt.incident_facets(eh);
            auto done = fh;
            
            CGAL_For_all(fh, done) {
                if (!rt.is_infinite(fh->first, fh->second)) {
                    auto line = rt.dual(fh->first, fh->second);
                    if (const Segment_3* segment = CGAL::object_cast<Segment_3>(&line)) {
                       // cell_vertices.push_back(segment->start());
                       // cell_vertices.push_back(segment->target());
                    } else if (const Ray_3* ray = CGAL::object_cast<Ray_3>(&line)) {
                        // cell_vertices.push_back(ray->start());
                        
                        auto intersection = intersection_with_bbox(*ray, bbox_polyhedron);
                        if (intersection.has_value()) {
                            cell_vertices.push_back(intersection.value());
                        }
                    }
                }
            }
        }
                    
        Polyhedron cell_polyhedron;
        CGAL::convex_hull_3(cell_vertices.begin(), cell_vertices.end(), cell_polyhedron);

        CGAL::Polygon_mesh_processing::clip(cell_polyhedron, bbox_polyhedron, CGAL::parameters::clip_volume(true), CGAL::parameters::clip_volume(true));
        voronoi_cells.push_back(cell_polyhedron);
    }
    return voronoi_cells;
}

Voronoi_mesh_3 compute_voronoi_mesh_2(const std::vector<Weighted_point_3>& wp, const Iso_cuboid_3& bbox, const float offset) {

    // Generate polyhedron of bounding box
    std::vector<Point_3> bbox_vertices;
    for (int i = 0; i < 8; i++) {
        bbox_vertices.push_back(bbox[i]);
    }
    Polyhedron bbox_polyhedron;
    CGAL::convex_hull_3(bbox_vertices.begin(), bbox_vertices.end(), bbox_polyhedron);
    
    // Triangulation 3D
    Regular_triangulation rt(wp.begin(), wp.end());
    
    // Results of Voronoi 3D
    Voronoi_mesh_3 voronoi_cells;
    
    // Iterate over all the vertex in Triangulation, which is the dual of polyhedron of Voronoi.
    // For each vertex, we generate a polyhedron
    
    for (int vi = 0; vi < wp.size(); vi++) {
        
        Vertex_handle vh;
        CGAL_assertion(rt.is_vertex(wp[vi], vh));

        // Initialize as Bbox
        Polyhedron cell_polyhedron = bbox_polyhedron;
        
        std::vector<Cell_handle> incident_cells;
        rt.incident_cells(vh, std::back_inserter(incident_cells));

        std::vector<Point_3> cell_vertex;
        bool has_infinite_cell = false;
        for (const auto& ch : incident_cells) {
            if (!rt.is_infinite(ch)) {
                cell_vertex.push_back(ch->weighted_circumcenter());
            } else {
                has_infinite_cell = true;
            }
        }
        if (!has_infinite_cell) {
            CGAL::convex_hull_3(cell_vertex.begin(), cell_vertex.end(), cell_polyhedron);
            CGAL::Polygon_mesh_processing::clip(cell_polyhedron, bbox_polyhedron, CGAL::parameters::clip_volume(true), CGAL::parameters::clip_volume(true));
            voronoi_cells.push_back(cell_polyhedron);
            continue;
        }
        
        // Iterate over all incident edges of the vertex in Triangulation, which is dual of facets of Voronoi
        std::vector<Edge> incident_edges;
        rt.incident_edges(vh, std::back_inserter(incident_edges));
        
        for (const auto& eh : incident_edges) {

            // Store all dual segments on the facets of Triangulation.
            std::vector<Point_3> polylines;

            // Iterator over all incident facets of the edges
            auto fh = rt.incident_facets(eh);
            auto done = fh;
            
            CGAL_For_all(fh, done) {
                if (!rt.is_infinite(fh->first, fh->second)) {
                    auto line = rt.dual(fh->first, fh->second);
                    if (const Segment_3* segment = CGAL::object_cast<Segment_3>(&line)) {
                        if (polylines.size() == 0) {
                            polylines.push_back(segment->start());
                        }
                        polylines.push_back(segment->target());
                    }
                }
            }
                        
            if (polylines.size() < 3) {
                CGAL_For_all(fh, done) {
                    if (polylines.size() < 3 && !rt.is_infinite(fh->first, fh->second)) {
                        auto line = rt.dual(fh->first, fh->second);
                        if (const Ray_3* ray = CGAL::object_cast<Ray_3>(&line)) {
                            if (polylines.size() == 0) {
                                polylines.push_back(ray->start());
                            }
                            Vector_3 step = ray->direction().vector();
                            polylines.push_back(ray->start() + step);
                        }
                    }
                }
            }
            if (polylines.size() >= 3) {
                Plane_3 plane = Plane_3(polylines[0], polylines[1], polylines[2]);
                
                if (!plane.has_on_positive_side(vh->point().point())) {
                    plane = plane.opposite();
                }

                /// CGAL::sqrt(plane.orthogonal_vector().squared_length()) *
                Point_3 newBase = plane.projection(vh->point().point())
                              + plane.orthogonal_vector() * offset;

                plane = Plane_3(newBase, plane.orthogonal_vector()).opposite();
                CGAL::Polygon_mesh_processing::clip(cell_polyhedron, plane, CGAL::parameters::clip_volume(true));
            }
        }
        voronoi_cells.push_back(cell_polyhedron);
    }
    return voronoi_cells;
}

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
                                int** trianglesOut, int** numTrianglesOut) {
        
        // Generate Voroinoi Cells
        std::vector<Weighted_point_3> wp;
        for (int i = 0; i < numPoints; i++) {
            wp.push_back(Weighted_point_3(Point_3(weighted_points[i * 4], weighted_points[i * 4 + 1], weighted_points[i * 4 + 2]), weighted_points[i * 4 + 3]));
        }
        Iso_cuboid_3 bbox(Point_3(minX, minY, minZ), Point_3(maxX, maxY, maxZ));
        Voronoi_mesh_3 voronoi = compute_voronoi_mesh(wp, bbox, offset);
        
        auto vertices = std::vector<std::vector<float>>(numPoints, std::vector<float>());
        auto triangles = std::vector<std::vector<int>>(numPoints, std::vector<int>());
        
        int totalVerticesOut = 0;
        int totalTrianglesOut = 0;
        for (int vi = 0; vi < voronoi.size(); vi++) {
            const Polyhedron& cell_polyhedron = voronoi[vi];
            
            std::map<Polyhedron::Vertex_const_handle, int> vertex_indices;
                        
            for (const auto& vh: cell_polyhedron.vertex_handles()) {
                vertex_indices[vh] = (int) vertex_indices.size();
                Point_3 p = vh->point();
                
                vertices[vi].push_back(CGAL::to_double(p.x()));
                vertices[vi].push_back(CGAL::to_double(p.y()));
                vertices[vi].push_back(CGAL::to_double(p.z()));
            }
            totalVerticesOut += vertices[vi].size();
            
            for (const auto& fh: cell_polyhedron.facet_handles()) {
                int a = vertex_indices[fh->halfedge()->vertex()];
                int b = vertex_indices[fh->halfedge()->next()->vertex()];
                int c = vertex_indices[fh->halfedge()->next()->next()->vertex()];
                
                CGAL_assertion(fh->is_triangle());
                
                triangles[vi].push_back(a);
                triangles[vi].push_back(b);
                triangles[vi].push_back(c);
            }
            totalTrianglesOut += triangles[vi].size();
        }
        
        *numVerticesOut = new int[numPoints];
        *numTrianglesOut = new int[numPoints];
        *verticesOut = new float[totalVerticesOut];
        *trianglesOut = new int[totalTrianglesOut];
        
        totalVerticesOut = 0;
        totalTrianglesOut = 0;
        for (int vi = 0; vi < numPoints; vi++) {
            for (auto ver: vertices[vi]) {
                (*verticesOut)[totalVerticesOut++] = ver;
            }
            (*numVerticesOut)[vi] = (int) vertices[vi].size();
            for (auto tri: triangles[vi]) {
                (*trianglesOut)[totalTrianglesOut++] = tri;
            }
            (*numTrianglesOut)[vi] = (int) triangles[vi].size();
        }
    }
}

