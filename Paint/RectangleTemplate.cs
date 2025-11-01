using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
namespace Paint;
// класс для рисования прямоугольников 
public class RectangleTemplate: DrawingTemplate
{
    private Rectangle rectangle;
// начинаем рисовать прямоугольник 
    public override void StartDrawing()
    {
        //делаем заготовку нового прямоугольника, настройки лежат в родителе
        rectangle = new Rectangle
        {
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            Fill = Fill,
            Width = 1,
            Height = 1
            
        };
        // начальная точка
        Canvas.SetLeft(rectangle, StartPoint.X);
        Canvas.SetTop(rectangle, StartPoint.Y);
        
        DrawingElement = rectangle;
    }

    public override void ProcessDrwing()
    {
        if (rectangle != null)
        {
            // вычисляем и устанавливаем размер
            double width = CurrentPoint.X - StartPoint.X;
            double height = CurrentPoint.Y - StartPoint.Y;
            rectangle.Width = Math.Abs(width);
            rectangle.Height = Math.Abs(height);
            // риусем в направлении, куда ведет курсор 
            if (width < 0)
                Canvas.SetLeft(rectangle, CurrentPoint.X);
            else
                Canvas.SetLeft(rectangle, StartPoint.X);
            if (height < 0)
                Canvas.SetTop(rectangle, CurrentPoint.Y);
            else
                Canvas.SetTop(rectangle, StartPoint.Y);
        }
    }

    public override void EndDrawing()
    {
        
    }
}