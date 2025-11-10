using System.Numerics;
using PinkFox.Core.Modules.Input;
using PinkFox.Core.Types;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Sample;

public class Sprite
{
    protected Texture2D Texture;
    protected Vector2 Position;
    protected float Angle;

    public Sprite(Texture2D texture, Vector2 position, float angle = 0f)
    {
        Texture = texture;
        Position = position;
        Angle = angle;
    }
    
    public void Update(float deltaTime, IKeyboard keyboard)
    {
        float moveSpeed = 300f;
        Angle += 200f * deltaTime;

        Vector2 positionUpdate = Vector2.Zero;

        if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_LEFT))
        {
            positionUpdate += new Vector2(-moveSpeed, 0);
        }
        else if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_RIGHT))
        {
            positionUpdate += new Vector2(moveSpeed, 0);
        }

        if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_UP))
        {
            positionUpdate += new Vector2(0, -moveSpeed);
        }
        else if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_DOWN))
        {
            positionUpdate += new Vector2(0, moveSpeed);
        }

        Position += positionUpdate * deltaTime;
    }

    public void Render(Renderer renderer)
    {
        Texture.Draw(
            renderer,
            new FRect(0, 0, Texture.Width, Texture.Height),
            new FRect(Position.X, Position.Y, Texture.Width, Texture.Height),
            Angle
        );
    }
}