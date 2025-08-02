using SDL3;

namespace PinkFox.Graphics.Rendering;

public class Animation
{
    public int CurrentFrame { get; private set; }
    private readonly int _NumberOfFrames;
    private readonly int _FrameWidth;
    private readonly int _FrameHeight;

    public Animation(int frameWidth, int frameHeight, int numberOfFrames, int currentFrame = 0)
    {
        _FrameWidth = frameWidth;
        _FrameHeight = frameHeight;
        _NumberOfFrames = numberOfFrames;
        CurrentFrame = currentFrame;
    }

    public SDL.FRect GetCurrentFrame()
    {
        return new SDL.FRect()
        {
            X = _FrameWidth * CurrentFrame,
            Y = 0,
            W = _FrameWidth,
            H = _FrameHeight
        };
    }

    public void IncrementCurrentFrame()
    {
        CurrentFrame = CurrentFrame >= _NumberOfFrames ? 0 : CurrentFrame + 1;
    }

    public void DecrementCurrentFrame()
    {
        CurrentFrame = CurrentFrame > 0 ? CurrentFrame - 1 : _NumberOfFrames - 1;
    }

    public void SetCurrentFrame(int index)
    {
        if (index > 0 || index < _NumberOfFrames)
        {
            CurrentFrame = index;
        }
        Console.WriteLine($"Animation frame {index} does not exist");
    }
}