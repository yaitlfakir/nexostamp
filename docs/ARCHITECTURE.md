# NexoStamp Architecture Documentation

## Overview

NexoStamp is built using the Model-View-ViewModel (MVVM) pattern with WPF, though simplified for clarity and maintainability.

## Architecture Layers

### 1. Models Layer (`/Models`)

Contains the data structures that represent stamp designs:

- **StampElement.cs**: Abstract base class for all design elements
  - Properties: Id, Position (X, Y), Size (Width, Height), Rotation, IsSelected, ZIndex
  - Abstract method: `Clone()` for duplicating elements

- **TextElement.cs**: Represents text in the design
  - Inherits from StampElement
  - Additional properties: Text, FontFamily, FontSize, FontWeight, FontStyle, TextAlignment
  
- **ShapeElement.cs**: Represents shapes (rectangles, circles, lines)
  - Inherits from StampElement
  - Additional properties: ShapeType (enum), StrokeThickness
  
- **StampDesign.cs**: Container for the complete design
  - Properties: Name, CanvasWidth, CanvasHeight, Elements (List<StampElement>)

### 2. Services Layer (`/Services`)

Business logic and external interactions:

- **FileService.cs**: Handles file I/O operations
  - `SaveDesign()`: Serializes StampDesign to JSON
  - `LoadDesign()`: Deserializes JSON to StampDesign
  - Custom JSON converter for polymorphic StampElement serialization

- **PrintService.cs**: Manages printing functionality
  - `PrintDesign()`: Sends design to printer
  - `ShowPrintPreview()`: Displays print preview window
  - `CreatePrintDocument()`: Generates printable canvas
  - `RenderStampToCanvas()`: Converts StampDesign to visual representation

### 3. UI Layer

- **MainWindow.xaml**: XAML layout definition
  - Menu bar (File, Edit, Help)
  - Toolbar with design tool buttons
  - Canvas area with zoom controls
  - Properties panel
  - Status bar

- **MainWindow.xaml.cs**: UI logic and event handlers
  - Design management (New, Open, Save)
  - Element creation and manipulation
  - Selection and interaction handling
  - Undo/redo stack management
  - Property panel updates

## Data Flow

### Creating a New Element

1. User clicks "Add Text" button
2. `AddText_Click()` handler creates new TextElement
3. Element is added to `_currentDesign.Elements`
4. `UpdateUI()` is called
5. Element is rendered on canvas via `AddElementToCanvas()`

### Selecting and Editing

1. User clicks on element
2. `Element_MouseLeftButtonDown()` handles click
3. Element is marked as selected (`IsSelected = true`)
4. `UpdateUI()` refreshes display with selection border
5. `UpdatePropertiesPanel()` populates property controls
6. User changes properties (text, size, etc.)
7. Property change handlers update element data
8. `UpdateUI()` refreshes display

### Saving a Design

1. User clicks File → Save
2. `SaveDesign_Click()` calls `_fileService.SaveDesign()`
3. FileService serializes StampDesign to JSON
4. JSON written to file with .nxs extension

### Printing

1. User clicks File → Print
2. `Print_Click()` calls `_printService.PrintDesign()`
3. PrintService creates Canvas representation
4. Elements are scaled and positioned for page size
5. White elements on black background are rendered
6. Windows print dialog is shown
7. Document is sent to selected printer

## Key Design Patterns

### 1. Composite Pattern
- StampElement is the component
- TextElement and ShapeElement are leaves
- StampDesign contains the composite structure

### 2. Command Pattern (Undo/Redo)
- Each action saves the entire design state to undo stack
- Redo stack stores undone states
- Simple but effective for small to medium designs

### 3. Strategy Pattern (Rendering)
- Different element types render differently
- Each element type knows how to render itself
- PrintService adapts rendering for printing

### 4. Polymorphic Serialization
- Custom JsonConverter handles StampElement polymorphism
- Inspects JSON properties to determine concrete type
- Enables proper deserialization of mixed element lists

## Color Scheme

The application uses a specific color scheme optimized for stamp printing:

- **Canvas Background**: Black (#000000)
- **Element Foreground**: White (#FFFFFF)
- **Selection Border**: Yellow
- **UI Background**: Light gray (#F5F5F5)
- **UI Accents**: Dark gray (#D0D0D0)

This provides maximum contrast for printing while being comfortable for design work.

## File Format

StampDesign files (.nxs) are JSON formatted:

```json
{
  "Name": "My Stamp",
  "CanvasWidth": 400,
  "CanvasHeight": 300,
  "Elements": [
    {
      "Id": "guid",
      "X": 50, "Y": 50,
      "Width": 200, "Height": 60,
      "Rotation": 0,
      "ZIndex": 0,
      "Text": "SAMPLE",
      "FontFamily": "Arial",
      "FontSize": 36,
      ...
    }
  ]
}
```

## Extension Points

The architecture supports easy extension:

### Adding New Element Types
1. Create new class inheriting from StampElement
2. Add rendering logic in MainWindow.AddElementToCanvas()
3. Add printing logic in PrintService.CreateShapeVisual()
4. Update FileService converter if needed
5. Add toolbar button and handler

### Adding New Properties
1. Add property to element class
2. Add UI controls to properties panel in MainWindow.xaml
3. Add event handler in MainWindow.xaml.cs
4. Property will automatically serialize

### Adding New Export Formats
1. Create new service class (e.g., PdfExportService)
2. Reuse RenderStampToCanvas() logic from PrintService
3. Add menu item and handler in MainWindow

## Performance Considerations

- **Rendering**: WPF uses GPU acceleration for smooth rendering
- **Undo/Redo**: Entire design is cloned, suitable for <1000 elements
- **File I/O**: JSON serialization is fast for typical design sizes
- **Printing**: Vector graphics scale without quality loss

## Thread Safety

Currently single-threaded as WPF UI operations must run on UI thread. All operations are synchronous, which is appropriate for user-driven design work.

## Error Handling

- File operations check for null and handle exceptions
- Invalid property values are ignored (fall back to default)
- Print errors are handled by Windows print system
- JSON deserialization failures return null (checked by caller)

## Future Enhancements

Potential areas for expansion:

1. **Image Support**: Add bitmap/vector image elements
2. **Gradient Fills**: Support gradients in addition to solid colors
3. **Layers**: Explicit layer management beyond Z-index
4. **Templates**: Built-in template gallery
5. **Export**: PDF, PNG, SVG export options
6. **Collaboration**: Cloud save/load
7. **Advanced Text**: Text on path, text effects
8. **Macros**: Recordable action sequences
