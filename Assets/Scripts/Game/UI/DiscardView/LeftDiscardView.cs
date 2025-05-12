using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;


public class LeftDiscardView : IDiscardView
{
    public override void RenderRow(List<Tile> rowTiles, Transform container, int row)
    {
        // Find Riichi position
        int riichiPos = rowTiles.FindIndex(t => t.Properties.Contains("Riichi"));
        if (riichiPos < 0)
        {
            for (int i = 0; i < rowTiles.Count; i++)
                InstantiateTile(container, rowTiles[i], false, i);
            return;
        }

        if (riichiPos > 0)
        {
            var leftGO = new GameObject("LeftContainer");
            leftGO.transform.SetParent(container, false);
            var leftLayout = leftGO.AddComponent<VerticalLayoutGroup>();
            leftLayout.childForceExpandHeight = false;
            leftLayout.childForceExpandWidth = false;
            leftLayout.reverseArrangement = true;
            leftLayout.spacing = SideContainerSpacing;
            for (int i = 0; i < riichiPos; i++)
                InstantiateTile(leftGO.transform, rowTiles[i], false, i);
        }


        InstantiateTile(container, rowTiles[riichiPos], true, riichiPos);


        var rightGO = new GameObject("RightContainer");
        rightGO.transform.SetParent(container, false);
        var rightLayout = rightGO.AddComponent<VerticalLayoutGroup>();
        rightLayout.childForceExpandHeight = false;
        rightLayout.childForceExpandWidth = false;
        rightLayout.reverseArrangement = true;
        rightLayout.spacing = SideContainerSpacing;
        for (int i = riichiPos + 1; i < rowTiles.Count; i++)
            InstantiateTile(rightGO.transform, rowTiles[i], false, i);
    }
    public void InstantiateTile(Transform parent, Tile tile, bool flip, int tile_num)
    {
        GameObject prefabToUse = flip ? TurnedTilePrefab : TilePrefab;

        // Instantiate the selected prefab
        var obj = Instantiate(prefabToUse, parent);

        // Set sorting order based on row
        var sprite = obj.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 5 + tile_num;

        // Initialize tile view
        var view = obj.GetComponent<TileView>();
        view.SetTile(tile);
    }
}

