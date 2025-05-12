using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;


public class TopDiscardView : IDiscardView
{
    public override void RenderRow(List<Tile> rowTiles, Transform container, int row)
    {
        // Find Riichi position
        int riichiPos = rowTiles.FindIndex(t => t.Properties.Contains("Riichi"));
        if (riichiPos < 0)
        {
            foreach (var tile in rowTiles)
                InstantiateTile(container, tile, false, row);
            return;
        }

        if (riichiPos > 0)
        {
            var leftGO = new GameObject("LeftContainer");
            leftGO.transform.SetParent(container, false);
            var leftLayout = leftGO.AddComponent<HorizontalLayoutGroup>();
            leftLayout.childForceExpandHeight = false;
            leftLayout.childForceExpandWidth = false;
            leftLayout.spacing = SideContainerSpacing;
            for (int i = 0; i < riichiPos; i++)
                InstantiateTile(leftGO.transform, rowTiles[i], false, row);
        }


        InstantiateTile(container, rowTiles[riichiPos], true, row);


        var rightGO = new GameObject("RightContainer");
        rightGO.transform.SetParent(container, false);
        var rightLayout = rightGO.AddComponent<HorizontalLayoutGroup>();
        rightLayout.childForceExpandHeight = false;
        rightLayout.childForceExpandWidth = false;
        rightLayout.spacing = SideContainerSpacing;
        for (int i = riichiPos + 1; i < rowTiles.Count; i++)
            InstantiateTile(rightGO.transform, rowTiles[i], false, row);
    }
    public void InstantiateTile(Transform parent, Tile tile, bool flip, int row)
    {
        // Choose the appropriate prefab
        GameObject prefabToUse = flip ? TurnedTilePrefab : TilePrefab;

        // Instantiate the selected prefab
        var obj = Instantiate(prefabToUse, parent);

        // Set sorting order based on row
        var sprite = obj.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 8 - row;

        // Initialize tile view
        var view = obj.GetComponent<TileView>();
        view.SetTile(tile);

        // No manual rotation needed for TurnedtilePrefab
    }

}

