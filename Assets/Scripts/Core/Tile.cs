using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        IsHonor = isHonor;
        Properties = new List<string>();
        if (property != null ) Properties.Add(property);
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

    
}

