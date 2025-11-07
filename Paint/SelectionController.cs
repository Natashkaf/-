using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Paint
{
    // класс, управляющий выделением фигур: выделяет, перемещает, масштабирует.
    public class SelectionController
    {
        private Canvas canvas;
        private FrameworkElement selectedElement;
        private Point startPoint;
        private bool isDragging = false;
        private bool isResizing = false;
        private ResizeHandle activeHandle;
        private Rectangle selectionBorder;
        private List<Rectangle> resizeHandles;
        private const double HandleSize = 8;
        private const double BorderThickness = 2;
        private Rect originalBounds;
        private Point originalMousePoint;
        private bool isSelectionMode = false;
        // визуализирует выделение фигуры 
        public Rectangle SelectionBorder => selectionBorder;
// конструктор класса 
        public SelectionController(Canvas canvas)
        {
            this.canvas = canvas;
            InitializeSelectionVisuals();
        }
        // включает и выключает выделение фигур 
        public void SetSelectionMode(bool enabled)
        {
            isSelectionMode = enabled;
            if (!enabled)
            {
                ClearSelection();
            }
        }
        // проверяет, был ли клик по квадратикам, которые масштабируют фигуру
        public bool IsResizeHandle(FrameworkElement element)
        {
            return resizeHandles.Contains(element);
        }
        // настраивает внешний вид выделения 
        private void InitializeSelectionVisuals()
        {
            //создали прямоугольник с  рамкой для  выделения, сделали ее голубой, пунктирной, фон квадратика прозрачным
            selectionBorder = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = BorderThickness,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                Fill = Brushes.Transparent,
                IsHitTestVisible = false
            };
            // создаем список с квадратиками для выделения
            resizeHandles = new List<Rectangle>();
            // в цикле создаем сами квадратики, которые изначально не видно и со своим курсором каждый 
            for (int i = 0; i < 8; i++)
            {
                int handleIndex = i;

                var handle = new Rectangle
                {
                    Width = HandleSize,
                    Height = HandleSize,
                    Fill = Brushes.White,
                    Stroke = Brushes.DodgerBlue,
                    StrokeThickness = 1,
                    Cursor = GetHandleCursor((ResizeHandle)handleIndex),
                    Visibility = Visibility.Collapsed
                };
                // обработчик клика по квадратику 
                handle.MouseLeftButtonDown += (s, e) =>
                {
                    // если включено выделение, то запускаем метод для масштабирования
                    if (!isSelectionMode) return;
                    e.Handled = true;
                    StartResizing((ResizeHandle)handleIndex, e.GetPosition(canvas));
                };
                //добавляем каждый квадратик в список 
                resizeHandles.Add(handle);
            }
        }
// отвечает за начало изменения размеров выделенной фигуры
        private void StartResizing(ResizeHandle handle, Point point)
        {
            // если не выделение или ничего не выбрано, выходим
            if (!isSelectionMode || selectedElement == null) return;
            // начинаем изменение размера, запоминаем за какой квадратик тянем, начальную позицию мыши и размер фигуры
            isResizing = true;
            activeHandle = handle;
            startPoint = point;
            originalMousePoint = point;
            originalBounds = GetElementBounds(selectedElement);
        }
// обрабатывает нажатие мыши на холсте в режиме выделения
        public void HandleMouseDown(Point point)
        {
            // если нет выделения выходим
            if (!isSelectionMode) return;
            // если фигура выделена и нажали на один из маленьких квадратиков
            if (selectedElement != null && IsPointOnResizeHandle(point, out var handle))
            {
                // включаем ищменение размера, запоминаем квадратик, позицию мыши и размер фигуры,
                // потом выходим потому что больше ничего нк надо 
                isResizing = true;
                activeHandle = handle;
                startPoint = point;
                originalMousePoint = point;
                originalBounds = GetElementBounds(selectedElement);
                return;
            }
            // иначе пытаемся найти фигуру под курсором 

            var element = FindElementAtPoint(point);
// если нашли 
            if (element != null)
            {
                // то выделяем фигуру, запоминаем позицию мыши, положение и позицию фигуры, ставим флаг, что начали двигать фигуру
                SelectElement(element);
                startPoint = point;
                originalMousePoint = point;
                originalBounds = GetElementBounds(selectedElement);
                isDragging = true;
            }
            // иначе снимаем выделение фигуры  
            else
            {
                ClearSelection();
            }
        }
// решает, что делать когда двигаем мышкой по холсту 
        public void HandleMouseMove(Point point)
        {
            //если выключено выделение, ничего не делаем, просто отображаем курсор 
            if (!isSelectionMode)
            {
                canvas.Cursor = Cursors.Arrow;
                return;
            }
            // если фигура выделена и включено перемещение, то вызываем метод, который изменяет координаты фигуры 

            if (isDragging && selectedElement != null)
            {
                MoveElement(point);
            }
            // если включено изменение размера, то вызываем метод, который изменяет размер фигруры 
            else if (isResizing && selectedElement != null)
            {
                ResizeElement(point);
            }
            // обновляет вид курсора в любом случае
            UpdateCursor(point);
        }
// отвечает за опускание кнопки мыши
        public void HandleMouseUp()
        {
            // если ничего не выделяем, то просто выходим
            if (!isSelectionMode) return;
            //иначе выключаем перетаскивание и изменение размера 
            isDragging = false;
            isResizing = false;
        }
// отвечает за выделение фигуры 
        private void SelectElement(FrameworkElement element)
        {
            //сначала убираем текущее выделение 
            ClearSelection();
            // запоминаем выбранную фигуру
            selectedElement = element;
            // показываем выделение
            ShowSelectionVisuals();
        }
// снимает выделение с фигуры
        public void ClearSelection()
        {
            // убираем визуал выделения
            HideSelectionVisuals();
            //убираем выделенную фигуру
            selectedElement = null;
        }
// показывает пользователю, что фигура выделена 
        private void ShowSelectionVisuals()
        {
            // если никака фигура не выбрана или выделение выключено - выходим
            if (selectedElement == null || !isSelectionMode) return;
            // елси нет пунктирного прямоугольника 
            if (!canvas.Children.Contains(selectionBorder))
            {
                //добавляем его на холст поверх всех фигур 
                Canvas.SetZIndex(selectionBorder, 1000);
                canvas.Children.Add(selectionBorder);
            }
            //проходим по квадратикам для изменения размера 
            for (int i = 0; i < resizeHandles.Count; i++)
            {
                //если какой-то из них не добавлен - добавляем 
                var handle = resizeHandles[i];
                if (!canvas.Children.Contains(handle))
                {
                    Canvas.SetZIndex(handle, 1001);
                    canvas.Children.Add(handle);
                }
                // и делаем его видимым и ставим ему его курсор
                handle.Visibility = Visibility.Visible;
                handle.Cursor = GetHandleCursor((ResizeHandle)i);
            }
// пересчитываем координаты рамки и квадратиков 
            UpdateSelectionVisuals();
        }
// скрывает визуал выделения 
        private void HideSelectionVisuals()
        {
            // если рамка есть и она на холсте, то удаляет ее
            if (selectionBorder != null && canvas.Children.Contains(selectionBorder))
                canvas.Children.Remove(selectionBorder);
// проходит по всем квадратикам
            foreach (var handle in resizeHandles)
            {
                // если они добавлены, то удаляет и делает невидимыми(на всякий слуйчай)
                if (canvas.Children.Contains(handle))
                    canvas.Children.Remove(handle);
                handle.Visibility = Visibility.Collapsed;
            }
        }
// обновляет позицию и размеры рамки и квадратиков выделения
        private void UpdateSelectionVisuals()
        {
            //если ничего не выбрано - выходим
            if (selectedElement == null) return;
// получаем координаты и размеры фигуры
            var bounds = GetElementBounds(selectedElement);
            //ставим рамку выше и шире фигуры
            Canvas.SetLeft(selectionBorder, bounds.Left - BorderThickness);
            Canvas.SetTop(selectionBorder, bounds.Top - BorderThickness);
            selectionBorder.Width = bounds.Width + BorderThickness * 2;
            selectionBorder.Height = bounds.Height + BorderThickness * 2;
            //определяем позиции квадратиков
            var handles = new[]
            {
                new Point(bounds.Left, bounds.Top),                    
                new Point(bounds.Left + bounds.Width / 2, bounds.Top), 
                new Point(bounds.Right, bounds.Top),                   
                new Point(bounds.Right, bounds.Top + bounds.Height / 2), 
                new Point(bounds.Right, bounds.Bottom),                
                new Point(bounds.Left + bounds.Width / 2, bounds.Bottom), 
                new Point(bounds.Left, bounds.Bottom),                
                new Point(bounds.Left, bounds.Top + bounds.Height / 2)  
            };
            //проходим по всем квадратикам и перемещаем их на эти позиции

            for (int i = 0; i < 8; i++)
            {
                Canvas.SetLeft(resizeHandles[i], handles[i].X - HandleSize / 2);
                Canvas.SetTop(resizeHandles[i], handles[i].Y - HandleSize / 2);
            }
        }
// отвечает за перемещение выделенной фигуры 
        private void MoveElement(Point currentPoint)
        {
            //считаем смещение 
            var deltaX = currentPoint.X - startPoint.X;
            var deltaY = currentPoint.Y - startPoint.Y;
//нт выбранной фигуры - выходим
            if (selectedElement == null) return;
            // определяем границы фигуры и холста 
            var bounds = GetElementBounds(selectedElement);
            double maxX = canvas.ActualWidth;
            double maxY = canvas.ActualHeight;
// проверка, чтобы нельзя было вытащить фигуру за границы холста 
            if (bounds.Left + deltaX < 0)
                deltaX = -bounds.Left;
            if (bounds.Top + deltaY < 0)
                deltaY = -bounds.Top;
            if (bounds.Right + deltaX > maxX)
                deltaX = maxX - bounds.Right;
            if (bounds.Bottom + deltaY > maxY)
                deltaY = maxY - bounds.Bottom;
            // особенное перемещение для многоугольника 
            if (selectedElement is Polygon polygon)
            {
                // проходим по каждой вершине многоугольника и смещаем ее
                for (int i = 0; i < polygon.Points.Count; i++)
                    polygon.Points[i] = new Point(polygon.Points[i].X + deltaX, polygon.Points[i].Y + deltaY);
            }
            // если это ломананная линия, то тоже самое 
            else if (selectedElement is Polyline polyline)
            {
                for (int i = 0; i < polyline.Points.Count; i++)
                    polyline.Points[i] = new Point(polyline.Points[i].X + deltaX, polyline.Points[i].Y + deltaY);
            }
            // если прямая  то двигаем только точку начала и конца 
            else if (selectedElement is Line line)
            {
                line.X1 += deltaX;
                line.Y1 += deltaY;
                line.X2 += deltaX;
                line.Y2 += deltaY;
            }
            // иначе получаем координаты фигуры, прибавляем смещение, следим за выходм за границу холста
            // и устанавливаем новые координаты 
            else
            {
                double left = Canvas.GetLeft(selectedElement);
                double top = Canvas.GetTop(selectedElement);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                double newLeft = left + deltaX;
                double newTop = top + deltaY;
                
                newLeft = Math.Max(0, Math.Min(newLeft, maxX - selectedElement.Width));
                newTop = Math.Max(0, Math.Min(newTop, maxY - selectedElement.Height));

                Canvas.SetLeft(selectedElement, newLeft);
                Canvas.SetTop(selectedElement, newTop);
            }
// обновляем точку отсчета и рамку выделения
            startPoint = currentPoint;
            UpdateSelectionVisuals();
        }
//отвечает за изменение размера выделенной фигуры 
        private void ResizeElement(Point currentPoint)
        {
            //посчет на сколько сдвинулась мышь
            var deltaX = currentPoint.X - originalMousePoint.X;
            var deltaY = currentPoint.Y - originalMousePoint.Y;
            //ограничиваем максимальное растяжение 
            double maxDelta = 200; 
            deltaX = Math.Max(Math.Min(deltaX, maxDelta), -maxDelta);
            deltaY = Math.Max(Math.Min(deltaY, maxDelta), -maxDelta);
            // определяем за какой именно квадратик потянули 
            switch (activeHandle)
            {
                case ResizeHandle.TopLeft:
                    ResizeTopLeft(deltaX, deltaY);
                    break;
                case ResizeHandle.Top:
                    ResizeTop(deltaY);
                    break;
                case ResizeHandle.TopRight:
                    ResizeTopRight(deltaX, deltaY);
                    break;
                case ResizeHandle.Right:
                    ResizeRight(deltaX);
                    break;
                case ResizeHandle.BottomRight:
                    ResizeBottomRight(deltaX, deltaY);
                    break;
                case ResizeHandle.Bottom:
                    ResizeBottom(deltaY);
                    break;
                case ResizeHandle.BottomLeft:
                    ResizeBottomLeft(deltaX, deltaY);
                    break;
                case ResizeHandle.Left:
                    ResizeLeft(deltaX);
                    break;
            }
            //обновляем позицию мыши и визуал выделения 
            originalMousePoint = currentPoint;
            UpdateSelectionVisuals();
        }
        // изменяет размер фигуры, когда тянем за левый верхний угол
        private void ResizeTopLeft(double deltaX, double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                var newWidth = Math.Max(10, rect.Width - deltaX);
                var newHeight = Math.Max(10, rect.Height - deltaY);

                rect.Width = newWidth;
                rect.Height = newHeight;
                Canvas.SetLeft(rect, Canvas.GetLeft(rect) + deltaX);
                Canvas.SetTop(rect, Canvas.GetTop(rect) + deltaY);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                var newWidth = Math.Max(10, ellipse.Width - deltaX);
                var newHeight = Math.Max(10, ellipse.Height - deltaY);

                ellipse.Width = newWidth;
                ellipse.Height = newHeight;
                Canvas.SetLeft(ellipse, Canvas.GetLeft(ellipse) + deltaX);
                Canvas.SetTop(ellipse, Canvas.GetTop(ellipse) + deltaY);
            }
            else if (selectedElement is Line line)
            {
                line.X1 += deltaX;
                line.Y1 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, deltaY, activeHandle);
            }
        }
        //изменяет размер фигуры если теянем за правый верхний угол
        private void ResizeTopRight(double deltaX, double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                var newWidth = Math.Max(10, rect.Width + deltaX);
                var newHeight = Math.Max(10, rect.Height - deltaY);

                rect.Width = newWidth;
                rect.Height = newHeight;
                Canvas.SetTop(rect, Canvas.GetTop(rect) + deltaY);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                var newWidth = Math.Max(10, ellipse.Width + deltaX);
                var newHeight = Math.Max(10, ellipse.Height - deltaY);

                ellipse.Width = newWidth;
                ellipse.Height = newHeight;
                Canvas.SetTop(ellipse, Canvas.GetTop(ellipse) + deltaY);
            }
            else if (selectedElement is Line line)
            {
                line.X2 += deltaX;
                line.Y1 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, deltaY, activeHandle);
            }
        }
// изменяем размер фигуры если тянем за нижний правый угол
        private void ResizeBottomRight(double deltaX, double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                rect.Width = Math.Max(10, rect.Width + deltaX);
                rect.Height = Math.Max(10, rect.Height + deltaY);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                ellipse.Width = Math.Max(10, ellipse.Width + deltaX);
                ellipse.Height = Math.Max(10, ellipse.Height + deltaY);
            }
            else if (selectedElement is Line line)
            {
                line.X2 += deltaX;
                line.Y2 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, deltaY, activeHandle);
            }
        }
//изменяем размер фигуры если тянем за нижний левый угол
        private void ResizeBottomLeft(double deltaX, double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                var newWidth = Math.Max(10, rect.Width - deltaX);
                var newHeight = Math.Max(10, rect.Height + deltaY);

                rect.Width = newWidth;
                rect.Height = newHeight;
                Canvas.SetLeft(rect, Canvas.GetLeft(rect) + deltaX);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                var newWidth = Math.Max(10, ellipse.Width - deltaX);
                var newHeight = Math.Max(10, ellipse.Height + deltaY);

                ellipse.Width = newWidth;
                ellipse.Height = newHeight;
                Canvas.SetLeft(ellipse, Canvas.GetLeft(ellipse) + deltaX);
            }
            else if (selectedElement is Line line)
            {
                line.X1 += deltaX;
                line.Y2 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, deltaY, activeHandle);
            }
        }
// если тянем за верх
        private void ResizeTop(double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                rect.Height = Math.Max(10, rect.Height - deltaY);
                Canvas.SetTop(rect, Canvas.GetTop(rect) + deltaY);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                ellipse.Height = Math.Max(10, ellipse.Height - deltaY);
                Canvas.SetTop(ellipse, Canvas.GetTop(ellipse) + deltaY);
            }
            else if (selectedElement is Line line)
            {
                line.Y1 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, 0, deltaY, activeHandle);
            }
        }
// если тянем за правую сторону
        private void ResizeRight(double deltaX)
        {
            if (selectedElement is Rectangle rect)
            {
                rect.Width = Math.Max(10, rect.Width + deltaX);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                ellipse.Width = Math.Max(10, ellipse.Width + deltaX);
            }
            else if (selectedElement is Line line)
            {
                line.X2 += deltaX;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, 0, activeHandle);
            }
        }
        //если тянем за низ
        private void ResizeBottom(double deltaY)
        {
            if (selectedElement is Rectangle rect)
            {
                rect.Height = Math.Max(10, rect.Height + deltaY);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                ellipse.Height = Math.Max(10, ellipse.Height + deltaY);
            }
            else if (selectedElement is Line line)
            {
                line.Y2 += deltaY;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, 0, deltaY, activeHandle);
            }
        }
        // если тянем за левую сторону
        private void ResizeLeft(double deltaX)
        {
            if (selectedElement is Rectangle rect)
            {
                var newWidth = Math.Max(10, rect.Width - deltaX);
                rect.Width = newWidth;
                Canvas.SetLeft(rect, Canvas.GetLeft(rect) + deltaX);
            }
            else if (selectedElement is Ellipse ellipse)
            {
                var newWidth = Math.Max(10, ellipse.Width - deltaX);
                ellipse.Width = newWidth;
                Canvas.SetLeft(ellipse, Canvas.GetLeft(ellipse) + deltaX);
            }
            else if (selectedElement is Line line)
            {
                line.X1 += deltaX;
            }
            else if (selectedElement is Polygon polygon || selectedElement is Polyline polyline)
            {
                ScalePolyShape(selectedElement, deltaX, 0, activeHandle);
            }
        }
// изменяет размер многоугольника и ломанной
        private void ScalePolyShape(FrameworkElement element, double deltaX, double deltaY, ResizeHandle handle)
        {
            //определяем точки фигуры 
            PointCollection points = null;

            if (element is Polygon polygon)
            {
                points = polygon.Points;
            }
            else if (element is Polyline polyline)
            {
                points = polyline.Points;
            }
//проверяем если точки вообще 
            if (points == null || points.Count == 0) return;
//определяем исходные границы фигуры 
            var bounds = originalBounds;
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                double minX = points.Min(p => p.X);
                double maxX = points.Max(p => p.X);
                double minY = points.Min(p => p.Y);
                double maxY = points.Max(p => p.Y);
                bounds = new Rect(minX, minY, Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));
            }
            //коэффициенты масштабирования 
            double scaleX = 1.0;
            double scaleY = 1.0;
            // точка, относительно которой масштабируется
            Point pivot = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
//определяем за какой угол потянули 
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    pivot = new Point(bounds.Right, bounds.Bottom);
                    scaleX = (bounds.Width - deltaX) / bounds.Width;
                    scaleY = (bounds.Height - deltaY) / bounds.Height;
                    break;
                case ResizeHandle.Top:
                    pivot = new Point(bounds.Left + bounds.Width / 2, bounds.Bottom);
                    scaleY = (bounds.Height - deltaY) / bounds.Height;
                    break;
                case ResizeHandle.TopRight:
                    pivot = new Point(bounds.Left, bounds.Bottom);
                    scaleX = (bounds.Width + deltaX) / bounds.Width;
                    scaleY = (bounds.Height - deltaY) / bounds.Height;
                    break;
                case ResizeHandle.Right:
                    pivot = new Point(bounds.Left, bounds.Top + bounds.Height / 2);
                    scaleX = (bounds.Width + deltaX) / bounds.Width;
                    break;
                case ResizeHandle.BottomRight:
                    pivot = new Point(bounds.Left, bounds.Top);
                    scaleX = (bounds.Width + deltaX) / bounds.Width;
                    scaleY = (bounds.Height + deltaY) / bounds.Height;
                    break;
                case ResizeHandle.Bottom:
                    pivot = new Point(bounds.Left + bounds.Width / 2, bounds.Top);
                    scaleY = (bounds.Height + deltaY) / bounds.Height;
                    break;
                case ResizeHandle.BottomLeft:
                    pivot = new Point(bounds.Right, bounds.Top);
                    scaleX = (bounds.Width - deltaX) / bounds.Width;
                    scaleY = (bounds.Height + deltaY) / bounds.Height;
                    break;
                case ResizeHandle.Left:
                    pivot = new Point(bounds.Right, bounds.Top + bounds.Height / 2);
                    scaleX = (bounds.Width - deltaX) / bounds.Width;
                    break;
            }
            // проверка на ошибки, чтобы не было деления на ноль или обратного масштабирования
            if (double.IsInfinity(scaleX) || double.IsNaN(scaleX) || scaleX <= 0) scaleX = 0.01;
            if (double.IsInfinity(scaleY) || double.IsNaN(scaleY) || scaleY <= 0) scaleY = 0.01;
            //чтобы не было совсем маленьких фигур
            double newWidth = bounds.Width * Math.Abs(scaleX);
            double newHeight = bounds.Height * Math.Abs(scaleY);
            if (newWidth < 10 || newHeight < 10) return;
            //применяем масштабирование к каждой точке 
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                double offsetX = p.X - pivot.X;
                double offsetY = p.Y - pivot.Y;
                double scaledX = pivot.X + offsetX * scaleX;
                double scaledY = pivot.Y + offsetY * scaleY;
                points[i] = new Point(scaledX, scaledY);
            }
        }
//возвращает координаты и размеры прямоугольника, в который вписывается фигура 
        private Rect GetElementBounds(FrameworkElement element)
        {
            // если элемент фигура 
            if (element is Shape shape)
            {
                // если фигура это линия
                if (shape is Line line)
                {
                    // считаем минимальные и максимальные координаты 
                    var minX = Math.Min(line.X1, line.X2);
                    var minY = Math.Min(line.Y1, line.Y2);
                    var maxX = Math.Max(line.X1, line.X2);
                    var maxY = Math.Max(line.Y1, line.Y2);
                    // возвращаем прямоугольник, в котором находится
                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                }
                // если фигура многоугольник 
                else if (shape is Polygon polygon)
                {
                    if (polygon.Points == null || polygon.Points.Count == 0)
                        return new Rect(0, 0, 0, 0);
// из всех вершин выбираем самую левую, правую, верхнюю, нижнюю
                    double minX = polygon.Points.Min(p => p.X);
                    double maxX = polygon.Points.Max(p => p.X);
                    double minY = polygon.Points.Min(p => p.Y);
                    double maxY = polygon.Points.Max(p => p.Y);
                    // и по ним строим прямоугольник выделения
                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                }
                // если фигура - ломаная  - то же , что и для многоугольника
                else if (shape is Polyline polyline)
                {
                    if (polyline.Points == null || polyline.Points.Count == 0)
                        return new Rect(0, 0, 0, 0);

                    double minX = polyline.Points.Min(p => p.X);
                    double maxX = polyline.Points.Max(p => p.X);
                    double minY = polyline.Points.Min(p => p.Y);
                    double maxY = polyline.Points.Max(p => p.Y);
                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                }
            }
// для остальных берем коодинаты на холсте и возвращем прямоугоьник, в который они вписываются
            var left = Canvas.GetLeft(element);
            var top = Canvas.GetTop(element);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            return new Rect(left, top, element.Width, element.Height);
        }
//определяет, на какой именно элемент кликнули 
        private FrameworkElement FindElementAtPoint(Point point)
        {
            // проходим по всем элементам на холсте 
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                var child = canvas.Children[i];
                // проверяем фигура ли этот элемент
                if (child is Shape shape &&
                    child != selectionBorder &&
                    !resizeHandles.Contains(child) &&
                    IsPointInElement(shape, point))
                {
                    //возвращаем ее
                    return shape;
                }
            }
            return null;
        }
// проверяет, внутри ли фигуры кликнули 
        public bool IsPointInElement(Shape shape, Point point)
        {
            // если многоугольник - для него отдельный метод, он особенный
            if (shape is Polygon polygon)
            {
                return IsPointInPolygon(polygon.Points, point);
            }
            else if (shape is Polyline polyline)
            {
                // ломаная тоже особенная
                return IsPointNearPolyline(polyline.Points, point, 5); 
            }
            else if (shape is Line line)
            {
                // и линия особенная
                return IsPointNearLine(line, point, 5);
            }
            // для остальных проверяем по границам
            else
            {
                var bounds = GetElementBounds(shape);
                return bounds.Contains(point);
            }
        }
        // для многоугольника проверка, что кликнули внутри него
        private bool IsPointInPolygon(PointCollection points, Point testPoint)
        {
            if (points == null || points.Count < 3)
                return false;

            bool inside = false;
            int count = points.Count;
// проходим по всем ребрам многоугольника 
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                //проверка,что луч пересекает какое-нибудь ребро многоугольника
                if (((points[i].Y > testPoint.Y) != (points[j].Y > testPoint.Y)) &&
                    (testPoint.X < (points[j].X - points[i].X) * (testPoint.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    // каждый раз меняем ответ
                    inside = !inside;
                }
            }
// и возвращаем конечный 
            return inside;
        }
        // проверяем, кликнули ли по ломаной 
        private bool IsPointNearPolyline(PointCollection points, Point testPoint, double tolerance)
        {
            if (points == null || points.Count < 2)
                return false;
// проходимя по каждому сегменту и для него отедльно проверяем 
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (IsPointNearLineSegment(points[i], points[i + 1], testPoint, tolerance))
                    return true;
            }

            return false;
        }
        // проверка, что кликнули по какому-либо сегменту ломаной 
        private bool IsPointNearLineSegment(Point p1, Point p2, Point testPoint, double tolerance)
        {
            // считаем  расстояние от точки до отрезка
            double lineLength = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            if (lineLength == 0)
                return DistanceBetweenPoints(p1, testPoint) <= tolerance;

            // показывает положение проекции точки на линию 
            double t = ((testPoint.X - p1.X) * (p2.X - p1.X) + (testPoint.Y - p1.Y) * (p2.Y - p1.Y)) / (lineLength * lineLength);

            // Ограничиваем  в пределах отрезка
            t = Math.Max(0, Math.Min(1, t));

            // Находим ближайшую точку на отрезке
            Point closestPoint = new Point(
                p1.X + t * (p2.X - p1.X),
                p1.Y + t * (p2.Y - p1.Y)
            );

            // Проверяем расстояние
            return DistanceBetweenPoints(testPoint, closestPoint) <= tolerance;
        }
        // проверяем, кликнули ли по линии
        private bool IsPointNearLine(Line line, Point testPoint, double tolerance)
        {
            // вызываем метод для сегментов 
            return IsPointNearLineSegment(
                new Point(line.X1, line.Y1),
                new Point(line.X2, line.Y2),
                testPoint,
                tolerance
            );
        }
        // считает расстояние между двумя точками на плоскости 
        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        // проверка, был ли клик по квадратику для изменения размера 
        private bool IsPointOnResizeHandle(Point point, out ResizeHandle handle)
        {
            handle = ResizeHandle.None;
//проходим по всем квадратикам
            for (int i = 0; i < resizeHandles.Count; i++)
            {
                if (resizeHandles[i].Visibility != Visibility.Visible) continue;
// для каждого создаем невидимую область 
                var handleBounds = new Rect(
                    Canvas.GetLeft(resizeHandles[i]),
                    Canvas.GetTop(resizeHandles[i]),
                    HandleSize,
                    HandleSize
                );
// проверяем что точка внутри этой области
                if (handleBounds.Contains(point))
                {
                    // если да, то запоминаем квадратик и возврщаем true 
                    handle = (ResizeHandle)i;
                    return true;
                }
            }

            return false;
        }
// обновляет вид курсора мыши 
        private void UpdateCursor(Point point)
        {
            if (!isSelectionMode)
            {
                canvas.Cursor = Cursors.Arrow;
                return;
            }

            if (selectedElement != null)
            {
                if (IsPointOnResizeHandle(point, out var handle))
                {
                    canvas.Cursor = GetHandleCursor(handle);
                }
                else if (IsPointInElement(selectedElement as Shape, point))
                {
                    canvas.Cursor = Cursors.SizeAll;
                }
                else
                {
                    canvas.Cursor = Cursors.Arrow;
                }
            }
            else
            {
                canvas.Cursor = Cursors.Arrow;
            }
        }
// возвращает нужный вид курсора в зависимости от квадратика
        private Cursor GetHandleCursor(ResizeHandle handle)
        {
            return handle switch
            {
                ResizeHandle.TopLeft => Cursors.SizeNWSE,
                ResizeHandle.Top => Cursors.SizeNS,
                ResizeHandle.TopRight => Cursors.SizeNESW,
                ResizeHandle.Right => Cursors.SizeWE,
                ResizeHandle.BottomRight => Cursors.SizeNWSE,
                ResizeHandle.Bottom => Cursors.SizeNS,
                ResizeHandle.BottomLeft => Cursors.SizeNESW,
                ResizeHandle.Left => Cursors.SizeWE,
                _ => Cursors.Arrow
            };
        }
        // возвращает выделенный элемент(надо для класса удаления)
        public FrameworkElement GetSelectedElement()
        {
            return selectedElement;
        }

    }
// список всех квадратиков 
    public enum ResizeHandle
    {
        TopLeft = 0,   
        Top = 1,     
        TopRight = 2,  
        Right = 3,      
        BottomRight = 4, 
        Bottom = 5,    
        BottomLeft = 6,  
        Left = 7,        
        None = 8         
    }

}
