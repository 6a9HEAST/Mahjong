using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerDiscardView : MonoBehaviour
{

    [Header("References")]
    public GameObject TilePrefab;
    public Transform DiscardContainer1;
    public Transform DiscardContainer2;
    public Transform DiscardContainer3;

    [Header("Layout Spacing")]
    public float DefaultRowSpacing = 1.37f;
    public float RiichiRowSpacing = 1.61f;      // Custom spacing when riichi is in row
    public float SideContainerSpacing = 5f;    // Spacing for left/right subcontainers


    public void Draw (List<Tile> Discard)
    {

        // 1. Clear previous
        ClearContainer(DiscardContainer1);
        ClearContainer(DiscardContainer2);
        ClearContainer(DiscardContainer3);

        // 2. Partition into three rows
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

        // 3. Render each row
        for (int r = 0; r < 3; r++)
        {
            Transform container = GetContainer(r);
            float spacing = rows[r].Exists(t => t.Properties.Contains("Riichi"))
                ? RiichiRowSpacing
                : DefaultRowSpacing;

            var layout = container.GetComponent<HorizontalLayoutGroup>();
            if (layout != null) layout.spacing = spacing;

            RenderRow(rows[r], container);
        }
    }

    /// <summary>
    /// Clears all children of a container.
    /// </summary>
    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Returns the corresponding container for the given row index.
    /// </summary>
    private Transform GetContainer(int index)
    {
        switch (index)
        {
            case 0: return DiscardContainer1;
            case 1: return DiscardContainer2;
            default: return DiscardContainer3;
        }
    }

    /// <summary>
    /// Renders a single row of tiles, inserting left/right subcontainers around a Riichi tile if present.
    /// </summary>
    private void RenderRow(List<Tile> rowTiles, Transform container)
    {
        // Find Riichi position
        int riichiPos = rowTiles.FindIndex(t => t.Properties.Contains("Riichi"));
        if (riichiPos < 0)
        {
            // No special tile: just instantiate all normally
            foreach (var tile in rowTiles)
                InstantiateTile(container, tile, false);
            return;
        }

        // 1) Left group
        if (riichiPos > 0)
        {
            var leftGO = new GameObject("LeftContainer");
            leftGO.transform.SetParent(container, false);
            var leftLayout = leftGO.AddComponent<HorizontalLayoutGroup>();
            leftLayout.spacing = SideContainerSpacing;
            for (int i = 0; i < riichiPos; i++)
                InstantiateTile(leftGO.transform, rowTiles[i], false);
        }

        // 2) Riichi tile (flipped)
        InstantiateTile(container, rowTiles[riichiPos], true);

        // 3) Right group
        var rightGO = new GameObject("RightContainer");
        rightGO.transform.SetParent(container, false);
        var rightLayout = rightGO.AddComponent<HorizontalLayoutGroup>();
        rightLayout.spacing = SideContainerSpacing;
        for (int i = riichiPos + 1; i < rowTiles.Count; i++)
            InstantiateTile(rightGO.transform, rowTiles[i], false);
    }

    /// <summary>
    /// Instantiates a tile prefab under the given parent, sets its data, and flips if needed.
    /// </summary>
    private void InstantiateTile(Transform parent, Tile tile, bool flip)
    {
        var obj = Instantiate(TilePrefab, parent);
        var view = obj.GetComponent<TileView>();
        view.SetTile(tile);
        if (flip)
        {
            // Flip horizontally
            //var scale = obj.transform.localScale;
            obj.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            // Ensure normal scale
            //obj.transform.localScale = Vector3.one;
        }


    }
}
