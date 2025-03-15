using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDiscardView : MonoBehaviour
{

    public GameObject TilePrefab; // Префаб плитки
    public Transform DiscardContainer1; // 1 ряд дискарда
    public Transform DiscardContainer2; // 2 ряд дискарда
    public Transform DiscardContainer3; // 3 ряд дискарда


    public void Draw (List<Tile> Discard)
    {
        foreach (Transform child in DiscardContainer1)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in DiscardContainer2)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in DiscardContainer3)
        {
            Destroy(child.gameObject);
        }
        int count = 0;
        foreach (var tile in Discard) 
        {
            GameObject tileObject = (count / 6) switch // на основе того сколько тайлов сброшено, выбирает ряд для сброса
            {
                0 => Instantiate(TilePrefab, DiscardContainer1), // <6 => первый
                1 => Instantiate(TilePrefab, DiscardContainer2), // <12 => второй
                2 => Instantiate(TilePrefab, DiscardContainer3), // >12 => третий
                _ => Instantiate(TilePrefab, DiscardContainer3),
            };
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(tile);
            count++; 
        }


    }
}
