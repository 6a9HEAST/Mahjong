using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHandView : IHandView
{
    public override void Draw(List<Tile> hand)
    {
        
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in HandContainerClone)
        {
            Destroy(child.gameObject);
        }

            // Отобразите каждую плитку в руке
        for (int i = 0; i < hand.Count; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, HandContainer);

            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
            tileView.OnTileClicked.AddListener(GameManager.HandleTileClick);
            tileView.OnTileHoverEnter.AddListener(GameManager.HandleTileHoverEnter);
            tileView.OnTileHoverExit.AddListener(GameManager.HandleTileHoverExit);
        }
    }

    public override void Draw(List<Tile> hand, int tilesToDraw)
    {
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in HandContainerClone)
        {
            Destroy(child.gameObject);
        }

        // Отобразите каждую плитку в руке
        for (int i = 0; i < tilesToDraw; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, HandContainer);
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
            tileView.OnTileClicked.AddListener(GameManager.HandleTileClick);
        }
    }


}