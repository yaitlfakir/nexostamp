using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using NexoStamp.Models;

namespace NexoStamp.Services
{
    /// <summary>
    /// Service for printing stamp designs
    /// </summary>
    public class PrintService
    {
        public void PrintDesign(StampDesign design, int stampsPerPage = 1, PageMediaSize? pageSize = null)
        {
            var printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() != true)
                return;

            // Create a document to print
            var document = CreatePrintDocument(design, stampsPerPage, printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            
            printDialog.PrintVisual(document, $"NexoStamp - {design.Name}");
        }

        public void ShowPrintPreview(StampDesign design, Window owner, int stampsPerPage = 1)
        {
            var printDialog = new PrintDialog();
            
            var previewWidth = printDialog.PrintableAreaWidth;
            var previewHeight = printDialog.PrintableAreaHeight;

            var document = CreatePrintDocument(design, stampsPerPage, previewWidth, previewHeight);
            
            // Create preview window
            var previewWindow = new Window
            {
                Title = "Print Preview",
                Width = 800,
                Height = 600,
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = document
            };

            previewWindow.Content = scrollViewer;
            previewWindow.ShowDialog();
        }

        private Canvas CreatePrintDocument(StampDesign design, int stampsPerPage, double pageWidth, double pageHeight)
        {
            var canvas = new Canvas
            {
                Width = pageWidth,
                Height = pageHeight,
                Background = Brushes.White
            };

            // Calculate layout for multiple stamps per page
            int columns = (int)Math.Ceiling(Math.Sqrt(stampsPerPage));
            int rows = (int)Math.Ceiling((double)stampsPerPage / columns);

            double stampWidth = pageWidth / columns - 20;
            double stampHeight = pageHeight / rows - 20;

            for (int i = 0; i < stampsPerPage; i++)
            {
                int row = i / columns;
                int col = i % columns;

                double x = col * (stampWidth + 20) + 10;
                double y = row * (stampHeight + 20) + 10;

                var stampCanvas = RenderStampToCanvas(design, stampWidth, stampHeight);
                Canvas.SetLeft(stampCanvas, x);
                Canvas.SetTop(stampCanvas, y);
                canvas.Children.Add(stampCanvas);
            }

            return canvas;
        }

        private Canvas RenderStampToCanvas(StampDesign design, double targetWidth, double targetHeight)
        {
            var canvas = new Canvas
            {
                Width = targetWidth,
                Height = targetHeight,
                Background = Brushes.Black
            };

            // Calculate scale to fit
            double scaleX = targetWidth / design.CanvasWidth;
            double scaleY = targetHeight / design.CanvasHeight;
            double scale = Math.Min(scaleX, scaleY);

            foreach (var element in design.Elements)
            {
                FrameworkElement? visual = null;

                if (element is Models.TextElement textElement)
                {
                    visual = new TextBlock
                    {
                        Text = textElement.Text,
                        FontFamily = new FontFamily(textElement.FontFamily),
                        FontSize = textElement.FontSize * scale,
                        FontWeight = textElement.FontWeight,
                        FontStyle = textElement.FontStyle,
                        Foreground = Brushes.White,
                        TextAlignment = textElement.TextAlignment,
                        Width = textElement.Width * scale,
                        Height = textElement.Height * scale
                    };
                }
                else if (element is Models.ShapeElement shapeElement)
                {
                    visual = CreateShape(shapeElement, scale);
                }

                if (visual != null)
                {
                    Canvas.SetLeft(visual, element.X * scale);
                    Canvas.SetTop(visual, element.Y * scale);
                    
                    if (element.Rotation != 0)
                    {
                        visual.RenderTransform = new RotateTransform(element.Rotation, 
                            (element.Width * scale) / 2, 
                            (element.Height * scale) / 2);
                    }

                    canvas.Children.Add(visual);
                }
            }

            return canvas;
        }

        private FrameworkElement CreateShape(ShapeElement shapeElement, double scale)
        {
            Shape? shape = null;

            switch (shapeElement.ShapeType)
            {
                case ShapeType.Rectangle:
                    shape = new Rectangle
                    {
                        Width = shapeElement.Width * scale,
                        Height = shapeElement.Height * scale,
                        Stroke = Brushes.White,
                        StrokeThickness = shapeElement.StrokeThickness * scale
                    };
                    break;

                case ShapeType.Circle:
                    shape = new Ellipse
                    {
                        Width = shapeElement.Width * scale,
                        Height = shapeElement.Height * scale,
                        Stroke = Brushes.White,
                        StrokeThickness = shapeElement.StrokeThickness * scale
                    };
                    break;

                case ShapeType.Line:
                    shape = new Line
                    {
                        X1 = 0,
                        Y1 = 0,
                        X2 = shapeElement.Width * scale,
                        Y2 = shapeElement.Height * scale,
                        Stroke = Brushes.White,
                        StrokeThickness = shapeElement.StrokeThickness * scale
                    };
                    break;
            }

            return shape ?? new Rectangle();
        }
    }
}
