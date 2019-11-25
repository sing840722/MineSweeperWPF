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
using UserInterface;

namespace MineSweeperGame
{
    //Declare an enum of GAME EVENT TYPE for comparison of GameboardEventArgs
    public enum GAME_EVENT
    {
        GAMEOVER, COLLECT_OBJECT,
    }

    //Create an EventArgs child class to handle gameboard events
    public class GameboardEventArgs : EventArgs
    {
        //Store the game event type 
        GAME_EVENT e;

        //Get the game event type, can be used to compare event type with in an event handler
        public GAME_EVENT GameboardEvent { get { return e; } }

        //Constructor, pass the event type (game over, collected object etc.
        public GameboardEventArgs(GAME_EVENT e) {this.e = e; }
    }
    
    //An abstruct class for Tile, will be inherited by single player tiles and multipler tiles
    public abstract class Tile
    {
        //Declare a delegate
        public delegate void GameEvent(Object sender, GameboardEventArgs e);

        //Declare a event handler of the type GameEvent (The delegate declared above)
        public GameEvent GameEventHandler;

        //Create a function to raise/fire gameboard event
        public void RaiseEvent(GameboardEventArgs e)
        {
            //Only execute if there are event listener listing to events
            if (GameEventHandler != null)
                GameEventHandler(this, e);
        }

        //Declare a button
        protected Button button;
        //A variable to store the refernce of the gameobard
        protected MineSweeper gameBoard;
        //The postiion of the tile (At which row in the grid)
        protected int row;
        //The position of the tile (At which column in the grid)
        protected int column;
        //The ID of the tile, convert the index of a 2D array to 1D array
        protected int tileID;
        //A flag to tell if this tile has a mine
        protected bool hasMine;
        //A flag to tell if this tile is finished (Checked)
        protected bool isFinish;

        //Encapsulation
        public bool HasMine { get { return hasMine; } }
        public bool IsFinish { get { return isFinish; } }

        //Left Click Event Handler, available for overriding
        public virtual void OnLeftClickTile(object sender, RoutedEventArgs e) { }
        //Right Click Event Handler, available for overriding
        public virtual void OnRightClickTile(object sender, RoutedEventArgs e) { }
        //Double Click Event Handler, available for overriding
        public virtual void OnDoubleClickTile(object sender, MouseButtonEventArgs e) { }
        
        //This fucntion check if the tile has a mine
        public virtual bool CheckHasObject()
        {
            //If the tile is already checked, return false
            if (isFinish)
                return false;
            else
            {
                //Check this button
                this.isFinish = true;
                //If this tile has a mine
                if (this.hasMine)
                {
                    //Raise a collect object event and return true for this function
                    this.RaiseEvent(new GameboardEventArgs(GAME_EVENT.COLLECT_OBJECT));
                    return true;
                }
                else
                {
                    //Check how many mines next to this tile
                    CheckObjectAroundTile();
                }
            }
            return false;
        }

        //Check the number of mine next to the tile
        public void CheckObjectAroundTile()
        {
            //Get the indecies of the surrounding tile and store them into list
            List<int> numbers = GetNeighbourIndecies(this.tileID);
            //count the number of mines using the list
            int count = CountObjFromGroup(numbers);
            //if there are no mines around the tile, automatically check all the surrounding tiles
            if (count == 0)
                InvokeGroupOfTile(numbers);
            else
                //Set the image of the tile
                SetTileImage(count.ToString());
        }

        //On CollectObject/ ClickMine Event Handler, only multiplayer game mode use it
        public virtual void OnCollectObject(Object sender, GameboardEventArgs e)
        {
            //Check event type
            if (e.GameboardEvent != GAME_EVENT.COLLECT_OBJECT)
                return;
            //Add score to the current player
            gameBoard.Players[gameBoard.Turn].Score++;
            //Update the score label
            gameBoard.TopBannerHUD.UpdateScore(gameBoard.Turn, gameBoard.Players[gameBoard.Turn].Score);
            //Flag the mine
            SetTileImage("F");
            //If player has found more than half of the mines, gameover
            if(gameBoard.Players[gameBoard.Turn].Score > gameBoard.Mine/2)
                gameBoard.RaiseEvent(new GameboardEventArgs(GAME_EVENT.GAMEOVER));
        }

        //Check a list of tiles
        public virtual void InvokeGroupOfTile(List<int> numbers)
        {
            //Disable the clicked button, this will change the style as well
            this.button.IsEnabled = false;
            //Check each tile from the list
            foreach (int i in numbers)
            {
                Tile temp = gameBoard.Tiles[i];
                temp.CheckHasObject();
            }
        }

        //Count the mines from a group (The tiles around the target tile)
        public virtual int CountObjFromGroup(List<int> numbers)
        {
            int count = 0;
            //Check each tile has mine, add one to count if true
            foreach (int i in numbers)  //Count number of mine
            {
                Tile temp = gameBoard.Tiles[i];
                if (temp.hasMine)
                    count++;
            }
            return count;
        }

        //Get the indecies for the surrounding tiles 
        protected List<int> GetNeighbourIndecies(int i)
        {
            //Get the x, y coodinate
            int x = i % this.gameBoard.Column;
            int y = i / this.gameBoard.Column;

            //Create a list to store the indecies
            List<int> numbers = new List<int>();

            //if not on the left column, add the left tile 
            if (x != 0)                             
                numbers.Add(i - 1);
            //if not on the right column, add the right tile
            if (x != this.gameBoard.Column - 1)                
                numbers.Add(i + 1);
            //if not on the top row, add the tile above
            if (y != 0)                             
                numbers.Add(i - this.gameBoard.Column);
            //if not on the top row AND not on the left column, add the top left tile
            if (y != 0 && x != 0)                   
                numbers.Add(i - this.gameBoard.Column - 1);
            //if not on the top row AND not on the right column, add the top right tile
            if (y != 0 && x != this.gameBoard.Column - 1)          
                numbers.Add(i - this.gameBoard.Column + 1);
            //if not on the button row, add the tile below
            if (y != this.gameBoard.Row - 1)                       
                numbers.Add(i + this.gameBoard.Column);
            //if not on the bottm row AND not on the right column, add the bottom right tile
            if (y != this.gameBoard.Row - 1 && x != this.gameBoard.Column - 1)    
                numbers.Add(i + this.gameBoard.Column + 1);
            //if not on the bottom row AND not on the left column, add the bottom left tile
            if (y != this.gameBoard.Row - 1 && x != 0)             
                numbers.Add(i + this.gameBoard.Column - 1);
            return numbers;
        }

        //Set Image for the tile, override in the child class for different images and cases
        public virtual void SetTileImage(string text)
        {
            switch (text)
            {
                //Set to Flag Image, colour depends on the current player
                case "F":   
                    if (gameBoard.Turn == 0)
                        button.Content = new Image() { Source = gameBoard.RedFlagImage };
                    else if (gameBoard.Turn == 1)
                        button.Content = new Image() { Source = gameBoard.BlueFlagImage };
                    break;
                //Write 0-9 on the tile
                default:
                    button.Content = text;
                    button.Background = Brushes.Gold;
                    break;
            }
        }

        //Give mine to the tile
        public void SetMine(bool mine)
        {
            this.hasMine = mine;
        }
    }

    //Abstracut gameboard class
    public abstract class MineSweeper
    {
        //Declare a delegate to be used for event handler
        public delegate void GameboardEventHandler(object sender, GameboardEventArgs e);
        //Declare a GameboardEvent Handler using the delegate type created above
        public event GameboardEventHandler GameboardEvent;
        //Create a function to fire event
        public void RaiseEvent(GameboardEventArgs e)
        {
            if (GameboardEvent != null)
                GameboardEvent(this, e);
        }

        /*Declare variables*/
        //Images for the tiles, mine, flag, red and blue flag
        protected BitmapImage mineImage;
        protected BitmapImage flagImage;
        protected BitmapImage redFlagImage;
        protected BitmapImage blueFlagImage;
        //window variable to store the main window
        protected Window gameWindow;
        //list container for tiles
        protected List<Tile> tiles;
        //The WPF grid as gameboard
        protected Grid gameBoard;
        //The number of row and column in the grid
        protected int row;
        protected int column;
        //The number of mine in this gameboard
        protected int mine;
        //Player's turn, 0 for player1, 1 for player2
        protected int turn = 0;
        //Top padding, leave 100px for at the top of the banner/UI
        protected double topPadding = 100;
        //The size of each tile
        protected float blockSize;
        //List container to manage players
        protected List<Player> players;
        //Canvas to draw 
        protected Canvas gameCanvas;
        //The top banner, this display the players name and score, win condition
        protected TopBanner topBannerHUD;

        //Encapsulation
        public BitmapImage RedFlagImage { get { return redFlagImage; } }
        public BitmapImage BlueFlagImage { get { return blueFlagImage; } }
        public BitmapImage MineImage { get { return mineImage; } }
        public BitmapImage FlagImage { get { return flagImage; } }
        public List<Tile> Tiles { get { return tiles; } }
        public Grid GameBoard { get { return gameBoard; } }
        public int Row { get { return row; } set { row = value; } }
        public int Mine { get { return mine; } set { mine = value; } }
        public int Column { get { return column; } set { column = value; } }
        public int Turn { get { return turn; } set { turn = value; } }
        public List<Player> Players { get { return players; } }
        public double TopPadding { get { return topPadding; } }
        public Canvas GameCanvas { get { return gameCanvas; } }
        public TopBanner TopBannerHUD { get { return topBannerHUD; } }

        //Initialize the gameboard
        public virtual void Initialize(int row, int column, int mine, Window window)
        {
            //Store the values
            this.gameWindow = window;
            this.row = row;
            this.column = column;
            this.mine = mine;

            //Create images and store them so they are ready to be used
            mineImage = new BitmapImage(new Uri("Resources/mine.bmp", UriKind.Relative));
            redFlagImage = new BitmapImage(new Uri("Resources/flag0.bmp", UriKind.Relative));
            blueFlagImage = new BitmapImage(new Uri("Resources/flag1.bmp", UriKind.Relative));
            flagImage = new BitmapImage(new Uri("Resources/flag.bmp", UriKind.Relative));

            //Create new objects and store them in variables
            gameCanvas = new Canvas();
            tiles = new List<Tile>();
            gameBoard = new Grid();
            //The more the row in the grid, the smaller the tiles are
            blockSize = 50 - row;
            turn = -1;
            players = new List<Player>();

            //Calculate the Width and Height for the grid, set the canvas size
            gameCanvas.Width = column * blockSize;
            gameCanvas.Height = row * blockSize + topPadding;

            // Create the Grid
            gameBoard = new Grid();
            gameBoard.Width = column * blockSize;
            gameBoard.Height = row * blockSize;
            gameBoard.SetValue(Canvas.TopProperty, topPadding);
            gameBoard.HorizontalAlignment = HorizontalAlignment.Left;
            gameBoard.VerticalAlignment = VerticalAlignment.Top;
            gameBoard.Background = new SolidColorBrush(Colors.LightSteelBlue);
        }

        //Create a list of number(Randomly) that represent mine on the grid
        protected HashSet<int> RandomNumber(int num_of_mine)
        {
            //Use hashset so the random numbers are not repeated
            HashSet<int> numbers = new HashSet<int>();
            Random random = new Random();
            //Generate random number
            while (numbers.Count < num_of_mine)
            {
                numbers.Add(random.Next(0, (row * column) - 1));
            }
            return numbers;
        }

        //Given a list of indices of the grid, put mine in to the tile
        protected void SetMine(HashSet<int> numbers, List<Tile> tiles)
        {
            foreach (int i in numbers)
                tiles[i].SetMine(true);
        }
    }

    //The tile used in Multiplayer game mode
    public class MP_Tile : Tile
    {
        //Constructor
        public MP_Tile(int row, int column, MineSweeper gameBoard)
        {
            //store the values
            this.column = column;
            this.row = row;
            this.gameBoard = gameBoard;
            this.tileID = column + row * gameBoard.Column;
            this.isFinish = false;
            //Create new button for the tile
            button = new Button();
            hasMine = false;
            //Give the button Left Click Event Handler
            button.Click += OnLeftClickTile;
            button.Background = Brushes.SkyBlue;
            //Set the position of the button in grid
            System.Windows.Controls.Grid.SetColumn(this.button, column);
            System.Windows.Controls.Grid.SetRow(this.button, row);
            //Add the button to grid
            gameBoard.GameBoard.Children.Add(button);
            //Give the tile OnCollectObject Event Handler (On Click Mine)
            GameEventHandler += OnCollectObject;
        }

        //Override Left Click Event Handler
        public override void OnLeftClickTile(object sender, RoutedEventArgs e)
        {
            //Call the parent's function
            base.OnLeftClickTile(sender, e);
            //return if the tile is checked
            if (isFinish)
                return;
            //If the tile has no mine, switch to another player
            if (!CheckHasObject())
                (gameBoard as MP_GameBoard).SwitchPlayerTurn();
        }
    }

    //Single Player Gameboard
    public class SP_GameBoard : MineSweeper
    {
        //Count the number of mine that has been flagged
        int finishCount;
        //Encapsulation
        public int FinishCount { get { return finishCount; } set { finishCount = value; } }

        //Constructor
        public SP_GameBoard() { }

        //Init
        public override void Initialize(int row, int column, int mine, Window window)
        {
            //Call the base Init - from the abstruct class
            base.Initialize(row, column, mine, window);
            //Set background colour
            gameBoard.Background = new SolidColorBrush(Colors.LightGray);

            //Create rows
            for (int i = 0; i < row; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(blockSize);
                gameBoard.RowDefinitions.Add(rowDef);
                // Create Columns
                for (int j = 0; j < column; j++)
                {
                    ColumnDefinition columnDef = new ColumnDefinition();
                    columnDef.Width = new GridLength(blockSize);
                    gameBoard.ColumnDefinitions.Add(columnDef);
                    Tile tile = new SP_Tile(i, j, this);
                    tiles.Add(tile);
                }
            }
            //Randomly place the mines
            SetMine(RandomNumber(mine), tiles);

            //Add the gameboard to the canvas
            gameCanvas.Children.Add(this.gameBoard);
            //Display the canvas
            gameWindow.Content = this.gameCanvas;
            //Window size to content
            gameWindow.SizeToContent = SizeToContent.WidthAndHeight;
        }

        //Shows all the mines (Call it when gameover)
        public void ShowAllMine()
        {
            foreach (Tile t in tiles)
            {
                if (t.HasMine)
                    t.SetTileImage("M");
            }
        }
    }

    //Single player tile
    public class SP_Tile : Tile
    {
        //Store reference of single player gameboard
        SP_GameBoard board;

        //Constructor
        public SP_Tile(int row, int column, MineSweeper gameBoard)
        {
            //Store values
            this.column = column;
            this.row = row;
            this.gameBoard = gameBoard;
            this.tileID = column + row * gameBoard.Column;
            this.isFinish = false;
            //Cast the gameboard to single player gameboard and store it
            board = gameBoard as SP_GameBoard;
            //New button
            button = new Button();
            hasMine = false;
            //Add Event Handlers to the button
            button.Click += OnLeftClickTile;
            button.MouseDoubleClick += OnDoubleClickTile;
            button.MouseRightButtonDown += OnRightClickTile;
            //Set button position (Within the grid)
            System.Windows.Controls.Grid.SetColumn(this.button, column);
            System.Windows.Controls.Grid.SetRow(this.button, row);
            //Add button to the grid
            gameBoard.GameBoard.Children.Add(button);
            //Add event handler for this tile (On Click the Mine)
            GameEventHandler += OnCollectObject;
        }

        //Override the On Click Mine Event Handler, Do not call the base.OnCollectObject();
        public override void OnCollectObject(object sender, GameboardEventArgs e)
        {
            //Check event type
            if (e.GameboardEvent != GAME_EVENT.COLLECT_OBJECT)
                return;
            //Show all the mine
            board.ShowAllMine();
            //Fire gameover event
            board.RaiseEvent(new GameboardEventArgs(GAME_EVENT.GAMEOVER));
        }

        //Override Left Click Event Handler
        public override void OnLeftClickTile(object sender, RoutedEventArgs e)
        {
            //Call the base function first
            base.OnLeftClickTile(sender, e);
            //return from function if the tile is already checked
            if (isFinish)
                return;
            //Check the tile
            this.isFinish = true;
            //Remove right click event handler (avoid flagging a checked tile)
            button.MouseRightButtonDown -= OnRightClickTile;
            //Count the checked tile
            board.FinishCount++;
            //If clicked on a mine, fire event
            if (CheckHasObject())
                RaiseEvent(new GameboardEventArgs(GAME_EVENT.COLLECT_OBJECT));
        }

        //override the check object/mine function
        public override bool CheckHasObject()
        {
            if (HasMine)
                return true;
            else
            {
                //Get the indices of the surrounding tiles
                List<int> numbers = GetNeighbourIndecies(this.tileID);
                //Count the number of mines for the surrounding tiles
                int count = CountObjFromGroup(numbers);
                //If not mines near
                if (count == 0)
                {
                    //Disable the button, change the style as well
                    this.button.IsEnabled = false;
                    //Automatically check the surrounding tiles
                    InvokeGroupOfTile(numbers);
                }
                else
                {
                    //Set the tile image (write number to the tile)
                    SetTileImage(count.ToString());
                    return false;
                }
            }
            return false;
        }

        //Override this function, Check a group of tile
        public override void InvokeGroupOfTile(List<int> numbers)
        {
            //First check if the surrounding tiles have mine
            foreach (int i in numbers)
            {
                //cast the tile stored tile to single player tile
                SP_Tile temp = gameBoard.Tiles[i] as SP_Tile;
                //if has mine and is not checked 
                if (temp.hasMine && !temp.isFinish)
                {
                    //fire event, return from the function so it does not check for the number of mines for the rest of the tile
                    temp.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    return;
                }
            }
            //Then check for each tile, how many mines are nearby
            foreach (int i in numbers)
            {
                SP_Tile temp = gameBoard.Tiles[i] as SP_Tile;
                temp.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        //Double Click Event Handler
        public override void OnDoubleClickTile(object sender, MouseButtonEventArgs e)
        {
            base.OnDoubleClickTile(sender, e);
            //Check button event, Check the group of tile that is surrouded this tile
            if (e.ChangedButton == MouseButton.Left)
                InvokeGroupOfTile(GetNeighbourIndecies(tileID));
        }

        //Right Click Event Handler
        public override void OnRightClickTile(object sender, RoutedEventArgs e)
        {
            base.OnRightClickTile(sender, e);
            //Flag the tile
            FlagTile();
        }

        //Flag the tile
        public void FlagTile()
        {
            //If the tile is not checked
            if (!isFinish)
            {
                //Give a flag image to the tile
                SetTileImage("F");
                //Remove Double Click Event Handler
                button.MouseDoubleClick -= OnDoubleClickTile;
                //Increase checked tile count
                board.FinishCount++;
                //if flagging a mine, minus 1 from the mine counter
                if (hasMine)
                    board.Mine--;
                //If no more mine in the gameboard, fire gameover event
                if (board.Mine == 0)
                    board.RaiseEvent(new GameboardEventArgs(GAME_EVENT.GAMEOVER));
            }
            else
            {
                //Remove the flag image from the tile
                SetTileImage("U");
                //Give the Double Click Event Handler back to the tile
                button.MouseDoubleClick += OnDoubleClickTile;
                //Add 1 back to the checked tile counter
                board.FinishCount--;
                //if unflagging a mine, add 1 back to the mine counter
                if (hasMine)
                    board.Mine++;
            }
            isFinish = !isFinish;
        }

        //Override the Set Image function
        public override void SetTileImage(string text)
        {
            switch (text)
            {
                //M stand for Mine, set the image to mine
                case "M":   
                    button.Content = new Image() { Source = gameBoard.MineImage };
                    break;
                //F for Flag, set the the image to flag
                case "F":   
                    button.Content = new Image() { Source = gameBoard.FlagImage };
                    break;
                //U for Unflag, remove the image and set the background colour back to LightGray
                case "U":
                    button.Content = "";
                    button.Background = Brushes.LightGray;
                    break;
                //Any numbers from 0 to 8 goes here, change the content to text and background colour
                default:
                    button.Content = text;
                    button.Background = Brushes.SkyBlue;
                    break;
            }
        }
    }

    //Multiplayer gameboard
    public class MP_GameBoard : MineSweeper
    {
        //Constructor
        public MP_GameBoard() { }

        //Initialize
        public override void Initialize(int row, int column, int mine, Window window)
        {
            //assign values to variables
            base.Initialize(row, column, mine, window);

            //Create rows
            for (int i = 0; i < row; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(blockSize);
                gameBoard.RowDefinitions.Add(rowDef);
                // Create Columns
                for (int j = 0; j < column; j++)
                {
                    ColumnDefinition columnDef = new ColumnDefinition();
                    columnDef.Width = new GridLength(blockSize);
                    gameBoard.ColumnDefinitions.Add(columnDef);
                    Tile tile = new MP_Tile(i, j, this);
                    tiles.Add(tile);
                }
            }
            //Randomly place mine to the gameboard
            SetMine(RandomNumber(mine), tiles);

            //Create the top banner to show the players information
            topBannerHUD = new TopBanner(this.gameCanvas.Width, topPadding, this.gameCanvas);
            //Update the win condition
            topBannerHUD.WinCondition.Content = mine / 2 + 1;
            //Create two players
            for (int i = 0; i < 2; i++)
            {
                players.Add(new Player(i, this));
            }
            //Add the gameboard(grid) to canvas
            gameCanvas.Children.Add(this.gameBoard);
            //Display the canvas
            gameWindow.Content = this.gameCanvas;
            //Window size to content
            gameWindow.SizeToContent = SizeToContent.WidthAndHeight;
            //Switch player turn - Start from 0
            SwitchPlayerTurn();
        }

        //Switch to next player
        public void SwitchPlayerTurn()
        {
            //cast the turn from int to boolean, so the variable turn can be toggle
            bool bTurn = Convert.ToBoolean(turn);
            //Remove the border on the top banner for the current player
            topBannerHUD.RemoveIndicator(Convert.ToInt32(bTurn));
            //toggle the turn
            bTurn = !bTurn;
            //Add the border on the top banner for the next player
            topBannerHUD.AddIndicator(Convert.ToInt32(bTurn));
            //Cast the bool to int assign to turn so the turn variable is updated
            turn = Convert.ToInt32(bTurn);
        }
    }

    //The player class to store id and score, can be further expanded to contain name
    public class Player
    {
        //Variables
        int score;
        int id;
        MP_GameBoard gameBoard;

        //Encapsulation
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public int Id { get { return id; } }

        //Constructor, store the player id, and gameboard reference
        public Player(int id, MP_GameBoard gameboard)
        {
            this.id = id;
            this.gameBoard = gameboard;
        }
    }
}
