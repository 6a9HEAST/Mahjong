using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandGenerateView: MonoBehaviour
{
    public GameObject LiyingTile; // Префаб плитки
    public GameObject StandingTile;
    public GameObject CloseLiyingTile;
    public Transform HandContainer; // Контейнер для плиток
    public GameManager GameManager;
    
    public float handSpacing = 10;
    public float setsSpacing = 10;

    public void Generate()
    {
        List<Tile> tiles=new List<Tile>()
        {
            new("Dragon","White"),
            new("Dragon","White"),
            new("Dragon","White"),

            new("Dragon","Green"),
            new("Dragon","Green"),
            new("Dragon","Green"),

            new("Dragon","Red"),
            new("Dragon","Red"),
            new("Dragon","Red"),
        };
        Draw7 (tiles);
    }

    public void Draw1(List<Tile> hand) //Все вместе кроме последнего
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        // Создаем горизонтальный контейнер для всех тайлов, кроме последнего
        GameObject groupObject = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        groupObject.transform.SetParent(HandContainer, false);
        var layout = groupObject.GetComponent<HorizontalLayoutGroup>();

        layout.spacing = 1.39f;

        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Добавляем все тайлы, кроме последнего, в groupObject
        for (int i = 0; i < hand.Count - 1; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, groupObject.transform);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
        }

        // Добавляем последний тайл напрямую в HandContainer
        GameObject lastTile = Instantiate(LiyingTile, HandContainer);
        var lastBlackBox = lastTile.transform.Find("BlackBox");
        lastBlackBox.gameObject.SetActive(false);

        TileView lastView = lastTile.GetComponent<TileView>();
        lastView.SetTile(hand[hand.Count - 1]);

        // Устанавливаем spacing для последнего тайла
        var spc =HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }

    public void Draw2(List<Tile> hand)//Вся рука разделена на тройки и пару
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        for (int i = 0; i < 4; i++)
        {
            GameObject groupObject = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            groupObject.transform.SetParent(HandContainer, false);
            var layout = groupObject.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = setsSpacing;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            for (int j = 0; j < 3; j++)
            {
                GameObject tileObject = Instantiate(LiyingTile, groupObject.transform);
                var blackBox = tileObject.transform.Find("BlackBox");
                blackBox.gameObject.SetActive(false);

                TileView tileView = tileObject.GetComponent<TileView>();
                tileView.SetTile(hand[0]);
                hand.RemoveAt(0);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, HandContainer);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
        }        
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }

    public void Draw3(List<Tile> hand) //Все вместе кроме последней тройки и двух тайлов
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        // Создаем горизонтальный контейнер для всех тайлов, кроме последнего
        GameObject groupObject = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        groupObject.transform.SetParent(HandContainer, false);
        var layout = groupObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = setsSpacing;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Добавляем все тайлы, кроме последнего, в groupObject
        for (int i = 0; i < 9; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, groupObject.transform);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[0]);
            hand.RemoveAt(0);
        }

        GameObject groupObject2 = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        groupObject2.transform.SetParent(HandContainer, false);
        var layout2 = groupObject2.GetComponent<HorizontalLayoutGroup>();
        layout2.spacing = setsSpacing;
        layout2.childForceExpandWidth = false;
        layout2.childForceExpandHeight = false;

        // Добавляем все тайлы, кроме последнего, в groupObject
        for (int i = 0; i < 3; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, groupObject2.transform);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[0]);
            hand.RemoveAt(0);
        }

        for (int i = 0; i < 2; i++)
        {            
            GameObject lastTile = Instantiate(LiyingTile, HandContainer);
            var lastBlackBox = lastTile.transform.Find("BlackBox");
            lastBlackBox.gameObject.SetActive(false);

            TileView lastView = lastTile.GetComponent<TileView>();
            lastView.SetTile(hand[i]);
        }

        // Устанавливаем spacing для последнего тайла
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }

    public void Draw4(List<Tile> hand) //Все раздельно
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        for (int i = 0; i < hand.Count; i++)
        {


            // Добавляем последний тайл напрямую в HandContainer
            GameObject lastTile = Instantiate(LiyingTile, HandContainer);
            var lastBlackBox = lastTile.transform.Find("BlackBox");
            lastBlackBox.gameObject.SetActive(false);

            TileView lastView = lastTile.GetComponent<TileView>();
            lastView.SetTile(hand[i]);
        }

        // Устанавливаем spacing для последнего тайла
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }

    public void Draw5(List<Tile> hand) //Сначала тройка, потом 3 четверки и пара
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        // Создаем горизонтальный контейнер для всех тайлов, кроме последнего
        GameObject groupObject = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        groupObject.transform.SetParent(HandContainer, false);
        var layout = groupObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = setsSpacing;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Добавляем все тайлы, кроме последнего, в groupObject
        for (int i = 0; i < 3; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, groupObject.transform);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[0]);
            hand.RemoveAt(0);
        }

        // Создаем горизонтальный контейнер для всех тайлов, кроме последнего


        for (int i = 0; i < 3; i++)
        {
            GameObject groupObject2 = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            groupObject2.transform.SetParent(HandContainer, false);
            var layout2 = groupObject2.GetComponent<HorizontalLayoutGroup>();
            layout2.spacing = setsSpacing;
            layout2.childForceExpandWidth = false;
            layout2.childForceExpandHeight = false;
            for (int j = 0; j < 4; j++)
            {
                GameObject tileObject = Instantiate(LiyingTile, groupObject2.transform);
                var blackBox = tileObject.transform.Find("BlackBox");
                blackBox.gameObject.SetActive(false);

                TileView tileView = tileObject.GetComponent<TileView>();
                tileView.SetTile(hand[0]);
                hand.RemoveAt(0);
            }
        }

        // Добавляем последний тайл напрямую в HandContainer
        for (int i = 0; i < 2; i++)
        {
            GameObject lastTile = Instantiate(LiyingTile, HandContainer);
        var lastBlackBox = lastTile.transform.Find("BlackBox");
        lastBlackBox.gameObject.SetActive(false);

        TileView lastView = lastTile.GetComponent<TileView>();
            lastView.SetTile(hand[i]);
        }

        // Устанавливаем spacing для последнего тайла
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }
    public void Draw6(List<Tile> hand) //Все вместе кроме последних 2х троек и двух тайлов
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        // Создаем горизонтальный контейнер для всех тайлов, кроме последнего
        GameObject groupObject = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        groupObject.transform.SetParent(HandContainer, false);
        var layout = groupObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = setsSpacing;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Добавляем все тайлы, кроме последнего, в groupObject
        for (int i = 0; i < 6; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, groupObject.transform);
            var blackBox = tileObject.transform.Find("BlackBox");
            blackBox.gameObject.SetActive(false);

            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[0]);
            hand.RemoveAt(0);
        }
        for (int j = 0; j < 2; j++)
        {
            GameObject groupObject2 = new GameObject("TileGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            groupObject2.transform.SetParent(HandContainer, false);
            var layout2 = groupObject2.GetComponent<HorizontalLayoutGroup>();
            layout2.spacing = setsSpacing;
            layout2.childForceExpandWidth = false;
            layout2.childForceExpandHeight = false;

            // Добавляем все тайлы, кроме последнего, в groupObject
            for (int i = 0; i < 3; i++)
            {
                GameObject tileObject = Instantiate(LiyingTile, groupObject2.transform);
                var blackBox = tileObject.transform.Find("BlackBox");
                blackBox.gameObject.SetActive(false);

                TileView tileView = tileObject.GetComponent<TileView>();
                tileView.SetTile(hand[0]);
                hand.RemoveAt(0);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject lastTile = Instantiate(LiyingTile, HandContainer);
            var lastBlackBox = lastTile.transform.Find("BlackBox");
            lastBlackBox.gameObject.SetActive(false);

            TileView lastView = lastTile.GetComponent<TileView>();
            lastView.SetTile(hand[i]);
        }

        // Устанавливаем spacing для последнего тайла
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = handSpacing;
    }
    public void Draw7(List<Tile> hand) //Все вместе
    {
        // Очистка предыдущих тайлов
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand.Count == 0)
            return;

        for (int i = 0; i < hand.Count; i++)
        {


            // Добавляем последний тайл напрямую в HandContainer
            GameObject lastTile = Instantiate(LiyingTile, HandContainer);
            var lastBlackBox = lastTile.transform.Find("BlackBox");
            lastBlackBox.gameObject.SetActive(false);

            TileView lastView = lastTile.GetComponent<TileView>();
            lastView.SetTile(hand[i]);
        }

        // Устанавливаем spacing для последнего тайла
        var spc = HandContainer.GetComponent<HorizontalLayoutGroup>();
        spc.spacing = 1.39f;
    }
    public void Sort(List<Tile> hand)
    {
        int count;
        if (hand.Count % 3 == 1)
            count = hand.Count;
        else count = hand.Count - 1;

        for (int i = 0; i < count - 1; i++)
        {
            for (int j = 0; j < count - i - 1; j++)
            {
                if (IsBigger(hand[j], hand[j + 1]))
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
    public bool IsBigger(Tile tile1, Tile tile2)
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