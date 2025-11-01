using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Paint;
// класс для рисования кистью/карандашом
public class DrawingFree : DrawingTemplate
{
    // создаем ломаную линию и список всех точек, через которые прошла мышь 
    private Polyline polyline;
    private List<Point> points = new List<Point>();

    public override void StartDrawing()
    {
        // создаем линию
        polyline = new Polyline
        {
            // это в родителе
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            // плавное соединение точек, крулое начало линии и конец соответственно
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };
        // запомнили, откуда начали, отдали точки ломанной
        points.Add(StartPoint);
        polyline.Points = new PointCollection(points);
        // показали, что начали рисовать
        DrawingElement = polyline;
    }

    public override void ProcessDrwing()
    {
        if (polyline != null)
        {
            // добавляем новую точку и обновляем список всех точек ломаной 
            points.Add(CurrentPoint);
            polyline.Points = new PointCollection(points);
        }
    }

    public override void EndDrawing()
    {
    }
}