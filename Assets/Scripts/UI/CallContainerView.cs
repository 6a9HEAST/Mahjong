using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallContainerView : MonoBehaviour
{
    public GameObject CallPrefab;
    public GameObject TilePrefab;
    public GameObject TileBackPrefab;
    public Transform CallContainer;
    public void Clear()
    {
        foreach (Transform child in CallContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void Draw(List<List<Tile>> calls)
    {
        // Очистка контейнера вызовов
        foreach (Transform child in CallContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (List<Tile> call in calls)
        {
            if (call.Count == 4)
            {
                DrawKan(call);
                continue;
            }
            GameObject callObj = Instantiate(CallPrefab, CallContainer);
            Transform tileContainer = callObj.transform;

            int i = 0;
            while (i < call.Count)
            {
                // Условие для формирования composite-группы: текущий и следующий тайлы неповернуты
                bool currentNonRotated = (call[i].Properties == null || !call[i].Properties.Contains("Called"));
                if (i < call.Count - 1)
                {
                    bool nextNonRotated = (call[i + 1].Properties == null || !call[i + 1].Properties.Contains("Called"));
                    if (currentNonRotated && nextNonRotated)
                    {
                        GameObject pairGroup = new GameObject("TilePairGroup", typeof(RectTransform));
                        pairGroup.transform.SetParent(tileContainer.transform, false);
                        
                        HorizontalLayoutGroup pairHLG = pairGroup.AddComponent<HorizontalLayoutGroup>();
                        pairHLG.spacing = 1.35f;
                        pairHLG.childAlignment = TextAnchor.MiddleCenter;
                        pairHLG.childForceExpandHeight = false;
                        pairHLG.childForceExpandWidth = false;

                        // Добавляем первый тайл в пару
                        GameObject tile1 = Instantiate(TilePrefab, pairGroup.transform);
                        TileView tile1View = tile1.GetComponent<TileView>();
                        if (tile1View != null)
                        {
                            tile1View.SetTile(call[i]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                        }

                        // Добавляем второй тайл в пару
                        GameObject tile2 = Instantiate(TilePrefab, pairGroup.transform);
                        TileView tile2View = tile2.GetComponent<TileView>();
                        if (tile2View != null)
                        {
                            tile2View.SetTile(call[i + 1]);
                        }
                        else
                        {
                            Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                        }

                        // Обрабатываем два тайла и переходим дальше
                        i += 2;
                        continue;
                    }
                }

                
                GameObject tileObj = Instantiate(TilePrefab, tileContainer);
                // Если у тайла в Properties содержится "Called", поворачиваем его на 90 градусов
                if (call[i].Properties != null && call[i].Properties.Contains("Called"))
                {
                    tileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(call[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }

                i++;
            }
        }
    }

    public void DrawKan(List<Tile> kan)
    {
        // Создаём объект для кан (Kan) по callprefab и получаем его контейнер для тайлов.
        GameObject callObj = Instantiate(CallPrefab, CallContainer);
        Transform tileContainer = callObj.transform;

        // Подсчитываем количество тайлов с "Called"
        int calledCount = 0;
        foreach (Tile tile in kan)
        {
            if (tile.Properties != null && tile.Properties.Contains("Called"))
            {
                calledCount++;
            }
        }

        // Сценарий 1: ровно 1 тайл с "Called" (учитываем только первые 3 тайла, 4-й никогда не переворачивается)
        if (calledCount == 1)
        {
            int calledIndex = -1;
            for (int i = 0; i < 3; i++)
            {
                if (kan[i].Properties != null && kan[i].Properties.Contains("Called"))
                {
                    calledIndex = i;
                    break;
                }
            }
            if (calledIndex == -1)
            {
                Debug.LogWarning("В кане ожидается один 'Called', но он не найден среди первых трёх тайлов.");
                calledIndex = 0;
            }

            // В данном случае всё размещается в callObj.
            // Создаём дочерний контейнер для группы нормальных тайлов с HorizontalLayoutGroup.
            GameObject trioGroup = new GameObject("NormalTilesGroup", typeof(RectTransform));
            trioGroup.transform.SetParent(tileContainer, false);
            HorizontalLayoutGroup trioHLG = trioGroup.AddComponent<HorizontalLayoutGroup>();
            trioHLG.spacing = 1.3f; // Отступ между тайлами внутри тройки
            trioHLG.childAlignment = TextAnchor.MiddleCenter;
            trioHLG.childForceExpandHeight = false;
            trioHLG.childForceExpandWidth = false;

            // В зависимости от позиции перевёрнутого тайла:
            // Если перевёрнутый тайл находится на позиции 0 – сначала добавляем его, затем группу остальных.
            // Если перевёрнутый тайл на позиции 1 или 2 – сначала добавляем группу остальных, затем перевёрнутый тайл.
            if (calledIndex == 0)
            {
                // Добавляем перевёрнутый тайл
                GameObject rotatedTileObj = Instantiate(TilePrefab, tileContainer, false);
                rotatedTileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView rotatedTileView = rotatedTileObj.GetComponent<TileView>();
                if (rotatedTileView != null)
                {
                    rotatedTileView.SetTile(kan[calledIndex]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
                // Затем устанавливаем контейнер для нормальных тайлов
                trioGroup.transform.SetParent(tileContainer, false);
            }
            else
            {
                // Сначала добавляем группу нормальных тайлов, затем перевёрнутый тайл
                trioGroup.transform.SetParent(tileContainer, false);
                GameObject rotatedTileObj = Instantiate(TilePrefab, tileContainer, false);
                rotatedTileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView rotatedTileView = rotatedTileObj.GetComponent<TileView>();
                if (rotatedTileView != null)
                {
                    rotatedTileView.SetTile(kan[calledIndex]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }

            // Собираем остальные три тайла (без перевёрнутого)
            for (int i = 0; i < kan.Count; i++)
            {
                if (i == calledIndex) continue;
                GameObject tileObj = Instantiate(TilePrefab, trioGroup.transform, false);
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }
        }
        // Сценарий 2: ровно 2 тайла с "Called"
        else if (calledCount == 2)
        {
            // Предполагается, что два тайла с "Called" идут подряд.
            int groupStart = -1;
            for (int i = 0; i < kan.Count - 1; i++)
            {
                if (kan[i].Properties != null && kan[i].Properties.Contains("Called") &&
                    kan[i + 1].Properties != null && kan[i + 1].Properties.Contains("Called"))
                {
                    groupStart = i;
                    break;
                }
            }
            if (groupStart == -1)
            {
                Debug.LogWarning("Некорректный Kan: два тайла с 'Called' не идут подряд.");
                groupStart = 0;
            }

            // Разбиваем массив на три части:
            // Левый блок – тайлы перед группой перевёрнутых,
            // Центральный блок – группа перевёрнутых (2 тайла),
            // Правый блок – тайлы после группы перевёрнутых.
            List<Tile> leftGroup = new List<Tile>();
            for (int i = 0; i < groupStart; i++)
                leftGroup.Add(kan[i]);

            List<Tile> calledGroup = new List<Tile>() { kan[groupStart], kan[groupStart + 1] };

            List<Tile> rightGroup = new List<Tile>();
            for (int i = groupStart + 2; i < kan.Count; i++)
                rightGroup.Add(kan[i]);

            // Используем callObj как родительский контейнер (уже создан callprefab).

            // Если есть левый блок – добавляем его.
            if (leftGroup.Count > 0)
            {
                GameObject leftContainer = new GameObject("LeftGroup", typeof(RectTransform));
                leftContainer.transform.SetParent(tileContainer, false);
                if (leftGroup.Count > 1)
                {
                    HorizontalLayoutGroup leftHLG = leftContainer.AddComponent<HorizontalLayoutGroup>();
                    leftHLG.spacing = 5f;
                    leftHLG.childAlignment = TextAnchor.MiddleCenter;
                    leftHLG.childForceExpandHeight = false;
                    leftHLG.childForceExpandWidth = false;
                }
                foreach (Tile tile in leftGroup)
                {
                    GameObject tileObj = Instantiate(TilePrefab, leftContainer.transform, false);
                    TileView tileView = tileObj.GetComponent<TileView>();
                    if (tileView != null)
                    {
                        tileView.SetTile(tile);
                    }
                    else
                    {
                        Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                    }
                }
            }

            // Центральный контейнер для перевёрнутых тайлов с VerticalLayoutGroup.
            GameObject centerContainer = new GameObject("CalledGroup", typeof(RectTransform));
            centerContainer.transform.SetParent(tileContainer, false);
            VerticalLayoutGroup centerVLG = centerContainer.AddComponent<VerticalLayoutGroup>();
            centerVLG.spacing = 1.35f;
            centerVLG.childAlignment = TextAnchor.MiddleCenter;
            centerVLG.childForceExpandHeight = false;
            centerVLG.childForceExpandWidth = false;
            // Задаём bottom padding – значение хранится как float, но RectOffset принимает int,
            // поэтому делаем приведение. (Если нужно более точное дробное значение, можно добавить отдельный LayoutElement с minHeight.)
            int bottomPadding = 1;
            RectOffset currentPadding = centerVLG.padding;
            centerVLG.padding = new RectOffset(currentPadding.left, currentPadding.right, currentPadding.top, bottomPadding);

            foreach (Tile tile in calledGroup)
            {
                GameObject tileObj = Instantiate(TilePrefab, centerContainer.transform, false);
                tileObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(tile);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }

            // Если есть правый блок – добавляем его.
            if (rightGroup.Count > 0)
            {
                GameObject rightContainer = new GameObject("RightGroup", typeof(RectTransform));
                rightContainer.transform.SetParent(tileContainer, false);
                if (rightGroup.Count > 1)
                {
                    HorizontalLayoutGroup rightHLG = rightContainer.AddComponent<HorizontalLayoutGroup>();
                    rightHLG.spacing = 1.35f;
                    rightHLG.childAlignment = TextAnchor.MiddleCenter;
                    rightHLG.childForceExpandHeight = false;
                    rightHLG.childForceExpandWidth = false;
                }
                foreach (Tile tile in rightGroup)
                {
                    GameObject tileObj = Instantiate(TilePrefab, rightContainer.transform, false);
                    TileView tileView = tileObj.GetComponent<TileView>();
                    if (tileView != null)
                    {
                        tileView.SetTile(tile);
                    }
                    else
                    {
                        Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                    }
                }
            }
        }
        // Сценарий 3: более 2-х тайлов с "Called"
        else if (calledCount > 2)
        {
            // В этом случае крайние (левый и правый) тайлы отрисовываются с использованием TileBackPrefab,
            // а два центральных – обычным TilePrefab без переворота.
            for (int i = 0; i < kan.Count; i++)
            {
                GameObject tileObj;
                if (i == 0 || i == kan.Count - 1)
                {
                    tileObj = Instantiate(TileBackPrefab, tileContainer, false);
                }
                else
                {
                    tileObj = Instantiate(TilePrefab, tileContainer, false);
                }
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab или TileBackPrefab не содержит компонент TileView.");
                }
            }
        }
        // Если ни одно условие не подошло, отрисовываем тайлы по умолчанию.
        else
        {
            for (int i = 0; i < kan.Count; i++)
            {
                GameObject tileObj = Instantiate(TilePrefab, tileContainer, false);
                TileView tileView = tileObj.GetComponent<TileView>();
                if (tileView != null)
                {
                    tileView.SetTile(kan[i]);
                }
                else
                {
                    Debug.LogWarning("TilePrefab не содержит компонент TileView.");
                }
            }
        }
    }



}
