# MI-83 Fantasy Game Console

![MI-83 Logo](./Images/mi_83_logo.png)

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
1. 384x256 pixels, 64x32 characters

### Palette
1. MaskColor??
1. Black
1. White
1. ...16 Colors Total (TBD)

## Programming:
Python (IronPython 2.7)

### Program Commands

|Command|Description|
|-------|-----------|
|GetPrgms||
|CreatePrgm||
|ReadPrgm||
|RunPrgm||

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
|GetKey(): keycode|Waits for a key to be pressed and returns code.|
|SetFG(palette_idx)|Sets foregroud color from palette. Takes effect on future commands.|
|SetBG(palette_idx)|Sets foregroud color from palette. Takes effect on future commands.|

### Graphics Screen Commands

|Command|Description|
|-------|-----------|
|ClrDraw(palette_idx)|Clears the Graphics Screen.|
|StorePic(buf_idx)|Takes a 'picture' of the current screen and stores to buffer index.|
|RecallPic(buf_idx)|Draws the picture at the buffer index.|
|Sprite(sprite_idx, x, y)|Draw a sprite from the sprite buffer to the screen at x/y coordinates.|
|Stroke(palette_idx)|Sets the color to use for drawing.|
|Fill(palette_idx)|Sets the color to fill shapes when drawing.|
|Pixel(x, y)|Changes pixel at x/y coordinates to color at palete idx.|
|Line(x1, y1, x2, y2)|Draws a line between points.|
|Horizontal(y)|Draws a horizontal line along the y coordinate.|
|Vertical(x)|Draws a vertical line along the x coordinate.|
|Circle(x, y, r)|Draws a circle centered on x/y coordinate with the given radius.|
|Rectangle(x1, y1, x2, y2)|Draws a rectangle from x1/y1 coordinates to x2/y2.|









