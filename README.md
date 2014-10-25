F# chess
========

## Introduction

Just a simple project to try F# and get more familiar with functional paradigm. UI and tests are written in C#, core logic in F#. Project is not finished, it lacks f.e.

* check and checkmate detection
* different types of moves f.e. promotion, promotion with capture, castle, EnPassant
* AI


[F# file]


## Screenshot


![img1]


## General observations

* mixing C# and F# is as seamless as if they were the same language. 
* writing `type PawnType = PAWN|ROOK|KNIGHT|BISHOP|QUEEN|KING` has some strange beauty
* functional or not, it is easy to turn the code into a mess if You are not careful


## Other

This project uses free WPF dark theme created by Brian Lagunas that can be found [here].

[F# file]: Chess/chess.fs
[img1]: https://raw.github.com/Scthe/Chess/master/screenshot.png
[here]: http://brianlagunas.com/free-metro-light-and-dark-themes-for-wpf-and-silverlight-microsoft-controls/
