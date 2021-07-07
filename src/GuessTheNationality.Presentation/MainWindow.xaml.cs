using GuessTheNationality.Service;
using GuessTheNationality.Service.Imp;
using GuessTheNationality.Service.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace GuessTheNationality
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PrivateVariables

        private readonly int _maxItems;
        private readonly ImagesDictionary _imagesDictionary;
        private PointsList pointsList { get; set; }
        private DispatcherTimer gameTimer;
        bool drag = false;
        private int itemNumber;
        private bool objectSelected = false;
        private Rectangle currentRectangle;
        Point startPoint;
        Point intermPoint;
        int score;
        int itemSpeed;
        private IScoreCalculator scoreCalculator;
        #endregion PrivateVariables

        public MainWindow()
        {
            InitializeComponent();
            _maxItems = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.MaximumImagesCountKey].ToString());
            _imagesDictionary = new ImagesDictionary(ConfigurationManager.AppSettings[Constants.ImagesFolderPathKey].ToString());
            gameTimer = new DispatcherTimer();
            MyCanvas.Focus();
            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(ConfigurationManager.AppSettings[Constants.GameTimerIntervalKey].ToString()));
            gameTimer.Start();
            pointsList = new PointsList();
            itemSpeed = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.ItemSpeedKey].ToString());
            scoreCalculator = new ScoreCalculator();
            itemNumber = 0;
            score = 0;
        }
        /// <summary>
        /// Main event that handle the complete game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEngine(object sender, EventArgs e)
        {
            if (!objectSelected)
            {

                if (currentRectangle != null)
                {
                    Canvas.SetTop(currentRectangle, Canvas.GetTop(currentRectangle) + itemSpeed);

                    if (Canvas.GetTop(currentRectangle) > 460)
                    {
                        RemoveShape(currentRectangle);
                    }
                }

                var child = MyCanvas.Children.OfType<Rectangle>();
                if (!child.Any() && itemNumber < _imagesDictionary.Images.Count && itemNumber < _maxItems)
                {
                    CreateShape(itemNumber);
                    itemNumber++;
                }
                else if (!child.Any())
                {
                    gameTimer.Stop();
                    MessageBox.Show("You Scored: " + score + Environment.NewLine + "Click ok to play again", "Game Finish");
                    ResetGame();
                }

            }

        }
        /// <summary>
        /// Reset the game to start
        /// </summary>
        private void ResetGame()
        {

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        /// <summary>
        /// responsible to create shapes to drop
        /// </summary>
        /// <param name="itemNumber"></param>
        private void CreateShape(int itemNumber)
        {

            var image = _imagesDictionary.Images[itemNumber];
            var icon = new ImageBrush { ImageSource = new BitmapImage(new Uri(image.ImagePath)) };

            Rectangle newRec = new Rectangle
            {
                Tag = image.Nationality,
                Width = 100,
                Height = 100,
                Fill = icon
            };
            newRec.MouseDown += rectangle_MouseDown;
            newRec.MouseUp += rectangle_MouseUp;
            newRec.MouseMove += rectangle_MouseMove;
            Canvas.SetTop(newRec, 10);
            Canvas.SetLeft(newRec, 200);
            MyCanvas.Children.Add(newRec);
            currentRectangle = newRec;
        }
        /// <summary>
        /// Responsible to remove shape from canvas
        /// </summary>
        /// <param name="rectangle"></param>
        private void RemoveShape(Rectangle rectangle)
        {
            MyCanvas.Children.Remove(rectangle);
            rectangle = null;
            objectSelected = false;
        }

        /// <summary>
        /// Tells the motion of mouse to cordinate
        /// </summary>
        /// <param name="newScore"></param>
        /// <returns></returns>
        private int CalculateMotion(Point newScore)
        {
            int motion = 0;
            if (newScore.X > 0 && newScore.Y > 0)
                motion = 2;
            if (newScore.X > 0 && newScore.Y < 0)
                motion = 1;
            if (newScore.X < 0 && newScore.Y > 0)
                motion = 3;
            if (newScore.X < 0 && newScore.Y < 0)
                motion = 0;

            return motion;
        }

        /// <summary>
        /// Hanlder method that is controller of post mouse movement actions
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="newPoint"></param>
        private void HandleShapeMovement(Point oldPoint, Point newPoint)
        {
            var newScore = Point.Subtract(newPoint, oldPoint);
            if (newScore.Length > 20)
            {
                var motion = CalculateMotion(((Point)newScore));
                var respPoint = pointsList.Points[motion];
                StartAnimations(currentRectangle, respPoint.Point.X, respPoint.Point.Y);
                score = scoreCalculator.CalculateScore((string)currentRectangle.Tag, respPoint.Name, score);
                scoreText.Content = $"Score: {score}";
            }
            objectSelected = false;
        }

        #region Events
        /// <summary>
        /// capture mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            drag = true;
            objectSelected = true;
            startPoint = Mouse.GetPosition(MyCanvas);
            intermPoint = startPoint;
        }
        /// <summary>
        /// capture mouse movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                Rectangle draggedRectangle = sender as Rectangle;
                Point newPoint = Mouse.GetPosition(MyCanvas);
                double left = Canvas.GetLeft(draggedRectangle);
                double top = Canvas.GetTop(draggedRectangle);
                Canvas.SetLeft(draggedRectangle, left + (newPoint.X - intermPoint.X));
                Canvas.SetTop(draggedRectangle, top + (newPoint.Y - intermPoint.Y));
                intermPoint = newPoint;
            }
        }
        /// <summary>
        /// capture mouse click up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // stop dragging
            drag = false;
            HandleShapeMovement(startPoint, Mouse.GetPosition(MyCanvas));
        }
        private void animation_completed(object sender, EventArgs e)
        {
            RemoveShape(currentRectangle);
            objectSelected = false;
        }
        #endregion Events

        #region Animations
        /// <summary>
        /// Animations controller method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        private void StartAnimations(Rectangle target, double newX, double newY)
        {
            StartMotionAnimation(target, newX, newY);
            StartFadingAnimation(target);
        }

        /// <summary>
        /// responsible for motion animation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        private void StartMotionAnimation(Rectangle target, double newX, double newY)
        {
            var duration = TimeSpan.FromSeconds(Convert.ToDouble(ConfigurationManager.AppSettings[Constants.MovingAnitmationTimeKey]));
            target.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(newX, duration));
            target.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(newY, duration));
        }
        /// <summary>
        /// responsible for fading animation
        /// </summary>
        /// <param name="target"></param>
        private void StartFadingAnimation(Rectangle target)
        {
            var interval = TimeSpan.FromSeconds(Convert.ToDouble(ConfigurationManager.AppSettings[Constants.AnimationsTimeIntervalKey]));
            var fadout = TimeSpan.FromSeconds(Convert.ToDouble(ConfigurationManager.AppSettings[Constants.FadingAnimationTimeKey]));
            var animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = interval,
                Duration = fadout,
                FillBehavior = FillBehavior.HoldEnd
            };
            animation.Completed += new EventHandler(animation_completed);
            target.BeginAnimation(UIElement.OpacityProperty, animation);
        }
        #endregion Animations
    }
}
