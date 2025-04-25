using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.XR;

public class VictoryScreenView : MonoBehaviour
{
    public GameObject Itself;

    public GameObject TilePrefab; // ������ ������
    public GameObject CallPrefab;
    public GameObject TileBackPrefab;
    public GameObject YakuPrefab;

    public Transform HandContainer; // ��������� ��� ������

    public Transform CallsContainer; // ��������� ��� �������

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

        // ���������� ������ ������ � ����
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
                // ������� ��� ������������ composite-������: ������� � ��������� ����� �����������
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

                        // ��������� ������ ���� � ����
                        GameObject tile1 = Instantiate(TilePrefab, pairGroup.transform);
                        tile1.GetComponent<SpriteRenderer>().sortingOrder = 10;
                        TileView tile1View = tile1.GetComponent<TileView>();
                        if (tile1View != null)
                        {
                            tile1View.SetTile(call[i]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                        }

                        // ��������� ������ ���� � ����
                        GameObject tile2 = Instantiate(TilePrefab, pairGroup.transform);
                        tile2.GetComponent<SpriteRenderer>().sortingOrder = 10;
                        TileView tile2View = tile2.GetComponent<TileView>();
                        if (tile2View != null)
                        {
                            tile2View.SetTile(call[i + 1]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                        }

                        // ������������ ��� ����� � ��������� ������
                        i += 2;
                        continue;
                    }
                }


                GameObject tileObj = Instantiate(TilePrefab, tileContainer);
                tileObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                // ���� � ����� � Properties ���������� "Called", ������������ ��� �� 90 ��������
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                }

                i++;
            }
        }
    }
    public void DrawKan(List<Tile> kan)
    {
        // ������ ������ ��� ��� (Kan) �� callprefab � �������� ��� ��������� ��� ������.
        GameObject callObj = Instantiate(CallPrefab, CallsContainer);
        Transform tileContainer = callObj.transform;

        // ������������ ���������� ������ � "Called"
        int calledCount = 0;
        foreach (Tile tile in kan)
        {
            if (tile.Properties != null && tile.Properties.Contains("Called"))
            {
                calledCount++;
            }
        }

        // �������� 1: ����� 1 ���� � "Called" (��������� ������ ������ 3 �����, 4-� ������� �� ����������������)
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
                Debug.LogWarning("� ���� ��������� ���� 'Called', �� �� �� ������ ����� ������ ��� ������.");
                calledIndex = 0;
            }

            // � ������ ������ �� ����������� � callObj.
            // ������ �������� ��������� ��� ������ ���������� ������ � HorizontalLayoutGroup.
            GameObject trioGroup = new GameObject("NormalTilesGroup", typeof(RectTransform));
            trioGroup.transform.SetParent(tileContainer, false);
            HorizontalLayoutGroup trioHLG = trioGroup.AddComponent<HorizontalLayoutGroup>();

            trioHLG.spacing = 1.3f; // ������ ����� ������� ������ ������
            trioHLG.childAlignment = TextAnchor.MiddleCenter;
            trioHLG.childForceExpandHeight = false;
            trioHLG.childForceExpandWidth = false;

            // � ����������� �� ������� ������������ �����:
            // ���� ����������� ���� ��������� �� ������� 0 � ������� ��������� ���, ����� ������ ���������.
            // ���� ����������� ���� �� ������� 1 ��� 2 � ������� ��������� ������ ���������, ����� ����������� ����.
            if (calledIndex == 0)
            {
                // ��������� ����������� ����
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                }
                // ����� ������������� ��������� ��� ���������� ������
                trioGroup.transform.SetParent(tileContainer, false);
            }
            else
            {
                // ������� ��������� ������ ���������� ������, ����� ����������� ����
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                }
            }

            // �������� ��������� ��� ����� (��� ������������)
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                }
            }
        }
        // �������� 2: ����� 2 ����� � "Called"
        else if (calledCount == 2)
        {
            // ��������������, ��� ��� ����� � "Called" ���� ������.
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
                Debug.LogWarning("������������ Kan: ��� ����� � 'Called' �� ���� ������.");
                groupStart = 0;
            }

            // ��������� ������ �� ��� �����:
            // ����� ���� � ����� ����� ������� �����������,
            // ����������� ���� � ������ ����������� (2 �����),
            // ������ ���� � ����� ����� ������ �����������.
            List<Tile> leftGroup = new List<Tile>();
            for (int i = 0; i < groupStart; i++)
                leftGroup.Add(kan[i]);

            List<Tile> calledGroup = new List<Tile>() { kan[groupStart], kan[groupStart + 1] };

            List<Tile> rightGroup = new List<Tile>();
            for (int i = groupStart + 2; i < kan.Count; i++)
                rightGroup.Add(kan[i]);

            // ���������� callObj ��� ������������ ��������� (��� ������ callprefab).

            // ���� ���� ����� ���� � ��������� ���.
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
                        Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                    }
                }
            }

            // ����������� ��������� ��� ����������� ������ � VerticalLayoutGroup.
            GameObject centerContainer = new GameObject("CalledGroup", typeof(RectTransform));
            centerContainer.transform.SetParent(tileContainer, false);
            VerticalLayoutGroup centerVLG = centerContainer.AddComponent<VerticalLayoutGroup>();
            centerVLG.spacing = 1.35f;
            centerVLG.childAlignment = TextAnchor.MiddleCenter;
            centerVLG.childForceExpandHeight = false;
            centerVLG.childForceExpandWidth = false;
            // ����� bottom padding � �������� �������� ��� float, �� RectOffset ��������� int,
            // ������� ������ ����������. (���� ����� ����� ������ ������� ��������, ����� �������� ��������� LayoutElement � minHeight.)
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                }
            }

            // ���� ���� ������ ���� � ��������� ���.
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
                        Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
                    }
                }
            }
        }
        // �������� 3: ����� 2-� ������ � "Called"
        else if (calledCount > 2)
        {
            // � ���� ������ ������� (����� � ������) ����� �������������� � �������������� TileBackPrefab,
            // � ��� ����������� � ������� TilePrefab ��� ����������.
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
                    Debug.LogWarning("TilePrefab ��� TileBackPrefab �� �������� ��������� TileView.");
                }
            }
        }
        // ���� �� ���� ������� �� �������, ������������ ����� �� ���������.
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
                    Debug.LogWarning("TilePrefab �� �������� ��������� TileView.");
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
            costText.text = yaku.Cost.ToString()+"���";
        }
    }

    public void DrawHan(int han)
    {
        HanIndicator.GetComponent<TextMeshProUGUI>().text = han.ToString()+"���";
    }

    public void DrawLimit(int han)
    {
        Dictionary<int, string> limits = new Dictionary<int, string>()
    {
        {5,"������"},

        {6,"�������"},
        {7,"�������"},

        {8,"������"},
        {9,"������"},
        {10,"������"},

        {11,"���������"},
        {12,"���������"},

        {13,"������"}
    };
        string limit;
        if (han<5) limit= "";
        else if (han>=13) limit = limits[13];
        else limit = limits[han];   
        LimitIndicator.GetComponent<TextMeshProUGUI>().text = limit;
    }

    public void DrawPoints(int points)
    {
        PointsIndicator.GetComponent<TextMeshProUGUI>().text = points.ToString()+" �����";
    }

    public void DrawPlayerBoxes(GameManager gameManager, int[] score_changes)
    {
        Dictionary<string, string> winds = new Dictionary<string, string>()
        {
            {"East","�" },
            {"South","�" },
            {"West","�" },
            {"North","�" }
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
            if (wind.text=="�") wind.color = Color.red;
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
