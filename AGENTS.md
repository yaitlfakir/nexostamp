# Agent Guidelines for NexoStamp

NexoStamp is a .NET 10.0 WPF application for designing white-on-black stamps. It is a Windows-only project.

## Developer Commands

- **Build**: `dotnet build -c Release`
- **Run**: `dotnet run --project src/NexoStamp`
- **Publish (Standalone EXE)**: 
  `dotnet publish src/NexoStamp/NexoStamp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

## Architecture & Project Structure

- **Pattern**: Model-Service-View (simplified MVVM).
- **Models** (`src/NexoStamp/Models/`): Data structures for stamp elements and designs.
- **Services** (`src/NexoStamp/Services/`): Logic for File I/O (`FileService`) and Printing (`PrintService`).
- **UI** (`src/NexoStamp/`): XAML and code-behind for the main window.
- **Templates** (`src/NexoStamp/Assets/Templates/`): Example `.nxs` files.

## Key Conventions

- **Color Scheme**: Always use white (#FFFFFF) for foreground elements and black (#000000) for the background.
- **File Format**: `.nxs` files are JSON-based with polymorphic serialization for stamp elements.
- **Rendering**: Use vector-based rendering for high-quality print output.
- **UI Development**: Follow WPF/XAML conventions. Ensure changes in `MainWindow.xaml` are mirrored correctly in `MainWindow.xaml.cs`.

## Verification Steps

1. **Build**: Run `dotnet build -c Release` to ensure no compilation errors or warnings.
2. **Functional Test**: Run the app and verify the feature using the Visual Editor.
3. **File I/O**: Verify that `.nxs` files can be saved and loaded without data loss.
4. **Print Preview**: Check the Print Preview window to ensure vector rendering is correct.
