﻿using System.Windows;
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
    private SelectionController selectionController;
    private DeletionController deletionController;


    public MainWindow()
    {
        InitializeComponent();
        toolController.CurrentTool = ToolType.Brush;
        selectionController = new SelectionController(mainCanvas);
        toolController.SetSelectionController(selectionController);
        deletionController = new DeletionController(mainCanvas, selectionController);

        this.Cursor = Cursors.Cross;
    }
// обрабочик смены цвета 
    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Background is SolidColorBrush brush)
        {
            toolController.CurrentColor = brush.Color;
            selectionController.SetSelectionMode(false);

        }
    }
// обрабочик кнопки прямоугольника
    private void RectangleButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Rectangle;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);

    }
// обработчик кнопки карандаша
    private void PencilButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Pencil;
        this.Cursor = Cursors.Pen;
        mainCanvas.Cursor = Cursors.Pen;
        selectionController.SetSelectionMode(false);

    }
    //обработчик кнопки кисти 

    private void BrushButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Brush;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);

    }
//обработчик кнопки линии
    private void LineButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Line;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);

    }
// обработчик кнопки эллипса
    private void EllipseButton_Click(object sender, RoutedEventArgs e)
    
    {
        toolController.CurrentTool = ToolType.Ellipse;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);

    }
// обработчик кнопки многоугольника
    private void PolygonButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Polygon;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);

    }
// обработчик кнопки заливки 
    private void ButtonFilling_click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Fill;
        this.Cursor = Cursors.Cross;
        mainCanvas.Cursor = Cursors.Cross;
        selectionController.SetSelectionMode(false);
        
    }
    //обработчик кнопкки выделения
    private void SelectionButton_Click(object sender, RoutedEventArgs e)
    {
        toolController.CurrentTool = ToolType.Selection;
        this.Cursor = Cursors.Arrow;

        // Включаем режим выделения
        selectionController.SetSelectionMode(true);
    }    
    // обработчик кнопки удаления
    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        deletionController.DeleteSelectedShape();
         }

// обработчик нажатия мыши, только передает что где нажато и особая проверка для многоугольника 
    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point clickPoint = e.GetPosition(mainCanvas);

            // Если выделение включено
            if (toolController.CurrentTool == ToolType.Selection)
            {
                bool clickedOnShape = false;

                // Проверяем, попали ли  в фигуру
                for (int i = mainCanvas.Children.Count - 1; i >= 0; i--)
                {
                    if (mainCanvas.Children[i] is Shape shape)
                    {
                        // Пропускаем рамку и квадратики для выделения
                        if (shape == selectionController.SelectionBorder ||
                            selectionController.IsResizeHandle(shape))
                            continue;

                        if (selectionController.IsPointInElement(shape, clickPoint))
                        {
                            clickedOnShape = true;
                            break;
                        }
                    }
                }

                // Если кликнули  по фигуре — оставляем выделение 
                if (clickedOnShape)
                {
                    selectionController.HandleMouseDown(clickPoint);
                    return;
                }

                // Если кликнули  по пустому месту — выключаем выделение и включаем кисть
                selectionController.SetSelectionMode(false);
                toolController.CurrentTool = ToolType.Brush;
                this.Cursor = Cursors.Cross;
                mainCanvas.Cursor = Cursors.Cross;
                return;
            }

            // Если двойной клик при включенном многоугольнике 
            if (e.ClickCount == 2 && toolController.CurrentTool == ToolType.Polygon)
            {
                toolController.HandlePolygonDoubleClick();
                return;
            }

            //для всех остальных 
            toolController.HandleMouseDown(clickPoint, mainCanvas);
        }
    }

// обрабочик движения мыши в процессе рисования
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        Point point = e.GetPosition(mainCanvas);
        
        if (toolController.CurrentTool == ToolType.Selection)
        {
            selectionController.HandleMouseMove(point);
        }
        else
        {
            toolController.HandleMouseMove(point);
        }
    }
// обработчик отпускания мыши
    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (toolController.CurrentTool == ToolType.Selection)
        {
            selectionController.HandleMouseUp();
        }
        else
        {
            toolController.HandleMouseUp();
        }
    }


}
}