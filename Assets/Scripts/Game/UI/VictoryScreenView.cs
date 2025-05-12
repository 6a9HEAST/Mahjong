using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class VictoryScreenView : MonoBehaviour
{
    public GameObject Itself;
    public GameObject Yakuscreen;

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

    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    public void GetSprites()
    {
        foreach (var sprite in Resources.LoadAll<Sprite>("Winds"))
        {

            spriteDictionary[sprite.name] = sprite;
        }
    }

    /// <summary>
    /// Используется при рон или цумо
    /// </summary>
    public IEnumerator Draw(IPlayer player, int[] score_change, int han, List<(string Yaku, int Cost)> yakus)
    {
        yield return new WaitForSeconds(2f);
        Itself.SetActive(true);
        GetSprites();
        Player = player;
        DrawHand(player.Hand);
        DrawCalls(player.Calls);
        DrawDoras();
        if (player.Riichi) DrawUraDoras();
        
        DrawHan(han);
        DrawLimit(han, score_change[player.index]);
        DrawPoints(score_change[player.index]);

        score_change[player.index] += player.GameManager.GetThenClearBet();

        DrawPlayerBoxes(player.GameManager, score_change);
        foreach (var plr in player.GameManager.Players)
        {
            plr.Score += score_change[plr.index];
        }

        StartCoroutine(DrawYakuAndAnimate(yakus, player.GameManager, score_change, 2f));
        DrawContinueButton();
    }
    /// <summary>
    /// Используется при ничьей
    /// </summary>
    public IEnumerator Draw(GameManager gameManager, int[] score_change)
    {
        yield return new WaitForSeconds(1f);
        Itself.SetActive(true);
        Yakuscreen.SetActive(false);
        GetSprites();
        foreach (var player in gameManager.Players)
            if (player.Wind == "East")
                if (player.WaitCosts.Count > 0) Player = player;
                else Player = gameManager.Players[(player.index + 1) % 4];
        DrawPlayerBoxes(gameManager, score_change);
        foreach (var plr in gameManager.Players)
        {
            plr.Score += score_change[plr.index];
        }

        
        yield return new WaitForSeconds(2f);
        AnimateScoreChanges(gameManager, score_change, 2f);
        DrawContinueButton();
    }

    private IEnumerator DrawYakuAndAnimate(List<(string Yaku, int Cost)> yakus, GameManager gm, int[] scoreChange, float duration)
    {
        // Сначала выполняем отрисовку яку
        yield return StartCoroutine(DrawYaku(yakus));
        // После завершения запускаем анимацию изменения счета
        AnimateScoreChanges(gm, scoreChange, duration);
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
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 25;
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

        foreach (var call in calls)
        {
            GameObject callObj = Instantiate(CallPrefab, CallsContainer);
            var hlg = callObj.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 1.3f;

            Transform tileContainer = callObj.transform;

            foreach (var tile in call)
            {
                GameObject tileObject = Instantiate(TilePrefab, tileContainer);
                tileObject.GetComponent<SpriteRenderer>().sortingOrder = 25;
                TileView tileView = tileObject.GetComponent<TileView>();
                tileView.SetTile(tile);
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
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 25;
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
            tileObject.GetComponent<SpriteRenderer>().sortingOrder = 25;
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(GameManager.UraDoraIndicator[i]);
        }
    }
    public IEnumerator DrawYaku(List<(string Yaku, int Cost)> yakus)
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
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
    }

    public void DrawHan(int han)
    {
        HanIndicator.GetComponent<TextMeshProUGUI>().text = han.ToString()+"хан";
    }

    public void DrawLimit(int han,int score)
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
        if (han < 5)
            if (score == 8000 || score == 12000) limit = "Манган";
            else limit= "";
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
            if (score_changes[i]!=0) 
            {                
                string plus = score_changes[i] > 0 ? "+" : "";
                deltaScore.text = plus + score_changes[i].ToString();
            }
            else deltaScore.text = "";

            var wind = PlayerBoxes[i].transform.Find("Image").GetComponent<Image>();
            if (wind != null)
                if (spriteDictionary.TryGetValue(gameManager.Players[i].Wind, out var sprite))
                {
                    wind.sprite= sprite;
                }
        }
    }

    public void AnimateScoreChanges(GameManager gameManager, int[] score_changes, float duration)
    {
        for (int i = 0; i < PlayerBoxes.Count; i++)
        {
            if (score_changes[i]!=0) 
                StartCoroutine(AnimatePlayerScore(
                    playerIndex: i,
                    targetScore: gameManager.Players[i].Score,
                    initialDelta: score_changes[i],
                    duration: duration
                ));
        }
    }

    private IEnumerator AnimatePlayerScore(int playerIndex, int targetScore, int initialDelta, float duration)
    {
        var scoreText = PlayerBoxes[playerIndex].transform.Find("Score").GetComponent<TextMeshProUGUI>();
        var deltaText = PlayerBoxes[playerIndex].transform.Find("DeltaScore").GetComponent<TextMeshProUGUI>();

        int startScore = targetScore - initialDelta;
        float timer = 0f;

        // Устанавливаем начальные значения для анимации
        scoreText.text = startScore.ToString();
        deltaText.text = $"{(initialDelta > 0 ? "+" : "")}{initialDelta}";

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            // Плавное изменение значений
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, progress));
            int currentDelta = Mathf.RoundToInt(Mathf.Lerp(initialDelta, 0, progress));

            // Обновляем UI
            scoreText.text = currentScore.ToString();
            deltaText.text = $"{(currentDelta > 0 ? "+" : "")}{currentDelta}";

            yield return null;
        }

        // Финализируем значения
        scoreText.text = targetScore.ToString();
        //deltaText.text = "";
    }

    public void DrawContinueButton()
    {
        Button buttonComponent = ContinueButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

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
        Yakuscreen.SetActive(true);
        Itself.SetActive(false);
    }
}
