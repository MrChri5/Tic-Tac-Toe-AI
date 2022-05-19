# TicTacToeText
**Noughts and Crosses (Tic-Tac-Toe) AI**

This program is a console based game of Noughts and Crosses. 
It can be played either as a 2 Player game or as 1 Player verses an AI.

A 3x3 game is simple enough for an algorithm to compare the current layout against a number of possible layouts and make the next move based on this.
The AI in this program does not use this method, instead it analyses each of the possible lines in the board and uses this to determine the next move.
This method can easily be scaled up to larger board sizes. 
This program can be modified to play larger board sizes by passing the size when initializing the board with:
`BoardLayout currentBoard = new BoardLayout(boardSize)`.

This project served as an introduction to C# classes. 
The BoardLayout class contains variables and methods which help to reduce the complexity of the main program.
