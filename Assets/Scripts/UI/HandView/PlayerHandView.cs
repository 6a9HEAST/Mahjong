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
            GameObject tileObject = Instantiate(TilePrefab, HandContainer);
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
            tileView.OnTileClicked.AddListener(GameManager.HandleTileClick);
        }
    }
    
}