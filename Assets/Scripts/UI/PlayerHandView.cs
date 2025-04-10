using System.Collections.Generic;
using UnityEngine;

public class PlayerHandView : MonoBehaviour
{
    public GameObject TilePrefab; // Префаб плитки
    public Transform HandContainer; // Контейнер для плиток
    public GameManager GameManager;

    public void Draw(List<Tile> hand)
    {
        foreach (Transform child in HandContainer)
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

    public void Sort(List<Tile> hand)
    {
        int count;
        if (hand.Count%3==1)
            count=hand.Count;
        else count=hand.Count-1;

        for (int i = 0; i < count - 1; i++)
        {
            for (int j = 0; j < count - i - 1; j++)
            {
                if (IsBigger(hand[j], hand[j+1]))
                {
                    Tile temp = hand[j];
                    hand[j] = hand[j + 1];
                    hand[j + 1] = temp;
                }
            }
        }
    }
    Dictionary<string, int> RankOrder = new Dictionary<string, int>()
        {
            {"Man", 0},
            {"Pin", 1},
            {"Sou", 2},
            {"Wind", 3},
            {"Dragon", 4}
        };
    Dictionary<string, int> WindOrder = new Dictionary<string, int>()
    {
        {"East", 0},
        {"South", 1},
        {"West", 2},
        {"North", 3}
    };
    Dictionary<string, int> DragonOrder = new Dictionary<string, int>()
    {
        {"Red", 0},
        {"White", 1},
        {"Green", 2}
    };
    public bool IsBigger (Tile tile1, Tile tile2)
    {
        if (tile1.Suit == tile2.Suit)
            switch (tile2.Suit) 
            {
                case "Dragon":
                    return DragonOrder[tile1.Rank] > DragonOrder[tile2.Rank];
                case "Wind":
                    return WindOrder[tile1.Rank] > WindOrder[tile2.Rank];
                default:
                    return int.Parse(tile1.Rank) > int.Parse(tile2.Rank);
            }
        else
            return RankOrder[tile1.Suit] > RankOrder[tile2.Suit];


    }
}
