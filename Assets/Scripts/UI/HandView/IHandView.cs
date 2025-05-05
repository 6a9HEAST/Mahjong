using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IHandView: MonoBehaviour
{
    public GameObject TilePrefab; // ������ ������
    public GameObject TileTop;
    public GameObject TileBack;
    public Transform HandContainer; // ��������� ��� ������
    public Transform HandContainerClone; //�������� ���������� ��� ��������
    public GameManager GameManager;

    public abstract void Draw(List<Tile> hand);
    public void Sort(List<Tile> hand)
    {
        int count;
        if (hand.Count % 3 == 1)
            count = hand.Count;
        else count = hand.Count - 1;

        for (int i = 0; i < count - 1; i++)
        {
            for (int j = 0; j < count - i - 1; j++)
            {
                if (IsBigger(hand[j], hand[j + 1]))
                {
                    Tile temp = hand[j];
                    hand[j] = hand[j + 1];
                    hand[j + 1] = temp;
                }
            }
        }
    }
    Dictionary<string, int> RankOrder = new Dictionary<string, int>()
        {
            {"Man", 0},
            {"Pin", 1},
            {"Sou", 2},
            {"Wind", 3},
            {"Dragon", 4}
        };
    Dictionary<string, int> WindOrder = new Dictionary<string, int>()
    {
        {"East", 0},
        {"South", 1},
        {"West", 2},
        {"North", 3}
    };
    Dictionary<string, int> DragonOrder = new Dictionary<string, int>()
    {
        {"Red", 0},
        {"White", 1},
        {"Green", 2}
    };
    public bool IsBigger(Tile tile1, Tile tile2)
    {
        if (tile1.Suit == tile2.Suit)
            switch (tile2.Suit)
            {
                case "Dragon":
                    return DragonOrder[tile1.Rank] > DragonOrder[tile2.Rank];
                case "Wind":
                    return WindOrder[tile1.Rank] > WindOrder[tile2.Rank];
                default:
                    return int.Parse(tile1.Rank) > int.Parse(tile2.Rank);
            }
        else
            return RankOrder[tile1.Suit] > RankOrder[tile2.Suit];

    }

    public void DrawOpenHand(List<Tile> hand)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHandOpening(hand));
    }

    private IEnumerator AnimateHandOpening(List<Tile> hand)
    {
        // ������� ����������� � ���������
        ClearContainer(HandContainer);
        ClearContainer(HandContainerClone);

        // ������� ������ ��� ��������
        List<GameObject> tileTops = new List<GameObject>();
        List<GameObject> tilePrefabs = new List<GameObject>();

        var temp_list=new List<Tile>(hand);

        // ������� ��� �������� �����
        foreach (Tile tile in temp_list)
        {
            // ������� TileTop � �����
            GameObject top = Instantiate(TileTop, HandContainerClone);
            top.transform.localPosition = Vector3.zero;
            SetupTileAlpha(top, 1f);
            tileTops.Add(top);

            // ������� TilePrefab � �������� ����������
            GameObject prefab = Instantiate(TilePrefab, HandContainer);
            prefab.GetComponent<TileView>().SetTile(tile);
            SetupTileAlpha(prefab, 0f);
            tilePrefabs.Add(prefab);
        }

        // ��������� �� ������ �����
        for (int i = 0; i < tileTops.Count; i++)
        {
            yield return StartCoroutine(AnimateTile(tileTops[i], tilePrefabs[i]));
            yield return new WaitForSeconds(0.01f);
        }

        // ������� �������� ������ ����� ��������
        ClearContainer(HandContainerClone);
    }

    private IEnumerator AnimateTile(GameObject top, GameObject prefab)
    {
        float duration = 0.03f;
        float elapsed = 0f;

        // �������� ��� ���������� ����������
        var topRenderers = GetVisualComponents(top);
        var prefabRenderers = GetVisualComponents(prefab);

        while (elapsed < duration)
        {
            if (top == null || prefab == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // ��������� ��� ���������� ������� ������
            foreach (var visual in topRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, 1 - t, visual.isImage);
            }

            // ��������� ��� ���������� �������
            foreach (var visual in prefabRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, t, visual.isImage);
            }

            yield return null;
        }

        // ��������� ���������
        foreach (var visual in topRenderers)
        {
            if (visual.renderer != null)
                SetAlpha(visual.renderer, 0, visual.isImage);
        }

        //Destroy(top);
    }
    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetAlpha(Component rendererComponent, float alpha, bool isImage)
    {
        if (isImage && rendererComponent is Image image)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
        else if (!isImage && rendererComponent is SpriteRenderer sr)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void SetupTileAlpha(GameObject tile, float alpha)
    {
        foreach (var visual in GetVisualComponents(tile))
        {
            SetAlpha(visual.renderer, alpha, visual.isImage);
        }
    }

    private List<(Component renderer, bool isImage)> GetVisualComponents(GameObject obj)
    {
        var components = new List<(Component, bool)>();

        // �������� SpriteRenderer
        if (obj.TryGetComponent<SpriteRenderer>(out var sr) && !IsDiscardArrow(obj))
            components.Add((sr, false));

        // ���������� ������� ���� �����
        foreach (Transform child in obj.transform)
        {
            ProcessChildComponents(child.gameObject, components);
        }

        return components;
    }

    private void ProcessChildComponents(GameObject child, List<(Component, bool)> components)
    {
        // ���������� DiscardArrow � ��� �����
        if (child.name == "DiscardArrow") return;

        // ��������� ���������� �������� �������
        if (child.TryGetComponent<Image>(out var image))
            components.Add((image, true));

        if (child.TryGetComponent<SpriteRenderer>(out var sr))
            components.Add((sr, false));

        // ���������� ������������ ��������� �������
        foreach (Transform grandChild in child.transform)
        {
            ProcessChildComponents(grandChild.gameObject, components);
        }
    }

    private bool IsDiscardArrow(GameObject obj)
    {
        return obj.name.Equals("DiscardArrow", System.StringComparison.OrdinalIgnoreCase);
    }

    public void DrawCloseHand(List<Tile> hand)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHandClosing(hand));
    }

    private IEnumerator AnimateHandClosing(List<Tile> hand)
    {
        // ������� ��� ����������
        ClearContainer(HandContainer);
        ClearContainer(HandContainerClone);

        var tempList = new List<Tile>(hand);
        var tileFaces = new List<GameObject>();
        var tileBacks = new List<GameObject>();

        // ������� ������ ��� ������� ����� � �������
        foreach (var tile in tempList)
        {
            // ������� ������� (TilePrefab) � �������� ����������
            var face = Instantiate(TilePrefab, HandContainer);
            face.GetComponent<TileView>().SetTile(tile);
            SetupTileAlpha(face, 1f); // ����� ��������� �����
            tileFaces.Add(face);

            // ������ (TileBack) � �����
            var back = Instantiate(TileBack, HandContainerClone);
            back.transform.localPosition = Vector3.zero;
            SetupTileAlpha(back, 0f); // ������� �������
            tileBacks.Add(back);
        }

        // �� ������ ��������������� �����
        for (int i = 0; i < tileFaces.Count; i++)
        {
            yield return StartCoroutine(AnimateTileClosing(tileFaces[i], tileBacks[i]));
            yield return new WaitForSeconds(0.01f);
        }

        // ������� ������� �����-����� (���� �����)
        ClearContainer(HandContainer);
    }

    private IEnumerator AnimateTileClosing(GameObject face, GameObject back)
    {
        float duration = 0.03f;
        float elapsed = 0f;

        var faceRenderers = GetVisualComponents(face);
        var backRenderers = GetVisualComponents(back);

        while (elapsed < duration)
        {
            if (face == null || back == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // ������� ����� ���������� ����������
            foreach (var visual in faceRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, 1 - t, visual.isImage);
            }
            // � ������ � ��������, �����������
            foreach (var visual in backRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, t, visual.isImage);
            }

            yield return null;
        }

        // ��������� ���������: ��������0, ������1
        foreach (var visual in faceRenderers)
            if (visual.renderer != null)
                SetAlpha(visual.renderer, 0, visual.isImage);

        foreach (var visual in backRenderers)
            if (visual.renderer != null)
                SetAlpha(visual.renderer, 1, visual.isImage);
    }
}