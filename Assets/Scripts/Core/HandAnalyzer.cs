using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandAnalyzer
{
    IPlayer Player { get; set; }
    YakuAnalyser YakuAnalyser { get; set; }

    public HandAnalyzer(IPlayer player)
    {
        Player = player;
    }

    private HashSet<string> _processedStates;

    public void AnalyzeHand()
    {
        if (Player.index != 0) return;
        // �������� ����� ���� ������
        List<Tile> sortedHand = new List<Tile>(Player.Hand);
        Sort(sortedHand);

        // ���������� ����������� ���������� (calls) � ������ ������� ������
        List<List<Tile>> initialBlocks = new List<List<Tile>>();
        if (Player.Calls != null)
            initialBlocks.AddRange(Player.Calls);

        _processedStates = new HashSet<string>();
        // ��������� ����������� ������ � ����� ���������
        RecursiveAnalyze(sortedHand, initialBlocks, initialBlocks.Count, 0, _processedStates);
    }

    private void RecursiveAnalyze(
    List<Tile> remaining,
    List<List<Tile>> blocks,
    int sets,
    int pairs,
    HashSet<string> processedStates)
    {
        string stateKey = GetStateKey(blocks, remaining);
        if (processedStates.Contains(stateKey))
            return;
        processedStates.Add(stateKey);
        // ���� �������� ���� � 4 ������������ ������� ��� � 3 ������� � ����� (��������� ������)
        if (sets == 4 || (sets == 3 && pairs == 1))
        {
            if (YakuAnalyser == null)
                YakuAnalyser = new YakuAnalyser(Player);

            List<KeyValuePair<Tile, List<Tile>>> discard_Wait_Options = ComputeDiscardWaits(remaining);
            List<List<Tile>> completeBlocks = new List<List<Tile>>(blocks);
            // ��������� �������� ���� (���������� ������) � ����� ��������� ������
            completeBlocks.Add(new List<Tile>(remaining));

            foreach (var discardOption in discard_Wait_Options.ToList())
            {
                if (discardOption.Value.Count == 0)
                    discard_Wait_Options.Remove(discardOption);  // ������� ���� KeyValuePair
            }

            if (discard_Wait_Options.Count != 0)
                YakuAnalyser.AnalyzeYaku(discard_Wait_Options, completeBlocks);
        }

        // ��������: ����� ���������� �� (������������������ �� 3 ������)
        for (int i = 0; i <= remaining.Count - 3; i++)
        {
            Tile current = remaining[i];
            int currentRank = current.TryGetRankAsInt();
            if (currentRank == -1)
                continue; // ���� ���� �� ��������

            // ���� ������ � ������� currentRank+1 � currentRank+2 � ��� �� ������
            Tile nextTile = null, nextNextTile = null;
            bool foundFirst = false, foundSecond = false;
            for (int j = i + 1; j < remaining.Count; j++)
            {
                Tile candidate = remaining[j];
                if (!foundFirst && candidate.Suit == current.Suit && candidate.TryGetRankAsInt() == currentRank + 1)
                {
                    nextTile = candidate;
                    foundFirst = true;
                }
                else if (foundFirst && !foundSecond && candidate.Suit == current.Suit && candidate.TryGetRankAsInt() == currentRank + 2)
                {
                    nextNextTile = candidate;
                    foundSecond = true;
                }
            }
            if (foundFirst && foundSecond)
            {
                List<Tile> chiBlock = new List<Tile> { current, nextTile, nextNextTile };
                List<Tile> newRemaining = new List<Tile>(remaining);
                RemoveTile(newRemaining, current);
                RemoveTile(newRemaining, nextTile);
                RemoveTile(newRemaining, nextNextTile);
                List<List<Tile>> newBlocks = new List<List<Tile>>(blocks);
                newBlocks.Add(chiBlock);
                RecursiveAnalyze(newRemaining, newBlocks, sets + 1, pairs, processedStates);
            }
        }

        // ��������: ����� ���������� ��� (������ ���������� ������)
        for (int i = 0; i <= remaining.Count - 3; i++)
        {
            Tile current = remaining[i];
            List<int> indices = new List<int>();
            for (int j = i; j < remaining.Count; j++)
            {
                if (remaining[j].Equals(current))
                {
                    indices.Add(j);
                    if (indices.Count == 3)
                        break;
                }
            }
            if (indices.Count == 3)
            {
                List<Tile> ponBlock = new List<Tile>
                {
                    remaining[indices[0]],
                    remaining[indices[1]],
                    remaining[indices[2]]
                };
                List<Tile> newRemaining = new List<Tile>(remaining);
                RemoveTile(newRemaining, remaining[indices[0]]);
                RemoveTile(newRemaining, remaining[indices[1]]);
                RemoveTile(newRemaining, remaining[indices[2]]);
                List<List<Tile>> newBlocks = new List<List<Tile>>(blocks);
                newBlocks.Add(ponBlock);
                RecursiveAnalyze(newRemaining, newBlocks, sets + 1, pairs, processedStates);
            }
        }

        // ��������: ����� ���� (��� ���������� ������)
        if (pairs==0)
        for (int i = 0; i <= remaining.Count - 2; i++)
        {
            Tile current = remaining[i];
            for (int j = i + 1; j < remaining.Count; j++)
            {
                if (current.Equals(remaining[j]))
                {
                    List<Tile> pairBlock = new List<Tile> { current, remaining[j] };
                    List<Tile> newRemaining = new List<Tile>(remaining);
                    RemoveTile(newRemaining, current);
                    RemoveTile(newRemaining, remaining[j]);
                    List<List<Tile>> newBlocks = new List<List<Tile>>(blocks);
                    newBlocks.Add(pairBlock);
                        RecursiveAnalyze(newRemaining, newBlocks, sets, pairs+1, processedStates);
                    }
            }
        }
    }

    /// <summary>
    /// ������� �� ������ list ���� ��������� ����� tile (��������� �� ����� � �����)
    /// </summary>
    private void RemoveTile(List<Tile> list, Tile tile)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(tile))
            {
                list.RemoveAt(i);
                break;
            }
        }
    }


    /// <summary>
    /// ��� ��������� ����� ��������� ��������: 
    /// ���� "���� ��� ������" � "������ ��������� ������", ������� ���������, ���� �� ����� �������� ���� ����.
    /// ���� � ����� 2 ����� � ��� �������� ����, 
    /// ���� � ����� 3 ����� � ��� �������� ���� ��� ������������������ (��� ���).
    /// </summary>
    /// <param name="incompleteBlock">
    /// ���� ���� �� ������� ����, ����� 2 �����; ���� �� ������� ���������� �����, ����� 3 �����.
    /// </param>
    /// <returns>������ ��������� ������ � ������������ ����������</returns>
    private List<KeyValuePair<Tile, List<Tile>>> ComputeDiscardWaits(List<Tile> incompleteBlock)
    {
        List<KeyValuePair<Tile, List<Tile>>> result = new List<KeyValuePair<Tile, List<Tile>>>();

        if (incompleteBlock == null || incompleteBlock.Count == 0)
            return result;

        // ���� �������� ���� ������� �� 2 ������ (��������� ����).
        if (incompleteBlock.Count == 2)
        {
            // ������� 1: ���������� ������ ����, ������� ��������
            List<Tile> waits1 = ComputeWaits(new List<Tile>() { incompleteBlock[1] });
            result.Add(new KeyValuePair<Tile, List<Tile>>(incompleteBlock[0], waits1));

            // ������� 2: ���������� ������ ����, ������� ��������
            List<Tile> waits2 = ComputeWaits(new List<Tile>() { incompleteBlock[0] });
            result.Add(new KeyValuePair<Tile, List<Tile>>(incompleteBlock[1], waits2));

            return result;
        }

        // ���� �������� ���� ������� �� 3 ������ (��������� ���������� ������� �����, ��������, �� ��� ���).
        if (incompleteBlock.Count == 3)
        {
            List<Tile> twoBlock;
            List<Tile> waits;

            // ������� 1: �������� ������ ����, ������� ���� �� ��������� � ��������� 1 � 2
            twoBlock = new List<Tile>() { incompleteBlock[1], incompleteBlock[2] };
            waits = ComputeWaits(twoBlock);
            result.Add(new KeyValuePair<Tile, List<Tile>>(incompleteBlock[0], waits));

            // ������� 2: �������� ������ ����, ������� ���� �� ��������� � ��������� 0 � 2
            twoBlock = new List<Tile>() { incompleteBlock[0], incompleteBlock[2] };
            waits = ComputeWaits(twoBlock);
            result.Add(new KeyValuePair<Tile, List<Tile>>(incompleteBlock[1], waits));

            // ������� 3: �������� ������ ����, ������� ���� �� ��������� � ��������� 0 � 1
            twoBlock = new List<Tile>() { incompleteBlock[0], incompleteBlock[1] };
            waits = ComputeWaits(twoBlock);
            result.Add(new KeyValuePair<Tile, List<Tile>>(incompleteBlock[2], waits));

            return result;
        }

        // ���� �������� ���� �������� ������ ���� ������ � ������ ������ �� ������������ (����� ��������� ���������)
        return result;
    }

    /// <summary>
    /// ��������� ������ ��������� ������ ��� ��������� �����. ������ ������� �� ����� ������:
    /// - ���� ���� ������� �� ����� ������, �� �������� ��� ��� � ��������� ��� ����� ����� �� ������.
    /// - ���� ���� ������� �� ���� ������, ��:
    ///    + ���� ��� ��������� � �������� ��� ��� (������ ����).
    ///    + ���� ��� �� ���������, �� ����������� ����� ����� � ��������, ��:
    ///         * ��� ������� � 1 (������) � ��������� ����� � ����� ������ (� ������ ��������� ��������).
    ///         * ��� ������� � 2 (������) � ��������� ������� ������.
    /// </summary>
    /// <param name="incompleteBlock">���� �� 1 ��� 2 ������</param>
    /// <returns>������ ��������� ��������� ������ ��� ���������� �����</returns>
    private List<Tile> ComputeWaits(List<Tile> incompleteBlock)
    {
        List<Tile> waits = new List<Tile>();

        if (incompleteBlock == null || incompleteBlock.Count == 0)
            return waits;

        // ���� �������� ���� ������� �� ����� ������ � ��� ��� ��������� ��� �� ������.
        if (incompleteBlock.Count == 1)
        {
            Tile tile = incompleteBlock[0];
            waits.Add(new Tile(tile.Suit, tile.Rank));
            return waits;
        }

        // ���� ���� ������� �� ���� ������
        if (incompleteBlock.Count == 2)
        {
            Tile first = incompleteBlock[0];
            Tile second = incompleteBlock[1];

            // ���� ����� �� ����� ����� � �������� ��������� ������.
            if (first.Suit != second.Suit)
                return waits;

            int r1 = first.TryGetRankAsInt();
            int r2 = second.TryGetRankAsInt();

            // ���� ���� �� ���� ���� �� �������� � ������� �������� ��� ���, ���� ��� ���������.
            if (r1 == -1 || r2 == -1)
            {
                if (first.Equals(second))
                    waits.Add(new Tile(first.Suit, first.Rank));
                return waits;
            }

            // ���� ����� ��������� � �������� ��� ���.
            if (first.Equals(second))
            {
                waits.Add(new Tile(first.Suit, first.Rank));
            }
            else
            {
                // ���������� ���, ����� r1 ���� ������ r2.
                if (r1 > r2)
                {
                    int temp = r1;
                    r1 = r2;
                    r2 = temp;
                }

                int diff = r2 - r1;
                if (diff == 1)
                {
                    // ������ � ������������� ��������.
                    if (r1 > 1)
                        waits.Add(new Tile(first.Suit, (r1 - 1).ToString()));
                    if (r2 < 9)
                        waits.Add(new Tile(first.Suit, (r2 + 1).ToString()));
                }
                else if (diff == 2)
                {
                    // ������� � �������� ������� ������.
                    waits.Add(new Tile(first.Suit, (r1 + 1).ToString()));
                }
            }
            return waits;
        }

        return waits;
    }


    /// <summary>
    /// ��������� ���� �� ������ � �����.
    /// </summary>
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

    private string GetStateKey(List<List<Tile>> blocks, List<Tile> remaining)
    {
        // ��������� ������ ���� � ����������� � ������
        var sortedBlocks = blocks.Select(block =>
        {
            var sortedBlock = new List<Tile>(block);
            Sort(sortedBlock);
            return string.Join(",", sortedBlock.Select(t => $"{t.Suit}-{t.Rank}"));
        }).OrderBy(blockStr => blockStr).ToList(); // ��������� �����

        // ��������� ���������� �����
        var sortedRemaining = new List<Tile>(remaining);
        Sort(sortedRemaining);
        string remainingStr = string.Join(",", sortedRemaining.Select(t => $"{t.Suit}-{t.Rank}"));

        // ��������� ����
        return $"[{string.Join("|", sortedBlocks)}]|[{remainingStr}]";
    }
}
