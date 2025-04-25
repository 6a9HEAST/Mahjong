using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CenterView:MonoBehaviour
{
    public Transform P1_Score;
    public Transform P2_Score;
    public Transform P3_Score;
    public Transform P4_Score;
    public Transform P1_Wind;
    public Transform P2_Wind;
    public Transform P3_Wind;
    public Transform P4_Wind;

    public List<Transform> BetIndicators;

    public Transform TilesRemaining;
    public Transform RoundWind;
    public Transform BetDisplay;
    public UnityEvent OnCenterClicked;
    int[] scores = new int[4];
    private void Awake()
    {
        if (OnCenterClicked == null)
            OnCenterClicked = new UnityEvent();

        OnCenterClicked.AddListener(HandleCenterClick);
    }

    

    public void UpdateScore(int p1, int p2, int p3, int p4)
    {
        P1_Score.GetComponent<TextMeshProUGUI>().text = p1.ToString();
        P1_Score.GetComponent<TextMeshProUGUI>().color = Color.white;
        P2_Score.GetComponent<TextMeshProUGUI>().text = p2.ToString();
        P2_Score.GetComponent<TextMeshProUGUI>().color = Color.white;
        P3_Score.GetComponent<TextMeshProUGUI>().text = p3.ToString();
        P3_Score.GetComponent<TextMeshProUGUI>().color = Color.white;
        P4_Score.GetComponent<TextMeshProUGUI>().text = p4.ToString();
        P4_Score.GetComponent<TextMeshProUGUI>().color = Color.white;
        scores[0] = p1;
        scores[1] = p2;
        scores[2] = p3;
        scores[3] = p4;
    }
    public void UpdateWinds(string p1, string p2, string p3, string p4)
    {
        var winds = new Dictionary<string, string>()
        {
            { "East", "В"},
            { "West", "З"},
            { "North", "С"},
            { "South", "Ю"}
        };

        P1_Wind.GetComponent<TextMeshProUGUI>().text = winds[p1];
        if (p1=="East") P1_Wind.GetComponent<TextMeshProUGUI>().color = Color.red; //Если дилер то буква красная
        else P1_Wind.GetComponent<TextMeshProUGUI>().color = Color.white;

        P2_Wind.GetComponent<TextMeshProUGUI>().text = winds[p2];
        if (p2 == "East") P2_Wind.GetComponent<TextMeshProUGUI>().color = Color.red;
        else P2_Wind.GetComponent<TextMeshProUGUI>().color = Color.white;

        P3_Wind.GetComponent<TextMeshProUGUI>().text = winds[p3];
        if (p3 == "East") P3_Wind.GetComponent<TextMeshProUGUI>().color = Color.red;
        else P3_Wind.GetComponent<TextMeshProUGUI>().color = Color.white;

        P4_Wind.GetComponent<TextMeshProUGUI>().text = winds[p4];
        if (p4 == "East") P4_Wind.GetComponent<TextMeshProUGUI>().color = Color.red;
        else P4_Wind.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    public void UpdateRoundWind(string wind)
    {
        var winds = new Dictionary<string, string>()
        {
            { "East", "Восточный"},
            { "West", "Западный"},
            { "North", "Северный"},
            { "South", "Южный"}
        };
        RoundWind.GetComponent<TextMeshProUGUI>().text = winds[wind]+" раунд";
    }

    public void UpdateTilesRemaining(int tiles)
    {
        TilesRemaining.GetComponent<TextMeshProUGUI>().text = tiles.ToString();
    }

    void OnMouseDown()
    {
        OnCenterClicked.Invoke();
    }

    void HandleCenterClick()
    {
        DisplayDifference(); //отображение разницы очков между игроком и соперниками
        StartCoroutine(StartTimer());
    }

    public float timerDuration = 2f;
    IEnumerator StartTimer()
    {
        // Ожидаем timerDuration секунд
        yield return new WaitForSeconds(timerDuration);
        UpdateScore(scores[0], scores[1], scores[2], scores[3]);// после прошествия времени счет  меняется на тсандартный
    }

    public void DisplayDifference()
    {
        //разница
        int delta1= scores[1] - scores[0];// с правым
        int delta2 = scores[2] - scores[0];//с центральным
        int delta3 = scores[3] - scores[0];// с левым

        P2_Score.GetComponent<TextMeshProUGUI>().text = delta1.ToString();
        if (delta1 <= 0) P2_Score.GetComponent<TextMeshProUGUI>().color = Color.red;//если разница отрицательная то цвет числа красный
        else P2_Score.GetComponent<TextMeshProUGUI>().color = Color.blue; // если положительная то синий

        P3_Score.GetComponent<TextMeshProUGUI>().text = delta2.ToString();
        if (delta2 <= 0) P3_Score.GetComponent<TextMeshProUGUI>().color = Color.red;
        else P3_Score.GetComponent<TextMeshProUGUI>().color = Color.blue;

        P4_Score.GetComponent<TextMeshProUGUI>().text = delta3.ToString();
        if (delta3 <= 0) P4_Score.GetComponent<TextMeshProUGUI>().color = Color.red;
        else P4_Score.GetComponent<TextMeshProUGUI>().color = Color.blue;
    }

    public void UpdateBet(int bet)
    {
        var number = BetDisplay.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        number.text = bet.ToString()+" x";
    }

    public void UpdatePlayerBetView(List<IPlayer> players)
    {
        foreach (var player in players) 
        {
            if (player.Riichi)
            {
                BetIndicators[player.index].gameObject.SetActive(true);
            }
            else BetIndicators[player.index].gameObject.SetActive(false);
        }
    }
}

