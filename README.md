# Pink Fox Game Engine

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![GitHub views](https://komarev.com/ghpvc/?username=I-SpiTfire-U&label=Views)](https://github.com/I-SpiTfire-U)
[![Author](https://img.shields.io/badge/Author-I--SpiTfire--U-181717?logo=github)](https://github.com/I-SpiTfire-U)

> A simple lightweight game engine built with C# and SDL3.

## Background

The Pink Fox Engine is a hobby project that I'm putting together as a way to learn more about game development and how different things work under the hood in real engines. I thought a game engine would be an intriguing project to create as a way of showcasing my ability to code but also as a tool that could actually be used realistically.

Despite the Pink Fox Engine being a hobby project, I do hope to develop it to a point that it can be used to make a fully functioning game, as I've always wanted to have my own game engine to use for my future game projects and to share with others!

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
- [ ] Add support for loading external files if needed.
- [ ] Implement simple UI element interfaces to allow for flexible UI creation and implement a couple of basic types such as labels, buttons, and checkboxes
- [ ] Implement a basic particle system
- [ ] Create a few basic sample projects to showcase different features
- [ ] Add more quality of life features to specific parts of the engine to make the developer experience more enjoyable

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
mkdir Assets
```

**3.** Set Up Your Program.cs

```cs
public class Program
{
    public static void Main()
    {
        const int InitialWindowWidth = 800;
        const int InitialWindowHeight = 600;

        ResourceManager.LoadResources();

        using Engine engine = new();
        engine.InitializeWindowAndRenderer("My Game", null, InitialWindowWidth, InitialWindowHeight);

        // engine.SetInputManager(new PinkFox.Input.InputManager());
        // engine.SetAudioManager(new PinkFox.Audio.AudioManager());
        // engine.SetVirtualRenderer(new PinkFox.Graphics.VirtualRenderer());

        engine.SetRenderClearColor(100, 149, 237);

        SceneManager.RegisterScene("Main Scene", new Scene(engine));
        SceneManager.PushScene("Main Scene");

        engine.Run();
    }
}
```

**4.** Create a Basic Scene

```cs
public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Engine Engine;
    
    private bool _Disposed;

    public unsafe Scene(Engine engine)
    {
        Engine = engine;
    }

    public void LoadContent() { }

    public void Update(float deltaTime) { }

    public void FixedUpdate(float fixedUpdateInterval) { }

    public unsafe void Draw(SDL_Renderer* renderer) { }

    public void OnWindowResize(int windowWidth, int windowHeight) { }

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
