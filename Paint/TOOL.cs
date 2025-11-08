using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
// класс-управляющий приложения, управляет фигурами, обрабатывает события мыши, чтобы не загромождать главный класс
namespace Paint
{
    // просто список всего, что можно нарисовать
    public enum ToolType
    {  Selection,
        Pencil, 
        Brush, 
        Rectangle, 
        Line, 
        Ellipse, 
        Polygon,
        Fill
    }

    public class ToolController
    {
        private DrawingTemplate currentDrawing;
        private bool isDrawing = false;
        private Canvas currentCanvas;
        private Filling _filling = new Filling();
        private SelectionController _selectionController; 
        private CancelReturnManager _cancelReturnManager;

        public ToolType CurrentTool { get; set; } = ToolType.Brush;
        public Color CurrentColor { get; set; } = Colors.Black;
        public double PencilThickness { get; set; } = 1;
        public double BrushThickness { get; set; } = 5;
        public double ShapeThickness { get; set; } = 2;
        public void SetSelectionController(SelectionController selectionController)
        {
            _selectionController = selectionController;
        }
        //связывает этот класс с классом CancelReturnManager
        public void SetCancelReturnManager(CancelReturnManager manager)
        {
            _cancelReturnManager = manager;
        }

        //обработка нажатия мыши на кнопку рисования какой-либо фигуры
        public void HandleMouseDown(Point point, Canvas canvas)
        {
            currentCanvas = canvas;
            if (CurrentTool == ToolType.Selection && _selectionController != null)
            {
                _selectionController.HandleMouseDown(point);
                return;
            }
            // если рисуем многоугольник - пропускаем всё, он особенный, для него свой обработчик 

            if (CurrentTool == ToolType.Polygon)
            {
                HandlePolygonMouseDown(point, canvas);
                return;
            }
//если заливка - тоже все пропускаем, для нее свой обработчик в ее классе 
            if (CurrentTool == ToolType.Fill)
            {
               _filling.HandleFillingMouseDown(point, canvas, CurrentColor);
                return;
            }
            // иначе, начинаем рисовать, настраиваем фигуру, запускаем метод рисования фигуры, добавляем ее на холст

            isDrawing = true;

            currentDrawing = CreateDrawingTool();
            currentDrawing.StartPoint = point;
            currentDrawing.CurrentPoint = point;
            currentDrawing.Stroke = new SolidColorBrush(CurrentColor);
            currentDrawing.StrokeThickness = GetCurrentThickness();

            currentDrawing.StartDrawing();
            canvas.Children.Add(currentDrawing.DrawingElement);
        }
// обработчик для многоугольника
        private void HandlePolygonMouseDown(Point point, Canvas canvas)
        {
            // если мы уже рисуем многоугольник, то добавляем к нему точку и обновляем
            if (currentDrawing is DrawingPolygon polygon && !polygon.IsCompleted)
            {
                polygon.CurrentPoint = point;
                polygon.EndDrawing();
            }
            // иначе создаем новый многоугольник, настраиваем его, запускаем метод начала рисования
            else
            {

                isDrawing = true;
                currentDrawing = new DrawingPolygon();
                currentDrawing.StartPoint = point;
                currentDrawing.CurrentPoint = point;
                currentDrawing.Stroke = new SolidColorBrush(CurrentColor);
                currentDrawing.StrokeThickness = ShapeThickness;
                currentDrawing.Fill = Brushes.Transparent;

                currentDrawing.StartDrawing();
                // особенное добавление на холст для особенной фигуры: вызываем метод, который добавляет на холст и саму фигуру и пунктирную линию
                if (currentDrawing is DrawingPolygon newPolygon)
                {
                    newPolygon.AddToCanvas(canvas);
                }
            }
        }
// обработка движения мыши при рисовании
        public void HandleMouseMove(Point point)
        {
            if (CurrentTool == ToolType.Selection && _selectionController != null)
            {
                _selectionController.HandleMouseMove(point);
                return;
            }
            if (!isDrawing || currentDrawing == null) return;
// если рисуем, то постоянно перерисовываем фигуру, относительно новой позиции курсора
            currentDrawing.CurrentPoint = point;
            currentDrawing.ProcessDrwing();
        }
// обработчик отпускания кнопки мыши
        public void HandleMouseUp()
        {
            if (CurrentTool == ToolType.Selection && _selectionController != null)
            {
                _selectionController.HandleMouseUp();
                return;
            }
            // если не многоугольник, то заканчиваем рисовать 
            if (!isDrawing || CurrentTool == ToolType.Polygon) return;

            isDrawing = false;
            currentDrawing?.EndDrawing();
            _cancelReturnManager?.SaveState();
            currentDrawing = null;
        }

        //завершение рисование многоугольника
        public void HandlePolygonDoubleClick()
        {
            if (currentDrawing is DrawingPolygon polygon)
            {
                polygon.CompletePolygon();
                isDrawing = false;
                _cancelReturnManager?.SaveState();
                currentDrawing = null;
            }
        }
//  фабричный метод который создает нужный инстумент в зависимости от выбора
        private DrawingTemplate CreateDrawingTool()
        {
            return CurrentTool switch
            {
                ToolType.Pencil => new DrawingFree(),
                ToolType.Brush => new DrawingFree(),
                ToolType.Rectangle => new RectangleTemplate(),
                ToolType.Line => new DrawingLine(),
                ToolType.Ellipse => new DrawingEllipse(),
                ToolType.Polygon => new DrawingPolygon(),
                _ => new DrawingFree()
            };
        }
// устанавливает толщину линии
        private double GetCurrentThickness()
        {
            return CurrentTool switch
            {
                ToolType.Pencil => PencilThickness,
                ToolType.Brush => BrushThickness,
                _ => ShapeThickness
            };
        }
        
        
    }
}