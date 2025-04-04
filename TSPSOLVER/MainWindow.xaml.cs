using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
//рандом - рандомні числа що віповідать вагам міст, тобто це ваги міст
//потрібно в xaml.cs дописати, щоб ваги які введе користувач зберігалися,
//оскільки далі саме вони будуть змінними в розвязані задачі комівояжера кнопка submit - повина відповідати за їх збереження
//додати всі алгоритми (жадібний(greedyAlgorithm), найближчий сусід(nearestNeighbor), симульоване відпускання(simulatedAnnealing))
namespace TSPSOLVER
{
    public partial class MainWindow : Window
    {
        private int _numberOfCities;
        private List<Ellipse> _cityCircles = new List<Ellipse>();
        private List<Tuple<Point, Point, TextBox>> _edgeWeights = new List<Tuple<Point, Point, TextBox>>();
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtCityCount_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCityCount.Text = "";
            txtCityCount.Foreground = Brushes.Black;
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCityCount.Text, out _numberOfCities) && _numberOfCities > 0)
            {
                GenerateCities();
            }
            else
            {
                MessageBox.Show("Please enter a valid number of cities.");
            }
        }

        private void GenerateCities()
        {
            canvasGraph.Children.Clear();
            _cityCircles.Clear();

            Random random = new Random();
            for (int i = 0; i < _numberOfCities; i++)
            {
                Ellipse cityCircle = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.Blue
                };

                Point position = new Point(random.Next((int)canvasGraph.ActualWidth - 20), random.Next((int)canvasGraph.ActualHeight - 20));
                Canvas.SetLeft(cityCircle, position.X);
                Canvas.SetTop(cityCircle, position.Y);

                canvasGraph.Children.Add(cityCircle);
                _cityCircles.Add(cityCircle);
            }

            GenerateEdgeWeights();
        }

        private void GenerateEdgeWeights()
        {
            _edgeWeights.Clear();
            EdgeWeightsList.Items.Clear();

            for (int i = 0; i < _cityCircles.Count; i++)
            {
                for (int j = i + 1; j < _cityCircles.Count; j++)
                {
                    Point start = new Point(Canvas.GetLeft(_cityCircles[i]) + 10, Canvas.GetTop(_cityCircles[i]) + 10);
                    Point end = new Point(Canvas.GetLeft(_cityCircles[j]) + 10, Canvas.GetTop(_cityCircles[j]) + 10);

                    Line edge = new Line
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    };

                    canvasGraph.Children.Add(edge);

                    TextBox weightTextBox = new TextBox { Width = 50, Text = ""};
                    StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                    panel.Children.Add(new TextBlock { Text = $"City {i + 1} - City {j + 1}: " });
                    panel.Children.Add(weightTextBox);

                    EdgeWeightsList.Items.Add(panel);
                    _edgeWeights.Add(Tuple.Create(start, end, weightTextBox));
                }
            }
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            foreach (var item in EdgeWeightsList.Items)
            {
                var panel = item as StackPanel;
                if (panel != null && panel.Children.Count > 1)
                {
                    var textBox = panel.Children[1] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = random.Next(1, 100).ToString();
                    }
                }
            }
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Processing solution...");
        }
        private void btnGreedyAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running Greedy Algorithm...");
        }

        private void btnNearestNeighborAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running Nearest Neighbor Algorithm...");
        }

        private void btnSimulatedAnnealingAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running Simulated Annealing Algorithm...");
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Solving TSP..."); 
        }        

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deleting solution...");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Saving solution...");
        }
    }
}