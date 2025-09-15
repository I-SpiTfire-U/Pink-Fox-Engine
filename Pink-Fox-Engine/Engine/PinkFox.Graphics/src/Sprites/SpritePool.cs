namespace PinkFox.Graphics.Sprites;

public class SpritePool
{
    public IReadOnlyList<ISprite2D> AllSprites => _Sprites;
    private List<ISprite2D>? _SortedCache = null;
    private readonly List<ISprite2D> _Sprites = [];

    private bool _RequiresSorting = true;

    public void RequestSort() => _RequiresSorting = true;
    public bool Contains(ISprite2D sprite) => _Sprites.Contains(sprite);

    public void Add(ISprite2D sprite)
    {
        _Sprites.Add(sprite);
        _RequiresSorting = true;
    }
    public void AddRange(params ISprite2D[] sprites)
    {
        _Sprites.AddRange(sprites);
        _RequiresSorting = true;
    }
    public void Remove(ISprite2D sprite)
    {
        _Sprites.Remove(sprite);
        _RequiresSorting = true;
    }

    public IEnumerable<ISprite2D> GetAllExcept(ISprite2D excluded)
    {
        foreach (ISprite2D sprite in _Sprites)
        {
            if (!ReferenceEquals(sprite, excluded))
            {
                yield return sprite;
            }
        }
    }

    public IEnumerable<ISprite2D> GetAllExcept(params ISprite2D[] excludedSprites)
    {
        HashSet<ISprite2D> excludedSet = [.. excludedSprites];
        foreach (ISprite2D sprite in _Sprites)
        {
            if (!excludedSet.Contains(sprite))
            {
                yield return sprite;
            }
        }
    }

    public IEnumerable<ISprite2D> GetSortedByLayer()
    {
        if (_RequiresSorting || _SortedCache is null)
        {
            _SortedCache = [.. _Sprites.OrderBy(s => s.Layer)];
            _RequiresSorting = false;
        }
        return _SortedCache;
    }

    public IEnumerable<ISprite2D> Where(Func<ISprite2D, bool> predicate)
    {
        foreach (ISprite2D sprite in _Sprites)
        {
            if (predicate(sprite))
            {
                yield return sprite;
            }
        }
    }

    public void Clear()
    {
        _Sprites.Clear();
        _RequiresSorting = true;
    }

    public void ClearAndDispose()
    {
        foreach (ISprite2D sprite in _Sprites)
        {
            if (sprite is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _Sprites.Clear();
        _RequiresSorting = true;
    }
}
