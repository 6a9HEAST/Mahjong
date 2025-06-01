using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YakuListView : MonoBehaviour
{
    public List<Button> Buttons;
    public Sprite DefaultSprite;
    public Sprite SelectedSprite;

    public List<Sprite> _1Han; int[] _1HanPages=new int[1] {1};
    public List<Sprite> _2Han;
    public List<Sprite> _3Han;
    public List<Sprite> _6Han;
    public List<Sprite> Mangan;
    public List<Sprite> Draw;
    
    public Transform YakuContainer;

    public GameObject YakuPrefab;

    public int elementsPerPage = 2;

    public List<List<Sprite>> Pages=new List<List<Sprite>>();
    public int pageIndex = 0;

    public GameObject pageButtonBox;
    public GameObject LeftButton;
    public GameObject RightButton;
    public TextMeshProUGUI PageIndicator;

    bool changedPos = false;

    void Start()
    {
        Buttons[0].onClick.AddListener(On1HanButtonClicked);
        Buttons[1].onClick.AddListener(On2HanButtonClicked);
        Buttons[2].onClick.AddListener(On3HanButtonClicked);
        Buttons[3].onClick.AddListener(On6HanButtonClicked);
        Buttons[4].onClick.AddListener(OnManganButtonClicked);
        Buttons[5].onClick.AddListener(OnYakumanButtonClicked);
        Buttons[6].onClick.AddListener(On2YakumanButtonClicked);
        Buttons[7].onClick.AddListener(OnDrawButtonClicked);
        LeftButton.GetComponent<Button>().onClick.AddListener(OnLeftButtonClicked);
        RightButton.GetComponent<Button>().onClick.AddListener(OnRightButtonClicked);

        DrawFirst();
    }

    public void DrawFirst()
    {
        SetSprite(Buttons[0]);
        CreatePages(_1Han,3.58f);
        DrawpageButtons();
        DrawPage();
    }

    private void On1HanButtonClicked()//0
    {
        SetSprite(Buttons[0]);
        CreatePages(_1Han,3.58f);
        DrawpageButtons();
        DrawPage();
    }
    private void On2HanButtonClicked()//1
    {
        SetSprite(Buttons[1]);
        CreatePages(_2Han);
        DrawpageButtons();
        DrawPage();
    }
    private void On3HanButtonClicked()//2
    {
        SetSprite(Buttons[2]);
        CreatePages(_3Han);
        DrawpageButtons();
        DrawPage();
    }
    private void On6HanButtonClicked()//3
    {
        SetSprite(Buttons[3]);
        CreatePages(_6Han);
        DrawpageButtons();
        DrawPage();
    }
    private void OnManganButtonClicked()//4
    {
        SetSprite(Buttons[4]);
        ChangeY(-8.28f);
        CreatePages(Mangan);
        DrawpageButtons();
        DrawPage();
    }
    private void OnYakumanButtonClicked()//5
    {

    }
    private void On2YakumanButtonClicked()//6
    {

    }
    private void OnDrawButtonClicked()//7
    {
        SetSprite(Buttons[7]);
        ChangeY(-8.84f);
        CreatePages(Draw);
        DrawpageButtons();
        DrawPage();
    }

    private void OnLeftButtonClicked()
    {
        pageIndex--;
        DrawPage();
        DrawpageButtons();
    }

    private void OnRightButtonClicked()
    {
        pageIndex++;
        DrawPage();
        DrawpageButtons();
    }

    private void SetSprite(Button _button)
    {
        foreach (Button button in Buttons)
        {
            button.image.sprite = DefaultSprite;
        }
        _button.image.sprite = SelectedSprite;
    }

    private void CreatePages(List<Sprite> _sprites,float spacing=3.39f)
    {
        YakuContainer.GetComponent<VerticalLayoutGroup>().spacing=spacing;
        pageIndex = 0;
        Pages.Clear();
        int count = 0;
        List<Sprite> page = new List<Sprite>();
        foreach (var sprite in _sprites)
        {
            if (count == elementsPerPage)
            {
                Pages.Add(page);
                page = new List<Sprite>();
                count = 0;
            }
            page.Add(sprite);
            count++;
        }
        if (page.Count > 0) Pages.Add(page);
    }
    private void DrawpageButtons()
    {
        if (Pages.Count <= 1)
        {
            pageButtonBox.SetActive(false);
            return;
        }
        else pageButtonBox.SetActive(true);

        if (pageIndex==0) LeftButton.SetActive(false);
        else LeftButton.SetActive(true);

        if (pageIndex == Pages.Count - 1) RightButton.SetActive(false);
        else RightButton.SetActive(true);

        PageIndicator.text = (pageIndex + 1).ToString() + "/" + Pages.Count.ToString();
    }

    private void DrawPage()
    {
        ChangeY();
        foreach (Transform child in YakuContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var sprite in Pages[pageIndex])
        {
            GameObject yaku = Instantiate(YakuPrefab, YakuContainer);
            yaku.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        changedPos = false;
    }

    private void ChangeY(float y= -8.1f)
    {
        var rect = YakuContainer.GetComponent<RectTransform>();
        if (!changedPos)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
            changedPos = true;
        }
    }
}
