using SDL;

namespace PinkFox.Graphics.Animations;

public interface IAnimationController
{
    /// <summary>
    /// Gets or sets the key/name of the current active animation.
    /// </summary>
    string CurrentAnimationKey { get; }

    /// <summary>
    /// Gets the current animation object.
    /// </summary>
    Animation CurrentAnimation { get; }

    /// <summary>
    /// Sets the current animation key.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="reset">Whether to set the animation back to it's first frame</param>
    void SetCurrentAnimation(string key, bool reset = false);

    /// <summary>
    /// Sets the current frame index for the active animation.
    /// </summary>
    /// <param name="frameIndex">Frame index to set.</param>
    void SetCurrentFrame(int frameIndex);

    /// <summary>
    /// Advances the current frame by a specified amount (e.g., 1 for next frame).
    /// </summary>
    /// <param name="amount">Number of frames to advance.</param>
    void UpdateCurrentFrame(int amount);

    /// <summary>
    /// Gets the source rectangle of the current frame for rendering.
    /// </summary>
    SDL_FRect GetCurrentFrameRect();

    /// <summary>
    /// Adds or replaces an animation with the specified key.
    /// </summary>
    /// <param name="key">Animation key/name.</param>
    /// <param name="animation">Animation object.</param>
    void AddAnimation(string key, Animation animation);

    /// <summary>
    /// Adds or replaces multiple animations with the specified keys.
    /// </summary>
    /// <param name="animations">A KeyValuePair array containing the animations to add.</param>
    void AddMultipleAnimations(IEnumerable<KeyValuePair<string, Animation>> animations);

    /// <summary>
    /// Removes an animation by key.
    /// </summary>
    /// <param name="key">Animation key/name.</param>
    /// <returns>True if removed, false if key not found.</returns>
    bool RemoveAnimation(string key);
}
