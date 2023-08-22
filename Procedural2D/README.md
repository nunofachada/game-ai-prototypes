# Procedural2D

## Summary

This example includes several fully parameterizable 2D procedural content
generation (PCG) scenarios, namely:

- Generation of random noise.
- Pseudo-random number generator (PRNG) correlation.
- Sample dispersion with quasi-RNGs.
- Landscapes with midpoint displacement.
- Pattern generation with 1D cellular automata (CA).
- Cave generation with 2D CA.
- Height maps with Perlin noise.
- Region demarcation with Voronoi diagrams, using either Euclidean or Manhattan
  distance.

In the cave generation scenario, for example, students can define different
neighborhoods, thresholds, generation steps, if the scenario is toroidal, and
so on. In all scenarios, students can select PRNGs of varying quality, as well
as save the generated content as an image. Each scenario is introduced after a
short exposition of the respective underlying algorithm or technique.

## Supporting code structure

Scenario classes extend [`AbstractScenario`] (or [`StochasticScenario`] if using
PRNGs) are automatically found via reflection, and can be configured in the
Unity editor. Adding new scenarios is simple, and consists of creating one such
class and placing it in the [`Assets/Scripts/Scenarios`] folder.

This prototype uses the `PCG`, `PRNG`, `QRNG`, and `Util` modules of the
[libgameai] library (MIT License).

## Other materials

- [Using the prototype (video)](https://youtu.be/8DcbAgghxnc)

[libgameai]:https://github.com/nunofachada/libgameai
[`AbstractScenario`]:Assets/Scripts/Scenarios/AbstractScenario.cs
[`StochasticScenario`]:Assets/Scripts/Scenarios/StochasticScenario.cs
[`Assets/Scripts/Scenarios`]:Assets/Scripts/Scenarios