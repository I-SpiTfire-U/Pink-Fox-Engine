using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Graphics.Sprites;

public class Animation
{
    public int GetCurrentFrame => _CurrentFrame;
    private int _CurrentFrame;

    private readonly int _NumberOfFrames;

    private readonly int _FramesPerColumn;
    private readonly int _FramesPerRow;
    private readonly float _FrameWidth;
    private readonly float _FrameHeight;

    public Animation(float frameWidth, float frameHeight, int numberOfFrames, int framesPerRow, int framesPerColumn, int currentFrame = 0)
    {
        _FrameWidth = frameWidth;
        _FrameHeight = frameHeight;
        _NumberOfFrames = numberOfFrames;
        _FramesPerRow = framesPerRow;
        _FramesPerColumn = framesPerColumn;
        _CurrentFrame = currentFrame;
    }

    public Animation(float frameWidth, float frameHeight, Texture2D texture, int currentFrame = 0)
    {
        _FrameWidth = frameWidth;
        _FrameHeight = frameHeight;

        _FramesPerRow = (int)(texture.Width / frameWidth);
        _FramesPerColumn = (int)(texture.Height / frameHeight);

        _NumberOfFrames = _FramesPerRow * _FramesPerColumn;
        _CurrentFrame = currentFrame;
    }

    public SDL_FRect GetCurrentFrameRect()
    {
        int row = _CurrentFrame / _FramesPerRow;
        int column = _CurrentFrame % _FramesPerRow;

        return new SDL_FRect()
        {
            x = column * _FrameWidth,
            y = row * _FrameHeight,
            w = _FrameWidth,
            h = _FrameHeight
        };
    }

    public void SetCurrentFrame(int index)
    {
        if (index < 0 || index >= _NumberOfFrames)
        {
            Console.WriteLine($"Animation frame {index} does not exist");
            return;
        }
        _CurrentFrame = index;
    }

    public void UpdateCurrentFrame(int amount)
    {
        _CurrentFrame += amount;

        if (_CurrentFrame < 0)
        {
            _CurrentFrame = _FramesPerColumn - 1;
        }
        if (_CurrentFrame >= _NumberOfFrames)
        {
            _CurrentFrame = 0;
        }
    }
}