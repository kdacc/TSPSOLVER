using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TSPSOLVER
{
    public partial class MainWindow : Window
    {
        private const string DEFAULT_CITY_COUNT_TEXT = "Enter number of cities [2; 15]";
        private const int MIN_CITIES = 2;
        private const int MAX_CITIES = 15;
        private const int MIN_EDGE_WEIGHT = 1; 
        private const int MAX_EDGE_WEIGHT = 100;

        private TSPProblemData _problemData;
        private ITSPSolver _selectedSolver;
        private TSPSolution _currentSolution;
        private List<Line> _solutionPathLines = new List<Line>();


        public MainWindow()
        {
            InitializeComponent();
            _problemData = new TSPProblemData();
            txtCityCount.Text = DEFAULT_CITY_COUNT_TEXT;
            txtCityCount.Foreground = Brushes.Gray;
        }

        private void txtCityCount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCityCount.Text == DEFAULT_CITY_COUNT_TEXT)
            {
                txtCityCount.Text = "";
                txtCityCount.Foreground = Brushes.Black;
            }
        }

        private void txtCityCount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCityCount.Text))
            {
                txtCityCount.Text = DEFAULT_CITY_COUNT_TEXT;
                txtCityCount.Foreground = Brushes.Gray;
            }
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCityCount.Text, out int numberOfCities))
            {
                if (numberOfCities < MIN_CITIES)
                {
                    MessageBox.Show($"Мінімальна кількість міст повинна бути {MIN_CITIES}.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (numberOfCities > MAX_CITIES)
                {
                    MessageBox.Show($"Максимальна кількість міст не повинна перевищувати {MAX_CITIES}.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _problemData.Clear(canvasGraph, EdgeWeightsList);
                canvasResults.Children.Clear(); 
                ClearSolutionPathVisuals();


                _problemData.GenerateCities(numberOfCities, canvasGraph, canvasGraph.ActualWidth, canvasGraph.ActualHeight);
                _problemData.GenerateEdgeControls(canvasGraph, EdgeWeightsList);
                _currentSolution = null; 
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректне число міст.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            if (_problemData.EdgeWeightControls == null || !_problemData.EdgeWeightControls.Any())
            {
                MessageBox.Show("Будь ласка, спочатку створіть міста (натиснувши кнопку 'Намалювати міста').", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Random random = new Random();
            foreach (var edgeData in _problemData.EdgeWeightControls)
            {
                 int weight = random.Next(MIN_EDGE_WEIGHT, MAX_EDGE_WEIGHT + 1);  
                edgeData.Item3.Text = weight.ToString();
                _problemData.UpdateEdgeWeightLabelOnCanvas(canvasGraph, edgeData.Item1, edgeData.Item2, weight.ToString());
            }
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_problemData.TrySubmitWeights(out string errorMessage))
            {
                MessageBox.Show("Ваги успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach (var edgeData in _problemData.EdgeWeightControls)
                {
                    _problemData.UpdateEdgeWeightLabelOnCanvas(canvasGraph, edgeData.Item1, edgeData.Item2, edgeData.Item3.Text);
                }
            }
            else
            {
                MessageBox.Show(errorMessage, "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGreedyAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedSolver = new GreedyTSPSolver();
            MessageBox.Show($"{_selectedSolver.Name} обрано.");
        }

        private void btnNearestNeighborAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedSolver = new NearestNeighborTSPSolver();
            MessageBox.Show($"{_selectedSolver.Name} обрано.");
        }

        private void btnSimulatedAnnealingAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            _selectedSolver = new SimulatedAnnealingTSPSolver();
            MessageBox.Show($"{_selectedSolver.Name} обрано.");
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSolver == null)
            {
                MessageBox.Show("Будь ласка, спочатку оберіть алгоритм.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_problemData.WeightsMatrix == null)
            {
                MessageBox.Show("Будь ласка, спочатку підтвердіть ваги ребер (кнопка 'Підтвердити ваги').", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                _currentSolution = _selectedSolver.Solve(_problemData.WeightsMatrix);

            DisplayResults(_currentSolution);
            VisualizeSolutionPath(_currentSolution.Path);
        }

        private void DisplayResults(TSPSolution solution)
        {
            canvasResults.Children.Clear();
            if (solution == null) return;

            TextBlock resultsText = new TextBlock
            {
                Text = $"Шлях: {string.Join(" → ", solution.Path.Select(i => (i + 1).ToString()))}",
                FontSize = 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(resultsText, 10);
            Canvas.SetTop(resultsText, 10);
            canvasResults.Children.Add(resultsText);

            TextBlock distanceText = new TextBlock
            {
                Text = $"Загальна відстань: {solution.TotalDistance:F2}", //ЗНАКИ ПІСЛЯ КОМИ
                FontSize = 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(distanceText, 10);
            Canvas.SetTop(distanceText, 40); 
            canvasResults.Children.Add(distanceText);

            TextBlock iterationsText = new TextBlock
            {
                Text = $"Кількість ітерацій: {solution.Iterations}", 
                FontSize = 16,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(iterationsText, 10);
            Canvas.SetTop(iterationsText, 70); 
            canvasResults.Children.Add(iterationsText);
        }

        private void ClearSolutionPathVisuals()
        {
            foreach (var line in _solutionPathLines)
            {
                canvasGraph.Children.Remove(line);
            }
            _solutionPathLines.Clear();
        }

        private void VisualizeSolutionPath(List<int> path)
        {
            ClearSolutionPathVisuals(); 

            if (path == null || path.Count < 2 || _problemData.Cities.Count == 0) return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                City city1 = _problemData.Cities[path[i]];
                City city2 = _problemData.Cities[path[i + 1]];

                Point start = new Point(
                    city1.Position.X + City.Radius,
                    city1.Position.Y + City.Radius
                );
                Point end = new Point(
                    city2.Position.X + City.Radius,
                    city2.Position.Y + City.Radius
                );

                Line pathLine = new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 3, 
                    StrokeEndLineCap = PenLineCap.Triangle, 
                    StrokeStartLineCap = PenLineCap.Round
                };
                Canvas.SetZIndex(pathLine, 1); 
                _solutionPathLines.Add(pathLine);
                canvasGraph.Children.Add(pathLine);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _problemData.Clear(canvasGraph, EdgeWeightsList);
            canvasResults.Children.Clear();
            ClearSolutionPathVisuals();

            _selectedSolver = null;
            _currentSolution = null;

            txtCityCount.Text = DEFAULT_CITY_COUNT_TEXT;
            txtCityCount.Foreground = Brushes.Gray;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSolution == null)
            {
                MessageBox.Show("Будь ласка, спочатку розв'яжіть задачу.", "Немає рішення", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_selectedSolver == null)
            {
                MessageBox.Show("Неможливо визначити використаний алгоритм.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = $"TSP_Solution_{_selectedSolver.Name.Replace(" ", "")}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        writer.WriteLine($"TSP Solution using {_selectedSolver.Name}");
                        writer.WriteLine($"Number of cities: {_problemData.NumberOfCities}");
                        writer.WriteLine("Path: " + string.Join(" -> ", _currentSolution.Path.Select(i => (i + 1).ToString())));
                        writer.WriteLine($"Total distance: {_currentSolution.TotalDistance:F2}");
                        writer.WriteLine("\nWeights Matrix:");
                        for (int i = 0; i < _problemData.NumberOfCities; i++)
                        {
                            for (int j = 0; j < _problemData.NumberOfCities; j++)
                            {
                                writer.Write(_problemData.WeightsMatrix[i, j] == double.MaxValue ? "Inf\t" : $"{_problemData.WeightsMatrix[i, j]}\t");
                            }
                            writer.WriteLine();
                        }
                    }
                    MessageBox.Show("Результати успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при збереженні файлу: {ex.Message}", "Помилка збереження", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}