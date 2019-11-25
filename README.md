# MineSweeperWPF
![image](https://user-images.githubusercontent.com/9387781/69513795-f15dc780-0f40-11ea-9bd1-d87014e63abe.png)


## Table of Content
* [Introduction](#introduction)
* [Gameplay](#gameplay)
* [Download](#download)
* [Run the Project](#run-the-project)
* [Reflection](#reflection)

## Introduction
This game was created using C# WPF in three days. The game consisted of single and multiplayer mode. Multiplayer is available only on the same machine.

## Gameplay
### Single Player
![image](https://user-images.githubusercontent.com/9387781/69513884-67fac500-0f41-11ea-9578-1d31b2b0fe1d.png)
* Click on a tile in the grid, the number on the tile indicates the number of mine around this tile
* Avoid and successfully flag all the mine from the board to win

### Multiplayer
![image](https://user-images.githubusercontent.com/9387781/69513818-0fc3c300-0f41-11ea-92a2-ebd71f8124c1.png)
* This is a turn-based game, each player take turn to find the mine
* The number on the tile indicates the number of mine around the tile
* Player that has successfully found the mine will earn the chance to move again
* In multiplayer game mode, player that flag out most of the mine win the game

## Download
1. Clone this repository `https://github.com/sing840722/MineSwepperWPF.git`
2. Go to MineSweeper/bin/Debug
3. Run MineSweeper.exe

## Run the Project
1. Clone this repository `https://github.com/sing840722/TrickyMoggy.git`
2. Open MineSweeper.sln
3. Press F5 to run the project

## Reflection
It was not too difficult to create the main game features and single player game mode, but as the game expanded the program get more complicated;

Some functions were placed in a class that it should not be, it was not difficult to move them away the class or namespace but remain the connection;

Spent half of the time to otimize the game and trying to apply ing SOLID principle, still some of the functions cannot follow the this principle;

The game assests are not otimized, and some of the UI elemnts are not positioning very well;

However, a fully functional mine sweeper game has been created in a short time and it has two different game mode;

For project like this in the future that uses WPF, XAML can be used more often and work together with C# to make C# programming easier;
