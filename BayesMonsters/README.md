# Bayes Monsters

## Summary

A simple game in which the player needs to defend itself against two enemy
types, demons, and dragons, which can attack at different speeds. The proper
defense, either a sword or a bow and arrow, depends on both the type of enemy
and its speed. After a few games, the human player becomes proficient and is
able to obtain better scores. However, the moves of the human player are used to
train a naive Bayes classifier, which, after a few rounds of observations, can
play the game by itself—at a similar proficiency to the human that played
before.

## Supporting code structure

This prototype uses:

- The `NaiveBayes` module of the [libgameai] library (MIT License).
- Various sprites available at [Flaticon], namely:
  - The [Dragon icon] created by [Nikita Golubev].
  - The [Devil icon] created by [Freepik].
  - The [Bow icon] created by [Smashicons].
  - The [Sword icon] created by [Freepik].

## Other materials

- [Using the prototype (video)](https://youtu.be/S0yA2MWCPf0)

[libgameai]:https://github.com/nunofachada/libgameai
[Dragon icon]:https://www.flaticon.com/free-icons/dragon
[Devil icon]:https://www.flaticon.com/free-icons/devil
[Bow icon]:https://www.flaticon.com/free-icons/archer
[Sword icon]:https://www.flaticon.com/free-icons/sword
[Nikita Golubev]:https://www.flaticon.com/authors/nikita-golubev
[Flaticon]:https://www.flaticon.com/
[Freepik]:https://www.flaticon.com/authors/freepik
[Smashicons]:https://www.flaticon.com/authors/smashicons
