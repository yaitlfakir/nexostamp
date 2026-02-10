# NexoStamp - Quick Start Guide

## Build and Run

### Prerequisites
- .NET 10.0 SDK or later
- Windows 10 or later (for running the application)
- Any code editor (Visual Studio, VS Code, Rider)

### Build Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/yaitlfakir/nexostamp.git
   cd nexostamp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build -c Release
   ```

4. **Run the application** (Windows only)
   ```bash
   dotnet run --project src/NexoStamp
   ```

### Create Standalone Executable

For a single-file executable that doesn't require .NET to be installed:

```bash
dotnet publish src/NexoStamp/NexoStamp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The executable will be in: `src/NexoStamp/bin/Release/net10.0-windows/win-x64/publish/NexoStamp.exe`

## Quick Feature Demo

### Create Your First Stamp (2 minutes)

1. **Launch NexoStamp**
   - You'll see a black canvas

2. **Add Text**
   - Click the "T" button
   - Click the text to select it
   - Change text to "APPROVED"
   - Set font size to 36
   - Check "Bold"

3. **Add a Circle**
   - Click the Circle button
   - Drag the circle around the text
   - Adjust size to 200x200

4. **Save It**
   - File → Save As
   - Name it "approved-stamp.nxs"

5. **Print Preview**
   - File → Print Preview
   - See how it looks!

### Try the Templates

Open one of the included templates to see examples:

1. File → Open
2. Navigate to `src/NexoStamp/Assets/Templates/`
3. Open `company-stamp.nxs` or `approved-stamp.nxs`
4. Modify as needed
5. Save with a new name

## File Structure

```
nexostamp/
├── src/NexoStamp/              # Main application
│   ├── Models/                 # Data models
│   ├── Services/               # Business logic
│   ├── Assets/Templates/       # Example templates
│   ├── MainWindow.xaml         # UI layout
│   └── MainWindow.xaml.cs      # UI logic
├── docs/                       # Documentation
│   ├── ARCHITECTURE.md         # Technical details
│   └── USER_GUIDE.md          # How to use
└── README.md                   # Overview
```

## Key Features at a Glance

✅ **Design Tools**
- Text with customizable fonts
- Shapes (rectangles, circles, lines)
- Drag & drop positioning
- Rotation controls
- Undo/Redo

✅ **File Operations**
- Save/Load designs (.nxs format)
- JSON-based, human-readable
- Example templates included

✅ **Printing**
- Print preview
- High-quality output
- Windows printer support

✅ **UI**
- Clean, intuitive interface
- Properties panel
- Zoom controls
- Keyboard shortcuts

## Common Use Cases

### Official Company Stamp
1. Add company name (large, bold)
2. Add department/division (smaller)
3. Add rectangle border
4. Add horizontal line separator

### Approval Stamp
1. Add "APPROVED" text (bold, large)
2. Add circle around it
3. Add "Date: ____" below

### Custom Logo Stamp
1. Add text elements for company name
2. Use shapes to create simple logo
3. Arrange and rotate elements
4. Save as template for reuse

## Development

### Project Structure
- **WPF Application** (.NET 10.0)
- **MVVM-inspired** architecture
- **JSON serialization** for file I/O
- **Windows Print System** integration

### Adding New Features

**New Element Type:**
1. Create class in `Models/` inheriting `StampElement`
2. Add rendering in `MainWindow.AddElementToCanvas()`
3. Add printing in `PrintService.CreateShapeVisual()`

**New Property:**
1. Add to element class
2. Add UI control in `MainWindow.xaml`
3. Add handler in `MainWindow.xaml.cs`

## Testing

Currently, testing is manual:

1. Build the application
2. Run on Windows
3. Test each feature:
   - Add text/shapes
   - Modify properties
   - Save/load
   - Print preview
   - Print

Future: Add unit tests for Models and Services.

## Troubleshooting

**Build fails with NETSDK1100:**
- You're on non-Windows platform
- This is expected - app targets Windows
- EnableWindowsTargeting is already set for cross-platform build
- To run, you need Windows OS

**Application won't start:**
- Ensure .NET 10.0 runtime is installed
- Check Windows version (10 or later required)

**Print doesn't work:**
- Verify printer is installed and working
- Try Print Preview first
- Check printer settings

## Next Steps

1. **Read the User Guide** (`docs/USER_GUIDE.md`)
   - Comprehensive how-to for all features
   
2. **Check the Architecture docs** (`docs/ARCHITECTURE.md`)
   - Understand the codebase
   - Learn how to extend it

3. **Explore the Templates** (`src/NexoStamp/Assets/Templates/`)
   - See examples of good designs
   - Use as starting points

4. **Create Your Own Stamps!**
   - Experiment with different designs
   - Share templates with your team

## Support

- **Documentation**: See `docs/` folder
- **Issues**: Report on GitHub
- **Questions**: Open a discussion on GitHub

---

**Happy Stamping! 📮**
