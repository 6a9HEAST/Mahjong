using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDiscardView : MonoBehaviour
{

    public GameObject TilePrefab; // ������ ������
    public Transform DiscardContainer1; // 1 ��� ��������
    public Transform DiscardContainer2; // 2 ��� ��������
    public Transform DiscardContainer3; // 3 ��� ��������


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
            GameObject tileObject = (count / 6) switch // �� ������ ���� ������� ������ ��������, �������� ��� ��� ������
            {
                0 => Instantiate(TilePrefab, DiscardContainer1), // <6 => ������
                1 => Instantiate(TilePrefab, DiscardContainer2), // <12 => ������
                2 => Instantiate(TilePrefab, DiscardContainer3), // >12 => ������
                _ => Instantiate(TilePrefab, DiscardContainer3),
            };
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(tile);
            count++; 
        }


    }
}
