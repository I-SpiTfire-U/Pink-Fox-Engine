using SDL;

namespace PinkFox.Graphics.Animations;

public class AnimationController : IAnimationController
{
    public string CurrentAnimationKey { get; private set; }
    public Animation CurrentAnimation => _Animations[CurrentAnimationKey];

    private readonly Dictionary<string, Animation> _Animations;

    public AnimationController(string initialAnimationKey, Dictionary<string, Animation> animations)
    {
        _Animations = animations ?? [];

        if (!_Animations.ContainsKey(initialAnimationKey))
        {
            throw new KeyNotFoundException($"Initial animation key '{initialAnimationKey}' not found.");
        }
        CurrentAnimationKey = initialAnimationKey;
    }

    public void SetCurrentAnimation(string key, bool reset = false)
    {
        if (!_Animations.ContainsKey(key))
        {
            return;
        }
        
        CurrentAnimationKey = key;
        if (reset)
        {
            CurrentAnimation.SetCurrentFrame(0);
        }
    }

    public void SetCurrentFrame(int frameIndex)
    {
        CurrentAnimation.SetCurrentFrame(frameIndex);
    }

    public void UpdateCurrentFrame(int amount)
    {
        CurrentAnimation.UpdateCurrentFrame(amount);
    }

    public SDL_FRect GetCurrentFrameRect()
    {
        return CurrentAnimation.GetCurrentFrameRect();
    }

    public void AddAnimation(string key, Animation animation)
    {
        if (_Animations.ContainsKey(key))
        {
            return;
        }
        _Animations.Add(key, animation);
    }

    public void AddMultipleAnimations(IEnumerable<KeyValuePair<string, Animation>> animations)
    {
        foreach (KeyValuePair<string, Animation> animation in animations)
        {
            AddAnimation(animation.Key, animation.Value);
        }
    }

    public bool RemoveAnimation(string key)
    {
        return _Animations.Remove(key);
    }
}