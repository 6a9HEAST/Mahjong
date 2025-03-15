using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Tile 
{
    public string Suit { get; private set; }  // ����� (���, ���, ���, ������� � �����)
    public string Rank { get; private set; }    // ����� (1-9)
    public bool IsHonor { get; private set; } // �������� ������

    public List<string> Properties { get; private set; }

    public Tile(string suit, string rank, bool isHonor = false)
    {
        Suit = suit;
        Rank = rank;
        IsHonor = isHonor;
        Properties = new List<string>();
    }

    public override string ToString()
    {
        return $"{Rank}{Suit}";
    }

    
}

