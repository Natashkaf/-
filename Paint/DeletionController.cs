using System.Windows.Controls;
using System.Windows.Shapes;
// класс для удаления выбранной фигуры 
namespace Paint
{
    public class DeletionController
    {
        //поля класса readonly для гарантии, что один и тот же холст и один и тот же контроллер
        //означает что задать эти поля можно только один раз - в конструкторе 
        private readonly Canvas _canvas;
        private readonly SelectionController _selectionController;

        public DeletionController(Canvas canvas, SelectionController selectionController)
        {
            _canvas = canvas;
            _selectionController = selectionController;
        }
        // сам метод удаления 
        public void DeleteSelectedShape()
        {
            // получаем выделенную фигуру
            var selected = _selectionController.GetSelectedElement();
// если есть что удалить
            if (selected == null)
                return;
// то удаляем с холста и убираем выделение 
            _canvas.Children.Remove(selected);
            _selectionController.ClearSelection();
        }
    }
}