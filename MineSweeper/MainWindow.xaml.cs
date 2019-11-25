using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MineSweeperGame;
using UserInterface;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        UserInterface.Menu menu;
        List<MineSweeperGame.MineSweeper> games = new List<MineSweeperGame.MineSweeper>();
        
        //Constructor
        public MainWindow()
        {
            //Call all the Initialize functions
            InitializeComponent();
            Initialize();
            InitializeMenu();
        }

        //Initialize the application
        public void Initialize()
        {
            //Create gameboard, add single player event handler to the gameboard,store it to the list
            games.Add(new MineSweeperGame.SP_GameBoard());
            games[0].GameboardEvent += OnSinglePlayerGameover;
            //Create another gameboard object, add multiplayer event handler to the gameobard, store it to the list
            games.Add(new MineSweeperGame.MP_GameBoard());
            games[1].GameboardEvent += OnMultiplayerGameover;
        }
        
        //Initialize Menu
        public void InitializeMenu()
        {
            //Create new menu object
            menu = new UserInterface.Menu(this);
            //Add event handler for the start button 
            menu.StartButton.Click += OnStartButtonClick;
        }

        //Event handle for gameover in multiplayer game mode
        public void OnMultiplayerGameover(object sender, GameboardEventArgs e)
        {
            //if the event type is not gameover, return from this function
            if (e.GameboardEvent != GAME_EVENT.GAMEOVER)
                return;
            //cast the object that fire the event to multiplayer gameboard object
            MP_GameBoard gameBoard = sender as MP_GameBoard;
            //Show message of the player and id that has found the more mines
            MessageBox.Show("Player" + (gameBoard.Turn + 1).ToString() + " wins!", "Congratulations!");
            //Return to menu
            InitializeMenu();
        }

        //Event handler for gameover in single player game mode
        public void OnSinglePlayerGameover(object sender, GameboardEventArgs e)
        {
            //return from this function if the the event is not gameover
            if (e.GameboardEvent != GAME_EVENT.GAMEOVER)
                return;
            
            //Cast the object that fire event to single player gameboard object
            SP_GameBoard game = sender as SP_GameBoard;

            //if player has flagged all the flag, print congratulations message
            if (game.Mine == 0)
                MessageBox.Show("Congratulations!");
            //else print gameover message and return to menu
            else
                MessageBox.Show("Gameover");
            InitializeMenu();
        }

        //Event Handler for the menu start button
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            switch (menu.LevelOption.SelectedIndex)
            {
                case 0: //Single player - Easy                 
                    games[0].Initialize(6, 6, 7, this);
                    break;
                case 1: //Single player - Normal
                    games[0].Initialize(16, 16, 50, this);
                    break;
                case 2: //Single player - Difficult
                    games[0].Initialize(25, 25, 100, this);
                    break;
                case 3: //Multiplayer - Easy
                    games[1].Initialize(8, 8, 10, this);
                    break;
                case 4: //Multiplayer - Normal
                    games[1].Initialize(16, 16, 50, this);
                    break;
                default:
                    break;
            }
        }
    }
}
