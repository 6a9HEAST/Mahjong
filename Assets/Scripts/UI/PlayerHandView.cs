using System.Collections.Generic;
using UnityEngine;

public class PlayerHandView : MonoBehaviour
{
    public GameObject TilePrefab; // ������ ������
    public Transform HandContainer; // ��������� ��� ������
    public GameManager GameManager;

    public void Draw(List<Tile> hand)
    {
        //Debug.Log("Show hand");
        // �������� ������ ������
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }
        
        // ���������� ������ ������ � ����
        for (int i = 0; i < hand.Count; i++)            //(hand.Count == 13 ? hand.Count : hand.Count - 1); i++)
        {
            GameObject tileObject = Instantiate(TilePrefab, HandContainer);
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
            tileView.OnTileClicked.AddListener(GameManager.HandleTileClick);
        }
        //if (hand.Count == 13) return;

        //GameObject TsumoObject = Instantiate(TilePrefab, HandContainer);
        //TileView TsumoView = TsumoObject.GetComponent<TileView>();
        //TsumoView.SetTile(hand[^1]); // ^1 ���������� ������ ������� � �����
        //TsumoView.OnTileClicked.AddListener(GameManager.HandleTileClick);
        //// �������� ������������� ���� ������
        //var rectTransform = TsumoObject.GetComponent<RectTransform>();
        //rectTransform.anchoredPosition += new Vector2(0f, 0f);
    }
}
