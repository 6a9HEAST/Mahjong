using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class YakuAnalyser
{

    IPlayer Player { get; set; }
    public List<IYaku> Checkers { get; set; }
    public List<(string yaku, int cost)> handCost { get; set; }
    public YakuAnalyser(IPlayer player)
    {
        Player = player;
        CreateCheckers();
        handCost = new List<(string yaku, int cost)>();
    }

    public void CreateCheckers()
    {
        Checkers = new List<IYaku>()
        {
            new Tanyao(),
            new RedDragon(),
            new GreenDragon(),
            new WhteDragon(),
            new RoundWind(),
            new PlayerWind(),
            new Pinfu(),
            new SevenPairs(),
            new Iipeikou_Ryanpeikou(),
            new Sanshoku(),
            new Ittsuu(),
            new Toitoi(),
            new Sanankou(),
            new Sankantsu(),
            new Sanshoku_triplets(),
            new Chanta (),
            new Junchan (),
            new Honitsu(),
            new Chinitsu (),
            new Honroto(),
            new Shosangen (),
        };
    }

    

    //List<KeyValuePair<Tile, List<Tile>>> discard_waits
    // /\
    // ||
    // список пар вида <тайл для сброса, список ожиданий которые будут после сброса этого тайла>

    //                                                                               в конце всегда незаконченный блок
    //                                                                                         ||
    //                                                                                         \/
    public void AnalyzeYaku(List<KeyValuePair<Tile, List<Tile>>> discard_waits,List<List<Tile>> completeBlocks)
    {
        Debug.Log(Player.Name + " AnalyzeYaku");
        foreach (var discard_wait in discard_waits)
        {
            DiscardWaitCost discardWaitCost = new()
            {
                Discard = discard_wait.Key
            };
            var temp_complete_hand = new List<List<Tile>>(completeBlocks);
           
            temp_complete_hand.Last().Remove(discard_wait.Key); //удаляем тайл который будем сбрасывать
            
            foreach (var tile in discard_wait.Value) //проходим по каждому тайлу из ожидания
            {
                WaitCost waitCost = new()
                {
                    Wait = tile
                };
                var fu_points = CalculateFu(temp_complete_hand,tile);
                //temp_complete_hand.Last().Add(tile); //добавляем тайл из ожидания чтобы сделать полную руку

                foreach (var checker in Checkers)//запускаем проверку на каждое яку
                {
                    var result = checker.Check(temp_complete_hand, Player);
                    if (result!=(null,0))
                        waitCost.Costs.Add(result);
                        
                }
                waitCost.Costs.Add(fu_points); // добавляем (string "Fu",int стоимость)
                discardWaitCost.WaitsCosts.Add(waitCost);

            }
            Debug.Log(Player.Name + " AnalyzeYaku->TryAddDiscardWaitCost");
            Player.TryAddDiscardWaitCost(discardWaitCost);
        }
    }

    private (string Yaku, int Cost) CalculateFu(List<List<Tile>> completeBlocks,Tile wait)
    {
        int fu = 0;
        // Получаем последний, недописанный блок ожидания
        var incomplete = completeBlocks.Last();
        // Убираем его из списка полных блоков
        completeBlocks.RemoveAt(completeBlocks.Count - 1);
        // Завершаем блок добавлением тайла ожидания
        var completedWaitBlock = new List<Tile>(incomplete) { wait };
        completeBlocks.Add(completedWaitBlock);

        // 1) Фу за пон и кан и пара
        foreach (var block in completeBlocks)
        {
            // Пон или кан
            if ((block.Count == 3 || block.Count == 4) && block.All(t => t.Equals(block[0])))
            {
                bool isClosedKan = block.Count == 4 && block.All(t => t.Properties.Contains("Called"));
                bool isOpenKan = block.Count == 4 && !isClosedKan;
                bool isClosedPon = block.Count == 3 && !block.Any(t => t.Properties.Contains("Called"));
                bool isOpenPon = block.Count == 3 && !isClosedPon;

                bool isTerminalOrHonor = IsTerminalOrHonor(block[0]);
                if (block.Count == 3) // Пон
                {
                    if (isClosedPon)
                        fu += isTerminalOrHonor ? 8 : 4;
                    else // открытый пон
                        fu += isTerminalOrHonor ? 4 : 2;
                }
                else // Кан
                {
                    if (isClosedKan)
                        fu += isTerminalOrHonor ? 32 : 16;
                    else // открытый кан
                        fu += isTerminalOrHonor ? 16 : 8;
                }
            }
            // Пара ценных тайлов
            else if (block.Count == 2)
            {
                var tile = block[0];
                if (tile.Suit == "Dragon" || (tile.Rank == Player.Wind || tile.Rank == Player.GameManager.RoundWind))
                    fu += 2;
            }
            // Шунцу (последовательность) фу не даёт
        }

        // 2) Фу за ожидание (tank i, kanchan, penchan)
        if (incomplete.Count == 1)
            fu += 2;
        else if (incomplete.Count == 2)
        {
            fu += CalculateWaitFu(incomplete[0], incomplete[1], wait);
        }

        // 3) Для открытой руки без любых фу даётся 2 fu
        bool isOpenHand = Player.Calls.Any();
        if (isOpenHand && fu == 0)
            fu += 2;

        return ("Fu", fu);
    }

    #region Вспомогательные методы

    private static bool IsTerminalOrHonor(Tile t)
    {
        // Терминалы (1 или 9) или тайлы 'Honor'
        return t.Suit == "Dragon" || t.Suit == "Wind" || t.Rank == "1" || t.Rank == "9";
    }

    
    /// <summary>
    /// Подсчитывает fu за ожидание: tanki (танки), kanchan, penchan. Возвращает 2 или 0.
    /// </summary>
    private static int CalculateWaitFu(Tile a, Tile b, Tile wait)
    {
        // Танки (когда a и b одинаковы и равны ожиданию)
        if (a.Equals(b) && wait.Equals(a))
            return 2;

        // Только для последовательностей
        if (a.Suit != wait.Suit || !IsNumeric(a) || !IsNumeric(b))
            return 0;

        int ra = int.Parse(a.Rank);
        int rb = int.Parse(b.Rank);
        int rw = int.Parse(wait.Rank);
        int min = Math.Min(ra, rb), max = Math.Max(ra, rb);

        // Penchan: 12 waiting 3 or 89 waiting 7
        if ((min == 1 && max == 2 && rw == 3) || (min == 8 && max == 9 && rw == 7))
            return 2;
        // Kanchan: среднее ожидание
        if (max - min == 2 && rw == min + 1)
            return 2;
        return 0;
    }

    private static bool IsNumeric(Tile t)
    {
        return int.TryParse(t.Rank, out _);
    }
    #endregion
}