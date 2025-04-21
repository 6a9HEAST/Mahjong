using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DoraIndicatorView:MonoBehaviour
{
    public GameObject TileFront;
    public GameObject TileBack;
    public Transform DoraContainer;

    public void Draw(List<Tile> doraIndicator,int doras)
    {
        foreach (Transform child in DoraContainer)
        {
            Destroy(child.gameObject);
        }
        int dorasdrawn = 0;
        
        for (int i = 0; i < doraIndicator.Count; i++)
        {
            GameObject tileObject;
            if ( dorasdrawn<doras) tileObject = Instantiate(TileFront, DoraContainer);
            else tileObject = Instantiate(TileBack, DoraContainer);
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(doraIndicator[i]);
            dorasdrawn++;
        }
    }
}