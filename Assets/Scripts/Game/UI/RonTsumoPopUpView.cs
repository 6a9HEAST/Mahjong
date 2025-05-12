using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RonTsumoPopUpView:MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public GameObject Iself;
    public RectTransform BackScreen;        // сразу RectTransform
    public TextMeshProUGUI Text;            // TMPUGUI + RectTransform

    [Header("Параметры анимации")]
    public float animationDuration = 1.0f;  // время трансформации
    public float duration = 1.0f;           // полная длительность (>= animationDuration)

    [Header("Начальный / конечный масштаб")]
    public float startScreenScale = 1.0f;
    public float endScreenScale = 0.0f;
    //public float startTextScale = 1.0f;
    //public float endTextScale = 0.0f;

    private RectTransform _textRect;

    private void Awake()
    {
        // кешируем RectTransform текста
        _textRect = Text.GetComponent<RectTransform>();
    }

    public Dictionary<string, string> wind_text = new Dictionary<string, string>()
    {
        {"East", "Восточный"},
        {"South", "Южный"},
        {"West", "Западный"},
        {"North", "Северный"}
    };

    public void Draw(string text)
    {
        Iself.SetActive(true);
        // сразу ставим начальные масштабы (на всякий случай)
        BackScreen.localScale = new Vector3(1f, startScreenScale, 1f);
        //_textRect.localScale = Vector3.one * startTextScale;
        Text.text = text;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;

        // фаза анимации масштабов
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // Lerp для BackScreen только по Y
            float screenY = Mathf.Lerp(startScreenScale, endScreenScale, t);
            BackScreen.localScale = new Vector3(1f, screenY, 1f);

            // Lerp для текста по всем осям
            //float textS = Mathf.Lerp(startTextScale, endTextScale, t);
            //_textRect.localScale = Vector3.one * textS;

            yield return null;
        }

        // убедимся, что дошли до конечных значений
        BackScreen.localScale = new Vector3(1f, endScreenScale, 1f);
        //_textRect.localScale = Vector3.one * endTextScale;

        // ждем остаток времени до полного duration
        float remaining = duration - animationDuration;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        // отключаем
        Iself.SetActive(false);
    }

}

