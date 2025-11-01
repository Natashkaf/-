using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace Paint
{
    public partial class MainWindow : Window
    {
        private bool isDrawing = false;
        private bool isPencilToolActive = false;
        private bool isBrushToolActive = false; 
        private bool isRectToolActive = false;
        private bool isLineToolActive = false;
        private bool isEllipseToolActive = false;

        private double pencilThickness = 1; 
        private double brushThickness = 5;  
        private Point startPoint;
        private Point currentPoint;
        private Rectangle currentRectangle;
        private Polyline currentStroke; 
        private List<Point> strokePoints = new List<Point>();
        
        private Color currentColor = Colors.Black;
        private double strokeThickness = 2;

        public MainWindow()
        {
            InitializeComponent();
            isBrushToolActive = true;
            isPencilToolActive = false;
            isRectToolActive = false;
            isLineToolActive = false;
            isEllipseToolActive = false;
            this.Cursor = Cursors.Cross;
    
            currentColor = Colors.Black;
        }

        private DrawingTemplate currentDrawing;

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Background is SolidColorBrush brush)
            {
                currentColor = brush.Color;
            }
        }
// настройки рисования прямоугольника
        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            isPencilToolActive = false;
            isBrushToolActive = false;
            isRectToolActive = true;
            isLineToolActive = false;
            isEllipseToolActive = false;
            this.Cursor = Cursors.Cross;
        }
// настройки рисования карандашом
        private void PencilButton_Click(object sender, RoutedEventArgs e)
        {
            isPencilToolActive = true;
            isBrushToolActive = false;
            isRectToolActive = false;
            isLineToolActive = false;
            isEllipseToolActive = false;
            this.Cursor = Cursors.Pen;
        }
// настройки рисования кистью
        private void BrushButton_Click(object sender, RoutedEventArgs e)
        {
            isPencilToolActive = false;
            isBrushToolActive = true; 
            isRectToolActive = false;
            isLineToolActive = false;
            isEllipseToolActive = false;
            this.Cursor = Cursors.Cross;
        } 
        // настройки рисования линии
        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            isPencilToolActive = false;
            isBrushToolActive = false;
            isRectToolActive = false;
            isLineToolActive = true;
            isEllipseToolActive = false;
            
            this.Cursor = Cursors.Cross;
        }
        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            isPencilToolActive = false;
            isBrushToolActive = false;
            isRectToolActive = false;
            isLineToolActive = false;
            isEllipseToolActive = true;
            
            this.Cursor = Cursors.Cross;
           
        }
        //проверка, находтся ли курсор мыши внутри холста 
        private bool IsPointInCanvas(Point point)
        {
            return point.X >= 0 && point.X <= mainCanvas.ActualWidth &&
                   point.Y >= 0 && point.Y <= mainCanvas.ActualHeight;
        }
        // начало рисования 
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsPointInCanvas(e.GetPosition(mainCanvas))) return;
        
                isDrawing = true;
                startPoint = e.GetPosition(mainCanvas);
                currentPoint = startPoint;

                if (isRectToolActive)
                {
                    currentDrawing = new RectangleTemplate
                    {
                        StartPoint = startPoint,
                        CurrentPoint = currentPoint,
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = strokeThickness,
                        Fill = Brushes.Transparent,
                    };
                    currentDrawing.StartDrawing();
                    mainCanvas.Children.Add(currentDrawing.DrawingElement);
                }
                
                else if (isPencilToolActive || isBrushToolActive)
                {
                    double thickness = isPencilToolActive ? pencilThickness : brushThickness;
            
                    currentDrawing = new DrawingFree
                    {
                        StartPoint = startPoint,
                        CurrentPoint = currentPoint,
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = thickness,
                    };
                }
                else if (isLineToolActive) 
                {
                    currentDrawing = new DrawingLine
                    {
                        StartPoint = startPoint,
                        CurrentPoint = currentPoint,
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = strokeThickness,  
                    };
                }
                else if (isEllipseToolActive) 
                {
                    currentDrawing = new DrawingEllipse
                    {
                        StartPoint = startPoint,
                        CurrentPoint = currentPoint,
                        Stroke = new SolidColorBrush(currentColor),
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };
                }
                if (currentDrawing != null)
                {
                    currentDrawing.StartDrawing();
                    mainCanvas.Children.Add(currentDrawing.DrawingElement);
                }
                mainCanvas.CaptureMouse();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentDrawing != null)
            {
                Point rawPoint = e.GetPosition(mainCanvas); 
                currentPoint = new Point(
                    Math.Max(0, Math.Min(rawPoint.X, mainCanvas.ActualWidth)),
                    Math.Max(0, Math.Min(rawPoint.Y, mainCanvas.ActualHeight))
                );

                currentDrawing.CurrentPoint = currentPoint;
                currentDrawing.ProcessDrwing();
            }
        }
//прекращаем рисовать
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                mainCanvas.ReleaseMouseCapture();

                if (currentDrawing != null)
                {
                    currentDrawing.EndDrawing();
                    currentDrawing = null;
                }
            }
        }

        private void ButtonEllipse_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}