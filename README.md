# Voice Leading Class Library
A voice leading algorithm for guitar (or any stringed instrument), written in C#. 

## Example 
You give it a starting guitar chord fingering, such as the following D min:

```
X
6
7
7
5
x
```

Then you give it a target chord, like the following for A7:

Root = A,
Intervals = 1, 3, 5, b7 

The algorithm then finds all guitar chord fingerings for A7 which lead back to the starting Dmin chord with good voice leading.

## Example Results
Here is a sample of some of the A7 chord fingerings found:

A7 Voicing 1 (with multiple fingerings)

```
X     0     3
8     8     5
9     X     2
7     7     x
X     4     4
9     X     X     etc...
```

A7 Voicing 2 (with multiple fingerings)

```
X     3     3
8     X     2
6     6     2
7     7     2
7     7     X
X     X     X     etc...
```

There are many more. All of the generated chord fingerings for A7 lead back to the starting Dmin chord with good voiceleading. This means that the notes jump by short distances. Note splitting and note convergence possibilities are also taken into account.

I hope to give code examples and document the configuration options.

I use this algorithm in my <a target="_blank" href="http://frankmodica.azurewebsites.net/voiceleader/index">Guitar Voiceleading Helper</a>.
