using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Paint
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Black;
            inkCanvas.DefaultDrawingAttributes.Width = 5;
            inkCanvas.DefaultDrawingAttributes.Height = 5;
        
            // Режим рисования вкл всегда
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
//для рисования выбранным цветом 
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Background is SolidColorBrush brush)
            {
                inkCanvas.DefaultDrawingAttributes.Color = brush.Color;

            }
        }
    }
    
}