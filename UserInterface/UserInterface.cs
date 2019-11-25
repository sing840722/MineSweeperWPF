using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UserInterface
{
    public class Menu
    {
        //The lowest layer of the UI, draw UI elements on top of canvas
        Canvas canvas;
        //The dropdown list to select difficulty and game mode
        ComboBox levelOption;
        //Item of the dropdown list - single player easy level (6 x 6 board with 7 mines)
        ComboBoxItem easy;
        //Item of the dropdown list - singple player mode normal level (16 x 16 board with 50 mines)
        ComboBoxItem normal;
        //Item of the dropdown list - single player mode difficult level (25 x 25 board with 100 mines)
        ComboBoxItem difficult;
        //Item of the dropdown list - multiplayer easy level (8 x 8 board with 10 mines)
        ComboBoxItem pvpEasy;
        //Item of the dropdown list - multiplayer normal level (16 x 16 board with 50 mines)
        ComboBoxItem pvpNormal;
        //Start button that generate the game with selected level and game mode
        Button startButton;
        //The application window
        Window gameWindow;
        //The brush to draw image using image source 
        ImageBrush imageBrush;

        public Button StartButton { get { return startButton; } }
        public ComboBox LevelOption { get { return levelOption; } }

        //Constructor
        public Menu(Window window)
        {
            //Take the application window reference and store it in variable within this class
            gameWindow = window;
            //Create a new image brush and give it an image source
            imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("Resources/background.bmp", UriKind.Relative));
            //Initialize the Menu (Create content)
            Initialize();
        }

        //Create the content for the menu
        public void Initialize()
        {
            //Create new Canvase object
            canvas = new Canvas();
            //Set the Height and Width for the menu
            canvas.Height = 250;
            canvas.Width = 250;
            //Set the alignment style for the menu
            canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = VerticalAlignment.Stretch;
            //set background image
            canvas.Background = imageBrush;
            
            //Create Dropdown menu
            levelOption = new ComboBox();
            //Set the Height and Width
            levelOption.Height = 22;
            levelOption.Width = 100;
            //Set the alignment style for the dropdown menu
            levelOption.HorizontalAlignment = HorizontalAlignment.Center;
            levelOption.VerticalAlignment = VerticalAlignment.Center;
            //Set the position of the dropdown menu, 150px down from the top of the canvas
            levelOption.SetValue(Canvas.TopProperty, (double)150);
            //80px to the right, from the left of the canvase
            levelOption.SetValue(Canvas.LeftProperty, (double)80);

            //Create new dropdown menu items and add them to the dropdown menu
            //Single player - Easy
            easy = new ComboBoxItem();
            easy.Content = "Easy";
            levelOption.Items.Add(easy);
            //Single player - Normal
            normal = new ComboBoxItem();
            normal.Content = "Normal";
            levelOption.Items.Add(normal);
            //Single player - difficult
            difficult = new ComboBoxItem();
            difficult.Content = "Difficult";
            levelOption.Items.Add(difficult);
            //Mutliplayer - Easy
            pvpEasy = new ComboBoxItem();
            pvpEasy.Content = "PvP (Easy)";
            pvpEasy.IsSelected = true;
            levelOption.Items.Add(pvpEasy);
            //Multiplayer - Difficult
            pvpNormal = new ComboBoxItem();
            pvpNormal.Content = "PvP (Normal)";
            pvpNormal.IsSelected = true;
            levelOption.Items.Add(pvpNormal);
            
            //Add the dropdown menu to the main menu
            canvas.Children.Add(levelOption);

            //Create start button
            startButton = new Button();
            //Set the width of the button to be 75px 
            startButton.Width = 75;
            //Write Start on the button
            startButton.Content = "Start";
            //Set the position of the button, 180px from the top of the canvas
            startButton.SetValue(Canvas.TopProperty, (double)180);
            //80px from the left of the canvas
            startButton.SetValue(Canvas.LeftProperty, (double)80);
            //Add the start button to main menu
            canvas.Children.Add(startButton);

            //Set the current content to be this menu (Display the menu)
            gameWindow.Content = this.canvas;
            //Make the window to be size to content
            gameWindow.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }

    //The top banner of the gameboard
    public class TopBanner
    {
        //width and height for the banner
        double width;
        double height;
        //Divide the banner by 3 horizontally
        //The lowest layer of the banner
        Rectangle background;
        //The left part of the banner, place it on the left above the background to divide the banner
        Rectangle leftField;   //Size: canvas.width/7*3
        //The right part of the banner, place it on the right and above the background to divide the banner
        Rectangle rightField;   //Size: canvas.width/7*3
        //The variable to store the reference of the gameboard canvas
        Canvas gameCanvas;

        //The label for the name on the left (Player 1)
        Label leftName;
        //The label for the score on the left (Player 1 score)
        Label leftScore;
        //The label for the name on the right (Player 2)
        Label rightName;
        //The label for the score on the right (Player 2 score)
        Label rightScore;
        //The label in the center to indicate the winning condiiton (Total mine/2 + 1)
        Label winCondition;
        //A list container to manage the score label
        List<Label> Scores;
        //A list containter to manger the fields (Add/Remove border to indicate player's turn)
        List<Rectangle> indicators;

        //Encapsulation
        public List<Rectangle> Indicators;
        public Label LeftName { get { return leftName; } set { leftName = value; } }
        public Label LeftScore { get { return leftScore; } set { leftScore = value; } }
        public Label RightName { get { return rightName; } set { rightName = value; } }
        public Label RightScore { get { return rightScore; } set { rightScore = value; } }
        public Label WinCondition { get { return winCondition; } set { winCondition = value; } }
        public Rectangle LeftField { get { return leftField; } set { leftField = value; } }
        public Rectangle RightField { get { return rightField; } set { rightField = value; } }

        //Constructor
        public TopBanner(double width, double height, Canvas canvas)
        {
            this.gameCanvas = canvas;
            this.width = width;
            this.height = height;

            Initialize();
        }

        //Initialize banner, can be called to recreate the banner
        public void Initialize()
        {
            //Create the list containter for the fields
            indicators = new List<Rectangle>();
            //Create the list containters for the scores label
            Scores = new List<Label>();

            //Create the background, set size and colour to HotPink
            background = new Rectangle()
            {
                Width = this.width,
                Height = this.height,
                Fill = Brushes.HotPink,
            };
            //Add the background to canvas
            gameCanvas.Children.Add(background);

            //Create the left field and change the style
            leftField = new Rectangle()
            {
                Width = this.width / 7 * 3,
                Height = this.height,
                Fill = Brushes.Red,
                StrokeThickness = 5,
            };
            //Add it to the canvas and store in the list
            gameCanvas.Children.Add(leftField);
            indicators.Add(leftField);

            //Create the right feild and change the style
            rightField = new Rectangle()
            {
                Width = this.width / 7 * 3,
                Height = this.height,
                Fill = Brushes.Blue,
                StrokeThickness = 5,
            };
            //Place the rectangle on the right
            rightField.SetValue(Canvas.RightProperty, 0.0);
            //Add it to the canvas and store it in the list
            gameCanvas.Children.Add(rightField);
            indicators.Add(RightField);

            //Create name label, change the font size to 35 and set default value to Player 1
            leftName = new Label();
            leftName.FontSize = 35;
            leftName.Content = "Player1";
            //Add it to the canvas
            gameCanvas.Children.Add(leftName);

            //Create score label, change the font size to 25 and set default vault to 0
            leftScore = new Label();
            leftScore.FontSize = 25;
            leftScore.Content = 0;
            //Place it to the center (vertically)
            leftScore.SetValue(Canvas.TopProperty, background.Height / 2);
            //Add it to the canvas and store to the list
            gameCanvas.Children.Add(leftScore);
            Scores.Add(LeftScore);

            //Create name label, change the font size to 35, set default value to Player 2
            rightName = new Label();
            rightName.FontSize = 35;
            //Place it on the right
            rightName.SetValue(Canvas.RightProperty, 0.0);
            rightName.Content = "Player2";
            //Add the label to canvas
            gameCanvas.Children.Add(rightName);

            //Create score label, change the font size to 25, default value to 0
            rightScore = new Label();
            rightScore.FontSize = 25;
            //Place it on the right
            rightScore.SetValue(Canvas.RightProperty, 0.0);
            //Place it in the center (Vertically)
            rightScore.SetValue(Canvas.TopProperty, background.Height / 2);
            rightScore.Content = 0;
            //Add the label to canvas and store to the list
            gameCanvas.Children.Add(rightScore);
            Scores.Add(rightScore);

            //Create the winning condition label, set font size, place it at the center of the banner, default value to 25
            winCondition = new Label();
            winCondition.FontSize = 40;
            winCondition.SetValue(Canvas.TopProperty, background.Height / 4);
            winCondition.SetValue(Canvas.RightProperty, background.Width / 2 - winCondition.FontSize / 1.5);
            winCondition.Content = 25;
            //Add the label to canvas
            gameCanvas.Children.Add(winCondition);
        }

        //Pass in the player id to remove indicator/border on the field (0 for the left, 1 for the right)
        public void RemoveIndicator(int i)
        {
            indicators[i].Stroke = null;
        }
        //Pass in the player id to add indicator/border on the field (0 for the left, 1 for the right)
        public void AddIndicator(int i)
        {
            indicators[i].Stroke = Brushes.GreenYellow;
        }
        //Pass in the turn/player id, and new score to update the value of the score label
        public void UpdateScore(int turn, int newScore)
        {
            Scores[turn].Content = newScore;
        }
    }
}