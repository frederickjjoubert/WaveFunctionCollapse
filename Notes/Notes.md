# Wave Function Collapse

Date: 2022-06-28 @ 08:38

Original Implementation ([https://github.com/mxgmn/WaveFunctionCollapse](https://github.com/mxgmn/WaveFunctionCollapse)) by [Maxim Gumin](https://github.com/mxgmn)

See also [Maxim Gumin Twitter](https://twitter.com/ExUtumno)

---

## Research

---

Still need to watch:

- <https://www.youtube.com/watch?v=AdCgi9E90jw&t=426s>
- <https://www.youtube.com/watch?v=fnFj3dOKcIQ>
- <https://www.youtube.com/watch?v=0bcZb-SsnrA>
-

---

### [Coding Challenge 171: Wave Function Collapse](https://www.youtube.com/watch?v=rI_y2GAlQFM) by [The Coding Train](https://www.youtube.com/c/TheCodingTrain)

Tiled Model (The one I made in unity) and the Overlap Model (the one that uses NxN)

In this video, Daniel makes the Tiled Model, which I already put together.

Overall it's pretty much the same high level implementation I did

Some interesting tid bits:

- The Tiles themselves have a function called analyze which takes in an array of all possible (including itself) and from that goes through all possible tiles and populates their own adjacency rules (i.e. valid neighbors)
- He does use a socket system with 3 values, Ex. AAA, ABA, AAB, BAA, and checks against the reverse of an edge to see if they can fit.

References:

- <https://discourse.processing.org/t/wave-collapse-function-algorithm-in-processing/12983>

---

### [Superpositions, Sudoku, the Wave Function Collapse algorithm.](https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=194s) by [Martin Donald](https://www.youtube.com/c/MartinDonald)

### Demos

- [https://bolddunkley.itch.io/wfc-mixed](https://bolddunkley.itch.io/wfc-mixed)
- [https://bolddunkley.itch.io/wave-function-collapse](https://bolddunkley.itch.io/wave-function-collapse)

### References

- [The Wavefunction Collapse Algorithm explained very clearly by Robert Heaton](https://robertheaton.com/2018/12/17/wavefunction-collapse-algorithm/)

### Terminology

Modules:

- The individual pieces

Sockets or Slots: A unique identifier for a square or cubes own faces.

- Tag symmetrical sockets with an s
- Tag asymmetrical sockets with an f
- Vertical sockets need a v and a rotation index

Prototypes are the metadata for our modules

- Contain which mesh to use
- The rotation to use
- The list of valid neighbors
  - Note: This can be created in code, no need to do it by hand. Create this list from sockets, the relevant rules, and make an algorithm to output the lists for each prototype.
- Output all your prototype data as JSON

### Setup

Can be done in 2d or 3d, 3d obviously just takes more time to set up.

1. Create Constraints / Rules, Modules, and Prototypes (Modules that point to the same mesh, but with different rotations)
2. Create Tiles or Cubes (Meshes)
3. Assign Sockets
4. Create algorithm to output neighbor lists for each prototype
5. Output JSON prototype data
6. Create a grid (like Sudoku) of a certain width and height with each coordinate containing all possible prototypes, load in your prototype data JSON

### Algorithm

- While the wave function hasn't collapsed, Iterate
  - Check it hasn't collapsed by checking each index, if any index still contains more than one possible prototype, the wave function has not yet collapsed.
- Iterate:
  - Pick a coordinate with the lowest possible entropy (randomly)
  - Collapse at coordinates by randomly or weighted pick a prototype to collapse that coordinate.
  - Propagate* this collapse to all neighbors, and so on, until all neighbors have been updated
    - This is the "beating heart" of the WFC algorithm
    - ![[wfc_propagate.png]]

Post Process:

- Make sure to rotate each prototype mesh according to its rotation index

Notice: Most of the complexity lies in creating the adjacency constraints.

Note: This example didn't cover any cases of contradiction in tile / mesh placement. Sometimes, it won't be able to properly collapse, need to guard against that and have some sort of back tracking or just start over.

- I believe a contradiction manifests after the propagation step, in which a specific coordinate will have no possible prototypes left, as opposed to just 1 which is what we are aiming for.
- I worry about this if I get to a point where I want to use this as part of chunking system, since new chunks will need to depend on the edges of already generated chunks, might in rare cases lead to a new chunk having problems.
- Note from the Original Author Maxim Gumin: "It may happen that during propagation all the coefficients for a certain pixel become zero. That means that the algorithm has run into a contradiction and can not continue. The problem of determining whether a certain bitmap allows other nontrivial bitmaps satisfying condition (C1) is NP-hard, so it's impossible to create a fast solution that always finishes. In practice, however, the algorithm runs into contradictions surprisingly rarely."

---

### [The Wavefunction Collapse Algorithm explained very clearly](https://robertheaton.com/2018/12/17/wavefunction-collapse-algorithm/)

By Robert Heaton

Code: [https://github.com/robert/wavefunction-collapse](https://github.com/robert/wavefunction-collapse)

Notes:

- "You keep repeating this process until either every seat’s wavefunction is collapsed (meaning that it has exactly 1 person sitting in it), or until you reach a _contradiction_. A contradiction is a seat in which nobody is able to sit, because they have all been ruled out by your previous choices. A contradiction makes fully collapsing the entire wavefunction impossible."
  - "If you reach a contradiction then the easiest thing to do is to start again. Throw away your work so far, find a new blank table plan, and re-start the algorithm by collapsing the wavefunction for a different random seat. You could also implement a backtracking system that allows you to undo individual choices instead of discarding everything (“well what happens if Shilpa goes in seat 54 instead?”). "But that is problem unto itself.
- "When analyzing the input image, we also need to record the frequency at which each of its tiles appears. We will later use these numbers as weights when deciding which square’s wavefunction to collapse, and when choosing which tile to assign to a square when it is being collapsed."

---

### [Why I'm Using Wave Function Collapse for Procedural Terrain | Unity Devlog](https://www.youtube.com/watch?v=20KHNA9jTsE)

Cool tid bit on Marching Cubes vs. Voxel/Boxel vs. WFC

---

Navigation Heuristic

- <https://twitter.com/OskSta/status/917405214638006273>

Tiled vs. Overlap Model

- <https://twitter.com/exppad/status/1267045322116734977>

---

## Existing Solutions

---

Possible Ready Made Solutions:

- [Wave Function Collapse content generator for Unity](https://github.com/oddmax/unity-wave-function-collapse-3d)
  - Seems like a good ready made solution, but I don't think this is so complex I can't write my own version of it.
- [Wave Function Collapse C# Library by Boris the Brave](https://boristhebrave.github.io/DeBroglie/articles/index.html)
  - This looks very robust and powerful, might be the way to go rather than making my own inefficient solution.
  - <https://www.nuget.org/packages/DeBroglie>
  - <https://stackoverflow.com/questions/53447595/nuget-packages-in-unity>
