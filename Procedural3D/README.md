# Procedural3D

## Summary

This prototype displays a textured 3D landscape, rendered from a 2D height map.
Students can stack and parameterize arbitrary generation layers, each layer able
to modify existing heights according to the specified algorithm. Currently, four
stackable algorithms are implemented:

- Diamond-square.
- Perlin noise (via [Unity's implementation][uperlin]).
- Fault generation.
- Cellular automata.
- Per Bak sandpile (very slow, and not very good results)

This prototype is used to showcase each technique after an in-lecture exposition
of the underlying algorithm.

## Supporting code and assets

Terrain layer classes extending [`AbstractGenConfig`] (or
[`StochasticGenConfig`] if using PRNGs) are automatically found via reflection,
and can be configured in the Unity editor. Adding new types of layer is simple,
and consists of creating one such class and placing it in the
[`Assets/Scripts/GenConfig`] folder.

This prototype uses:

- The `PCG` and `Utils` module of the [libgameai] library (MIT License).
- [Mysteryem](https://opengameart.org/node/8070) by
  [qubodup](https://opengameart.org/users/qubodup)
  ([CC-BY-SA 3.0](http://creativecommons.org/licenses/by-sa/3.0/))

## Other materials

- [Using the prototype (video)](https://youtu.be/ucg_KcJSSTs) (deprecated)

[libgameai]:https://github.com/nunofachada/libgameai
[height maps]:https://docs.unity3d.com/Manual/Textures.html#TerrainHeightmaps
[uperlin]:https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
[`AbstractGenConfig`]:Assets/Scripts/GenConfig/AbstractGenConfig.cs
[`StochasticGenConfig`]:Assets/Scripts/GenConfig/StochasticGenConfig.cs
[`Assets/Scripts/GenConfig`]:Assets/Scripts/GenConfig
