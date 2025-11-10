# Pink Fox Engine

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![GitHub views](https://komarev.com/ghpvc/?username=I-SpiTfire-U&label=Views)](https://github.com/I-SpiTfire-U)
[![Author](https://img.shields.io/badge/Author-I--SpiTfire--U-181717?logo=github)](https://github.com/I-SpiTfire-U)

> The Pink Fox Engine is a lightweight, SDL3-powered game engine written in C# for creating 2D games with minimal setup and maximum flexibility.

## Background

The Pink Fox Engine is a hobby project that I'm putting together as a way to learn more about game development and how different things work under the hood in real engines. I thought a game engine would be an intriguing project to create as a way of showcasing how far I've come but also as a tool that could actually be used realistically.

Despite the Pink Fox Engine being a hobby project, I do hope to develop it to a point that it can be used to make a fully functioning game, as I've always wanted to have my own game engine to use for my future game projects and to share with others!

## Is It Ready For Use?

Yes, but it's still evolving. The core systems are functional and you can make games with it in its current state. However, features are still being added over time, so expect changes.
I’d love to see what people create with the engine, and I welcome any feedback or criticism that comes with that.

## To-Do List

- [X] Get the base engine set up and running
- [X] Create a scene manager and a basic scene interface to use for different scene types in the future
- [X] Set up interfaces for components such as audio managers, input managers, and others
- [X] Create a few basic collision object types and a simple collision system to test for collisions between them
- [X] Create a basic texture class to use for any future graphics drawing needs and make a few functions for creating basic shape textures like rectangles and circles
- [X] Create a sprite class and an interface for it to inherit off of that other sprite types can use such as an animated sprite
- [X] Create an animation system that utilizes texture atlases
- [X] Abstract the main engine code away from the developer so they can focus on their game instead of messing with engine settings
- [X] Implement a simple camera interface and set up a basic follow camera system that just follows a point
- [X] Create an audio system with music track and sound effect classes that can be played
- [X] Create an input system that manages keyboard and mouse input, but can support gamepads and anything else later on
- [X] Add gamepad support to the input system that can handle multiple gamepads at once
- [X] Set up a basic UI text system for drawing text to the screen
- [X] Add a bitmap font system as it's more efficient and customizable than the PinkFox.UI font system.
- [X] Modify the engine to use embedded resources as opposed to loading external files.
- [X] Add more functionality to the text UI system and optimize it.
- [X] Make the animation system more user friendly.
- [X] Multi-Window support.
- [ ] Multi-Threading for improved performance with multiple windows.
- [X] Add support for loading non-embedded resources if needed.
- [ ] Implement simple UI element interfaces to allow for flexible UI creation and implement a couple of basic types such as labels, buttons, and checkboxes.
- [ ] Implement a basic particle system.
- [ ] Create a few basic sample projects to showcase different features.
- [ ] Add more quality of life features to specific parts of the engine to make the developer experience more enjoyable.
- [ ] Optimize more where possible.
- [ ] Add documentation comments.
- [ ] Improve and optimize input system more.
- [X] Add built-in functionality for converting font files to bitmap fonts so that developers don't have to use external tools
to do so.
- [ ] Add SAT collision support on top of the basic collision system that already exists.
- [ ] Implement more debugging/crash report features including debug messages that draw to the window.
- [ ] Separate module concerns better and make specific sections of the engine more modular.
- [ ] Get an actual logo created for the engine.

## Creating and Running a Game

### Option 1: Without Template

**1.** Clone the Repository

```sh
git clone https://github.com/I-SpiTfire-U/PinkFox.git
cd PinkFox
```

**2.** Create a Game Project

```sh
dotnet new console -n BasicGame
cd BasicGame
dotnet add reference ../PinkFox-Fox-Engine/Engine/PinkFox.Core/PinkFox.Core.csproj
dotnet package add ppy.SDL3-CS
mkdir Assets
```

**3.** Set Up Your Program.cs

```cs
using PinkFox.Core;
using PinkFox.Core.Types;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        const int WindowWidth = 1600;
        const int WindowHeight = 900;
        const string WindowTitle = "PinkFox Game Template";

        using Engine engine = new();
        engine.Initialize();

        ResourceManager.LoadResources();

        Window mainWindow = Window.Create(WindowWidth, WindowHeight, WindowTitle, null, 0, null);
        mainWindow.Scenes.RegisterScene("MainScene", new Scene(mainWindow));
        mainWindow.Scenes.PushScene("MainScene");

        engine.AddWindow(mainWindow);

        engine.FPSIsLimited = true;
        engine.SetTargetFPS(60);
        engine.SetFixedUPS(60);

        engine.Run();
    }
}
```

**4.** Create a Basic Scene

```cs
using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using SDL;

namespace BasicGame;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Window Window;
    protected Renderer Renderer => Window.Renderer!;
    
    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:

    }

    public void Render(float deltaTime)
    {
        // TODO: Draw game elements to the screen below:

    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }
        
        if (disposing) { }

        _Disposed = true;
    }
}
```

**5.** Build and Run

```sh
dotnet build
dotnet run
```

**6.** Publish a Standalone Binary

Select the runtime ID for whichever platform you are building for, a list of available runtime ID's can be found here: [RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

```sh
dotnet publish -c Release -r <RID> --self-contained
```

### Option 2: With Template

**1.** Clone the Repository

```sh
git clone https://github.com/I-SpiTfire-U/PinkFox.git
cd PinkFox
```

**2.** Install the Template

```sh
dotnet new install ../PinkFox-Fox-Engine/Templates/PinkFox.GameTemplate/PinkFox.GameTemplate.csproj
```

**3.** Create a Game Project

```sh
dotnet new pinkfox-game -n BasicGame
```

**4.** Build and Run

```sh
dotnet build
dotnet run
```

**5.** Publish a Standalone Binary

Select the runtime ID for whichever platform you are building for, a list of available runtime ID's can be found here: [RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

```sh
dotnet publish -c Release -r <RID> --self-contained
```
