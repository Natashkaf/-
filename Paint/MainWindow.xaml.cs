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
    private ToolController toolController = new ToolController();

    public MainWindow()
    {
        InitializeComponent();
        toolController.CurrentTool = ToolType.Brush;
        this.Cursor = Cursors.Cross;
    }
// обрабочик смены цвета 
    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Background is SolidColorBrush brush)
        {
            toolController.CurrentColor = brush.Color;
        }
    }
// обрабочик кнопки прямоугольника
    private void RectangleButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Rectangle;
        this.Cursor = Cursors.Cross;
    }
// обработчик кнопки карандаша
    private void PencilButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Pencil;
        this.Cursor = Cursors.Pen;
    }
    //обработчик кнопки кисти 

    private void BrushButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Brush;
        this.Cursor = Cursors.Cross;
    }
//обработчик кнопки линии
    private void LineButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Line;
        this.Cursor = Cursors.Cross;
    }
// обработчик кнопки эллипса
    private void EllipseButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Ellipse;
        this.Cursor = Cursors.Cross;
    }
// обработчик кнопки многоугольника
    private void PolygonButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Polygon;
    }
// обработчик кнопки заливки 
    private void ButtonFilling_click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Fill;
        this.Cursor = Cursors.Cross;
    }
// обработчик нажатия мыши, только передает что где нажато и особая проверка для многоугольника 
    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point clickPoint = e.GetPosition(mainCanvas);
            
            if (e.ClickCount == 2 && toolController.CurrentTool == ToolType.Polygon)
            {
                toolController.HandlePolygonDoubleClick();
                return;
            }
        
            toolController.HandleMouseDown(clickPoint, mainCanvas);
        }
    }
// обрабочик движения мыши в процессе рисования
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        Point point = e.GetPosition(mainCanvas);
        toolController.HandleMouseMove(point);
    }
// обработчик отпускания мыши
    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        toolController.HandleMouseUp();
    }
}
}