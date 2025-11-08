using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    public class CancelReturnManager
    {
        private readonly Stack<List<Shape>> _undoStack = new();
        private readonly Stack<List<Shape>> _redoStack = new();
        private readonly Canvas _canvas;
        private const int MaxHistorySteps = 5;

        public CancelReturnManager(Canvas canvas)
        {
            _canvas = canvas;
            // Сохраняем начальное пустое состояние
            SaveState();
        }

        public void SaveState()
        {
            // Сохраняем копии всех текущих фигур
            var currentShapes = new List<Shape>();
            foreach (var shape in _canvas.Children.OfType<Shape>())
            {
                currentShapes.Add(DeepCloneShape(shape));
            }

            _undoStack.Push(currentShapes);
            
            // Ограничиваем историю
            if (_undoStack.Count > MaxHistorySteps)
            {
                // Удаляем самое старое состояние
                var tempList = new List<List<Shape>>();
                while (_undoStack.Count > 0)
                {
                    tempList.Add(_undoStack.Pop());
                }
                // Оставляем только последние 5 состояний
                for (int i = tempList.Count - 1; i >= 0 && _undoStack.Count < MaxHistorySteps; i--)
                {
                    _undoStack.Push(tempList[i]);
                }
            }
            
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count <= 1) return;

            // Сохраняем текущее состояние 
            var currentState = new List<Shape>();
            foreach (var shape in _canvas.Children.OfType<Shape>())
            {
                currentState.Add(DeepCloneShape(shape));
            }
            _redoStack.Push(currentState);

            // Ограничиваем стек
            if (_redoStack.Count > MaxHistorySteps)
            {
                var tempList = new List<List<Shape>>();
                while (_redoStack.Count > 0)
                {
                    tempList.Add(_redoStack.Pop());
                }
                for (int i = tempList.Count - 1; i >= 0 && _redoStack.Count < MaxHistorySteps; i--)
                {
                    _redoStack.Push(tempList[i]);
                }
            }

            // Переходим к предыдущему состоянию
            _undoStack.Pop(); // Убираем текущее состояние
            
            if (_undoStack.Count > 0)
            {
                var previousState = _undoStack.Peek();
                RestoreState(previousState);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) return;

            // Сохраняем текущее состояние 
            var currentState = new List<Shape>();
            foreach (var shape in _canvas.Children.OfType<Shape>())
            {
                currentState.Add(DeepCloneShape(shape));
            }
            _undoStack.Push(currentState);

            // Ограничиваем  стек
            if (_undoStack.Count > MaxHistorySteps)
            {
                var tempList = new List<List<Shape>>();
                while (_undoStack.Count > 0)
                {
                    tempList.Add(_undoStack.Pop());
                }
                for (int i = tempList.Count - 1; i >= 0 && _undoStack.Count < MaxHistorySteps; i--)
                {
                    _undoStack.Push(tempList[i]);
                }
            }

            // Восстанавливаем состояние 
            var nextState = _redoStack.Pop();
            RestoreState(nextState);
        }

        private void RestoreState(List<Shape> shapes)
        {
            _canvas.Children.Clear();
            foreach (var shape in shapes)
            {// добавляем копию
                _canvas.Children.Add(DeepCloneShape(shape)); 
            }
        }
// создаем глубокие копии всех фигур
        private Shape DeepCloneShape(Shape original)
        {
            if (original == null) return null;

            Shape clone = original switch
            {
                System.Windows.Shapes.Rectangle rect => new System.Windows.Shapes.Rectangle
                {
                    Width = rect.Width,
                    Height = rect.Height,
                    Fill = rect.Fill?.CloneCurrentValue(),
                    Stroke = rect.Stroke?.CloneCurrentValue(),
                    StrokeThickness = rect.StrokeThickness,
                    RadiusX = rect.RadiusX,
                    RadiusY = rect.RadiusY
                },
                Ellipse ellipse => new Ellipse
                {
                    Width = ellipse.Width,
                    Height = ellipse.Height,
                    Fill = ellipse.Fill?.CloneCurrentValue(),
                    Stroke = ellipse.Stroke?.CloneCurrentValue(),
                    StrokeThickness = ellipse.StrokeThickness
                },
                Line line => new Line
                {
                    X1 = line.X1,
                    Y1 = line.Y1,
                    X2 = line.X2,
                    Y2 = line.Y2,
                    Stroke = line.Stroke?.CloneCurrentValue(),
                    StrokeThickness = line.StrokeThickness
                },
                Polygon polygon => new Polygon
                {
                    Points = new PointCollection(polygon.Points),
                    Fill = polygon.Fill?.CloneCurrentValue(),
                    Stroke = polygon.Stroke?.CloneCurrentValue(),
                    StrokeThickness = polygon.StrokeThickness
                },
                Polyline polyline => new Polyline
                {
                    Points = new PointCollection(polyline.Points),
                    Stroke = polyline.Stroke?.CloneCurrentValue(),
                    StrokeThickness = polyline.StrokeThickness
                },
                _ => null
            };

            if (clone != null)
            {
                // Копируем позицию на холст
                var left = Canvas.GetLeft(original);
                var top = Canvas.GetTop(original);
                
                if (!double.IsNaN(left))
                    Canvas.SetLeft(clone, left);
                if (!double.IsNaN(top))
                    Canvas.SetTop(clone, top);
            }

            return clone;
        }
        
    }
}

