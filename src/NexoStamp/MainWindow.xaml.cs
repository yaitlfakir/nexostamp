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
    private StampDesign _currentDesign = new();
    private FileService _fileService = new();
    private PrintService _printService = new();
    private string? _currentFilePath;
    
    // Global exception handler
    public MainWindow()
    {
        InitializeComponent();

        InitializeFontList();
        InitializeLocalizedStrings();
        UpdateUI();
        UpdateUndoRedoButtons();

        // Subscribe to language changes
        LocalizationService.Instance.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "FlowDirection" || string.IsNullOrEmpty(e.PropertyName))
            {
                UpdateFlowDirection();
            }
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                UpdateLocalizedStrings();
            }
        };
    }
    
    private List<StampDesign> _undoStack = new();
    private List<StampDesign> _redoStack = new();
    
    private StampElement? _selectedElement;
    private FrameworkElement? _capturedElement;
    private Point _dragStartPoint;
    private bool _isDragging;
    private Point _elementStartPosition;
    
    // Multi-select support
    private bool _isMultiSelectMode = false;
    private List<StampElement> _selectedElements = new();
    private Dictionary<StampElement, Point> _dragStartPositions = new();
    
    private void InitializeLocalizedStrings()
    {
        var loc = LocalizationService.Instance;
        Title = loc.GetString("AppTitle");
        UpdateLocalizedStrings();
    }

    private void UpdateLocalizedStrings()
    {
        var loc = LocalizationService.Instance;

        // File menu
        FileMenuItem.Header = loc.GetString("Menu_File");
        NewMenuItem.Header = loc.GetString("Menu_New");
        OpenMenuItem.Header = loc.GetString("Menu_Open");
        SaveMenuItem.Header = loc.GetString("Menu_Save");
        SaveAsMenuItem.Header = loc.GetString("Menu_SaveAs");
        PrintPreviewMenuItem.Header = loc.GetString("Menu_PrintPreview");
        PrintMenuItem.Header = loc.GetString("Menu_Print");
        ExitMenuItem.Header = loc.GetString("Menu_Exit");

        // Edit menu
        EditMenuItem.Header = loc.GetString("Menu_Edit");
        UndoMenuItem.Header = loc.GetString("Menu_Undo");
        RedoMenuItem.Header = loc.GetString("Menu_Redo");
        DeleteMenuItem.Header = loc.GetString("Menu_Delete");
        DuplicateMenuItem.Header = loc.GetString("Menu_Duplicate");

        // Help menu
        HelpMenuItem.Header = loc.GetString("Menu_Help");
        AboutMenuItem.Header = loc.GetString("Menu_About");

        // Language menu
        LanguageMenu.Header = loc.GetString("Menu_Language");
        EnglishMenuItem.Header = loc.GetString("Menu_English");
        ArabicMenuItem.Header = loc.GetString("Menu_Arabic");

        // Toolbar tooltips
        AddTextButton.ToolTip = loc.GetString("Toolbar_AddText");
        AddRectangleButton.ToolTip = loc.GetString("Toolbar_AddRectangle");
        AddCircleButton.ToolTip = loc.GetString("Toolbar_AddCircle");
        AddCircularStampButton.ToolTip = loc.GetString("Toolbar_CircularStamp");
        AddCurvedTextButton.ToolTip = loc.GetString("Toolbar_CurvedText");
        AddLineButton.ToolTip = loc.GetString("Toolbar_AddLine");
        ZoomLabel.Text = loc.GetString("Toolbar_Zoom");

        // Properties panel
        PropertiesTitleText.Text = loc.GetString("Properties_Title");
        NoSelectionText.Text = loc.GetString("Properties_NoSelection");

        // Text Properties
        TextPropertiesTitle.Text = loc.GetString("Properties_TextProperties");
        TextLabel.Content = loc.GetString("Properties_Text");
        FontFamilyLabel.Content = loc.GetString("Properties_FontFamily");
        FontSizeLabelProp.Content = loc.GetString("Properties_FontSize");
        BoldCheckBox.Content = loc.GetString("Properties_Bold");
        ItalicCheckBox.Content = loc.GetString("Properties_Italic");

        // Curved Text Properties
        CurvedTextPropertiesTitle.Text = loc.GetString("Properties_CurvedText");
        CurvedTextLabel.Content = loc.GetString("Properties_Text");
        CurvedFontFamilyLabel.Content = loc.GetString("Properties_FontFamily");
        CurvedFontSizeLabelProp.Content = loc.GetString("Properties_FontSize");
        CurvedBoldCheckBox.Content = loc.GetString("Properties_Bold");
        CurvedItalicCheckBox.Content = loc.GetString("Properties_Italic");
        RadiusLabel.Content = loc.GetString("Properties_Radius");
        StartAngleLabel.Content = loc.GetString("Properties_StartAngle");
        EndAngleLabel.Content = loc.GetString("Properties_EndAngle");
        CurveInvertedCheckBox.Content = loc.GetString("Properties_InvertDirection");

        // Shape Properties
        ShapePropertiesTitle.Text = loc.GetString("Properties_ShapeProperties");
        StrokeThicknessLabel.Content = loc.GetString("Properties_StrokeThickness");

        // Common Properties
        PositionLabel.Content = loc.GetString("Properties_Position");
        XLabel.Content = "X:";
        YLabel.Content = "Y:";
        SizeLabel.Content = loc.GetString("Properties_Size");
        WLabel.Content = "W:";
        HLabel.Content = "H:";
        RotationLabelProp.Content = loc.GetString("Properties_Rotation");
    }

    private void UpdateFlowDirection()
    {
        var loc = LocalizationService.Instance;
        FlowDirection = loc.FlowDirection;
    }

    // Language switching handlers
    private void EnglishMenuItem_Click(object sender, RoutedEventArgs e)
    {
        LocalizationService.Instance.SetEnglish();
    }

    private void ArabicMenuItem_Click(object sender, RoutedEventArgs e)
    {
        LocalizationService.Instance.SetArabic();
    }

    private void InitializeFontList()
    {
        var fonts = new[] {
            // English fonts
            "Arial", "Times New Roman", "Courier New", "Verdana",
            "Georgia", "Comic Sans MS", "Impact", "Trebuchet MS",
            // Arabic fonts
            "Traditional Arabic", "Arabic Typesetting", "Segoe UI",
            "Simplified Arabic", "Traditional Arabic"
        };

        foreach (var font in fonts)
        {
            FontFamilyComboBox.Items.Add(font);
            CurvedFontFamilyComboBox.Items.Add(font);
        }
        FontFamilyComboBox.SelectedIndex = 0;
        CurvedFontFamilyComboBox.SelectedIndex = 0;
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
                TextWrapping = TextWrapping.Wrap,
                Tag = element
            };
        }
        else if (element is CurvedTextElement curvedElement)
        {
            visual = CreateCurvedTextVisual(curvedElement);
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

    private FrameworkElement CreateCurvedTextVisual(CurvedTextElement curvedElement)
    {
        var canvas = new Canvas
        {
            Width = curvedElement.Width,
            Height = curvedElement.Height,
            Tag = curvedElement,
            Background = Brushes.Transparent // Ensure it's hittable
        };

        double centerX = curvedElement.Width / 2;
        double centerY = curvedElement.Height / 2;
        
        string text = curvedElement.Text;
        double totalAngle = curvedElement.EndAngle - curvedElement.StartAngle;
        if (totalAngle == 0) totalAngle = 1;
        
        double angleStep = totalAngle / (text.Length > 1 ? text.Length - 1 : 1);
        
        for (int i = 0; i < text.Length; i++)
        {
            double angleDeg = curvedElement.StartAngle + (i * angleStep);
            double angleRad = (Math.PI / 180.0) * angleDeg;
            
            if (curvedElement.IsInverted)
            {
                angleRad = -angleRad;
            }

            double x = centerX + curvedElement.Radius * Math.Cos(angleRad);
            double y = centerY + curvedElement.Radius * Math.Sin(angleRad);

            var charBlock = new TextBlock
            {
                Text = text[i].ToString(),
                FontFamily = new FontFamily(curvedElement.FontFamily),
                FontSize = curvedElement.FontSize,
                FontWeight = curvedElement.FontWeight,
                FontStyle = curvedElement.FontStyle,
                Foreground = Brushes.White
            };

            // Rotate character to face the center or follow the path
            var rotate = new RotateTransform(angleDeg + 90, 0, 0);
            charBlock.RenderTransform = rotate;

            Canvas.SetLeft(charBlock, x - (charBlock.ActualWidth / 2));
            Canvas.SetTop(charBlock, y - (charBlock.ActualHeight / 2));
            
            // Since ActualWidth is 0 during creation, we might need a small hack or 
            // just accept slight offset if not measured. In a real WPF app, 
            // we'd use a layout pass.
            
            canvas.Children.Add(charBlock);
        }

        return canvas;
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
            CurvedTextPropertiesPanel.Visibility = Visibility.Collapsed;
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

        // Update position sliders
        PositionXSlider.Value = _selectedElement.X;
        PositionXSliderLabel.Text = $"X: {_selectedElement.X:F0}";
        PositionYSlider.Value = _selectedElement.Y;
        PositionYSliderLabel.Text = $"Y: {_selectedElement.Y:F0}";

        // Update size sliders
        WidthSlider.Value = _selectedElement.Width;
        WidthSliderLabel.Text = $"W: {_selectedElement.Width:F0}";
        HeightSlider.Value = _selectedElement.Height;
        HeightSliderLabel.Text = $"H: {_selectedElement.Height:F0}";

        RotationSlider.Value = _selectedElement.Rotation;
        RotationLabel.Text = $"{_selectedElement.Rotation:F0}°";
        
        if (_selectedElement is TextElement textElement)
        {
            TextPropertiesPanel.Visibility = Visibility.Visible;
            CurvedTextPropertiesPanel.Visibility = Visibility.Collapsed;
            ShapePropertiesPanel.Visibility = Visibility.Collapsed;
            
            TextContentBox.Text = textElement.Text;
            FontFamilyComboBox.SelectedItem = textElement.FontFamily;
            FontSizeSlider.Value = textElement.FontSize;
            FontSizeLabel.Text = $"{textElement.FontSize:F0}pt";
            BoldCheckBox.IsChecked = textElement.FontWeight == FontWeights.Bold;
            ItalicCheckBox.IsChecked = textElement.FontStyle == FontStyles.Italic;
        }
        else if (_selectedElement is CurvedTextElement curvedElement)
        {
            TextPropertiesPanel.Visibility = Visibility.Collapsed;
            CurvedTextPropertiesPanel.Visibility = Visibility.Visible;
            ShapePropertiesPanel.Visibility = Visibility.Collapsed;
            
            CurvedTextContentBox.Text = curvedElement.Text;
            CurvedFontFamilyComboBox.SelectedItem = curvedElement.FontFamily;
            CurvedFontSizeSlider.Value = curvedElement.FontSize;
            CurvedFontSizeLabel.Text = $"{curvedElement.FontSize:F0}pt";
            CurvedBoldCheckBox.IsChecked = curvedElement.FontWeight == FontWeights.Bold;
            CurvedItalicCheckBox.IsChecked = curvedElement.FontStyle == FontStyles.Italic;
            
            CurveRadiusSlider.Value = curvedElement.Radius;
            CurveRadiusLabel.Text = $"{curvedElement.Radius:F0}px";
            CurveStartAngleSlider.Value = curvedElement.StartAngle;
            CurveStartAngleLabel.Text = $"{curvedElement.StartAngle:F0}°";
            CurveEndAngleSlider.Value = curvedElement.EndAngle;
            CurveEndAngleLabel.Text = $"{curvedElement.EndAngle:F0}°";
            CurveInvertedCheckBox.IsChecked = curvedElement.IsInverted;
        }
        else if (_selectedElement is ShapeElement shapeElement)
        {
            TextPropertiesPanel.Visibility = Visibility.Collapsed;
            CurvedTextPropertiesPanel.Visibility = Visibility.Collapsed;
            ShapePropertiesPanel.Visibility = Visibility.Visible;

            // Show CirclePropertiesPanel only for Circle shapes
            if (shapeElement.ShapeType == ShapeType.Circle)
            {
                CirclePropertiesPanel.Visibility = Visibility.Visible;
                CircleRadiusSlider.Value = shapeElement.Width / 2;
                CircleRadiusLabel.Text = $"{shapeElement.Width / 2:F0}px";
                KeepSquareCheckBox.IsChecked = (shapeElement.Width == shapeElement.Height);
            }
            else
            {
                CirclePropertiesPanel.Visibility = Visibility.Collapsed;
            }

            StrokeThicknessSlider.Value = shapeElement.StrokeThickness;
            StrokeThicknessLabelValue.Text = $"{shapeElement.StrokeThickness:F0}px";
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

    private void SaveAsTemplate_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "NexoStamp Templates (*.nxs)|*.nxs",
            Title = "Save as Template",
            FileName = _currentDesign.Name + " Template"
        };
        
        if (dialog.ShowDialog() == true)
        {
            _fileService.SaveDesign(_currentDesign, dialog.FileName);
            StatusText.Text = $"Template saved: {dialog.FileName}";
            MessageBox.Show("Template saved successfully!\n\nYou can find it in the Templates folder.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private void LoadTemplate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem?.Tag is string templateName)
            {
                // Try multiple possible paths
                string baseDir = System.IO.Directory.GetCurrentDirectory();
                string templatePath1 = System.IO.Path.Combine(baseDir, "src", "NexoStamp", "Assets", "Templates", templateName);
                string templatePath2 = System.IO.Path.Combine(baseDir, "NexoStamp", "bin", "Release", "net8.0-windows", "Assets", "Templates", templateName);
                string templatePath3 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Templates", templateName);
                
                string? foundPath = null;
                
                if (System.IO.File.Exists(templatePath1)) foundPath = templatePath1;
                else if (System.IO.File.Exists(templatePath2)) foundPath = templatePath2;
                else if (System.IO.File.Exists(templatePath3)) foundPath = templatePath3;
                else
                {
                    MessageBox.Show("Template file not found!\n\nTried:\n" + templatePath1 + "\n" + templatePath2 + "\n" + templatePath3, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var design = _fileService.LoadDesign(foundPath);
                if (design != null)
                {
                    _currentDesign = design;
                    _currentFilePath = null;
                    _selectedElement = null;
                    _undoStack.Clear();
                    _redoStack.Clear();
                    UpdateUI();
                    UpdateUndoRedoButtons();
                    StatusText.Text = "Loaded: " + templateName;
                }
                else
                {
                    MessageBox.Show("Failed to load template: " + templateName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message + "\n\nStack: " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("NexoStamp v1.0\n\nA stamp design and printing application.\n\n" +
            "Features:\n- Design custom stamps\n- White on black color scheme\n- Print preview and printing\n- Save and load designs\n- Predefined templates",
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

    // Size Calculator (mm conversion)
    private const double DPI = 96.0;
    private const double MM_PER_INCH = 25.4;
    
    private double PixelsToMm(double pixels) => (pixels * MM_PER_INCH) / DPI;
    private double MmToPixels(double mm) => (mm * DPI) / MM_PER_INCH;
    
    private void OpenSizeCalculator_Click(object sender, RoutedEventArgs e)
    {
        var selected = _isMultiSelectMode && _selectedElements.Count > 0 ? _selectedElements : 
                     (_selectedElement != null ? new List<StampElement> { _selectedElement } : new List<StampElement>());
        
        if (selected.Count == 0)
        {
            MessageBox.Show("No element selected.\nSelect an element first or enable Multi-Select.", "Size Calculator", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        
        // Calculate bounding box of selected elements
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
        foreach (var el in selected)
        {
            minX = Math.Min(minX, el.X);
            minY = Math.Min(minY, el.Y);
            maxX = Math.Max(maxX, el.X + el.Width);
            maxY = Math.Max(maxY, el.Y + el.Height);
        }
        
        double widthMm = PixelsToMm(maxX - minX);
        double heightMm = PixelsToMm(maxY - minY);
        
        var dialog = new Window
        {
            Title = "Size Calculator",
            Width = 350,
            Height = 280,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };
        
        var panel = new StackPanel { Margin = new Thickness(20) };
        
        panel.Children.Add(new TextBlock { Text = "Element Size Calculator", FontSize = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 15) });
        panel.Children.Add(new TextBlock { Text = $"Width:  {widthMm:F1} mm  ({maxX - minX:F0} pixels)", FontSize = 14, Margin = new Thickness(5) });
        panel.Children.Add(new TextBlock { Text = $"Height: {heightMm:F1} mm  ({maxY - minY:F0} pixels)", FontSize = 14, Margin = new Thickness(5) });
        
        panel.Children.Add(new Separator { Margin = new Thickness(5, 15, 5, 15) });
        panel.Children.Add(new TextBlock { Text = "Create with Size:", FontSize = 14, FontWeight = FontWeights.Bold, Margin = new Thickness(5) });
        
        var widthInput = new TextBox { Text = widthMm.ToString("F1"), Width = 100, Margin = new Thickness(5) };
        var heightInput = new TextBox { Text = heightMm.ToString("F1"), Width = 100, Margin = new Thickness(5) };
        
        var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
        rowPanel.Children.Add(new TextBlock { Text = "Width mm: ", VerticalAlignment = VerticalAlignment.Center });
        rowPanel.Children.Add(widthInput);
        rowPanel.Children.Add(new TextBlock { Text = "  Height mm: ", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) });
        rowPanel.Children.Add(heightInput);
        panel.Children.Add(rowPanel);
        
        var applyBtn = new Button { Content = "Apply Size", Width = 100, Margin = new Thickness(5, 15, 5, 0) };
        var finalWidth = widthInput;
        var finalHeight = heightInput;
        applyBtn.Click += (s, args) => 
        {
            if (double.TryParse(finalWidth.Text, out double w) && double.TryParse(finalHeight.Text, out double h))
            {
                double newWidthPx = MmToPixels(w);
                double newHeightPx = MmToPixels(h);
                
                foreach (var el in selected)
                {
                    el.Width = newWidthPx;
                    el.Height = newHeightPx;
                    
                    if (el is ShapeElement se && se.ShapeType == ShapeType.Circle)
                    {
                        // Keep circle round
                        el.Width = Math.Min(newWidthPx, newHeightPx);
                        el.Height = el.Width;
                    }
                }
                UpdateUI();
                dialog.Close();
            }
        };
        panel.Children.Add(applyBtn);
        
        dialog.Content = panel;
        dialog.ShowDialog();
    }
    
    private void ToggleMultiSelect_Click(object sender, RoutedEventArgs e)
    {
        _isMultiSelectMode = !_isMultiSelectMode;
        
        if (_isMultiSelectMode)
        {
            MultiSelectStatusText.Text = "Multi-Select ON (Ctrl+Click to select)";
            MultiSelectStatusText.Foreground = Brushes.Green;
            
            if (_selectedElement != null)
            {
                _selectedElements.Clear();
                _selectedElements.Add(_selectedElement);
            }
        }
        else
        {
            MultiSelectStatusText.Text = "Single Select";
            MultiSelectStatusText.Foreground = Brushes.Gray;
            _selectedElements.Clear();
        }
        
        UpdateSizeDisplay();
    }
    
    private void UpdateSizeDisplay()
    {
        var selected = _isMultiSelectMode ? _selectedElements : 
                     (_selectedElement != null ? new List<StampElement> { _selectedElement } : new List<StampElement>());
        
        if (selected.Count == 0)
        {
            SizeDisplayText.Text = "Size: 0×0 mm";
            return;
        }
        
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
        foreach (var el in selected)
        {
            minX = Math.Min(minX, el.X);
            minY = Math.Min(minY, el.Y);
            maxX = Math.Max(maxX, el.X + el.Width);
            maxY = Math.Max(maxY, el.Y + el.Height);
        }
        
        double widthMm = PixelsToMm(maxX - minX);
        double heightMm = PixelsToMm(maxY - minY);
        
        SizeDisplayText.Text = $"Size: {widthMm:F1} × {heightMm:F1} mm";
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
            if (_isMultiSelectMode)
            {
                // Toggle selection in multi-select mode
                if (_selectedElements.Contains(stampElement))
                {
                    _selectedElements.Remove(stampElement);
                    stampElement.IsSelected = false;
                }
                else
                {
                    _selectedElements.Add(stampElement);
                    stampElement.IsSelected = true;
                }
                _selectedElement = _selectedElements.Count > 0 ? _selectedElements[0] : null;
            }
            else
            {
                // Deselect previous
                if (_selectedElement != null)
                    _selectedElement.IsSelected = false;
                
                // Select new
                _selectedElement = stampElement;
                _selectedElement.IsSelected = true;
                _selectedElements.Clear();
                _selectedElements.Add(stampElement);
            }
            
            _dragStartPoint = e.GetPosition(DesignCanvas);
            _elementStartPosition = new Point(stampElement.X, stampElement.Y);
            _isDragging = true;
            _capturedElement = element;
            
            // Store start positions for all selected elements in multi-select
            _dragStartPositions.Clear();
            foreach (var sel in _selectedElements)
            {
                _dragStartPositions[sel] = new Point(sel.X, sel.Y);
            }
            
            element.CaptureMouse();
            UpdateUI();
            UpdateSizeDisplay();
        }
    }

    private void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Clicked on canvas, deselect all
        if (_selectedElement != null)
        {
            _selectedElement.IsSelected = false;
        }
        foreach (var sel in _selectedElements)
        {
            sel.IsSelected = false;
        }
        _selectedElement = null;
        _selectedElements.Clear();
        UpdateUI();
        UpdateSizeDisplay();
    }

    private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && _selectedElements.Count > 0 && e.LeftButton == MouseButtonState.Pressed)
        {
            var currentPoint = e.GetPosition(DesignCanvas);
            var delta = currentPoint - _dragStartPoint;
            
            foreach (var sel in _selectedElements)
            {
                if (_dragStartPositions.TryGetValue(sel, out var startPos))
                {
                    sel.X = startPos.X + delta.X;
                    sel.Y = startPos.Y + delta.Y;
                }
            }
            
            UpdateUI();
            UpdateSizeDisplay();
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

    // Circle Property Change Handlers
    private void CircleRadius_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is ShapeElement shape && shape.ShapeType == ShapeType.Circle)
        {
            SaveToUndoStack();
            double radius = e.NewValue;
            shape.Width = radius * 2;
            shape.Height = radius * 2;
            WidthBox.Text = shape.Width.ToString("F0");
            HeightBox.Text = shape.Height.ToString("F0");
            CircleRadiusLabel.Text = $"{radius:F0}px";
            UpdateUI();
        }
    }

    private void KeepSquare_Changed(object sender, RoutedEventArgs e)
    {
        if (_selectedElement is ShapeElement shape && shape.ShapeType == ShapeType.Circle)
        {
            // When checked, force W = H
            if (shape.Width != shape.Height)
            {
                SaveToUndoStack();
                shape.Height = shape.Width;
                HeightBox.Text = shape.Height.ToString("F0");
                CircleRadiusSlider.Value = shape.Width / 2;
                UpdateUI();
            }
        }
    }

    // Center selected circle inside another circle
    private void CenterInside_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedElement == null) return;

        // Find another circle element to center inside
        ShapeElement? targetCircle = null;

        foreach (var el in _currentDesign.Elements)
        {
            if (el is ShapeElement shape && shape.ShapeType == ShapeType.Circle && el != _selectedElement)
            {
                targetCircle = shape;
                break;
            }
        }

        if (targetCircle == null)
        {
            MessageBox.Show("No other circle found to center inside.\nAdd another circle first.",
                "Center Inside", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        SaveToUndoStack();

        // Calculate center position: target's center minus half of current element's size
        double targetCenterX = targetCircle.X + (targetCircle.Width / 2);
        double targetCenterY = targetCircle.Y + (targetCircle.Height / 2);

        _selectedElement.X = targetCenterX - (_selectedElement.Width / 2);
        _selectedElement.Y = targetCenterY - (_selectedElement.Height / 2);

        // Update position text boxes
        PositionXBox.Text = _selectedElement.X.ToString("F0");
        PositionYBox.Text = _selectedElement.Y.ToString("F0");

        UpdateUI();
    }

    // Center selected element in the canvas
    private void CenterInCanvas_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedElement == null) return;

        SaveToUndoStack();

        // Center in canvas (canvas is 400x300 based on XAML)
        double canvasCenterX = (400 - _selectedElement.Width) / 2;
        double canvasCenterY = (300 - _selectedElement.Height) / 2;

        _selectedElement.X = Math.Max(0, canvasCenterX);
        _selectedElement.Y = Math.Max(0, canvasCenterY);

        PositionXBox.Text = _selectedElement.X.ToString("F0");
        PositionYBox.Text = _selectedElement.Y.ToString("F0");

        UpdateUI();
    }

    // Slider Change Handlers
    private void PositionXSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement != null && PositionXSlider.IsVisible)
        {
            _selectedElement.X = e.NewValue;
            PositionXBox.Text = _selectedElement.X.ToString("F0");
            PositionXSliderLabel.Text = $"X: {_selectedElement.X:F0}";
            UpdateUI();
        }
    }

    private void PositionYSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement != null && PositionYSlider.IsVisible)
        {
            _selectedElement.Y = e.NewValue;
            PositionYBox.Text = _selectedElement.Y.ToString("F0");
            PositionYSliderLabel.Text = $"Y: {_selectedElement.Y:F0}";
            UpdateUI();
        }
    }

    private void WidthSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement != null && WidthSlider.IsVisible)
        {
            _selectedElement.Width = e.NewValue;
            WidthBox.Text = _selectedElement.Width.ToString("F0");
            WidthSliderLabel.Text = $"W: {_selectedElement.Width:F0}";
            UpdateUI();
        }
    }

    private void HeightSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement != null && HeightSlider.IsVisible)
        {
            _selectedElement.Height = e.NewValue;
            HeightBox.Text = _selectedElement.Height.ToString("F0");
            HeightSliderLabel.Text = $"H: {_selectedElement.Height:F0}";
            UpdateUI();
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
            if (StrokeThicknessLabelValue != null)
                StrokeThicknessLabelValue.Text = $"{shapeElement.StrokeThickness:F0}px";
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

    private void AddCircularStamp_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        double radius = 100;
        double size = radius * 2;
        double centerX = (_currentDesign.CanvasWidth - size) / 2;
        double centerY = (_currentDesign.CanvasHeight - size) / 2;

        // 1. Outer Circle
        var outerCircle = new ShapeElement
        {
            ShapeType = ShapeType.Circle,
            X = centerX,
            Y = centerY,
            Width = size,
            Height = size,
            StrokeThickness = 4,
            ZIndex = _currentDesign.Elements.Count
        };

        // 2. Inner Circle (20px apart from outer)
        var innerCircle = new ShapeElement
        {
            ShapeType = ShapeType.Circle,
            X = centerX + 20,
            Y = centerY + 20,
            Width = size - 40,
            Height = size - 40,
            StrokeThickness = 2,
            ZIndex = _currentDesign.Elements.Count + 1
        };

        // 3. Center Text
        var centerText = new TextElement
        {
            Text = "STAMP",
            X = centerX + (size / 2) - 50,
            Y = centerY + (size / 2) - 25,
            Width = 100,
            Height = 50,
            TextAlignment = TextAlignment.Center,
            ZIndex = _currentDesign.Elements.Count + 2
        };

        _currentDesign.Elements.Add(outerCircle);
        _currentDesign.Elements.Add(innerCircle);
        _currentDesign.Elements.Add(centerText);
        
        UpdateUI();
    }

    private void AddCurvedText_Click(object sender, RoutedEventArgs e)
    {
        SaveToUndoStack();
        
        var curvedElement = new CurvedTextElement
        {
            X = 100,
            Y = 100,
            ZIndex = _currentDesign.Elements.Count
        };
        
        _currentDesign.Elements.Add(curvedElement);
        UpdateUI();
    }

    private void CurvedTextContent_Changed(object sender, TextChangedEventArgs e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.Text = CurvedTextContentBox.Text;
            UpdateUI();
        }
    }

    private void CurvedFontFamily_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_selectedElement is CurvedTextElement curvedElement && CurvedFontFamilyComboBox.SelectedItem != null)
        {
            curvedElement.FontFamily = CurvedFontFamilyComboBox.SelectedItem.ToString() ?? "Arial";
            UpdateUI();
        }
    }

    private void CurvedFontSize_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.FontSize = CurvedFontSizeSlider.Value;
            if (CurvedFontSizeLabel != null)
                CurvedFontSizeLabel.Text = $"{curvedElement.FontSize:F0}pt";
            UpdateUI();
        }
    }

    private void CurvedFontStyle_Changed(object sender, RoutedEventArgs e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.FontWeight = CurvedBoldCheckBox.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            curvedElement.FontStyle = CurvedItalicCheckBox.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;
            UpdateUI();
        }
    }

    private void CurveRadius_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.Radius = CurveRadiusSlider.Value;
            curvedElement.Width = curvedElement.Radius * 2;
            curvedElement.Height = curvedElement.Radius * 2;
            if (CurveRadiusLabel != null)
                CurveRadiusLabel.Text = $"{curvedElement.Radius:F0}px";
            UpdateUI();
        }
    }

    private void CurveStartAngle_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.StartAngle = CurveStartAngleSlider.Value;
            if (CurveStartAngleLabel != null)
                CurveStartAngleLabel.Text = $"{curvedElement.StartAngle:F0}°";
            UpdateUI();
        }
    }

    private void CurveEndAngle_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.EndAngle = CurveEndAngleSlider.Value;
            if (CurveEndAngleLabel != null)
                CurveEndAngleLabel.Text = $"{curvedElement.EndAngle:F0}°";
            UpdateUI();
        }
    }

    private void CurveInverted_Changed(object sender, RoutedEventArgs e)
    {
        if (_selectedElement is CurvedTextElement curvedElement)
        {
            curvedElement.IsInverted = CurveInvertedCheckBox.IsChecked == true;
            UpdateUI();
        }
    }
}
