using System.Collections.Generic;
using UnityEngine;

public static class TileFactory
{
    public static List<Tile> CreateWall()
    {
        List<Tile> wall = new List<Tile>();
        string[] suits = { "Man", "Pin", "Sou" };
        foreach (var suit in suits)
        {
            for (int i = 1; i <= 9; i++)
            {
                int k = 4;
                if (i==5) //Добавление красных пятерок вместо одной из 4 пятерок
                { 
                    wall.Add(new Tile(suit,i.ToString(),false,"Red"));
                    k--;
                }

                for (int j = 0; j < k; j++)
                { // 4 одинаковые плитки
                    wall.Add(new Tile(suit, i.ToString()));
                }
            }
        }

        
        string[] Winds = { "East", "South", "West", "North" };

        foreach (var wind in Winds)
        {
            for (int j = 0; j < 4; j++)
            {
                wall.Add(new Tile("Wind", wind, true));
            }
        }

        string[] Dragons = { "Red", "White", "Green" };

        foreach (var dragon in Dragons)
        {
            for (int j = 0; j < 4; j++)
            {
                wall.Add(new Tile("Dragon", dragon, true));
            }
        }

        List<Tile> shuffledWall=new List<Tile>();
        

        for (int i=wall.Count-1; i >= 0; i--)
        {
            int randomTile=Random.Range(0, i);
            shuffledWall.Add(wall[randomTile]);
            wall.Remove(wall[randomTile]);
        }

        return shuffledWall;
    }
}

