# TicTacToe

## Summary

This prototype is initially provided to students with a human and a random
player, but can automatically detect additional players implemented according to
a [specific interface][`IPlayer`] (and can measure the execution time of any
player). The prototype is used in-between expositions of Minimax-style
algorithms and their various optimizations, board game heuristics, and Monte
Carlo Tree Search (MCTS), where students are asked to implement:

- [Negamax]—a slightly optimized version of Minimax.
- [Negamax with alpha-beta pruning][NegamaxAB].
- A [simple heuristic], for avoiding infinite search depth.

Students observe that play processing time decreases with the various
improvements—from a few seconds without any optimization, to almost no delay
with alpha-beta pruning and limited depth with a heuristic.

An [MCTS] implementation is provided after its in-lecture exposition so that
students can compare it with the previous algorithms. MCTS is not implemented in
class since it is considerably more complex than Minimax-style algorithms.

## Supporting code and assets

AI players need only to implement the [`IPlayer`] interface, and are
automatically discovered via reflection.

## Other materials

- [Using the prototype (video)](https://youtu.be/qfgWUM3qHNk)

[`IPlayer`]:Assets/Scripts/IPlayer.cs
[MCTS]:Assets/Scripts/MCTSAIPlayer.cs
[Negamax]:Assets/Scripts/NegamaxAIPlayer.cs
[NegamaxAB]:Assets/Scripts/ABNegamaxAIPlayer.cs
[simple heuristic]:Assets/Scripts/AvailableLinesHeuristic.cs