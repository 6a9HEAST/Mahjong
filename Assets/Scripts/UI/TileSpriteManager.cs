using System.Collections.Generic;
using UnityEngine;

public static class TileSpriteManager
{
    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    // ������������� ��������
    public static void Initialize()
    {
        //Debug.Log("�������� �������");
        // ��������� ��� ������� � �������
        foreach (var sprite in Resources.LoadAll<Sprite>("Tiles"))
        {
            //Debug.Log($"�������� ������: {sprite.name}");
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
