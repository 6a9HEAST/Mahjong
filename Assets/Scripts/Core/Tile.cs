using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Xml.Linq;
using System;
using static UnityEngine.InputManagerEntry;

public class Tile 
{
    public string Suit { get; private set; }  // Масть (ман, пин, соу, драконы и ветры)
    public string Rank { get; private set; }    // Номер (1-9)
    public bool IsHonor { get; private set; } // Почётные плитки

    public List<string> Properties { get; private set; }

    public Tile(string suit, string rank, bool isHonor = false,string property=null)
    {
        Suit = suit;
        Rank = rank;
        if (suit == "Dragon" || suit == "Wind")
            IsHonor = true;
        else IsHonor = false;
        //IsHonor = isHonor;
        Properties = new List<string>();
        if (property != null ) Properties.Add(property);
    }

    public Tile (Tile tile)
    {
        Suit = tile.Suit;
        Rank = tile.Rank;
        Properties = new List<string>();
        foreach (var property in tile.Properties)
        {
            Properties.Add(property);
        }
    }

    public bool IsRedFive()
    {
        return int.Parse(Rank) == 5 && Properties.Contains("Red");
    }

    public override string ToString()
    {
        if (Properties.Contains("Red")) return $"Red{Rank}{Suit}";
        return $"{Rank}{Suit}";
    }

    public override bool Equals(object obj)
    {
        if (obj is not Tile other)
            return false;
        return Suit == other.Suit && Rank == other.Rank;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Suit, Rank);
    }

    public Tile Clone()
    {
        return new Tile(Suit, Rank);
    }


    /// <summary>
    /// Преобразует ранг в число (если возможно). Если преобразование не удалось – возвращает -1.
    /// </summary>
    public int TryGetRankAsInt()
    {
        int result;
        if (int.TryParse(Rank, out result))
            return result;
        return -1;
    }

    public void AddDiscardable()
    {
        if (!Properties.Contains("Discardable"))
            Properties.Add("Discardable");
    }

    public bool IsDiscardable()
    {
        return Properties.Contains("Discardable");
    }

    public void RemoveDiscardable()
    {
        if (Properties.Contains("Discardable"))
            Properties.Remove("Discardable");
    }

    public static readonly List<Tile> AllTiles = new()
    {
        // Маны
        new("Man", "1"), new("Man", "2"),new("Man", "3"),new("Man", "4"),new("Man", "5"),new("Man", "6"),new("Man", "7"),new("Man", "8"),new("Man", "9"),

        // Пины
        new("Pin", "1"), new("Pin", "2"),new("Pin", "3"),new("Pin", "4"),new("Pin", "5"),new("Pin", "6"),new("Pin", "7"),new ("Pin", "8"),new("Pin", "9"),

        // Соу
        new("Sou", "1"), new("Sou", "2"),new("Sou", "3"),new("Sou", "4"),new("Sou", "5"),new("Sou", "6"),new("Sou", "7"),new("Sou", "8"),new("Sou", "9"),

        // Четыре ветра
        new("Wind", "East"),
        new("Wind", "South"),
        new("Wind", "West"),
        new("Wind", "North"),

        // Три дракона
        new("Dragon", "White"),
        new("Dragon", "Green"),
        new("Dragon", "Red"),
    };
}

