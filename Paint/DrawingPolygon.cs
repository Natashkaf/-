using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Paint;
// класс для многоугольника
public class DrawingPolygon : DrawingTemplate
{
    public Polygon polygon;
    public Line rubberBand;
    public List<Point> points = new List<Point>();
    public const double radius = 15;
    public bool _iscompleted = false;
    
    public bool IsCompleted => _iscompleted;
// начало рисования, настройка многоугольника, создание пунктирной линийй
    public override void StartDrawing()
    {
        polygon = new Polygon
        {
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            Fill = Fill,
            StrokeLineJoin = PenLineJoin.Round
        };
        
        rubberBand = new Line
        {
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            StrokeDashArray = new DoubleCollection { 4, 2 } 
        };
        
        points.Add(StartPoint);
        UpdatePolygonPoints();
        
        DrawingElement = polygon;
    }
// рисуем, обновляем резиновую линию
    public override void ProcessDrwing()
    {
        if (polygon != null && rubberBand != null && points.Count > 0 && !_iscompleted)
        {
            var lastPoint = points[points.Count - 1];
            rubberBand.X1 = lastPoint.X;
            rubberBand.Y1 = lastPoint.Y;
            rubberBand.X2 = CurrentPoint.X;
            rubberBand.Y2 = CurrentPoint.Y;
        }
    }
// особенный метод для многоугольника, он не завершает рисование, а лишь добавлет новую точку
    public override void EndDrawing()
    {
        if (!_iscompleted)
        {

            points.Add(CurrentPoint);

            UpdatePolygonPoints();
// если нажали рядом с первой точкой, то замыкаем многоугольник 
            if (points.Count >= 3 && IsFinishpoint(CurrentPoint))
            {
                points.Add(points[0]);
                _iscompleted = true;
                UpdatePolygonPoints();
                // и убираем резиновую линию
                if (rubberBand != null && rubberBand.Parent is Canvas canvas)
                {
                    canvas.Children.Remove(rubberBand);
                }
            }
        }
    }
// особенный метод для многоугольника - он уже завершает рисование  и убирает резиновую линию 
    public void CompletePolygon()
    {
        if (points.Count >= 3 && !_iscompleted)
        {
            points.Add(points[0]); 
            _iscompleted = true;
            UpdatePolygonPoints();
            
            if (rubberBand != null && rubberBand.Parent is Canvas canvas)
            {
                canvas.Children.Remove(rubberBand);
            }
        }
    }
    // особенный метод добавления на холст, по отдельности добавляет фигуру и резиновую линию 
    public void AddToCanvas(Canvas canvas)
    {
        canvas.Children.Add(polygon);
        canvas.Children.Add(rubberBand);
    }
// обновляет точки многоугольника, чтобы он был виден в процессе рисования, иначе придется рисовать вслепую 
    private void UpdatePolygonPoints()
    {
        polygon.Points = new PointCollection(points);
    }
// проверка, что точка достаточно близко к первой точке, просто считаем расстояние по формуле 
// нужна в завершении рисовании многоугольника 
    private bool IsFinishpoint(Point point)
    {
        if (points.Count == 0) return false;
        
        Point firstPoint = points[0];
        double dist = Math.Sqrt(Math.Pow(point.X - firstPoint.X, 2) + Math.Pow(point.Y - firstPoint.Y, 2));
        return dist <= radius;
    }
}