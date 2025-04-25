using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.XR;

public class VictoryScreenView : MonoBehaviour
{
    public GameObject Itself;

    public GameObject TilePrefab; // Префаб плитки
    public GameObject CallPrefab;
    public GameObject TileBackPrefab;
    public GameObject YakuPrefab;

    public Transform HandContainer; // Контейнер для плиток

    public Transform CallsContainer; // Контейнер для вызовов

    public Transform DoraContainer;

    public Transform UraDoraContainer;
    public Transform UraDora_Text;

    public Transform YakuContainer;

    public Transform HanIndicator;

    public Transform LimitIndicator;

    public Transform PointsIndicator;

    public List<Transform> PlayerBoxes;

    public Transform ContinueButton;

    public GameManager GameManager;

    private IPlayer Player;

    public void Draw(IPlayer player, int[] score_change, int han, List<(string Yaku, int Cost)> yakus)
    {
        Itself.SetActive(true);
        Player = player;
        DrawHand(player.Hand);
        DrawCalls(player.Calls);
        DrawDoras();
        if (player.Riichi) DrawUraDoras();
        DrawYaku(yakus);
        DrawHan(han);
        DrawLimit(han);
        DrawPoints(score_change[player.index]);

        score_change[player.index] += player.GameManager.GetThenClearBet();

        foreach (var plr in player.GameManager.Players)
        {
            plr.Score += score_change[plr.index];
        }

            DrawPlayerBoxes(player.GameManager, score_change);
        DrawContinueButton();
    }

    public void DrawHand(List<Tile> hand)
    {
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        // Отобразите каждую плитку в руке
        for (int i = 0; i < hand.Count; i++)
        {
            GameObject tileObject = Instantiate(TilePrefab, HandContainer);
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]); 
        }
    }

    public void DrawCalls(List<List<Tile>> calls)
    {
        foreach (Transform child in CallsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (List<Tile> call in calls)
        {
            if (call.Count == 4)
            {
                DrawKan(call);
                continue;
            }
            GameObject callObj = Instantiate(CallPrefab, CallsContainer);
            Transform tileContainer = callObj.transform;

            int i = 0;
            while (i < call.Count)
            {
                // Условие для формирования composite-группы: текущий и следующий тайлы неповернуты
                bool currentNonRotated = (call[i].Properties == null || !call[i].Properties.Contains("Called"));
                if (i < call.Count - 1)
                {
                    bool nextNonRotated = (call[i + 1].Properties == null || !call[i + 1].Properties.Contains("Called"));
                    if (currentNonRotated && nextNonRotated)
                    {
                        GameObject pairGroup = new GameObject("TilePairGroup", typeof(RectTransform));
                        pairGroup.transform.SetParent(tileContainer.transform, false);

                        HorizontalLayoutGroup pairHLG = pairGroup.AddComponent<HorizontalLayoutGroup>();
                        pairHLG.spacing = 1.35f;
                        pairHLG.childAlignment = TextAnchor.MiddleCenter;
                        pairHLG.childForceExpandHeight = false;
                        pairHLG.childForceExpandWidth = false;

                        // Добавляем первый тайл в пару
                        GameObject tile1 = Instantiate(TilePrefab, pairGroup.transform);
                        tile1.GetComponent<SpriteRenderer>().sortingOrder = 10;
                        TileView tile1View = tile1.GetComponent<TileView>();
                        if (tile1View != null)
                        {
                            tile1View.SetTile(call[i]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                        }

                        // Добавляем второй тайл в пару
                        GameObject tile2 = Instantiate(TilePrefab, pairGroup.transform);
                        tile2.GetComponent<SpriteRenderer>().sortingOrder = 10;
                        TileView tile2View = tile2.GetComponent<TileView>();
                        if (tile2View != null)
                        {
                            tile2View.SetTile(call[i + 1]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                        }

                        // Обрабатываем два тайла и переходим дальше
                        i += 2;
                        continue;
                    }
                }


                GameObject tileObj = Instantiate(TilePrefab, tileContainer);
                tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                // Если у тайла в Properties содержится "Called", поворачиваем его на 90 градусов
                if (call[i].Properties != null && call[i].Properties.Contains("Called"))
                {
                    tileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                TileView tileView = tileObj.GetComponent<TileView>();
                tileView.GetComponent<SpriteRenderer>().sortingOrder = 10;
                if (tileView != null)
                {
                    tileView.SetTile(call[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }

                i++;
            }
        }
    }
    public void DrawKan(List<Tile> kan)
    {
        // Создаём объект для кан (Kan) по callprefab и получаем его контейнер для тайлов.
        GameObject callObj = Instantiate(CallPrefab, CallsContainer);
        Transform tileContainer = callObj.transform;

        // Подсчитываем количество тайлов с "Called"
        int calledCount = 0;
        foreach (Tile tile in kan)
        {
            if (tile.Properties != null && tile.Properties.Contains("Called"))
            {
                calledCount++;
            }
        }

        // Сценарий 1: ровно 1 тайл с "Called" (учитываем только первые 3 тайла, 4-й никогда не переворачивается)
        if (calledCount == 1)
        {
            int calledIndex = -1;
            for (int i = 0; i < 3; i++)
            {
                if (kan[i].Properties != null && kan[i].Properties.Contains("Called"))
                {
                    calledIndex = i;
                    break;
                }
            }
            if (calledIndex == -1)
            {
                Debug.LogWarning("В кане ожидается один 'Called', но он не найден среди первых трёх тайлов.");
                calledIndex = 0;
            }

            // В данном случае всё размещается в callObj.
            // Создаём дочерний контейнер для группы нормальных тайлов с HorizontalLayoutGroup.
            GameObject trioGroup = new GameObject("NormalTilesGroup", typeof(RectTransform));
            trioGroup.transform.SetParent(tileContainer, false);
            HorizontalLayoutGroup trioHLG = trioGroup.AddComponent<HorizontalLayoutGroup>();

            trioHLG.spacing = 1.3f; // Отступ между тайлами внутри тройки
            trioHLG.childAlignment = TextAnchor.MiddleCenter;
            trioHLG.childForceExpandHeight = false;
            trioHLG.childForceExpandWidth = false;

            // В зависимости от позиции перевёрнутого тайла:
            // Если перевёрнутый тайл находится на позиции 0 – сначала добавляем его, затем группу остальных.
            // Если перевёрнутый тайл на позиции 1 или 2 – сначала добавляем группу остальных, затем перевёрнутый тайл.
            if (calledIndex == 0)
            {
                // Добавляем перевёрнутый тайл
                GameObject rotatedTileObj = Instantiate(TilePrefab, tileContainer, false);
                rotatedTileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                rotatedTileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView rotatedTileView = rotatedTileObj.GetComponent<TileView>();

                if (rotatedTileView != null)
                {
                    rotatedTileView.SetTile(kan[calledIndex]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
                // Затем устанавливаем контейнер для нормальных тайлов
                trioGroup.transform.SetParent(tileContainer, false);
            }
            else
            {
                // Сначала добавляем группу нормальных тайлов, затем перевёрнутый тайл
                trioGroup.transform.SetParent(tileContainer, false);
                GameObject rotatedTileObj = Instantiate(TilePrefab, tileContainer, false);
                rotatedTileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                rotatedTileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView rotatedTileView = rotatedTileObj.GetComponent<TileView>();
                if (rotatedTileView != null)
                {
                    rotatedTileView.SetTile(kan[calledIndex]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }

            // Собираем остальные три тайла (без перевёрнутого)
            for (int i = 0; i < kan.Count; i++)
            {
                if (i == calledIndex) continue;
                GameObject tileObj = Instantiate(TilePrefab, trioGroup.transform, false);
                tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }
        }
        // Сценарий 2: ровно 2 тайла с "Called"
        else if (calledCount == 2)
        {
            // Предполагается, что два тайла с "Called" идут подряд.
            int groupStart = -1;
            for (int i = 0; i < kan.Count - 1; i++)
            {
                if (kan[i].Properties != null && kan[i].Properties.Contains("Called") &&
                    kan[i + 1].Properties != null && kan[i + 1].Properties.Contains("Called"))
                {
                    groupStart = i;
                    break;
                }
            }
            if (groupStart == -1)
            {
                Debug.LogWarning("Некорректный Kan: два тайла с 'Called' не идут подряд.");
                groupStart = 0;
            }

            // Разбиваем массив на три части:
            // Левый блок – тайлы перед группой перевёрнутых,
            // Центральный блок – группа перевёрнутых (2 тайла),
            // Правый блок – тайлы после группы перевёрнутых.
            List<Tile> leftGroup = new List<Tile>();
            for (int i = 0; i < groupStart; i++)
                leftGroup.Add(kan[i]);

            List<Tile> calledGroup = new List<Tile>() { kan[groupStart], kan[groupStart + 1] };

            List<Tile> rightGroup = new List<Tile>();
            for (int i = groupStart + 2; i < kan.Count; i++)
                rightGroup.Add(kan[i]);

            // Используем callObj как родительский контейнер (уже создан callprefab).

            // Если есть левый блок – добавляем его.
            if (leftGroup.Count > 0)
            {
                GameObject leftContainer = new GameObject("LeftGroup", typeof(RectTransform));
                leftContainer.transform.SetParent(tileContainer, false);
                if (leftGroup.Count > 1)
                {
                    HorizontalLayoutGroup leftHLG = leftContainer.AddComponent<HorizontalLayoutGroup>();
                    leftHLG.spacing = 5f;
                    leftHLG.childAlignment = TextAnchor.MiddleCenter;
                    leftHLG.childForceExpandHeight = false;
                    leftHLG.childForceExpandWidth = false;
                }
                foreach (Tile tile in leftGroup)
                {
                    GameObject tileObj = Instantiate(TilePrefab, leftContainer.transform, false);
                    tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                    TileView tileView = tileObj.GetComponent<TileView>();
                    if (tileView != null)
                    {
                        tileView.SetTile(tile);
                    }
                    else
                    {
                        Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                    }
                }
            }

            // Центральный контейнер для перевёрнутых тайлов с VerticalLayoutGroup.
            GameObject centerContainer = new GameObject("CalledGroup", typeof(RectTransform));
            centerContainer.transform.SetParent(tileContainer, false);
            VerticalLayoutGroup centerVLG = centerContainer.AddComponent<VerticalLayoutGroup>();
            centerVLG.spacing = 1.35f;
            centerVLG.childAlignment = TextAnchor.MiddleCenter;
            centerVLG.childForceExpandHeight = false;
            centerVLG.childForceExpandWidth = false;
            // Задаём bottom padding – значение хранится как float, но RectOffset принимает int,
            // поэтому делаем приведение. (Если нужно более точное дробное значение, можно добавить отдельный LayoutElement с minHeight.)
            int bottomPadding = 1;
            RectOffset currentPadding = centerVLG.padding;
            centerVLG.padding = new RectOffset(currentPadding.left, currentPadding.right, currentPadding.top, bottomPadding);

            foreach (Tile tile in calledGroup)
            {
                GameObject tileObj = Instantiate(TilePrefab, centerContainer.transform, false);
                tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                tileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(tile);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }

            // Если есть правый блок – добавляем его.
            if (rightGroup.Count > 0)
            {
                GameObject rightContainer = new GameObject("RightGroup", typeof(RectTransform));
                rightContainer.transform.SetParent(tileContainer, false);
                if (rightGroup.Count > 1)
                {
                    HorizontalLayoutGroup rightHLG = rightContainer.AddComponent<HorizontalLayoutGroup>();
                    rightHLG.spacing = 1.35f;
                    rightHLG.childAlignment = TextAnchor.MiddleCenter;
                    rightHLG.childForceExpandHeight = false;
                    rightHLG.childForceExpandWidth = false;
                }
                foreach (Tile tile in rightGroup)
                {
                    GameObject tileObj = Instantiate(TilePrefab, rightContainer.transform, false);
                    tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                    TileView tileView = tileObj.GetComponent<TileView>();
                    if (tileView != null)
                    {
                        tileView.SetTile(tile);
                    }
                    else
                    {
                        Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                    }
                }
            }
        }
        // Сценарий 3: более 2-х тайлов с "Called"
        else if (calledCount > 2)
        {
            // В этом случае крайние (левый и правый) тайлы отрисовываются с использованием TileBackPrefab,
            // а два центральных – обычным TilePrefab без переворота.
            for (int i = 0; i < kan.Count; i++)
            {
                GameObject tileObj;
                if (i == 0 || i == kan.Count - 1)
                {
                    tileObj = Instantiate(TileBackPrefab, tileContainer, false);
                    tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                }
                else
                {
                    tileObj = Instantiate(TilePrefab, tileContainer, false);
                    tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                }
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab или TileBackPrefab не содержит компонент TileView.");
                }
            }
        }
        // Если ни одно условие не подошло, отрисовываем тайлы по умолчанию.
        else
        {
            for (int i = 0; i < kan.Count; i++)
            {
                GameObject tileObj = Instantiate(TilePrefab, tileContainer, false);
                tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }
        }
    
}

    public void DrawDoras()
    {
        foreach (Transform child in DoraContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameManager.DorasShown; i++)
        {
            GameObject tileObject;
            tileObject = Instantiate(TilePrefab, DoraContainer);
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(GameManager.DoraIndicator[i]);
        }
    }

    public void DrawUraDoras()
    {
        UraDora_Text.GetComponent<TextMeshProUGUI>().enabled = true;
        foreach (Transform child in UraDoraContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameManager.DorasShown; i++)
        {
            GameObject tileObject;
            tileObject = Instantiate(TilePrefab, UraDoraContainer);
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(GameManager.UraDoraIndicator[i]);
        }
    }
    public void DrawYaku(List<(string Yaku, int Cost)> yakus)
    {
        foreach (Transform child in YakuContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var yaku in yakus)
        {
            GameObject yakuobject = Instantiate(YakuPrefab, YakuContainer);
            var nameText = yakuobject.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var costText = yakuobject.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
            nameText.text = yaku.Yaku;
            costText.text = yaku.Cost.ToString()+"хан";
        }
    }

    public void DrawHan(int han)
    {
        HanIndicator.GetComponent<TextMeshProUGUI>().text = han.ToString()+"хан";
    }

    public void DrawLimit(int han)
    {
        Dictionary<int, string> limits = new Dictionary<int, string>()
    {
        {5,"Манган"},

        {6,"Ханеман"},
        {7,"Ханеман"},

        {8,"Байман"},
        {9,"Байман"},
        {10,"Байман"},

        {11,"Санбайман"},
        {12,"Санбайман"},

        {13,"Якуман"}
    };
        string limit;
        if (han<5) limit= "";
        else if (han>=13) limit = limits[13];
        else limit = limits[han];   
        LimitIndicator.GetComponent<TextMeshProUGUI>().text = limit;
    }

    public void DrawPoints(int points)
    {
        PointsIndicator.GetComponent<TextMeshProUGUI>().text = points.ToString()+" очков";
    }

    public void DrawPlayerBoxes(GameManager gameManager, int[] score_changes)
    {
        Dictionary<string, string> winds = new Dictionary<string, string>()
        {
            {"East","В" },
            {"South","Ю" },
            {"West","З" },
            {"North","С" }
        };

        for (int i = 0; i < PlayerBoxes.Count; i++)
        {
            var name=PlayerBoxes[i].transform.Find("Name").GetComponent<TextMeshProUGUI>();
            name.text = gameManager.Players[i].Name;

            var score = PlayerBoxes[i].transform.Find("Score").GetComponent<TextMeshProUGUI>();            
            score.text = gameManager.Players[i].Score.ToString();

            var deltaScore = PlayerBoxes[i].transform.Find("DeltaScore").GetComponent<TextMeshProUGUI>();
            string plus = score_changes[i] > 0 ? "+" : "";
            deltaScore.text = plus + score_changes[i].ToString();

            var wind = PlayerBoxes[i].transform.Find("Wind").GetComponent<TextMeshProUGUI>();
            wind.text = winds[gameManager.Players[i].Wind];
            if (wind.text=="В") wind.color = Color.red;
        }
    }

    public void DrawContinueButton()
    {
        Button buttonComponent = ContinueButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnContinueButtonClicked());
        }
    }

    public void OnContinueButtonClicked()
    {

        int dealer = -1;
        if (Player.Wind == "East") dealer = Player.index;
        Hide();
        GameManager.NextRound(dealer);
        
    }

    public void Hide()
    {
        Itself.SetActive(false);
    }
}
