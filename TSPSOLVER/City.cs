using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TSPSOLVER
{
    public class City
    {
        public static int Diameter { get; } = 30; 
        public static int Radius { get; } = Diameter / 2;
        public int Id { get; } 
        public Point Position { get; private set; }
        public Ellipse CircleVisual { get; private set; }
        public TextBlock LabelVisual { get; private set; }

        public City(int id, Point position)
        {
            Id = id;
            Position = position;
        }

        public void CreateVisuals(bool isStartCity = false)
        {
            CircleVisual = new Ellipse
            {
                Width = Diameter,
                Height = Diameter,
                Fill = isStartCity ? Brushes.Green : Brushes.Blue
            };
            Canvas.SetLeft(CircleVisual, Position.X);
            Canvas.SetTop(CircleVisual, Position.Y);

            LabelVisual = new TextBlock
            {
                Text = (Id + 1).ToString(), 
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14
            };
            Canvas.SetLeft(LabelVisual, Position.X + Radius - 5);
            Canvas.SetTop(LabelVisual, Position.Y + Radius - 10);
            Canvas.SetZIndex(LabelVisual, 1);
        }

        public void AddToCanvas(Canvas canvas)
        {
            if (CircleVisual != null) canvas.Children.Add(CircleVisual);
            if (LabelVisual != null) canvas.Children.Add(LabelVisual);
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            if (CircleVisual != null) canvas.Children.Remove(CircleVisual);
            if (LabelVisual != null) canvas.Children.Remove(LabelVisual);
        }
    }
}