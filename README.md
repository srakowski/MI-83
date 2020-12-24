![MI-83 Logo](./Images/mi_83_logo.png)

# MI-83 Fantasy Game Console

Takes inspiration from the TI-83 Calculator and combines it with features from other fantasy consoles and game systems.

Initial prototyping done with [FC-360](https://github.com/srakowski/FC360). Process live streamed on YouTube at [The Code Mage](https://www.youtube.com/channel/UCwRuD5EmUMu-JAi_AW5jJLw).

## Supported Platforms
Windows

## Display

#### 2 Modes
1. Home Screen (Console/Text)
1. Graphics Screen

#### Resolution Options, Graphics|Home
1. 192x128 pixels, 32x16 characters (Default)
1. 96x64 pixels, 16x8 characters
1. 288x192 pixels, 48x24 characters

### Palette

16 Colors based loosely on the colors of PICO-8.

1. 0x00, 0x00, 0x00 // 0 Black (Transparent)
1. 0x36, 0x36, 0x96 // 1 Dark-Blue
1. 0x57, 0x1E, 0x57 // 2 Dark-Purple
1. 0x3F, 0x5E, 0x3F // 3 Dark-Green
1. 0x7C, 0x5E, 0x40 // 4 Brown
1. 0x46, 0x46, 0x40 // 5 Dark-Gray
1. 0x9E, 0x9E, 0x91 // 6 Light-Gray
1. 0xDD, 0xDD, 0xCC // 7 White
1. 0xB4, 0x4A, 0x4A // 8 Red
1. 0xD6, 0x68, 0x20 // 9 Orange
1. 0xDD, 0xD2, 0x11 // 10 Yellow
1. 0x7D, 0xCC, 0x7C // 11 Green
1. 0x54, 0x54, 0xCC // 12 Blue
1. 0x8F, 0x67, 0xAA // 13 lavender
1. 0xC9, 0x5C, 0xC9 // 14 Pink
1. 0x5F, 0xC4, 0xA7 // 15 Peach Puff

## Programming:
Python (IronPython 2.7)

### Systems Commands

|Command|Description|
|-------|-----------|
|GetPrgms: [prgmName]|Returns a list of programs.|
|CreatePrgm(prgmName)|Creates an empty program.|
|EditPrgm(prgmName)|Runs the program editor.|
|RunPrgm(prgmName)|Runs the specified program.|
|GetKey(): keycode|Returns the code of the last key pressed.|
|GetSuppDispRes(): [dispRes]|Return a list of supported  resolutions.|
|GetDispRes(dispResIdx)|Gets the index of the current resolution of the display.|
|SetDispRes(dispResIdx)|Sets the resolution of the display.|

### Home Screen Commands 

|Command|Description|
|-------|-----------|
|ClrHome()|Clears the home screen.|
|Output(row, col, text)|Outputs text value at x/y coordinates.|
|Disp(text)|Displays a line of text.|
|DispGraph|Switches to Graphics Screen.|
|Input(prompt): value|Reads a line of text.|
|Pause()|Waits for the user to press the [Enter] key.|
|Menu([(tab_name, [menu_opt_1, ...]), ...]): (tab_idx, opt_idx)|Displays a menu.|
|SetFG(palette_idx)|Sets foregroud color from palette. Takes effect on future commands.|
|SetBG(palette_idx)|Sets foregroud color from palette. Takes effect on future commands.|

### Graphics Screen Commands

|Command|Description|
|-------|-----------|
|ClrDraw(palette_idx)|Clears the Graphics Screen.|
|StorePic(buf_idx)|Takes a 'picture' of the current screen and stores to buffer index.|
|RecallPic(buf_idx)|Draws the picture at the buffer index.|
|Stroke(palette_idx)|Sets the color to use for drawing.|
|Fill(palette_idx)|Sets the color to fill shapes when drawing.|
|Pixel(x, y)|Changes pixel at x/y coordinates to color at palete idx.|
|Line(x1, y1, x2, y2)|Draws a line between points.|
|Horizontal(y)|Draws a horizontal line along the y coordinate.|
|Vertical(x)|Draws a vertical line along the x coordinate.|
|Circle(x, y, r)|Draws a circle centered on x/y coordinate with the given radius.|
|Rectangle(x1, y1, x2, y2)|Draws a rectangle from x1/y1 coordinates to x2/y2.|
|Sprite(sprite_idx, x, y)|Draw a sprite from the sprite buffer to the screen at x/y coordinates.|
