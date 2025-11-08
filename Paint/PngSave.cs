using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Paint
{
    public static class PngSave
    {
        public static void ExportCanvasToPng(Canvas canvas)
        {
            //открываем диалог сохранения
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                DefaultExt = "png",
                FileName = "myDrawing.png"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            string filePath = saveFileDialog.FileName;

            // обновляем размеры холста
            canvas.UpdateLayout();

            double width = Math.Max(canvas.ActualWidth, canvas.RenderSize.Width);
            double height = Math.Max(canvas.ActualHeight, canvas.RenderSize.Height);

            // Создаём "виртуальную камеру" для рендера
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(width),
                (int)Math.Ceiling(height),
                96d, 96d,
                PixelFormats.Pbgra32);

            // создаем временный визуальный контейнер
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                // Рисуем фон или делаем белым
                if (canvas.Background is SolidColorBrush bg)
                    context.DrawRectangle(bg, null, new Rect(0, 0, width, height));
                else
                    context.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                // Рисуем содержимое холста
                VisualBrush vb = new VisualBrush(canvas);
                context.DrawRectangle(vb, null, new Rect(0, 0, width, height));
            }

            renderBitmap.Render(visual);

            // сохраняем в файл
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream fs = File.OpenWrite(filePath))
            {
                encoder.Save(fs);
            }
            
        }
    }
}




