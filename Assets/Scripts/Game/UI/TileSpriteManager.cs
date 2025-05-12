using System.Collections.Generic;
using UnityEngine;

public static class TileSpriteManager
{
    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    // Инициализация спрайтов
    public static void Initialize()
    {

        foreach (var sprite in Resources.LoadAll<Sprite>("Tiles"))
        {

            spriteDictionary[sprite.name] = sprite;
        }

        var discardArrow = Resources.Load<Sprite>("DiscardArrow100x100"); // Без расширения [[5]]

        spriteDictionary[discardArrow.name] = discardArrow;
    }

    public static Sprite GetSprite(Tile tile)
    {
        
        if (spriteDictionary.TryGetValue(tile.ToString(), out var sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Sprite for tile {tile.ToString()} not found!");
        return null;
    }

    public static Sprite GetDiscardArrow()
    {
        if (spriteDictionary.TryGetValue("DiscardArrow100x100", out var sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Sprite for DiscardArrow100x100 not found!");
        return null;
    }
}
