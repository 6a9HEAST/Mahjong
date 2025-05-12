using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CenterView:MonoBehaviour
{
    public Transform P1_Score;
    public Transform P2_Score;
    public Transform P3_Score;
    public Transform P4_Score;

    public List<Transform> Winds;

    public List<GameObject> WindLights;

    public List<Transform> BetIndicators;

    public Transform TilesRemaining;
    public Transform RoundWind;
    public Transform BetDisplay;
    public UnityEvent OnCenterClicked;
    int[] scores = new int[4];

    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (OnCenterClicked == null)
            OnCenterClicked = new UnityEvent();

        OnCenterClicked.AddListener(HandleCenterClick);

        foreach (var sprite in Resources.LoadAll<Sprite>("Winds"))
        {

            spriteDictionary[sprite.name] = sprite;
        }
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
        List<string> strings= new List<string>() { p1,p2,p3,p4};

        for (int i=0;i<4;i++)
        {
            var wind = Winds[i].GetComponent<Image>();
            if (wind != null)
                if (spriteDictionary.TryGetValue(strings[i], out var sprite))
                {
                    wind.sprite = sprite;
                }
        }
    }
    public float MaxScale = 0.551f;
    public float MinScale = 0.43f;
    public float time = 0.2f;
    private Coroutine _pulseCoroutine;
    public void TurnWindLightOn(int index,bool start_animation=true)
    {
        foreach (var light in WindLights)
        {
            light.SetActive(false);
        }
        WindLights[index].SetActive(true);
        var go = WindLights[index];
        go.SetActive(true);
        if (!start_animation) return;
            // Запускаем анимацию «пульсации»
            if (_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);

        // Запускаем бесконечную «пульсацию»
        _pulseCoroutine = StartCoroutine(PulseScaleLoop(go.transform as RectTransform));
    }

    private IEnumerator PulseScaleLoop(RectTransform rt)
    {
        Vector3 maxVec = Vector3.one * MaxScale;
        Vector3 minVec = Vector3.one * MinScale;
        float half = time * 0.5f;

        while (true) // бесконечный цикл
        {
            // Фаза уменьшения
            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(maxVec, minVec, t / half);
                yield return null;
            }
            rt.localScale = minVec;

            // Фаза увеличения
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(minVec, maxVec, t / half);
                yield return null;
            }
            rt.localScale = maxVec;
        }
    }

    // При выключении объекта можно остановить корутину
    private void OnDisable()
    {
        if (_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);
    }

    public void UpdateRoundWind(string wind,int round_number)
    {
        var winds = new Dictionary<string, string>()
        {
            { "East", "Восточный"},
            { "West", "Западный"},
            { "North", "Северный"},
            { "South", "Южный"}
        };
        RoundWind.GetComponent<TextMeshProUGUI>().text = winds[wind]+" "+round_number;
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

