using System.Windows;      
using System.Windows.Media; 
using System.Windows.Controls;
using System.Windows.Shapes; 

namespace Paint;

public class Filling
{
    // обработка заливки, если жмем внутри фигуры - запускаем метод заливки фигуры, иначе, заливаем весь холст
    public void HandleFillingMouseDown(Point point, Canvas canvas, Color color)
    {
        var figure = FindFigurePoint(canvas, point);
        if (figure != null)
        {
            ApplyFillFigure(figure, color);
        }
        else
        {
            FillingCanvas(canvas, point, color);
        }

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
            var figure = canvas.Children[i] as FrameworkElement;
            if (figure != null && IsPointInFigure(figure, point))
            {
                return figure;
            }
        }
        return null;
    }
// проверка, что точка внутри фигуры: нашли координаты фигуры, если они есть, то создаем прямоугольник, с координатами и размером фигуры, и проверяем, в нем ли точка 
    private bool IsPointInFigure(FrameworkElement figure, Point point)
    {
        double left = Canvas.GetLeft(figure);
        double top = Canvas.GetTop(figure);
        if (double.IsNaN(left))
        {
            left = 0;
        }

        if (double.IsNaN(top))
        {
            top = 0;
        }
        var bounds = new  Rect(left, top, figure.ActualWidth, figure.ActualHeight);
        return bounds.Contains(point);

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
}