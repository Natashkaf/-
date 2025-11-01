using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Paint;
// класс для рисования линий
public class DrawingLine : DrawingTemplate
{
    private Line line;
    public override void StartDrawing()
    {
        line = new Line()
        {
            Stroke = Stroke,
            StrokeThickness = StrokeThickness,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            // начальные кооординаты и конечные совпадают, пока линия - точка
            X1 = StartPoint.X,
            X2 = StartPoint.X,
            Y1 = StartPoint.Y,
            Y2 = StartPoint.Y,

        };
        DrawingElement = line;

    }

    public override void ProcessDrwing()
    {
        if (line != null)
        {
            // обновляем конечные координаты 
           line.X2 = CurrentPoint.X;
           line.Y2 = CurrentPoint.Y;
        }
    }

    public override void EndDrawing()
    {
        // пока пусто, но будет работать, когда сделаю выбор цвета для заливки фигуры и ее контура
    }
}