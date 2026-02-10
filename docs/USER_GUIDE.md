# NexoStamp User Guide

## Table of Contents

1. [Getting Started](#getting-started)
2. [Interface Overview](#interface-overview)
3. [Working with Text](#working-with-text)
4. [Working with Shapes](#working-with-shapes)
5. [Manipulating Elements](#manipulating-elements)
6. [Saving and Loading](#saving-and-loading)
7. [Printing](#printing)
8. [Tips and Tricks](#tips-and-tricks)

## Getting Started

### First Launch

When you first launch NexoStamp, you'll see:
- A black canvas in the center (this is where you design your stamp)
- A toolbar at the top with buttons for adding elements
- A menu bar with File, Edit, and Help options
- A properties panel on the right (initially showing "No element selected")
- A status bar at the bottom

### Your First Stamp

Let's create a simple "APPROVED" stamp:

1. Click the **"T" (Text)** button in the toolbar
2. You'll see white text saying "Text" appear on the black canvas
3. Click on the text to select it (it will get a yellow border)
4. In the Properties panel on the right:
   - Change the text to "APPROVED"
   - Set Font Size to 48
   - Check the "Bold" checkbox
5. Click the **Circle** button in the toolbar
6. A white circle outline will appear
7. Drag it to surround the text
8. Adjust the size by changing Width and Height in Properties
9. You've created your first stamp!

## Interface Overview

### Menu Bar

**File Menu:**
- **New** (Ctrl+N): Start a new blank design
- **Open** (Ctrl+O): Load a saved design
- **Save** (Ctrl+S): Save current design
- **Save As**: Save with a new name
- **Print Preview** (Ctrl+P): See how it will print
- **Print**: Send to printer
- **Exit**: Close the application

**Edit Menu:**
- **Undo** (Ctrl+Z): Undo last action
- **Redo** (Ctrl+Y): Redo previously undone action
- **Delete** (Del): Remove selected element
- **Duplicate** (Ctrl+D): Copy selected element

**Help Menu:**
- **About**: Application information

### Toolbar

From left to right:
1. **T (Text)**: Add a text element
2. **Rectangle**: Add a rectangle outline
3. **Circle**: Add a circle/ellipse outline
4. **Line**: Add a straight line
5. **Zoom Slider**: Adjust canvas zoom (10% to 300%)
6. **Zoom Label**: Shows current zoom percentage

### Canvas Area

- The black canvas is where you design
- Click and drag elements to move them
- Click empty canvas to deselect all elements
- Use the zoom slider for detailed work

### Properties Panel

Shows properties of the selected element:

**For All Elements:**
- Position (X, Y coordinates)
- Size (Width, Height)
- Rotation (0-360 degrees)

**For Text Elements:**
- Text content
- Font family
- Font size
- Bold and Italic options

**For Shape Elements:**
- Stroke thickness

## Working with Text

### Adding Text

1. Click the **"T"** button
2. Text appears at position (50, 50)
3. Click to select it

### Customizing Text

**Changing the Text:**
1. Select the text element
2. Type in the "Text:" field in Properties
3. Press Enter or click elsewhere

**Changing Font:**
1. Select the text element
2. Choose from the Font Family dropdown
3. Available fonts: Arial, Times New Roman, Courier New, Verdana, Georgia, Comic Sans MS, Impact, Trebuchet MS

**Adjusting Size:**
1. Select the text element
2. Drag the Font Size slider (8pt to 72pt)
3. Or type a specific value

**Styling:**
- Check **Bold** for thicker letters
- Check **Italic** for slanted letters
- Use both for bold italic

### Text Best Practices

- **Bold fonts print better**: They're more visible on stamps
- **Larger is clearer**: 24pt minimum for readability
- **Simple fonts work best**: Arial and Impact are excellent choices
- **ALL CAPS**: Often more professional for stamps

## Working with Shapes

### Rectangle

1. Click the Rectangle button
2. A 100x100 white rectangle outline appears
3. Adjust size to frame text or create borders

**Common Uses:**
- Borders around entire stamp
- Dividing lines (make very thin)
- Decorative frames

### Circle

1. Click the Circle button
2. A 100x100 white circle outline appears
3. Make Width = Height for perfect circles
4. Different Width/Height creates ellipses

**Common Uses:**
- Circular stamps
- Surrounding important text
- Decorative accents

### Line

1. Click the Line button
2. A horizontal line appears
3. Change Width for line length
4. Change Height to make it diagonal
5. Use Rotation for exact angles

**Common Uses:**
- Dividing sections
- Underlines
- Decorative elements

### Adjusting Stroke Thickness

1. Select any shape
2. Use the "Stroke Thickness" slider
3. Range: 1px (thin) to 10px (thick)
4. Thicker lines are more visible when printed

## Manipulating Elements

### Selecting

- Click any element to select it
- Selected element shows yellow border
- Click empty canvas to deselect

### Moving

**Method 1: Drag and Drop**
1. Click and hold on an element
2. Drag to new position
3. Release mouse button

**Method 2: Precise Positioning**
1. Select element
2. Type exact X and Y coordinates in Properties
3. Good for alignment

### Resizing

1. Select element
2. In Properties, change Width and/or Height
3. For text, you might also want to adjust font size

### Rotating

1. Select element
2. Drag the Rotation slider in Properties
3. Range: 0° to 360°
4. Useful for angled text and decorative elements

### Layering

- Elements are automatically layered by creation order
- Newer elements appear on top
- Can't currently reorder manually

### Duplicating

1. Select element you want to copy
2. Press Ctrl+D or Edit → Duplicate
3. Copy appears slightly offset
4. Useful for creating patterns

### Deleting

1. Select element
2. Press Delete key or Edit → Delete
3. Can be undone with Ctrl+Z

## Saving and Loading

### Saving Your Work

**First Time:**
1. File → Save or Save As
2. Choose location and filename
3. File is saved with .nxs extension

**After First Save:**
1. File → Save (Ctrl+S)
2. Overwrites existing file
3. File → Save As to save copy with new name

### Loading a Design

1. File → Open (Ctrl+O)
2. Browse to .nxs file
3. Click Open
4. Design loads into canvas

### File Format

- Files are saved in .nxs format
- Actually JSON text files
- Can be edited in text editor if needed
- Human-readable format

## Printing

### Print Preview

1. File → Print Preview (Ctrl+P)
2. See exactly how stamp will print
3. Check size and clarity
4. Close preview window when satisfied

### Printing

1. File → Print
2. Windows print dialog appears
3. Select your printer
4. Adjust print settings if needed
5. Click Print

### Print Settings

- **Paper Size**: Usually A4 or Letter
- **Orientation**: Usually Portrait
- **Quality**: Set to highest for best results
- **Color**: Black and white is fine (stamp is already B&W)

### Multiple Stamps Per Page

The PrintService supports multiple stamps per page (configured in code). This saves paper when printing many stamps.

### Print Quality Tips

1. Use highest printer quality setting
2. Ensure printer has plenty of ink/toner
3. Use good quality paper
4. Test print on regular paper first
5. Bold elements print more clearly

## Tips and Tricks

### Design Tips

1. **Start with text, then add shapes around it**
   - Easier to center and align

2. **Use odd number of elements**
   - Visually more pleasing (design principle)

3. **Keep designs simple**
   - Fewer elements = clearer stamp
   - Professional stamps are often minimalist

4. **High contrast**
   - White on black provides maximum visibility

5. **Test at actual size**
   - Use zoom to see real size
   - 100% zoom ≈ actual print size

### Workflow Tips

1. **Save frequently**
   - Use Ctrl+S often
   - Prevents losing work

2. **Use templates**
   - Start with example templates
   - Modify to suit your needs

3. **Duplicate for variations**
   - Create one good design
   - Save As with different name
   - Modify for different departments/uses

4. **Test print**
   - Always preview before final print
   - Print one copy first to verify

### Keyboard Shortcuts

Master these for faster work:

- `Ctrl+N` - New design
- `Ctrl+O` - Open
- `Ctrl+S` - Save
- `Ctrl+Z` - Undo (use liberally!)
- `Ctrl+Y` - Redo
- `Ctrl+D` - Duplicate
- `Del` - Delete
- `Ctrl+P` - Print Preview

### Common Mistakes to Avoid

1. **Text too small**
   - Minimum 16pt for legibility
   - Smaller text may not print clearly

2. **Too many elements**
   - Keep it simple
   - 3-5 elements is usually enough

3. **Not using bold**
   - Regular weight can look thin
   - Bold prints much better

4. **Forgetting to save**
   - Save before printing
   - Save before closing

5. **Not testing print**
   - Always preview first
   - Test on cheap paper

### Advanced Techniques

**Creating Circular Text Effect:**
1. Add multiple short text elements
2. Rotate each one slightly
3. Position around a circle
4. Takes patience but looks great

**Layered Look:**
1. Add shape
2. Add smaller shape inside
3. Add text in center
4. Creates professional nested design

**Signature Line:**
1. Add horizontal line
2. Add small text below: "Authorized Signature"
3. Common for official stamps

## Troubleshooting

**Can't select element:**
- Click directly on the element, not near it
- Try zooming in for easier clicking

**Text is cut off:**
- Increase the text element's Width and/or Height
- Or reduce font size

**Can't see changes:**
- Make sure element is selected (yellow border)
- Some changes require clicking elsewhere to apply

**Print looks different than screen:**
- Print preview shows exact output
- Screen rendering may vary slightly

**Undo not working:**
- Undo only works after making changes
- Can't undo file operations (open, save)

---

**Need More Help?**

Check the README.md file for additional information or open an issue on GitHub for support.
