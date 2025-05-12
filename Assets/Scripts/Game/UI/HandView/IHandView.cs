using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IHandView: MonoBehaviour
{
    public GameObject LiyingTile; // Префаб плитки
    public GameObject StandingTile;
    public GameObject CloseLiyingTile;
    public Transform HandContainer; // Контейнер для плиток
    public Transform HandContainerClone; //Дубликат контейнера для анимации
    public GameManager GameManager;

    public int start_sortingOrder;
    public int sotingMultiplier;

    public int scale_multiplier = 1;

    public int turn = 0;

    public abstract void Draw(List<Tile> hand);
    public abstract void Draw(List<Tile> hand,int tilesToDraw);
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

    public void DrawCloseHand(List<Tile> hand)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHandClosing(hand));
    }

    public void DrawRiichiHand(List<Tile> hand)
    {
        foreach (Transform child in HandContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in HandContainerClone)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject tileObject = Instantiate(LiyingTile, HandContainer);
            TileView tileView = tileObject.GetComponent<TileView>();
            tileView.SetTile(hand[i]);
            if (!hand[i].IsDiscardable())
            {
                var blackBox=tileObject.transform.Find("BlackBox");
                blackBox.gameObject.SetActive(true);
            }
            else
            {
                tileView.OnTileClicked.AddListener(GameManager.HandleTileClick);
                tileView.OnTileHoverEnter.AddListener(GameManager.HandleTileHoverEnter);
                tileView.OnTileHoverExit.AddListener(GameManager.HandleTileHoverExit);
            }           
        }
    }

    private IEnumerator AnimateHandOpening(List<Tile> hand)
    {
        // Очистка контейнеров с проверкой
        ClearContainer(HandContainer);
        ClearContainer(HandContainerClone);

        // Создаем списки для анимации
        List<GameObject> tileTops = new List<GameObject>();
        List<GameObject> tilePrefabs = new List<GameObject>();

        var temp_list=new List<Tile>(hand);

        // Создаем все элементы сразу
        for (int i = 0; i < temp_list.Count; i++)
        {
            // Создаем TileTop в клоне
            GameObject top = Instantiate(StandingTile, HandContainerClone);
            var sprite1 = top.GetComponent<SpriteRenderer>();
            sprite1.sortingOrder = i;
            top.GetComponent<TileView>().SetTile(temp_list[i]);
            top.transform.localPosition = Vector3.zero;
            SetupTileAlpha(top, 1f);
            tileTops.Add(top);

            // Создаем TilePrefab в основном контейнере
            GameObject prefab = Instantiate(LiyingTile, HandContainer);

            var rect = prefab.GetComponent<RectTransform>();

            var scale=rect.localScale;
            rect.localScale = new Vector3(scale.x * scale_multiplier, scale.y, scale.z);

            rect.localRotation = rect.localRotation * Quaternion.Euler(0, 0, turn);

            var sprite = prefab.GetComponent<SpriteRenderer>();
            sprite.sortingOrder = i;
            prefab.GetComponent<TileView>().SetTile(temp_list[i]);
            SetupTileAlpha(prefab, 0f);
            tilePrefabs.Add(prefab);
        }

        // Анимируем по одному тайлу
        for (int i = 0; i < tileTops.Count; i++)
        {
            yield return StartCoroutine(AnimateTile(tileTops[i], tilePrefabs[i]));
            yield return new WaitForSeconds(0.01f);
        }
        
        // Удаляем закрытые плитки после анимации
        ClearContainer(HandContainerClone);
    }

    private IEnumerator AnimateTile(GameObject top, GameObject prefab)
    {
        float duration = 0.03f;
        float elapsed = 0f;

        // Получаем все визуальные компоненты
        var topRenderers = GetVisualComponents(top);
        var prefabRenderers = GetVisualComponents(prefab);

        while (elapsed < duration)
        {
            if (top == null || prefab == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Анимируем все компоненты верхней плитки
            foreach (var visual in topRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, 1 - t, visual.isImage);
            }

            // Анимируем все компоненты префаба
            foreach (var visual in prefabRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, t, visual.isImage);
            }

            yield return null;
        }

        // Финальное состояние
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
        // Старая логика для Image / SpriteRenderer
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

        // Новая логика для Canvas
        // если на том же объекте или на детях есть Canvas — его включаем/выключаем в зависимости от alpha
        // NOTE: мы проверяем здесь только сам компонент, а не его прозрачность
        if (rendererComponent is Canvas canvas)
        {
            canvas.enabled = alpha >= 0.99f;
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

        // Основной SpriteRenderer
        if (obj.TryGetComponent<SpriteRenderer>(out var sr) && !IsDiscardArrow(obj))
            components.Add((sr, false));

        // Рекурсивно обходим всех детей
        foreach (Transform child in obj.transform)
        {
            ProcessChildComponents(child.gameObject, components);
        }

        return components;
    }

    private void ProcessChildComponents(GameObject child, List<(Component, bool)> components)
    {
        // Пропускаем DiscardArrow и его детей
        if (child.name == "DiscardArrow") return;

        // Добавляем компоненты текущего ребенка
        if (child.TryGetComponent<Image>(out var image))
            components.Add((image, true));

        if (child.TryGetComponent<SpriteRenderer>(out var sr))
            components.Add((sr, false));

        if (child.TryGetComponent<Canvas>(out var canvas))
            components.Add((canvas, false));

        // Рекурсивно обрабатываем вложенные объекты
        foreach (Transform grandChild in child.transform)
        {
            ProcessChildComponents(grandChild.gameObject, components);
        }
    }

    private bool IsDiscardArrow(GameObject obj)
    {
        return obj.name.Equals("DiscardArrow", System.StringComparison.OrdinalIgnoreCase);
    }    

    private IEnumerator AnimateHandClosing(List<Tile> hand)
    {
        // Очищаем оба контейнера
        ClearContainer(HandContainer);
        ClearContainer(HandContainerClone);

        var tempList = new List<Tile>(hand);
        var tileFaces = new List<GameObject>();
        var tileBacks = new List<GameObject>();

        // Сначала создаём все лицевые тайлы и задники
        for (int i=0;i<tempList.Count;i++)
        {
            // Лицевая сторона (TilePrefab) в основном контейнере
            var face = Instantiate(LiyingTile, HandContainer);

            var sprite1 = face.GetComponent<SpriteRenderer>();
            sprite1.sortingOrder = start_sortingOrder+ sotingMultiplier * i;

            face.GetComponent<TileView>().SetTile(tempList[i]);
            SetupTileAlpha(face, 1f); // сразу полностью видна
            tileFaces.Add(face);

            // Задник (TileBack) в клоне
            var back = Instantiate(CloseLiyingTile, HandContainerClone);

            var sprite2 = back.GetComponent<SpriteRenderer>();
            sprite2.sortingOrder = start_sortingOrder + sotingMultiplier * i;

            back.transform.localPosition = Vector3.zero;
            SetupTileAlpha(back, 0f); // сначала невидим
            tileBacks.Add(back);
        }

        // По одному «переворачиваем» тайлы
        for (int i = 0; i < tileFaces.Count; i++)
        {
            yield return StartCoroutine(AnimateTileClosing(tileFaces[i], tileBacks[i]));
            yield return new WaitForSeconds(0.01f);
        }

        // Удаляем лицевые тайлы-клоны (если нужно)
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

            // Лицевая часть становится прозрачнее
            foreach (var visual in faceRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, 1 - t, visual.isImage);
            }
            // А задник — наоборот, проявляется
            foreach (var visual in backRenderers)
            {
                if (visual.renderer != null)
                    SetAlpha(visual.renderer, t, visual.isImage);
            }

            yield return null;
        }

        // Финальное состояние: лицевая—0, задник—1
        foreach (var visual in faceRenderers)
            if (visual.renderer != null)
                SetAlpha(visual.renderer, 0, visual.isImage);

        foreach (var visual in backRenderers)
            if (visual.renderer != null)
                SetAlpha(visual.renderer, 1, visual.isImage);
    }
}