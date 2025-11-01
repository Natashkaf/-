using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Paint;
// класс для рисования эллипса(точь-в-точь прямоугольник)
public class DrawingEllipse : DrawingTemplate
{
    private Ellipse ellipse;
    public override void StartDrawing()
    {
        ellipse = new Ellipse
        {
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            Fill = Fill,
            Width = 1,
            Height = 1
        };
        Canvas.SetLeft(ellipse, StartPoint.X);
        Canvas.SetTop(ellipse, StartPoint.Y);
        
        DrawingElement = ellipse;
    }

    public override void ProcessDrwing()
    {
        if (ellipse != null)
        {
            // вычисляем и устанавливаем размер
            double width = CurrentPoint.X - StartPoint.X;
            double height = CurrentPoint.Y - StartPoint.Y;
            ellipse.Width = Math.Abs(width);
            ellipse.Height = Math.Abs(height);
            // риусем в направлении, куда ведет курсор 
            if (width < 0)
                Canvas.SetLeft(ellipse, CurrentPoint.X);
            else
                Canvas.SetLeft(ellipse, StartPoint.X);
            if (height < 0)
                Canvas.SetTop(ellipse, CurrentPoint.Y);
            else
                Canvas.SetTop(ellipse, StartPoint.Y);
        }
    }

    public override void EndDrawing()
    {
        
    }
}