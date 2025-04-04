using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public Transform TilesRemaining;

    public void UpdateScore(int p1, int p2, int p3, int p4)
    {
        P1_Score.GetComponent<TextMeshProUGUI>().text = p1.ToString();
        P2_Score.GetComponent<TextMeshProUGUI>().text = p2.ToString();
        P3_Score.GetComponent<TextMeshProUGUI>().text = p3.ToString();
        P4_Score.GetComponent<TextMeshProUGUI>().text = p4.ToString();
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

    public void UpdateTilesRemaining(int tiles)
    {
        TilesRemaining.GetComponent<TextMeshProUGUI>().text = tiles.ToString();
    }



    }
