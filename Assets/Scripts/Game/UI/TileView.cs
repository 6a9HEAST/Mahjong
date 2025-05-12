using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [Header("Main Images")]
    public Image TileImage;      // ������ �� ��������� Image ����� ������
    public Image DiscardArrow;   // ������ �� ������� ������
    public GameObject YellowArrow;

    [Header("Rank Display")]
    public GameObject RedNumber;      // ������������ ������ ��� �����
    public TextMeshProUGUI RankText;  // ��� ��������� ���������
    public TextMeshProUGUI RankText2;

    private Tile tileData;
    public UnityEvent<Tile> OnTileClicked;
    public UnityEvent<TileView> OnTileHoverEnter;
    public UnityEvent<TileView> OnTileHoverExit;

    //public GameManager gameManager;

    private bool displayRedNumber;

    private void Awake()
    {
        if (OnTileClicked == null)
            OnTileClicked = new UnityEvent<Tile>();
        if(OnTileHoverEnter == null)
            OnTileHoverEnter = new UnityEvent<TileView>();       // :contentReference[oaicite:1]{index=1}
        if (OnTileHoverExit == null)
            OnTileHoverExit = new UnityEvent<TileView>();
        
    }

    private void OnEnable()
    {
        displayRedNumber = GameManager.Instance.DISPLAY_TILE_RED_NUMBER;
    }

    public void SetTile(Tile tile)
    {
        tileData = tile;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        
        if (TileImage != null)
        {
            TileImage.sprite = TileSpriteManager.GetSprite(tileData);
            TileImage.preserveAspect = true;
        }

        
        if (DiscardArrow != null)
        {
            if (tileData.IsDiscardable())
            {
                DiscardArrow.sprite = TileSpriteManager.GetDiscardArrow();
                DiscardArrow.preserveAspect = true;
                DiscardArrow.color = Color.white;
            }
            else
            {
                DiscardArrow.color = new Color(0, 0, 0, 0);
            }
        }

        // ��������� ����������� �����
        if (RedNumber != null && RankText != null&&RankText2!=null)
        {
            if (displayRedNumber)
                if (tileData.Suit == "Dragon" || tileData.Suit == "Wind")
                {
                    RankText.text = RankToLetter[tileData.Rank];
                    RankText2.text = RankToLetter[tileData.Rank];
                }
                else
                {
                    RankText.text = tileData.Rank.ToString();
                    RankText2.text = tileData.Rank.ToString();
                }
            else RedNumber.SetActive(false);
        }
        //else RedNumber.SetActive(false);
    }

    public Dictionary<string, string> RankToLetter = new Dictionary<string, string>()
    {
        {"East", "�"},
        {"West", "�"},
        {"South", "�"},
        {"North", "�"},
        {"Red", "�"},
        {"Green", "�"},
        {"White", "�"},
    };

    //public void HandleTileHoverEnter(Tile t)
    //{
    //    YellowArrow.SetActive(true);
    //}
    //public void HandleTileHoverExit(Tile t)
    //{
    //    YellowArrow.SetActive(false);
    //}

    public void EnableYellowArrow()
    {
        YellowArrow.SetActive(true);
    }
    public void DisableYellowArrow()
    {
        YellowArrow.SetActive(false);
    }

    private void OnMouseDown()
    {
        OnTileClicked.Invoke(tileData);
    }

    private void OnMouseEnter()
    {
        OnTileHoverEnter.Invoke(this);   // ������ ������
    }

    private void OnMouseExit()
    {
        OnTileHoverExit.Invoke(this);    // ������ ������
    }
}
