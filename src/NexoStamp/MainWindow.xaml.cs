using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using NexoStamp.Models;
using NexoStamp.Services;

namespace NexoStamp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private StampDesign _currentDesign;
    private FileService _fileService;
    private PrintService _printService;
    private string? _currentFilePath;
    
    private List<StampDesign> _undoStack = new();
    private List<StampDesign> _redoStack = new();
    
    private StampElement? _selectedElement;
    private FrameworkElement? _capturedElement;
    private Point _dragStartPoint;
    private bool _isDragging;
    private Point _elementStartPosition;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _currentDesign = new StampDesign();
        _fileService = new FileService();
        _printService = new PrintService();
        
        InitializeFontList();
        UpdateUI();
        UpdateUndoRedoButtons();
    }

    private void InitializeFontList()
    {
        var fonts = new[] { "Arial", "Times New Roman", "Courier New", "Verdana", 
            "Georgia", "Comic Sans MS", "Impact", "Trebuchet MS" };
        
        foreach (var font in fonts)
        {
            FontFamilyComboBox.Items.Add(font);
        }
        FontFamilyComboBox.SelectedIndex = 0;
    }

    private void SaveToUndoStack()
    {
        var designCopy = CloneDesign(_currentDesign);
        _undoStack.Add(designCopy);
        _redoStack.Clear();
        UpdateUndoRedoButtons();
    }

    private StampDesign CloneDesign(StampDesign design)
    {
        var clone = new StampDesign
        {
            Name = design.Name,
            CanvasWidth = design.CanvasWidth,
            CanvasHeight = design.CanvasHeight
        };
        
        foreach (var element in design.Elements)
        {
            clone.Elements.Add(element.Clone());
        }
        
        return clone;
    }

    private void UpdateUI()
    {
        DesignCanvas.Children.Clear();
        
        foreach (var element in _currentDesign.Elements)
        {
            AddElementToCanvas(element);
        }
        
        UpdatePropertiesPanel();
        StatusText.Text = $"Design: {_currentDesign.Name} - {_currentDesign.Elements.Count} elements";
    }

    private void AddElementToCanvas(StampElement element)
    {
        FrameworkElement? visual = null;
        
        if (element is TextElement textElement)
        {
            visual = new TextBlock
            {
                Text = textElement.Text,
                FontFamily = new FontFamily(textElement.FontFamily),
                FontSize = textElement.FontSize,
                FontWeight = textElement.FontWeight,
                FontStyle = textElement.FontStyle,
                Foreground = Brushes.White,
                TextAlignment = textElement.TextAlignment,
                Width = textElement.Width,
                Height = textElement.Height,
                Tag = element
            };
        }
        else if (element is ShapeElement shapeElement)
        {
            visual = CreateShapeVisual(shapeElement);
        }
        
        if (visual != null)
        {
            Canvas.SetLeft(visual, element.X);
            Canvas.SetTop(visual, element.Y);
            Panel.SetZIndex(visual, element.ZIndex);
            
            if (element.Rotation != 0)
            {
                visual.RenderTransform = new RotateTransform(element.Rotation, 
                    element.Width / 2, element.Height / 2);
            }
            
            visual.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            
            // Add selection border if selected
            if (element.IsSelected)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Yellow,
                    BorderThickness = new Thickness(2),
                    Width = element.Width + 4,
                    Height = element.Height + 4,
                    IsHitTestVisible = false
                };
                
                Canvas.SetLeft(border, element.X - 2);
                Canvas.SetTop(border, element.Y - 2);
                Panel.SetZIndex(border, 1000);
                DesignCanvas.Children.Add(border);
            }
            
            DesignCanvas.Children.Add(visual);
        }
    }

    private FrameworkElement CreateShapeVisual(ShapeElement shapeElement)
    {
        Shape? shape = null;
        
        switch (shapeElement.ShapeType)
        {
            case ShapeType.Rectangle:
                shape = new Rectangle
                {
                    Width = shapeElement.Width,
                    Height = shapeElement.Height,
                    Stroke = Brushes.White,
                    StrokeThickness = shapeElement.StrokeThickness,
                    Tag = shapeElement
                };
                break;
                
            case ShapeType.Circle:
                shape = new Ellipse
                {
                    Width = shapeElement.Width,
                    Height = shapeElement.Height,
                    Stroke = Brushes.White,
                    StrokeThickness = shapeElement.StrokeThickness,
                    Tag = shapeElement
                };
                break;
                
            case ShapeType.Line:
                shape = new Line
                {
                    X1 = 0,
                    Y1 = 0,
                    X2 = shapeElement.Width,
                    Y2 = shapeElement.Height,
                    Stroke = Brushes.White,
                    StrokeThickness = shapeElement.StrokeThickness,
                    Tag = shapeElement
                };
                break;
        }
        
        return shape ?? new Rectangle();
    }

    private void UpdatePropertiesPanel()
    {
        if (_selectedElement == null)
        {
            NoSelectionText.Visibility = Visibility.Visible;
            TextPropertiesPanel.Visibility = Visibility.Collapsed;
            ShapePropertiesPanel.Visibility = Visibility.Collapsed;
            CommonPropertiesPanel.Visibility = Visibility.Collapsed;
            return;
        }
        
        NoSelectionText.Visibility = Visibility.Collapsed;
        CommonPropertiesPanel.Visibility = Visibility.Visible;
        
        // Update common properties
        PositionXBox.Text = _selectedElement.X.ToString("F0");
        PositionYBox.Text = _selectedElement.Y.ToString("F0");
        WidthBox.Text = _selectedElement.Width.ToString("F0");
        HeightBox.Text = _selectedElement.Height.ToString("F0");
        RotationSlider.Value = _selectedElement.Rotation;
        RotationLabel.Text = $"{_selectedElement.Rotation:F0}°";
        
        if (_selectedElement is TextElement textElement)
        {
            TextPropertiesPanel.Visibility = Visibility.Visible;
            ShapePropertiesPanel.Visibility = Visibility.Collapsed;
            
            TextContentBox.Text = textElement.Text;
            FontFamilyComboBox.SelectedItem = textElement.FontFamily;
            FontSizeSlider.Value = textElement.FontSize;
            FontSizeLabel.Text = $"{textElement.FontSize:F0}pt";
            BoldCheckBox.IsChecked = textElement.FontWeight == FontWeights.Bold;
            ItalicCheckBox.IsChecked = textElement.FontStyle == FontStyles.Italic;
        }
        else if (_selectedElement is ShapeElement shapeElement)
        {
            TextPropertiesPanel.Visibility = Visibility.Collapsed;
            ShapePropertiesPanel.Visibility = Visibility.Visible;
            
            StrokeThicknessSlider.Value = shapeElement.StrokeThickness;
            StrokeThicknessLabel.Text = $"{shapeElement.StrokeThickness:F0}px";
        }
    }

    private void UpdateUndoRedoButtons()
    {
        UndoMenuItem.IsEnabled = _undoStack.Count > 0;
        RedoMenuItem.IsEnabled = _redoStack.Count > 0;
    }

    // Menu Handlers
    private void NewDesign_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Create a new design? Unsaved changes will be lost.", 
            "New Design", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _currentDesign = new StampDesign();
            _currentFilePath = null;
            _undoStack.Clear();
            _redoStack.Clear();
            _selectedElement = null;
            UpdateUI();
            UpdateUndoRedoButtons();
        }
    }

    private void OpenDesign_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "NexoStamp Files (*.nxs)|*.nxs|All Files (*.*)|*.*",
            Title = "Open Stamp Design"
        };
        
        if (dialog.ShowDialog() == true)
        {
            var design = _fileService.LoadDesign(dialog.FileName);
            if (design != null)
            {
                _currentDesign = design;
                _currentFilePath = dialog.FileName;
                _selectedElement = null;
                UpdateUI();
                StatusText.Text = $"Loaded: {dialog.FileName}";
            }
            else
            {
                MessageBox.Show("Failed to load design file.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveDesign_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            SaveDesignAs_Click(sender, e);
        }
        else
        {
            _fileService.SaveDesign(_currentDesign, _currentFilePath);
            StatusText.Text = $"Saved: {_currentFilePath}";
        }
    }

    private void SaveDesignAs_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "NexoStamp Files (*.nxs)|*.nxs|All Files (*.*)|*.*",
            Title = "Save Stamp Design",
            FileName = _currentDesign.Name
        };
        
        if (dialog.ShowDialog() == true)
        {
            _fileService.SaveDesign(_currentDesign, dialog.FileName);
            _currentFilePath = dialog.FileName;
            StatusText.Text = $"Saved: {dialog.FileName}";
        }
    }

    private void PrintPreview_Click(object sender, RoutedEventArgs e)
    {
        _printService.ShowPrintPreview(_currentDesign, this);
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        _printService.PrintDesign(_currentDesign);
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        if (_undoStack.Count > 0)
        {
            _redoStack.Add(CloneDesign(_currentDesign));
            _currentDesign = _undoStack[_undoStack.Count - 1];
            _undoStack.RemoveAt(_undoStack.Count - 1);
            _selectedElement = null;
            UpdateUI();
            UpdateUndoRedoButtons();
        }
    }

    private void Redo_Click(object sender, RoutedEventArgs e)
    {
        if (_redoStack.Count > 0)
        {
            _undoStack.Add(CloneDesign(_currentDesign));
            _currentDesign = _redoStack[_redoStack.Count - 1];
            _redoStack.RemoveAt(_redoStack.Count - 1);
            _selectedElement = null;
            UpdateUI();
            UpdateUndoRedoButtons();
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedElement != null)
        {
            SaveToUndoStack();
            _currentDesign.Elements.Remove(_selectedElement);
            _selectedElement = null;
            UpdateUI();
        }
    }

    private void Duplicate_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedElement != null)
        {
            SaveToUndoStack();
            var clone = _selectedElement.Clone();
            _currentDesign.Elements.Add(clone);
            
            // Deselect old and select new
            _selectedElement.IsSelected = false;
            _selectedElement = clone;
            _selectedElement.IsSelected = true;
            
            UpdateUI();
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("NexoStamp v1.0\n\nA stamp design and printing application.\n\n" +
            "Features:\n- Design custom stamps\n- White on black color scheme\n- Print preview and printing\n- Save and load designs",
            "About NexoStamp", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Toolbar Handlers
    private void AddText_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        var textElement = new TextElement
        {
            X = 50,
            Y = 50,
            ZIndex = _currentDesign.Elements.Count
        };
        
        _currentDesign.Elements.Add(textElement);
        UpdateUI();
    }

    private void AddRectangle_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        var shapeElement = new ShapeElement
        {
            ShapeType = ShapeType.Rectangle,
            X = 50,
            Y = 50,
            ZIndex = _currentDesign.Elements.Count
        };
        
        _currentDesign.Elements.Add(shapeElement);
        UpdateUI();
    }

    private void AddCircle_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        var shapeElement = new ShapeElement
        {
            ShapeType = ShapeType.Circle,
            X = 50,
            Y = 50,
            ZIndex = _currentDesign.Elements.Count
        };
        
        _currentDesign.Elements.Add(shapeElement);
        UpdateUI();
    }

    private void AddLine_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        var shapeElement = new ShapeElement
        {
            ShapeType = ShapeType.Line,
            X = 50,
            Y = 50,
            Width = 100,
            Height = 0,
            ZIndex = _currentDesign.Elements.Count
        };
        
        _currentDesign.Elements.Add(shapeElement);
        UpdateUI();
    }

    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (ZoomLabel != null)
        {
            ZoomLabel.Text = $"{(int)(ZoomSlider.Value * 100)}%";
            CanvasViewbox.LayoutTransform = new ScaleTransform(ZoomSlider.Value, ZoomSlider.Value);
        }
    }

    // Canvas Interaction
    private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        
        var element = (FrameworkElement)sender;
        var stampElement = element.Tag as StampElement;
        
        if (stampElement != null)
        {
            // Deselect previous
            if (_selectedElement != null)
                _selectedElement.IsSelected = false;
            
            // Select new
            _selectedElement = stampElement;
            _selectedElement.IsSelected = true;
            
            _dragStartPoint = e.GetPosition(DesignCanvas);
            _elementStartPosition = new Point(stampElement.X, stampElement.Y);
            _isDragging = true;
            _capturedElement = element;
            
            element.CaptureMouse();
            UpdateUI();
        }
    }

    private void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Clicked on canvas, deselect
        if (_selectedElement != null)
        {
            _selectedElement.IsSelected = false;
            _selectedElement = null;
            UpdateUI();
        }
    }

    private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && _selectedElement != null && e.LeftButton == MouseButtonState.Pressed)
        {
            var currentPoint = e.GetPosition(DesignCanvas);
            var delta = currentPoint - _dragStartPoint;
            
            _selectedElement.X = _elementStartPosition.X + delta.X;
            _selectedElement.Y = _elementStartPosition.Y + delta.Y;
            
            UpdateUI();
        }
    }

    private void DesignCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging && _capturedElement != null)
        {
            _isDragging = false;
            _capturedElement.ReleaseMouseCapture();
            _capturedElement = null;
        }
    }

    // Property Change Handlers
    private void TextContent_Changed(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.Text = TextContentBox.Text;
            UpdateUI();
        }
    }

    private void FontFamily_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_selectedElement is TextElement textElement && FontFamilyComboBox.SelectedItem != null)
        {
            textElement.FontFamily = FontFamilyComboBox.SelectedItem.ToString() ?? "Arial";
            UpdateUI();
        }
    }

    private void FontSize_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.FontSize = FontSizeSlider.Value;
            if (FontSizeLabel != null)
                FontSizeLabel.Text = $"{textElement.FontSize:F0}pt";
            UpdateUI();
        }
    }

    private void FontStyle_Changed(object sender, RoutedEventArgs e)
    {
        if (_selectedElement is TextElement textElement)
        {
            textElement.FontWeight = BoldCheckBox.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            textElement.FontStyle = ItalicCheckBox.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;
            UpdateUI();
        }
    }

    private void StrokeThickness_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is ShapeElement shapeElement)
        {
            shapeElement.StrokeThickness = StrokeThicknessSlider.Value;
            if (StrokeThicknessLabel != null)
                StrokeThicknessLabel.Text = $"{shapeElement.StrokeThickness:F0}px";
            UpdateUI();
        }
    }

    private void Position_Changed(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        
        if (double.TryParse(PositionXBox.Text, out double x))
            _selectedElement.X = x;
        if (double.TryParse(PositionYBox.Text, out double y))
            _selectedElement.Y = y;
        
        UpdateUI();
    }

    private void Size_Changed(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement == null) return;
        
        if (double.TryParse(WidthBox.Text, out double width))
            _selectedElement.Width = width;
        if (double.TryParse(HeightBox.Text, out double height))
            _selectedElement.Height = height;
        
        UpdateUI();
    }

    private void Rotation_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement != null)
        {
            _selectedElement.Rotation = RotationSlider.Value;
            if (RotationLabel != null)
                RotationLabel.Text = $"{_selectedElement.Rotation:F0}°";
            UpdateUI();
        }
    }
}