using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    public Image TileImage; // ������� ������ �� ��������� Image
    public Image DiscardArrow;
    private Tile tileData;
    public UnityEvent<Tile> OnTileClicked;

    private void Awake()
    {
        if (OnTileClicked == null)
            OnTileClicked = new UnityEvent<Tile>();
    }

    public void SetTile(Tile tile)
    {
        tileData = tile;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        
        if (TileImage != null)
        {
            
            // ��������� ��������������� �������� ��� ���� ������
            TileImage.sprite = TileSpriteManager.GetSprite(tileData);
            TileImage.preserveAspect = true; // ��������� ���������
        }

        if (DiscardArrow != null)
            if (tileData.IsDiscardable())    
            {
                DiscardArrow.sprite = TileSpriteManager.GetDiscardArrow();
                DiscardArrow.preserveAspect = true; // ��������� ���������
            }
            else DiscardArrow.color = new Color(0, 0, 0, 0);
    }

    void OnMouseDown()
    {
        OnTileClicked.Invoke(tileData);
    }

}
