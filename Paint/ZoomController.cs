using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
// класс для масштабирования холста 
namespace Paint
{
    public class ZoomController
    {
        private Canvas _canvas;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _transformGroup;
        // коэффициент уменьешния
        private const double ZoomFactor = 1.2;
        //минимальный и максимальный масштаб 
        private const double MinZoom = 0.1;
        private const double MaxZoom = 5.0;

        public ZoomController(Canvas canvas)
        {
            _canvas = canvas;
            InitializeZoom();
        }

        private void InitializeZoom()
        {
            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _translateTransform = new TranslateTransform(0, 0);
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);
            _canvas.RenderTransform = _transformGroup;
        }
        
        // Сброс масштаба до 100%
        public void ResetZoom()
        {
            _scaleTransform.ScaleX = 1.0;
            _scaleTransform.ScaleY = 1.0;
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
        }

        // Масштабирование колесиком мыши
        public void HandleMouseWheel(MouseWheelEventArgs e, ScrollViewer scrollViewer = null)
        {
            // Получаем позицию курсора относительно холста
            var mousePos = e.GetPosition(_canvas);

            // Текущий масштаб
            var currentScale = _scaleTransform.ScaleX;

            // Определяем направление зумирования
            double newScale = e.Delta > 0 ? currentScale * ZoomFactor : currentScale / ZoomFactor;

            // Ограничиваем масштаб
            if (newScale < MinZoom || newScale > MaxZoom)
                return;

            // Применяем масштабирование относительно позиции курсора
            ApplyZoomWithMousePosition(newScale, mousePos);

            e.Handled = true;
        }

        // Применение масштабирования относительно позиции мыши
        private void ApplyZoomWithMousePosition(double newScale, Point mousePos)
        {
            double currentScale = _scaleTransform.ScaleX;
            
            // Вычисляем изменение масштаба
            double scaleChange = newScale / currentScale;

            // Вычисляем новые координаты трансляции для зума под курсором
            double newX = mousePos.X - (mousePos.X - _translateTransform.X) * scaleChange;
            double newY = mousePos.Y - (mousePos.Y - _translateTransform.Y) * scaleChange;

            // Применяем новую трансформацию
            _scaleTransform.ScaleX = newScale;
            _scaleTransform.ScaleY = newScale;
            _translateTransform.X = newX;
            _translateTransform.Y = newY;
        }
        
        
    }
}
