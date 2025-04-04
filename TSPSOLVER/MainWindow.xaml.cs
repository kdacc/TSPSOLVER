using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TSPSOLVER
{
    public partial class MainWindow : Window
    {
        // --- Constants ---
        private const string DEFAULT_CITY_COUNT_TEXT = "Enter number of cities";
        private const int CITY_DIAMETER = 20;
        private const int CITY_RADIUS = CITY_DIAMETER / 2;

        // --- State Variables ---
        private int _numberOfCities = 0;
        private List<Ellipse> _cityCircles = new List<Ellipse>();
        // Stores edge info: Item1=City Index 1, Item2=City Index 2, Item3=Weight TextBox
        private List<Tuple<int, int, TextBox>> _edgeWeights = new List<Tuple<int, int, TextBox>>();
        private double[,] _weightsMatrix; // Adjacency matrix storing edge weights
        private List<Line> _lines = new List<Line>();

        public MainWindow()
        {
            InitializeComponent();
            // Initialize UI elements
            txtCityCount.Text = DEFAULT_CITY_COUNT_TEXT;
            txtCityCount.Foreground = Brushes.Gray; // Use Gray or the XAML #D9D9D9
        }

        private void txtCityCount_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCityCount.Text = "";
            txtCityCount.Foreground = Brushes.Black;
        }

        // Add this handler corresponding to the placeholder text logic
        private void txtCityCount_LostFocus(object sender, RoutedEventArgs e)
        {
            // Restore placeholder text if empty
            if (string.IsNullOrWhiteSpace(txtCityCount.Text))
            {
                txtCityCount.Text = DEFAULT_CITY_COUNT_TEXT;
                txtCityCount.Foreground = Brushes.Gray; // Use Gray or the XAML #D9D9D9
            }
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
            _edgeWeights.Clear();
            EdgeWeightsList.Items.Clear();
            _lines.Clear();

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

            foreach (var line in _lines)
            {
                canvasGraph.Children.Remove(line);
            }
            _lines.Clear();

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

                    _lines.Add(edge);
                    canvasGraph.Children.Add(edge);

                    TextBox weightTextBox = new TextBox { Width = 50, Text = "" };
                    StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                    panel.Children.Add(new TextBlock { Text = $"City {i + 1} - City {j + 1}: " });
                    panel.Children.Add(weightTextBox);

                    EdgeWeightsList.Items.Add(panel);
                    _edgeWeights.Add(Tuple.Create(i, j, weightTextBox));
                }
            }
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            // Generate random weights for all edges
            Random random = new Random();

            // Check if _edgeWeights is initialized and not empty
            if (_edgeWeights == null || _edgeWeights.Count == 0)
            {
                MessageBox.Show("Please create cities first by clicking the Draw button.");
                return;
            }

            foreach (var edgeData in _edgeWeights)
            {
                // Generate random weight between 1 and 100
                int weight = random.Next(1, 101);
                edgeData.Item3.Text = weight.ToString();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Make sure cities have been created
            if (_numberOfCities <= 0 || _edgeWeights == null || _edgeWeights.Count == 0)
            {
                MessageBox.Show("Please create cities first by clicking the Draw button.");
                return;
            }

            // Initialize weights matrix
            _weightsMatrix = new double[_numberOfCities, _numberOfCities];

            // Initialize matrix with zeros or infinity
            for (int i = 0; i < _numberOfCities; i++)
            {
                for (int j = 0; j < _numberOfCities; j++)
                {
                    _weightsMatrix[i, j] = i == j ? 0 : double.MaxValue;
                }
            }

            // Populate matrix with edge weights
            bool allValid = true;
            int edgeIndex = 0;

            for (int i = 0; i < _numberOfCities; i++)
            {
                for (int j = i + 1; j < _numberOfCities; j++)
                {
                    if (edgeIndex < _edgeWeights.Count)
                    {
                        var edgeData = _edgeWeights[edgeIndex];
                        if (double.TryParse(edgeData.Item3.Text, out double weight))
                        {
                            // Store weight in both directions (symmetric matrix)
                            _weightsMatrix[i, j] = weight;
                            _weightsMatrix[j, i] = weight;
                        }
                        else
                        {
                            MessageBox.Show($"Invalid weight for edge between City {i + 1} and City {j + 1}. Please enter a valid number.");
                            allValid = false;
                            break;
                        }
                        edgeIndex++;
                    }
                }

                if (!allValid)
                    break;
            }

            if (allValid)
                MessageBox.Show("Weights saved successfully!");
        }

        private void btnGreedyAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedAlgorithm = "Greedy";
            MessageBox.Show("Greedy algorithm selected");
        }

        private void btnNearestNeighborAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedAlgorithm = "NearestNeighbor";
            MessageBox.Show("Nearest Neighbor algorithm selected");
        }

        private void btnSimulatedAnnealingAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedAlgorithm = "SimulatedAnnealing";
            MessageBox.Show("Simulated Annealing algorithm selected");
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            if (_weightsMatrix == null)
            {
                MessageBox.Show("Please set edge weights first by clicking the Submit button.");
                return;
            }

            // Clear previous results
            canvasResults.Children.Clear();

            // Initialize pathArray to avoid CS0165 error
            int[] pathArray = new int[0];
            double totalDistance = 0;

            // Call the selected algorithm
            switch (_selectedAlgorithm)
            {
                case "Greedy":
                    (pathArray, totalDistance) = GreedyTSP.GreedyAlgorithm(_weightsMatrix);
                    break;
                case "NearestNeighbor":
                    (pathArray, totalDistance) = NearestNeighbor.NearestNeighborTSP(_weightsMatrix);
                    break;
                case "SimulatedAnnealing":
                    // Simulated Annealing algorithm is not implemented yet
                    break;
                default:
                    MessageBox.Show("Please select an algorithm first.");
                    return;
            }

            // Convert array to list for consistency with other methods
            List<int> path = pathArray.ToList();
            _resultPath = path;
            _resultDistance = totalDistance;

            // Display results
            DisplayResults(path, totalDistance);
        }

        // You'll still need these fields and methods:
        private string _selectedAlgorithm;
        private List<int> _resultPath;
        private double _resultDistance;

        private void DisplayResults(List<int> path, double totalDistance)
        {
            // Display the path and total distance in the results canvas
            TextBlock resultsText = new TextBlock
            {
                Text = $"Path: {string.Join(" → ", path.Select(i => (i + 1).ToString()))}",
                FontSize = 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(resultsText, 10);
            Canvas.SetTop(resultsText, 10);
            canvasResults.Children.Add(resultsText);

            TextBlock distanceText = new TextBlock
            {
                Text = $"Total Distance: {totalDistance:F2}",
                FontSize = 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(distanceText, 10);
            Canvas.SetTop(distanceText, 40);
            canvasResults.Children.Add(distanceText);

            // Also visualize the solution on the graph
            VisualizeSolution(path);
        }

        private void VisualizeSolution(List<int> path)
        {
            // Remove previous solution lines
            foreach (var child in canvasGraph.Children.OfType<Line>().Where(l => l.Stroke == Brushes.Red).ToList())
            {
                canvasGraph.Children.Remove(child);
            }

            // Draw the solution path
            for (int i = 0; i < path.Count - 1; i++)
            {
                Point start = new Point(
                    Canvas.GetLeft(_cityCircles[path[i]]) + 10,
                    Canvas.GetTop(_cityCircles[path[i]]) + 10
                );

                Point end = new Point(
                    Canvas.GetLeft(_cityCircles[path[i + 1]]) + 10,
                    Canvas.GetTop(_cityCircles[path[i + 1]]) + 10
                );

                Line pathLine = new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };

                canvasGraph.Children.Add(pathLine);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            canvasGraph.Children.Clear();
            canvasResults.Children.Clear();
            _cityCircles.Clear();
            _edgeWeights.Clear();
            EdgeWeightsList.Items.Clear();
            _lines.Clear();
            _weightsMatrix = null;
            _numberOfCities = 0;
            txtCityCount.Text = "Enter number of cities";
            txtCityCount.Foreground = Brushes.Gray;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_resultPath == null || _resultPath.Count == 0)
            {
                MessageBox.Show("Please solve the problem first.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt"
            };

            if (saveDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                {
                    writer.WriteLine($"TSP Solution using {_selectedAlgorithm} algorithm");
                    writer.WriteLine($"Number of cities: {_numberOfCities}");
                    writer.WriteLine("Path: " + string.Join(" -> ", _resultPath.Select(i => (i + 1).ToString())));
                    writer.WriteLine($"Total distance: {_resultDistance}");
                }

                MessageBox.Show("Results saved successfully!");
            }
        }
    }
}