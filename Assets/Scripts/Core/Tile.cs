using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Xml.Linq;
using System;

public class Tile 
{
    public string Suit { get; private set; }  // ����� (���, ���, ���, ������� � �����)
    public string Rank { get; private set; }    // ����� (1-9)
    public bool IsHonor { get; private set; } // �������� ������

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
}

