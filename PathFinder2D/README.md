# PathFinder2D

## Summary

This prototype displays a 2D grid environment, with an agent trying to find the
exit through several obstacles. Students can:

- Configure the grid size.
- Interactively place/remove obstacles.
- Change the pathfinding algorithm (_Dijkstra_ or _A*_)
- In the case of _A*_:
  - Specify whether to perform an early exit (select the destination as soon as
    it enters the list of open nodes).
  - Change the employed heuristic (_Euclidean_ or _Manhattan_ distance).
  - Modify the weight of the heuristic against the current cost of a given path.

If gizmos are activate, the prototype shows the path to the destination, as well
as the tiles (i.e., nodes) being evaluated, therefore allowing students to
assess the selected algorithm’s efficiency.

## Supporting code and assets

This prototype uses:

- The `PathFinding` module of the [libgameai] library (MIT License).
- Sprites created by Jerome, PancInteractive (Alex Jackson), available at
  <https://opengameart.org/content/additions-to-32x32-fantasy-tileset>
  (CC BY-SA 3.0).

## Other materials

- [Using the prototype (video)](https://youtu.be/912M4zDCLB4)

[libgameai]:https://github.com/nunofachada/libgameai