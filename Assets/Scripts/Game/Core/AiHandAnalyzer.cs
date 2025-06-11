using System;
using System.Collections.Generic;
using System.Linq;
class AiHandAnalyzer
{
    private Dictionary<Tile, int> _usedTileCache;
    private IPlayer Player { get; set; }
    private GameManager GameManager { get; set; }

    public AiHandAnalyzer(IPlayer player)
    {
        Player = player;
        GameManager = player.GameManager;
    }

    public Tile GetBestDiscard()
    {
        List<Tile> originalHand = new List<Tile>(Player.Hand);
        int originalShanten = RemoveCompletedGroups(originalHand);

        Tile bestDiscard = null;
        float maxEfficiency = -1;

        foreach (Tile tile in originalHand)
        {
            // Формируем временную руку без текущего кандидата на сброс
            var tempHand = new List<Tile>(originalHand);
            tempHand.Remove(tile);

            // Получаем и efficiency, и новый шантен
            var (efficiency, newShanten) = CalculateBasicEfficiency(tempHand);

            // Пропускаем варианты, которые ухудшают шантен
            if (newShanten > originalShanten)
                continue;

            // Выбираем сброс с максимальной basic efficiency
            if (efficiency > maxEfficiency)
            {
                maxEfficiency = efficiency;
                bestDiscard = tile;
            }
        }

        return bestDiscard ?? originalHand.First();
    }

    private (int efficiency, int shanten) CalculateBasicEfficiency(List<Tile> hand)
    {
        // 1) Считаем шантен для текущей руки
        int currentShanten = ShantenCalculator.CalculateShanten(hand);

        // 2) Перебираем все типы тайлов и накапливаем оставшиеся улучшающие тайлы
        int eff = 0;
        foreach (Tile t in Tile.AllTiles)
        {
            int remain = GetRemainingTileCount(t);
            if (remain <= 0) continue;

            hand.Add(t);
            int newShanten = ShantenCalculator.CalculateShanten(hand);
            hand.RemoveAt(hand.Count - 1);

            if (newShanten < currentShanten)
                eff += remain;
        }

        return (eff, currentShanten);
    }

    private int GetRemainingTileCount(Tile tile)
    {
        int count = 4;

        // Проверяем руку игрока
        count -= Player.Hand.Count(t => t.Equals(tile));

        // Проверяем дискарды и объявления всех игроков
        foreach (var player in GameManager.Players)
        {
            count -= player.Discard.Count(t => t.Equals(tile));
            count -= player.Calls.Sum(call => call.Count(t => t.Equals(tile)));
        }

        return Math.Max(0, count);
    }    
    private int RemoveCompletedGroups(List<Tile> hand)
    {
        // Отсортируем руку для удобства
        Sort(hand);
        // Изначальное число групп, уже вызванных игроком
        int groups_removed = Player.Calls.Count;

        // Удаление понов (три одинаковых тайла)
        bool ponFound;
        do
        {
            ponFound = false;
            // Группируем по масти и рангу
            var ponGroup = hand
                .GroupBy(t => new { t.Suit, t.Rank })
                .FirstOrDefault(g => g.Count() >= 3);
            if (ponGroup != null)
            {
                // Удаляем три тайла
                var tiles = hand.Where(t => t.Suit == ponGroup.Key.Suit && t.Rank == ponGroup.Key.Rank)
                              .Take(3)
                              .ToList();
                foreach (var tile in tiles)
                    hand.Remove(tile);

                groups_removed++;
                ponFound = true;
            }
        } while (ponFound);

        // Удаление чи (последовательностей из трех тайлов одной масти)
        bool chiFound;
        do
        {
            chiFound = false;
            // Проходим по всем мастям
            foreach (var suitGroup in hand.GroupBy(t => t.Suit))
            {
                // Преобразуем ранги в числа
                var ranks = suitGroup
                    .Select(t => t.TryGetRankAsInt())
                    .OrderBy(r => r)
                    .ToList();

                // Ищем последовательность i, i+1, i+2
                for (int i = 0; i < ranks.Count - 2; i++)
                {
                    int r = ranks[i];
                    if (ranks.Contains(r + 1) && ranks.Contains(r + 2))
                    {
                        // Удаляем по одному тайлу с рангами r, r+1 и r+2
                        var tile1 = hand.First(t => t.Suit == suitGroup.Key && t.Rank == r.ToString());
                        var tile2 = hand.First(t => t.Suit == suitGroup.Key && t.Rank == (r + 1).ToString());
                        var tile3 = hand.First(t => t.Suit == suitGroup.Key && t.Rank == (r + 2).ToString());

                        hand.Remove(tile1);
                        hand.Remove(tile2);
                        hand.Remove(tile3);

                        groups_removed++;
                        chiFound = true;
                        break;
                    }
                }
                if (chiFound)
                    break;
            }
        } while (chiFound);

        // Пересчет шантена после удаления групп
        int shanten = ShantenCalculator.CalculateShanten(hand, groups_removed);
        return shanten;
    }


    public void Sort(List<Tile> hand)
    {
        int count;
        if (hand.Count % 3 == 1)
            count = hand.Count;
        else
            count = hand.Count - 1;

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
        {
            switch (tile2.Suit)
            {
                case "Dragon":
                    return DragonOrder[tile1.Rank] > DragonOrder[tile2.Rank];
                case "Wind":
                    return WindOrder[tile1.Rank] > WindOrder[tile2.Rank];
                default:
                    return int.Parse(tile1.Rank) > int.Parse(tile2.Rank);
            }
        }
        else
            return RankOrder[tile1.Suit] > RankOrder[tile2.Suit];
    }
}

public static class ShantenCalculator
{
    public static int CalculateShanten(List<Tile> hand, int completedGroups = 0)
    {
        // Группируем тайлы
        var man = new int[10];
        var pin = new int[10];
        var sou = new int[10];
        var honor = new int[7];

        foreach (var t in hand)
        {
            if (t.Suit=="Wind"|| t.Suit == "Dragon")
                honor[HonorIndex(t)]++;
            else
            {
                int r = t.TryGetRankAsInt();
                switch (t.Suit)
                {
                    case "Man": man[r]++; break;
                    case "Pin": pin[r]++; break;
                    case "Sou": sou[r]++; break;
                }
            }
        }

        // Считаем блоки и флаг пары для каждой группы
        bool hasPair = false;
        int totalBlocks = 0;

        var res1 = CountBlocksAndPair(man);
        totalBlocks += res1.blocks; hasPair |= res1.hasPair;

        var res2 = CountBlocksAndPair(pin);
        totalBlocks += res2.blocks; hasPair |= res2.hasPair;

        var res3 = CountBlocksAndPair(sou);
        totalBlocks += res3.blocks; hasPair |= res3.hasPair;

        var res4 = CountBlocksAndPair(honor);
        totalBlocks += res4.blocks; hasPair |= res4.hasPair;

        // Формула shanten
        int shanten = 8
                      - 2 * completedGroups
                      - totalBlocks;
        if (!hasPair) shanten++;
        if (totalBlocks >= 6) shanten++;

        return shanten;
    }

    private static int HonorIndex(Tile t)
    {
        return t.Suit switch
        {
            "Wind" => t.Rank switch
            {
                "East" => 0,
                "South" => 1,
                "West" => 2,
                "North" => 3,
                _ => throw new ArgumentException($"Unknown wind: {t.Rank}")
            },
            "Dragon" => t.Rank switch
            {
                "White" => 4,
                "Green" => 5,
                "Red" => 6,
                _ => throw new ArgumentException($"Unknown dragon: {t.Rank}")
            },
            _ => throw new ArgumentException($"Tile is not honor: {t.Suit}")
        };
    }

    /// <summary>
    /// Возвращает (blocks, hasPair) по массиву счётчиков cnt.
    /// </summary>
    private static (int blocks, bool hasPair) CountBlocksAndPair(int[] cnt)
    {
        int best = 0;
        bool hasPair = false;

        void Dfs(int[] c, int idx, int blocks, bool pair)
        {
            while (idx < c.Length && c[idx] == 0) idx++;
            if (idx >= c.Length)
            {
                if (pair) hasPair = true;
                best = Math.Max(best, blocks);
                return;
            }

            // Триплет
            if (c[idx] >= 3)
            {
                c[idx] -= 3;
                Dfs(c, idx, blocks + 1, pair);
                c[idx] += 3;
            }

            // Стрейт / рянмен / канчан (только для длины >7)
            if (c.Length > 7 && idx <= c.Length - 3)
            {
                if (c[idx] > 0 && c[idx + 1] > 0 && c[idx + 2] > 0)
                {
                    c[idx]--; c[idx + 1]--; c[idx + 2]--;
                    Dfs(c, idx, blocks + 1, pair);
                    c[idx]++; c[idx + 1]++; c[idx + 2]++;
                }
                if (c[idx] > 0 && c[idx + 1] > 0)
                {
                    c[idx]--; c[idx + 1]--;
                    Dfs(c, idx, blocks + 1, pair);
                    c[idx]++; c[idx + 1]++;
                }
                if (c[idx] > 0 && c[idx + 2] > 0)
                {
                    c[idx]--; c[idx + 2]--;
                    Dfs(c, idx, blocks + 1, pair);
                    c[idx]++; c[idx + 2]++;
                }
            }

            // Пара (раз)
            if (!pair && c[idx] >= 2)
            {
                c[idx] -= 2;
                Dfs(c, idx, blocks, true);
                c[idx] += 2;
            }

            // Пропустить один тайл
            c[idx]--;
            Dfs(c, idx, blocks, pair);
            c[idx]++;
        }

        Dfs((int[])cnt.Clone(), 0, 0, false);
        return (best, hasPair);
    }
}