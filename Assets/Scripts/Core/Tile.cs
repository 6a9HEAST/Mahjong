using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    public override string ToString()
    {
        if (Properties.Contains("Red")) return $"Red{Rank}{Suit}";
        return $"{Rank}{Suit}";
    }

    
}

