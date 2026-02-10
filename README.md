# NexoStamp

A Windows desktop application for designing and printing custom stamps with white graphics on black backgrounds.

## Overview

NexoStamp is a professional stamp design application built with .NET and WPF. It provides an intuitive interface for creating custom stamps with text and shapes, perfect for creating official stamps, approval marks, and decorative designs.

## Features

### Design Interface
- **Visual Editor**: Real-time WYSIWYG canvas for designing stamps
- **White on Black**: Optimized color scheme (white elements on black background) for printing
- **Zoom Controls**: Adjustable zoom level (10% to 300%) for precise editing
- **Interactive Canvas**: Drag and drop elements, click to select

### Design Elements
- **Text Elements**: 
  - Customizable fonts (Arial, Times New Roman, Courier New, Verdana, Georgia, and more)
  - Adjustable font sizes (8pt to 72pt)
  - Bold and italic styling
  - Text alignment options
  
- **Shape Elements**:
  - Rectangles
  - Circles/Ellipses
  - Lines
  - Adjustable stroke thickness (1px to 10px)

### Element Manipulation
- **Positioning**: Precise X/Y coordinate control
- **Sizing**: Width and height adjustment
- **Rotation**: Rotate elements 0-360 degrees
- **Z-Order**: Automatic layering of elements
- **Selection**: Click to select, yellow border shows selected element

### File Operations
- **New Design**: Create a new stamp design
- **Open**: Load existing designs (.nxs format)
- **Save/Save As**: Save designs in JSON format for easy editing
- **Undo/Redo**: Full undo/redo support for all actions
- **Duplicate**: Clone selected elements

### Printing
- **Print Preview**: Preview your design before printing
- **Print Dialog**: Standard Windows print dialog with all settings
- **Multiple Stamps**: Print multiple copies on a single page
- **High Quality**: Vector-based rendering for crisp output

## System Requirements

- **Operating System**: Windows 10 or later
- **.NET Runtime**: .NET 10.0 or later
- **Display**: 1024x768 minimum resolution recommended
- **Printer**: Any Windows-compatible printer

## Installation

### From Source

1. Clone the repository:
   ```bash
   git clone https://github.com/yaitlfakir/nexostamp.git
   cd nexostamp
   ```

2. Build the application:
   ```bash
   dotnet build -c Release
   ```

3. Run the application:
   ```bash
   dotnet run --project src/NexoStamp
   ```

### Building an Executable

To create a standalone executable:

```bash
cd nexostamp
dotnet publish src/NexoStamp/NexoStamp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `src/NexoStamp/bin/Release/net10.0-windows/win-x64/publish/`

## Usage Guide

### Getting Started

1. **Launch NexoStamp** - The application opens with a blank black canvas

2. **Add Elements**:
   - Click the "T" button to add text
   - Click rectangle, circle, or line buttons to add shapes
   - Elements appear at position (50, 50) by default

3. **Customize Elements**:
   - Click on any element to select it (yellow border appears)
   - Use the Properties panel on the right to modify:
     - Text content and formatting
     - Position (X, Y coordinates)
     - Size (Width, Height)
     - Rotation angle
     - Shape stroke thickness

4. **Move Elements**:
   - Click and drag elements on the canvas
   - Or use the Position fields in the Properties panel for precise placement

5. **Save Your Design**:
   - File → Save or Save As
   - Files are saved with .nxs extension

6. **Print Your Stamp**:
   - File → Print Preview to see how it will print
   - File → Print to print the stamp

### Keyboard Shortcuts

- `Ctrl+N` - New design
- `Ctrl+O` - Open design
- `Ctrl+S` - Save design
- `Ctrl+P` - Print preview
- `Ctrl+Z` - Undo
- `Ctrl+Y` - Redo
- `Ctrl+D` - Duplicate selected element
- `Del` - Delete selected element

### Example Templates

The application includes example templates in `src/NexoStamp/Assets/Templates/`:

- **company-stamp.nxs** - Professional company stamp template
- **approved-stamp.nxs** - Circular approval stamp template

To use a template:
1. File → Open
2. Navigate to the Templates folder
3. Select a template file
4. Modify as needed and save with a new name

## Project Structure

```
nexostamp/
├── src/
│   └── NexoStamp/
│       ├── Models/           # Data models
│       │   ├── StampElement.cs      # Base class for all elements
│       │   ├── TextElement.cs       # Text element model
│       │   ├── ShapeElement.cs      # Shape element model
│       │   └── StampDesign.cs       # Complete design model
│       ├── Services/         # Business logic
│       │   ├── FileService.cs       # Save/load functionality
│       │   └── PrintService.cs      # Print functionality
│       ├── Assets/
│       │   └── Templates/    # Example stamp templates
│       ├── MainWindow.xaml           # Main UI layout
│       ├── MainWindow.xaml.cs        # Main UI logic
│       └── NexoStamp.csproj          # Project file
├── NexoStamp.sln             # Solution file
└── README.md                 # This file
```

## File Format

NexoStamp saves designs in JSON format (.nxs files), making them human-readable and easy to edit. Example structure:

```json
{
  "Name": "My Stamp",
  "CanvasWidth": 400,
  "CanvasHeight": 300,
  "Elements": [
    {
      "Text": "APPROVED",
      "FontFamily": "Arial",
      "FontSize": 36,
      "X": 50,
      "Y": 50,
      "Width": 200,
      "Height": 60
    }
  ]
}
```

## Technical Details

- **Framework**: .NET 10.0 with WPF
- **UI**: XAML-based Windows Presentation Foundation
- **Serialization**: System.Text.Json for file I/O
- **Rendering**: WPF vector graphics for high-quality output
- **Printing**: Windows Print System integration

## Tips for Best Results

1. **Use Bold Fonts**: Bold text prints more clearly on stamps
2. **Keep It Simple**: Fewer elements = clearer stamp
3. **Test Print**: Always use Print Preview before final printing
4. **High Contrast**: White on black provides maximum contrast
5. **Standard Sizes**: Common stamp sizes are 300x300px or 400x300px

## Troubleshooting

**Q: The application won't run**
- A: Make sure .NET 10.0 runtime is installed. Download from https://dotnet.microsoft.com/

**Q: Elements are hard to see**
- A: Use the zoom slider to increase magnification

**Q: Print output is low quality**
- A: NexoStamp uses vector graphics which should print at high quality. Check your printer settings for best quality.

**Q: Can't select an element**
- A: Make sure you're clicking directly on the element, not empty canvas space

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is open source and available under the MIT License.

## Version History

- **v1.0** (2026-02-09) - Initial release
  - Design interface with text and shapes
  - Save/load functionality
  - Print preview and printing
  - Undo/redo support
  - Example templates

## Contact

For questions, issues, or suggestions, please open an issue on GitHub.

---

**NexoStamp** - Professional Stamp Design Made Easy