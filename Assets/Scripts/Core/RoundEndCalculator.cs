using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.Math;

public class RoundEndCalculator
{
    Dictionary<int,int> limits= new Dictionary<int, int>()
    {
        {5,2000},

        {6,3000},
        {7,3000},

        {8,4000},
        {9,4000},
        {10,4000},

        {11,6000},
        {12,6000},

        {13,8000}
    };

    public void CountTsumoPoints(IPlayer winner,List<(string Yaku,int Cost)> yakus)
    {
        var fu = yakus.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов
        
        yakus.Remove(fu);

        int han= yakus.Sum(a => a.Cost);

        han += CalculateDoras(winner);
        if (winner.Riichi)
            han+= CalculateUraDoras(winner);
        han+=CalculateRedFives(winner);

        var points = CalculateTsumoPoints(han, fu.Cost);

        int[] pts_changes = new int[4] {0,0,0,0};
        int player_gain = 0;

        foreach (var player in winner.GameManager.Players)
        {
            if (player.index == winner.index) continue;

            if (player.Wind == "East") // с дилера снимаем очки-с-дилера
            {
                pts_changes[player.index] -= points.dealer;
                player_gain += points.dealer;
            }
            else if (winner.Wind=="East") //если сам победитель диллер, то с каждого снимаем очки-с-дилера
            {
                pts_changes[player.index] -= points.dealer;
                player_gain += points.dealer;
            }
            else //если победитель не диллер, то снимаем очки-не-с-дилера
            {
                pts_changes[player.index] -= points.non_dealer;
                player_gain += points.non_dealer;
            }
        }
        pts_changes[winner.index] += player_gain;

        winner.GameManager.RoundWin(winner, pts_changes);
    }

    public void CountRonPoints(IPlayer winner, IPlayer deal_in, List<(string Yaku, int Cost)> yakus)
    {
        var fu = yakus.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов

        yakus.Remove(fu);

        int han = yakus.Sum(a => a.Cost);

        han += CalculateDoras(winner);
        if (winner.Riichi)
            han += CalculateUraDoras(winner);
        han += CalculateRedFives(winner);

        var points = CalculateRonPoints(han, fu.Cost);

        int[] pts_changes = new int[4] { 0, 0, 0, 0 };

        int final_points;
        if (winner.Wind == "East") final_points= (int)Ceiling(points*6 / 1000.0) * 1000;   
        else final_points = (int)Ceiling(points * 4 / 1000.0) * 1000;
            
        pts_changes[winner.index] += final_points;
        pts_changes[deal_in.index] -= final_points;

        winner.GameManager.RoundWin(winner, pts_changes);
    }

    private (int dealer,int non_dealer) CalculateTsumoPoints(int han, int fu)
    {
        if (han >= 5) return (limits[han]*2, limits[han]); //если хан 5 и выше, то фиксированная стоимость 
                                                           //(с диллера/для диллера стоимость_лимита x 2,
                                                           //с недиллера стоимость_лимита)

        int pts = (int)(fu * Pow(2, 2 + han));
        int non_dealer= (int)Ceiling(pts / 100.0) * 100;
        int dealer= (int)Ceiling(pts*2 / 100.0) * 100;
        return (dealer, non_dealer);
    }

    private int CalculateRonPoints(int han, int fu)
    {
        if (han >= 5) return limits[han] * 4; //если хан 5 и выше, то фиксированная стоимость
                                              //(стоимость лимита*4)
        else
            return (int)(fu * Pow(2, 2 + han));
    }

    private int CalculateDoras(IPlayer player)
    {
        List<Tile> doras = player.GameManager.GetDoras();
        

        int han = 0;

        foreach (var  tile in player.Hand)
        {
            foreach (var dora in doras)
            {
                if (dora.Equals(tile))
                    han++;
            }           
        }
        return han;

    }

    private int CalculateUraDoras(IPlayer player)
    {
        List<Tile> ura_doras = player.GameManager.GetUraDoras();
        int han = 0;

        foreach (var tile in player.Hand)
        {
            foreach (var ura_dora in ura_doras)
            {
                if (ura_dora.Equals(tile))
                    han++;
            }
        }
        return han;
    }

    private int CalculateRedFives(IPlayer player)
    {
        int han = 0;

        foreach (var tile in player.Hand)
        {
            if (tile.Properties.Contains("Red"))
                han++;
        }
        return han;
    }


}
