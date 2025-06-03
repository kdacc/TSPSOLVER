using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;


namespace TSPSOLVER
{
    public class TSPProblemData
    {
        public List<City> Cities { get; private set; }
        public double[,] WeightsMatrix { get; set; }
        public List<Tuple<int, int, TextBox>> EdgeWeightControls { get; private set; } 
        public List<Line> EdgeLines { get; private set; }

        const double MIN_VALID_DISTANCE = 1e-2;
        const double MAX_VALID_DISTANCE = 1e2;

        public int NumberOfCities => Cities != null ? Cities.Count : 0;  //перевірка на null 

        public TSPProblemData()
        {
            Cities = new List<City>();
            EdgeWeightControls = new List<Tuple<int, int, TextBox>>();
            EdgeLines = new List<Line>();
        }

        public void Clear(Canvas graphCanvas, ItemsControl edgeWeightsList)
        {
            foreach (var city in Cities)
            {
                city.RemoveFromCanvas(graphCanvas);
            }
            Cities.Clear();

            foreach (var line in EdgeLines)
            {
                graphCanvas.Children.Remove(line);
            }
            EdgeLines.Clear();

            var weightLabels = graphCanvas.Children.OfType<TextBlock>()
                                       .Where(tb => tb.Name != null && tb.Name.StartsWith("WeightLabel_"))
                                       .ToList();
            foreach (var label in weightLabels)
            {
                graphCanvas.Children.Remove(label);
            }


            EdgeWeightControls.Clear();
            edgeWeightsList.Items.Clear();
            WeightsMatrix = null;
        }

        public void GenerateCities(int count, Canvas graphCanvas, double canvasWidth, double canvasHeight)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                Point position = new Point(
                    random.Next((int)canvasWidth - City.Diameter),
                    random.Next((int)canvasHeight - City.Diameter)
                );
                var city = new City(i, position);
                city.CreateVisuals(i == 0); 
                city.AddToCanvas(graphCanvas);
                Cities.Add(city);
            }
        }

        public void GenerateEdgeControls(Canvas graphCanvas, ItemsControl edgeWeightsList)
        {
            EdgeWeightControls.Clear();
            edgeWeightsList.Items.Clear();

            foreach (var line in EdgeLines) 
            {
                graphCanvas.Children.Remove(line);
            }
            EdgeLines.Clear();

            var oldWeightLabels = graphCanvas.Children.OfType<TextBlock>()
                                           .Where(tb => tb.Name != null && tb.Name.StartsWith("WeightLabel_"))
                                           .ToList();
            foreach (var label in oldWeightLabels)
            {
                graphCanvas.Children.Remove(label);
            }


            for (int i = 0; i < NumberOfCities; i++)
            {
                for (int j = i + 1; j < NumberOfCities; j++)
                {
                    City city1 = Cities[i];
                    City city2 = Cities[j];

                    Point start = new Point(city1.Position.X + City.Radius, city1.Position.Y + City.Radius);
                    Point end = new Point(city2.Position.X + City.Radius, city2.Position.Y + City.Radius);

                    Line edgeLine = new Line
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1
                    };
                    EdgeLines.Add(edgeLine);
                    graphCanvas.Children.Add(edgeLine);
                    Canvas.SetZIndex(edgeLine, -1); 

                    TextBox weightTextBox = new TextBox { Width = 50, Text = "" };
                    StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                    panel.Children.Add(new TextBlock { Text = $"Місто {i + 1} - Місто {j + 1}: " });
                    panel.Children.Add(weightTextBox);

                    edgeWeightsList.Items.Add(panel);
                    EdgeWeightControls.Add(Tuple.Create(i, j, weightTextBox));
                }
            }
        }
        public bool TrySubmitWeights(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (NumberOfCities <= 0)
            {
                errorMessage = "Будь ласка, спочатку створіть міста.";
                return false;
            }
            if (EdgeWeightControls == null || !EdgeWeightControls.Any())
            {
                errorMessage = "Немає даних про ваги ребер.";
                return false;
            }

            WeightsMatrix = new double[NumberOfCities, NumberOfCities];
            for (int i = 0; i < NumberOfCities; i++)
            {
                for (int j = 0; j < NumberOfCities; j++)
                {
                    WeightsMatrix[i, j] = (i == j) ? 0 : double.MaxValue;
                }
            }

            foreach (var edgeData in EdgeWeightControls)
            {
                int cityIndex1 = edgeData.Item1;
                int cityIndex2 = edgeData.Item2;
                TextBox weightTextBox = edgeData.Item3;

                if (string.IsNullOrWhiteSpace(weightTextBox.Text))
                {
                    errorMessage = $"Будь ласка, введіть вагу для ребра між містами {cityIndex1 + 1} та {cityIndex2 + 1}.";
                    return false;
                }

                if (weightTextBox.Text.Length > 15)
                {
                    errorMessage = $"Вага для ребра між містом {cityIndex1 + 1} та містом {cityIndex2 + 1} не може перевищувати 15 символів.";
                    return false;
                }

                if (double.TryParse(weightTextBox.Text, out double weight))
                {
                    if (weight < MIN_VALID_DISTANCE || weight > MAX_VALID_DISTANCE)
                    {
                        errorMessage = $"Відстань ({weight}) для ребра між містом {cityIndex1 + 1} та містом {cityIndex2 + 1} повинна бути в діапазоні від {MIN_VALID_DISTANCE} до {MAX_VALID_DISTANCE}.";
                        return false;
                    }
                    if (weight > 0)
                    {
                        WeightsMatrix[cityIndex1, cityIndex2] = weight;
                        WeightsMatrix[cityIndex2, cityIndex1] = weight;
                    }
                    else
                    {
                        errorMessage = $"Вага для ребра між містами {cityIndex1 + 1} та {cityIndex2 + 1} ({weight}) повинна бути додатнім числом.";
                        return false;
                    }
                }
                else
                {
                    errorMessage = $"Некоректна вага '{weightTextBox.Text}' для ребра між містами {cityIndex1 + 1} та {cityIndex2 + 1}.";
                    return false;
                }
            }
            return true;
        }

        public void UpdateEdgeWeightLabelOnCanvas(Canvas canvasGraph, int city1Idx, int city2Idx, string weightText)
        {
            if (city1Idx < 0 || city1Idx >= Cities.Count || city2Idx < 0 || city2Idx >= Cities.Count) return;

            City city1 = Cities[city1Idx];
            City city2 = Cities[city2Idx];

            Point start = new Point(
                city1.Position.X + City.Radius,
                city1.Position.Y + City.Radius
            );

            Point end = new Point(
                city2.Position.X + City.Radius,
                city2.Position.Y + City.Radius
            );

            Point midpoint = new Point(
                (start.X + end.X) / 2,
                (start.Y + end.Y) / 2
            );

            string labelName = $"WeightLabel_{city1Idx}_{city2Idx}";
            var existingLabel = canvasGraph.Children.OfType<TextBlock>()
                                          .FirstOrDefault(tb => tb.Name == labelName);
            if (existingLabel != null)
            {
                canvasGraph.Children.Remove(existingLabel);
            }

            if (string.IsNullOrWhiteSpace(weightText) || !double.TryParse(weightText, out _)) return;


            TextBlock weightLabel = new TextBlock
            {
                Name = labelName, 
                Text = weightText,
                Foreground = Brushes.DarkSlateGray,
                Background = Brushes.LightYellow,
                Padding = new Thickness(2),
                FontSize = 10,
                FontWeight = FontWeights.Normal
            };

            canvasGraph.Children.Add(weightLabel);
            weightLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity)); 
            Canvas.SetLeft(weightLabel, midpoint.X - weightLabel.ActualWidth / 2);
            Canvas.SetTop(weightLabel, midpoint.Y - weightLabel.ActualHeight / 2);
            Canvas.SetZIndex(weightLabel, 2); 
            Canvas.SetZIndex(weightLabel, 2); 
        }
    }
}