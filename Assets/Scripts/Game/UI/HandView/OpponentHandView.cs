using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class OpponentHandView : IHandView
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
            GameObject tileObject = Instantiate(StandingTile, HandContainer);
            var sprite = tileObject.GetComponent<SpriteRenderer>();
            sprite.sortingOrder = i;

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);

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
            GameObject tileObject = Instantiate(StandingTile, HandContainer);
            var sprite = tileObject.GetComponent<SpriteRenderer>();
            sprite.sortingOrder =i;
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
        }
    }



}