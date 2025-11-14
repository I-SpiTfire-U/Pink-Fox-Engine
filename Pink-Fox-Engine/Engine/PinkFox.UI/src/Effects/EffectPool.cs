using PinkFox.UI.Interfaces;

namespace PinkFox.UI.Effects;

public class EffectPool
{
    public Dictionary<string, IEffect> EffectDictionary { get; init; }

    public EffectPool()
    {
        EffectDictionary = [];
    }

    public void Update(float deltaTime)
    {
        foreach (KeyValuePair<string, IEffect> effect in EffectDictionary)
        {
            effect.Value.Update(deltaTime);
        }
    }

    public bool RegisterEffect(string ID, IEffect effect)
    {
        return EffectDictionary.TryAdd(ID, effect);
    }

    public bool RemoveEffect(string ID)
    {
        return EffectDictionary.Remove(ID);
    }

    public void InitializeEffect(string ID)
    {
        EffectDictionary[ID].Initialize();
    }
}