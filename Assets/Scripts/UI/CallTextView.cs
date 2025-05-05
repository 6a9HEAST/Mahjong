using TMPro;
using UnityEngine;

public class CallTextView : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Draw(string str)
    {
        text.text = str;
    }

    // Update is called once per frame
    public void Clear()
    {
        text.text = "";
    }
}
