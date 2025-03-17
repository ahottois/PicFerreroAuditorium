# Ferrero Auditorium Picture Resizer

An application that allows you to easily duplicate and resize images. 
Perfect for creating sequences of the same image side-by-side for auditorium presentations, event backdrops, or any other scenario where you need multiple copies of an image arranged horizontally.

## Features

- **Load any image** - Supports common formats including JPG, PNG, GIF, and BMP
- **Create multiple copies** - Place the same image side-by-side as many times as needed
- **Custom sizing** - Specify exact height and width for each image copy
- **Maintain aspect ratio** - Automatically adjust dimensions to preserve the original image proportions
- **Zoom options** - Choose between preserving the image proportions or stretching to fit
- **Save in multiple formats** - Export your final image as JPG, PNG, or BMP
- **Settings persistence** - Application remembers your preferences between sessions

## How It Works

1. **Load an image** - Click the "Load Picture" button to select an image from your computer
2. **Set number of copies** - Use the numeric control to specify how many copies to place side-by-side
3. **Adjust dimensions** - Enter the desired height and width for each copy of the image
4. **Configure options** - Choose whether to maintain aspect ratio and/or zoom the images
5. **Save the result** - Click "Save Picture" to export the final composite image

## Installation

### Option 1: Download the Release
1. Go to the [Releases page](https://github.com/ahottois/FerreroPictureResizer/releases)
2. Download the latest release ZIP file
3. Extract the files to your preferred location
4. Run setup.exe

### Option 2: Build from Source
1. Clone this repository:
   ```
   git clone https://github.com/YourUsername/FerreroPictureResizer.git
   ```
2. Open the solution in Visual Studio 2022
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## System Requirements

- Windows 7/8/10/11
- .NET Framework 4.7.2 or higher
- 50MB of available disk space

## Settings Storage

The application automatically saves your preferences in:
```
%localappdata%\Ferrero\hottois\picture\settings.xml
```

This includes:
- Last used directory
- Number of copies
- Image dimensions
- Aspect ratio and zoom preferences

## Credits

- Developed by: Lalo
- Special thanks to: Ferrero

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request
