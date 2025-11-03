using System.Windows;      
using System.Windows.Media; 
using System.Windows.Controls;
using System.Windows.Shapes; 

namespace Paint;

public class Filling
{
    // обработка заливки, если жмем внутри фигуры - запускаем метод заливки фигуры, если жмем по обводке - меняем цвет обводки, иначе  заливаем весь холст
    public void HandleFillingMouseDown(Point point, Canvas canvas, Color color)
    {
        var figure = FindFigurePoint(canvas, point);
        var stroke = FindStrokePoint(canvas, point);
        if (figure != null)
        {
            ApplyFillFigure(figure, color);
            return;
        }

        if (stroke != null)
        {
            ApplyColorStroke(stroke, color);
            return;
        }
        else
        {
            FillingCanvas(canvas, point, color);
        }

    }
// меняем цвет обводки 
    private FrameworkElement FindStrokePoint(Canvas canvas, Point point)
    {
        for (int i = canvas.Children.Count - 1; i >= 0; i--)
        {
            var element = canvas.Children[i] as Shape;
            if (element != null && IsPointStroke(element, point))
            {
                return element;
            }
        }
        return null;
    }
//меняем цвет всего холста 
    private void FillingCanvas(Canvas canvas, Point point, Color color)
    {
        canvas.Background = new SolidColorBrush(color);
    }
// проверка, куда нажали, проходимся по всем элементам на холсте, если этот - фигура и курсор внутри него, возвращаем, что за фигура, и null, если не найдено
    private FrameworkElement FindFigurePoint(Canvas canvas, Point point)
    {
        for (int i = canvas.Children.Count - 1; i >= 0; i--)
        {
            var element = canvas.Children[i] as Shape;
            if (element != null && IsPointInFigure(element, point))
            {
                return element;
            }
        }
        return null;
    }
// проверяем, нажали ли на обводку 
    private bool IsPointStroke(Shape shape, Point point)
    {
        double left = Canvas.GetLeft(shape);
        double top = Canvas.GetTop(shape);
        if (double.IsNaN(left)) left = 0;
        if (double.IsNaN(top)) top = 0;
        
        Point relativePoint = new Point(point.X - left, point.Y - top);
        
        Geometry geometry = GetShapeGeometry(shape);
        Pen pen = new Pen(shape.Stroke, shape.StrokeThickness);
        
        return geometry.StrokeContains(pen, relativePoint);
    }
// проверка, что точка внутри фигуры: нашли координаты фигуры, если они есть, то создаем прямоугольник, с координатами и размером фигуры, и проверяем, в нем ли точка 
    private bool IsPointInFigure(Shape shape, Point point)
    {
        double left = Canvas.GetLeft(shape);
        double top = Canvas.GetTop(shape);
        if (double.IsNaN(left)) left = 0;
        if (double.IsNaN(top)) top = 0;
        
        Point relativePoint = new Point(point.X - left, point.Y - top);
        
        Geometry geometry = GetShapeGeometry(shape);
        Pen pen = new Pen(shape.Stroke, shape.StrokeThickness);
        
        // Точка в заливке И НЕ на обводке
        return geometry.FillContains(relativePoint) && 
               !geometry.StrokeContains(pen, relativePoint);

    }
    //получаем математичкое представление фигуры
    private Geometry GetShapeGeometry(Shape shape)
    {
        return shape.RenderedGeometry;
    }
// заливка фигур, где можно залить - заливаем, где нет - меняем цвет
    private void ApplyFillFigure(FrameworkElement element, Color color)
    {

        var brush = new SolidColorBrush(color);
        
        if (element is Rectangle rectangle)
        {
            rectangle.Fill = brush;
        }
        else if (element is Ellipse ellipse)
        {
            ellipse.Fill = brush;
        }
        else if (element is Polygon polygon)
        {

            polygon.Fill = brush;
        }
        else if (element is Polyline polyline)
        {
            polyline.Stroke = brush;
        }
        else if (element is Line line)
        {
            line.Stroke = brush;
        }
    }
    // заливаем обводку цветом 
        private void ApplyColorStroke(FrameworkElement element, Color color)
        {
            if (element is Shape shape)
            {
                shape.Stroke = new SolidColorBrush(color);
            }
        }
}