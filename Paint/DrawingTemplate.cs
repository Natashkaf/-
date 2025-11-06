using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    public abstract class DrawingTemplate
    {
        public Shape DrawingElement { get; set; }
        public Point StartPoint { get; set; }
        public Point CurrentPoint { get; set; }
        public Brush Stroke { get; set; }
        public Brush Fill { get; set; }
        public double StrokeThickness { get; set; }

        public abstract void StartDrawing();
        public abstract void ProcessDrwing();
        public abstract void EndDrawing();
        
    }
}