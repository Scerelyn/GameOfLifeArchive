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
using GameOfLife.Cells;
using GameOfLife.Converters;
using System.Timers;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The cellular automata rule to use
        /// </summary>
        private static string rule = "B3/S23";
        /// <summary>
        /// The CellGrid object for this MainWindow to use
        /// </summary>
        private CellGrid cg { get; set; } = new CellGrid(10, 10, rule);
        /// <summary>
        /// The specific BoolToBrushConverter that this MainWindow instance will use
        /// </summary>
        private BoolToBrushConverter B2BConv = new BoolToBrushConverter()
        {
            TrueBrush = Cell.TrueBrush,
            FalseBrush = Cell.FalseBrush
        };
        /// <summary>
        /// The timer instance that this MainWindow object uses
        /// </summary>
        private Timer timer = new Timer() { Enabled = false, AutoReset = true };
        public MainWindow()
        {
            InitializeComponent();
            GenerateButton.Click += GenerateGrid;
            StepOnceButton.Click += StepOnce;
            timer.Elapsed += DoAStep;
            PlayButton.Click += PlayAndStop;
            RandomButton.Click += Randomize;
            ClearButton.Click += Clear;
            LockSquare.Click += LockSizeToSquare;

            GenerateGrid(null,null); //make the grid on startup
        }

        /// <summary>
        /// Grid generation method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void GenerateGrid(object sender, RoutedEventArgs args)
        {
            //Console.WriteLine("generating grid");
            this.Title = "Game of Life **(Generating grid, hold on a sec...)**";
            CellSpace.Children.Clear();
            int x = (int)xSlider.Value;
            int y = (int)ySlider.Value;
            rule = RuleTextBox.Text;
            cg = new CellGrid(x, y, rule);
            Binding labelBind = new Binding("RuleString");
            RuleLabel.DataContext = cg;
            RuleLabel.SetBinding(Label.ContentProperty, labelBind);
            for (int i = 0; i < x; i++)
            {
                StackPanel sp = new StackPanel(); //A stack panel of stack panels to keep things organized, and grid's manual placement isn't fun in code
                for (int j = 0; j < y; j++)
                {
                    Rectangle r = new Rectangle()
                    {
                        Height = CellSpace.ActualHeight != 0 ? CellSpace.ActualHeight / y : 58, //on startup, the stackpanel has an actualwidth/height of 0, which would've made rectangles 0x0, so we force retangle size to force stackpanel size
                        Width = CellSpace.ActualWidth != 0 ? CellSpace.ActualWidth / x : 58,
                        RadiusX = 5, //make the rectangle a bit circular to see cells better, since borders dont really exist on them
                        RadiusY = 5, //also the gap between rectangles is barely visible, and margin messes up formatting
                        //Margin = new Thickness() { Bottom = 1, Top = 1, Left = 1, Right = 1 },
                        DataContext = cg.Grid[i,j],
                    };
                    r.MouseLeftButtonDown += (s,ro) => { //set left mouse down event with a lambda
                        Cell context = (Cell)r.DataContext;
                        context.IsAlive = !context.IsAlive; //just invert
                    };
                    Binding b = new Binding("IsAlive");
                    b.Converter = B2BConv; //set the converter
                    r.SetBinding(Rectangle.FillProperty, b); //set bindind property to the rectangle
                    sp.Children.Add(r); //give to stack panel and let that do the formatting
                }
                CellSpace.Children.Add(sp); //insert the stackpanel to the main stackpanel, let that do the formatting
            }
            this.Title = "Game of Life";
            //Console.WriteLine($"CellSpace: {CellSpace.ActualWidth}x{CellSpace.ActualHeight}, Rect: {CellSpace.ActualWidth/x}x{CellSpace.ActualHeight/y}");
        }

        /// <summary>
        /// Single step/tick advancement method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void StepOnce(object sender, RoutedEventArgs args)
        {
            cg.Step();
        }

        /// <summary>
        /// Single step/tick advancement method for the timer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DoAStep(Object source, ElapsedEventArgs e)
        {
            cg.Step();
        } 

        /// <summary>
        /// Timer start and stop method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PlayAndStop(object sender, RoutedEventArgs args)
        {
            if (!timer.Enabled)
            {
                timer.Start();
                timer.Interval = 1000 / TickrateSlider.Value;
                //Console.WriteLine(timer.Interval);
                ((Button)sender).Content = "Stop";
            }
            else
            {
                timer.Stop();
                ((Button)sender).Content = "Play";
            }
        }

        /// <summary>
        /// Randomization method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Randomize(object sender, RoutedEventArgs e)
        {
            this.Title = "Game of Life **(Randomizing, hold on a sec...)**";
            cg.Randomize(PercentageSlider.Value);
            this.Title = "Game of Life";
        }

        /// <summary>
        /// Clear the grid method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Clear(object sender, RoutedEventArgs args)
        {
            this.Title = "Game of Life **(Clearing grid...)**";
            cg.Clear();
            this.Title = "Game of Life";
        }

        /// <summary>
        /// Locks the sizing sliders to equivalent values, making square dimensions, or unlocks them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockSizeToSquare(object sender, RoutedEventArgs e)
        {
            if (LockSquare.IsChecked == true)
            {
                Binding b = new Binding("Value");
                ySlider.DataContext = xSlider;
                ySlider.SetBinding(Slider.ValueProperty, b); //locks y to x, so when either changes the other matches in value
            }
            else
            {
                BindingOperations.ClearBinding(ySlider, Slider.ValueProperty); //remove binding to allow independent movement again
            }
        }
    }

}
