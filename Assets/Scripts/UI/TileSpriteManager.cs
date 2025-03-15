using System.Collections.Generic;
using UnityEngine;

public static class TileSpriteManager
{
    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    // Инициализация спрайтов
    public static void Initialize()
    {
        //Debug.Log("Загрузка текстур");
        // Загрузите все спрайты в словарь
        foreach (var sprite in Resources.LoadAll<Sprite>("Tiles"))
        {
            //Debug.Log($"Загружен спрайт: {sprite.name}");
            spriteDictionary[sprite.name] = sprite;
        }
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
}
