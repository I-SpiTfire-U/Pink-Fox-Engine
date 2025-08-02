using SDL3;

namespace PinkFox.Graphics.Rendering;

public class Animation
{
    public int GetCurrentFrame => _CurrentFrame;
    private int _CurrentFrame;
    private readonly int _NumberOfFrames;
    private readonly int _FrameWidth;
    private readonly int _FrameHeight;

    public Animation(int frameWidth, int frameHeight, int numberOfFrames, int currentFrame = 0)
    {
        _FrameWidth = frameWidth;
        _FrameHeight = frameHeight;
        _NumberOfFrames = numberOfFrames;
        _CurrentFrame = currentFrame;
    }

    public SDL.FRect GetCurrentFrameRect()
    {
        return new SDL.FRect()
        {
            X = _FrameWidth * _CurrentFrame,
            Y = 0,
            W = _FrameWidth,
            H = _FrameHeight
        };
    }

    public void SetCurrentFrame(int index)
    {
        if (index > 0 || index < _NumberOfFrames)
        {
            _CurrentFrame = index;
        }
        Console.WriteLine($"Animation frame {index} does not exist");
    }

    public void UpdateCurrentFrame(int amount)
    {
        _CurrentFrame += amount;

        if (_CurrentFrame < 0)
        {
            _CurrentFrame = _NumberOfFrames - 1;
        }
        if (_CurrentFrame >= _NumberOfFrames)
        {
            _CurrentFrame = 0;
        }
    }
}