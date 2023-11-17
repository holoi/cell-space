# Voronoi Generator for Cell Space

We use Mathematica to simulate the cell space.
```Mathematica
n = 12; nframe = 200; width = 30; height = 5; length = 30; margin = 6; 
averageheightofuser = 1.6; steplengthofuser = 0; heightmovementofuser \
= 0.3; pts = 
 Join[Join[RandomReal[{0 + margin, length - margin}, {n, 1}], 
   RandomReal[{0 + margin, width - margin}, {n, 1}], 2], 
  RandomReal[{averageheightofuser - 0.1, 
    averageheightofuser + 0.1}, {n, 1}], 2];
deltapaths = 
  Transpose[
   Function[deltastep, 
     Join[Accumulate[
       RandomReal[{-steplengthofuser - deltastep, 
         steplengthofuser + deltastep}, {nframe, 2}]], 
      Accumulate[
       RandomReal[{-heightmovementofuser, 
         heightmovementofuser}, {nframe, 1}]], 2]] /@ {0.2, 0.2, 0.2, 
     0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.00, 0.00, 0.00}];
pathpts = Function[deltapt, pts + deltapt] /@ deltapaths;
frames = 
  Function[pts, 
    voronoi = 
     VoronoiMesh[pts, {{0, length}, {0, width}, {0, height}},
      MeshCellStyle -> {{1, All} -> Black, {2, All} -> 
         Opacity[0.2, White]}, MeshCellLabel -> {}, 
      PlotTheme -> "Lines"]; 
    delaunay = 
     DelaunayMesh[pts, 
      MeshCellStyle -> {{1, All} -> Red, {2} -> Opacity[0]}];
    Show[voronoi, delaunay, 
     Graphics3D[{Blue, PointSize[Large], Point[pts]}]]] /@ pathpts;

ListAnimate[frames, 60]
```

<img width="377" alt="image" src="https://github.com/holoi/cell-space/assets/2534431/08df057b-c383-474f-8b5b-4176072ceb36">
<img width="384" alt="image" src="https://github.com/holoi/cell-space/assets/2534431/b40a17b3-dac0-4436-acde-215a8a8741ef">

![fg](https://github.com/holoi/cell-space/assets/2534431/c2e97fb1-52d8-4898-88d1-f23fd1a8157d)


