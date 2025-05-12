using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class IDiscardView: MonoBehaviour
{

    [Header("References")]
    public GameObject TilePrefab;
    public GameObject TurnedTilePrefab;
    public Transform DiscardContainer1;
    public Transform DiscardContainer2;
    public Transform DiscardContainer3;

    [Header("Layout Spacing")]
    public float DefaultRowSpacing = 1.21f;
    public float RiichiRowSpacing = 1.61f;      // Custom spacing when riichi is in row
    public float SideContainerSpacing = 1.21f;    // Spacing for left/right subcontainers
    public bool isVertical = false;

    public void Draw(List<Tile> Discard)
    {
        ClearContainer(DiscardContainer1);
        ClearContainer(DiscardContainer2);
        ClearContainer(DiscardContainer3);

        var rows = new List<Tile>[3] {
            new List<Tile>(),
            new List<Tile>(),
            new List<Tile>()
        };
        for (int i = 0; i < Discard.Count; i++)
        {
            int rowIndex = Mathf.Min(2, i / 6);
            rows[rowIndex].Add(Discard[i]);
        }

        for (int r = 0; r < 3; r++)
        {
            Transform container = GetContainer(r);
            float spacing = rows[r].Exists(t => t.Properties.Contains("Riichi"))
                ? RiichiRowSpacing
                : DefaultRowSpacing;
            if (isVertical)
            {
                var layout = container.GetComponent<VerticalLayoutGroup>();
                if (layout != null) layout.spacing = spacing;
            }
            else
            {
                var layout = container.GetComponent<HorizontalLayoutGroup>();
                if (layout != null) layout.spacing = spacing;
            }
            

            RenderRow(rows[r], container, r);
        }
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    private Transform GetContainer(int index)
    {
        switch (index)
        {
            case 0: return DiscardContainer1;
            case 1: return DiscardContainer2;
            default: return DiscardContainer3;
        }
    }

    public abstract void RenderRow(List<Tile> rowTiles, Transform container, int row);   

    public Vector3 GetLastTilePosition(List<Tile> Discard)
    {
        if (Discard == null || Discard.Count == 0)
            return Vector3.zero;

        // Определяем индекс последней строки
        int lastRowIndex = Mathf.Min(2, (Discard.Count - 1) / 6);
        Transform lastRowContainer = GetContainer(lastRowIndex);

        // Принудительное обновление лейаута
        LayoutRebuilder.ForceRebuildLayoutImmediate(lastRowContainer as RectTransform);

        // Ищем последний тайл с учетом вложенных контейнеров
        Transform lastTile = FindLastTileRecursive(lastRowContainer);

        return lastTile != null
            ? lastTile.position
            : Vector3.zero;
    }

    private Transform FindLastTileRecursive(Transform container)
    {
        // Проверяем детей в обратном порядке
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);

            // Если это сам тайл
            if (child.GetComponent<TileView>() != null)
                return child;

            // Рекурсивный поиск во вложенных контейнерах
            Transform foundTile = FindLastTileRecursive(child);
            if (foundTile != null)
                return foundTile;
        }
        return null;
    }

}

