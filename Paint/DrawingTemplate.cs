using System.Windows;
using System.Windows.Media;

namespace Paint;
// класс-шаблон для рисования
public abstract class DrawingTemplate
{
    // база: цвет кисти, толщина, заливка фигуры прозрачная, методы, реализованные в дочерних классах и готовая фигура
    public Brush Stroke { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public Brush Fill { get; set; } = Brushes.Transparent;
    public Point StartPoint { get; set; }
    public Point CurrentPoint { get; set; }
    public abstract void StartDrawing();
    public abstract void ProcessDrwing();
    public abstract void EndDrawing();
    public FrameworkElement DrawingElement { get;  protected set; }
}