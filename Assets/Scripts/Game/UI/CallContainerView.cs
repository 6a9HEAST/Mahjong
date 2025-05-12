using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerPosition { Bottom, Right, Top, Left }

public class CallContainerView : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject TilePrefab;
    public GameObject TurnedTilePrefab;
    public GameObject TileBackPrefab;

    [Header("Layout Settings")]
    public float threeTilesSpacing = 1.63f;
    public float pairSpacing = 1.31f;
    public float kanSpacing = 1.33f;

    [Header("Sorting Order Settings")]
    public int startOrder = 5;
    public int orderMultiplier = 1;

    [Header("Player Settings")]
    public PlayerPosition playerPosition = PlayerPosition.Bottom;  // задаётся в инспекторе

    [Header("Container")]
    public Transform CallContainer;

    // Глобальный счётчик для sortingOrder
    private int currentOrder;

    public void Clear()
    {
        foreach (Transform c in CallContainer)
            Destroy(c.gameObject);
    }

    GameObject CreateTile(Tile tile)
    {
        // Выбираем префаб по наличию поворота
        return Instantiate(
            tile.Properties?.Contains("Called") == true
                ? TurnedTilePrefab
                : TilePrefab
        );
    }

    void AddTileToGroup(RectTransform parent, Tile tile)
    {
        var go = CreateTile(tile);
        go.transform.SetParent(parent, false);

        // Настраиваем sortingOrder единым счётчиком
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = startOrder + currentOrder * orderMultiplier;
            currentOrder++;
        }

        // Заполняем данные
        if (go.TryGetComponent<TileView>(out var tv))
            tv.SetTile(tile);
    }

    RectTransform CreateGroup(PlayerPosition pos, bool isPairGroup = false)
    {
        var go = new GameObject(isPairGroup ? "PairGroup" : "Group", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(CallContainer, false);

        if (pos.IsSide())
        {
            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = isPairGroup ? pairSpacing : threeTilesSpacing;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childAlignment = TextAnchor.MiddleCenter;
        }
        else
        {
            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = isPairGroup ? pairSpacing : threeTilesSpacing;
            hlg.childForceExpandHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childAlignment = TextAnchor.MiddleCenter;
        }

        return rt;
    }

    public void DrawCalls(List<List<Tile>> calls)
    {
        Clear();
        currentOrder = 0; // сброс перед началом отрисовки
        foreach (var call in calls)
        {
            if (call.Count == 4)
                DrawKan(call);
            else
                DrawTiles(call);
        }
    }

    void DrawTiles(List<Tile> call)
    {
        // Один корневой контейнер
        var root = CreateGroup(playerPosition);

        int idx = 0;
        while (idx < call.Count)
        {
            bool curNR = !(call[idx].Properties?.Contains("Called") == true);
            bool nextNR = idx < call.Count - 1 && !(call[idx + 1].Properties?.Contains("Called") == true);

            if (curNR && nextNR)
            {
                // Вложенная пара
                var pair = CreateGroup(playerPosition, true);
                pair.SetParent(root, false);

                AddTileToGroup(pair, call[idx]);
                AddTileToGroup(pair, call[idx + 1]);
                idx += 2;
            }
            else
            {
                AddTileToGroup(root, call[idx]);
                idx += 1;
            }
        }
    }

    void DrawKan(List<Tile> kan)
    {
        var root = CreateGroup(playerPosition);
        // Устанавливаем кастомный spacing для корневой группы Kan
        if (playerPosition.IsSide())
        {
            var vlg = root.GetComponent<VerticalLayoutGroup>();
            if (vlg != null) vlg.spacing = kanSpacing;
        }
        else
        {
            var hlg = root.GetComponent<HorizontalLayoutGroup>();
            if (hlg != null) hlg.spacing = kanSpacing;
        }

        var calledIndices = new List<int>();
        for (int i = 0; i < kan.Count; i++)
            if (kan[i].Properties?.Contains("Called") == true)
                calledIndices.Add(i);

        if (calledIndices.Count == 1)
        {
            int ci = calledIndices[0];
            if (ci == 0)
                AddTileToGroup(root, kan[ci]);

            var trio = CreateGroup(playerPosition);
            trio.SetParent(root, false);
            // Устанавливаем spacing для группы троих
            if (playerPosition.IsSide())
            {
                var vlg = trio.GetComponent<VerticalLayoutGroup>(); if (vlg != null) vlg.spacing = kanSpacing;
            }
            else
            {
                var hlg = trio.GetComponent<HorizontalLayoutGroup>(); if (hlg != null) hlg.spacing = kanSpacing;
            }
            for (int i = 0; i < kan.Count; i++)
                if (i != ci)
                    AddTileToGroup(trio, kan[i]);

            if (ci > 0)
                AddTileToGroup(root, kan[ci]);
        }
        else if (calledIndices.Count == 2)
        {
            int start = calledIndices[0], end = calledIndices[1];

            if (start > 0)
            {
                var left = CreateGroup(playerPosition);
                left.SetParent(root, false);
                if (playerPosition.IsSide())
                {
                    var vlg = left.GetComponent<VerticalLayoutGroup>(); if (vlg != null) vlg.spacing = kanSpacing;
                }
                else
                {
                    var hlg = left.GetComponent<HorizontalLayoutGroup>(); if (hlg != null) hlg.spacing = kanSpacing;
                }
                for (int i = 0; i < start; i++)
                    AddTileToGroup(left, kan[i]);
            }

            var center = CreateGroup(playerPosition);
            center.SetParent(root, false);
            if (playerPosition.IsSide())
            {
                var vlg = center.GetComponent<VerticalLayoutGroup>(); if (vlg != null) vlg.spacing = kanSpacing;
            }
            else
            {
                var hlg = center.GetComponent<HorizontalLayoutGroup>(); if (hlg != null) hlg.spacing = kanSpacing;
            }
            for (int i = start; i <= end; i++)
                AddTileToGroup(center, kan[i]);

            if (end < kan.Count - 1)
            {
                var right = CreateGroup(playerPosition);
                right.SetParent(root, false);
                if (playerPosition.IsSide())
                {
                    var vlg = right.GetComponent<VerticalLayoutGroup>(); if (vlg != null) vlg.spacing = kanSpacing;
                }
                else
                {
                    var hlg = right.GetComponent<HorizontalLayoutGroup>(); if (hlg != null) hlg.spacing = kanSpacing;
                }
                for (int i = end + 1; i < kan.Count; i++)
                    AddTileToGroup(right, kan[i]);
            }
        }
        else if (calledIndices.Count > 2)
        {
            for (int i = 0; i < kan.Count; i++)
            {
                GameObject prefab = (i == 0 || i == kan.Count - 1)
                    ? TileBackPrefab
                    : TilePrefab;
                var go = Instantiate(prefab, root, false);
                if (go.TryGetComponent<TileView>(out var tv))
                    tv.SetTile(kan[i]);

                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = startOrder + currentOrder * orderMultiplier;
                    currentOrder++;
                }
            }
        }
        else
        {
            foreach (var tile in kan)
                AddTileToGroup(root, tile);
        }
    }
}

public static class PlayerPositionExtensions
{
    public static bool IsSide(this PlayerPosition pos)
    {
        return pos == PlayerPosition.Left || pos == PlayerPosition.Right;
    }
}
