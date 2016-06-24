# ConversionTool

How to setup the Conversion Tool:

1. Create a Winform Project in Visual Studio.
2. Add this project to your Windows Form Application solution.
3. Insert the line of code "ParseDesign.Input(this);" after the "InitializeComponent();" in the form your designing.
4. "Start" the application and the Neoforce Control file for your game is created in the directory you designate per the "OutputDirectory".

There is an option in the Design Parser now to generate the code behind for events.

Ideal usage: The user will work on the layout of the window in the Winform designer and you can double click on buttons to generate an event. When the generate code behind is enabled, it will generate a new class in a sub directory (.\CodeBehind) where the windows are stored for the event calls. If you need to come back to designing the window you would turn the code behind feature off so that the event logic portion is not overwritten.


###Note: there are a few variable that may need to be changed for your project:

ManagerName --- Used for the manager name variable in the code.

WindowName --- Used for the root or main window, I use a base class for each window and the base window name is "MainWindow".

NameSpace --- This is the namespace of your project / folder where the windows are located for the Neoforce Controls.


Example output:
![alt tag](https://snag.gy/KcohOP.jpg)


![alt tag](https://snag.gy/5SdVoa.jpg)
