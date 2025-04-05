using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public interface IPlayer
{
    public string Name { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive {  get; set; }
    public string Wind {  get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public abstract void AddTile(Tile tile);

    public abstract void StartTurn();

    public abstract void DiscardTile(Tile tile);
    public abstract bool CheckForPon(Tile tile);
    public abstract bool CheckForKan(Tile tile);
    public abstract List<List<Tile>> CheckForChi(Tile tile);
    public void Clear()
    {
        Hand.Clear();
        Discard.Clear();
        //Debug.Log($"{Name} hand={Hand.Count} Discard={Discard.Count}");
    }
}

public class AiPlayer : IPlayer
{
    public string Name { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive { get; set; }
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public AiPlayer(string name, GameManager gameManager, PlayerDiscardView playerDiscardView)
    {
        Name = name;
        Hand = new List<Tile>();
        Discard = new List<Tile>();
        IsActive = false;
        GameManager = gameManager;
        PlayerDiscardView = playerDiscardView;
    }

    public void AddTile(Tile tile)
    {
        Hand.Add(tile);
    }

    public void DiscardTile(Tile tile)
    {
        //Debug.Log($"{Name} discards {tile.ToString()}");
        Hand.Remove(tile);
        Discard.Add(tile);
        PlayerDiscardView.Draw(Discard);
        GameManager.CheckForCalls(tile,this);
    }

    public  void StartTurn()
    {
        int tile=Random.Range(0, Hand.Count);
        DiscardTile(Hand[tile]);
        GameManager.EndTurn();
    }
    public bool CheckForPon(Tile tile)
    {
        return false;
    }
    public bool CheckForKan(Tile tile)
    {
        return false;
    }

    public  List<List<Tile>> CheckForChi(Tile tile)
    {
        return null;
    }
}

public class RealPlayer : IPlayer
{
    public string Name { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive { get; set; }
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public RealPlayer(string name, GameManager gameManager, PlayerDiscardView playerDiscardView)
    {
        Name = name;
        Hand = new List<Tile>();
        Discard = new List<Tile>();
        IsActive = false;
        GameManager = gameManager;
        PlayerDiscardView = playerDiscardView;
        TileCounts = new Dictionary<string, int>();
    }

    public void AddTile(Tile tile)
    {
        Hand.Add(tile);
        if (TileCounts.ContainsKey(tile.ToString()))
            TileCounts[tile.ToString()]++;
        else TileCounts[tile.ToString()] = 1;
    }
    public void DiscardTile(Tile tile)
    {
        Hand.Remove(tile);
        Discard.Add(tile);
        TileCounts[tile.ToString()]--;
        GameManager.CheckForCalls(tile,this);
    }
    public void StartTurn()
    {
        

    }

    public bool CheckForPon(Tile tile)
    {   if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 2) return true;
        return false;
    }

    public bool CheckForKan(Tile tile)
    {
        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 3) return true;
        return false;
    }

    public List<List<Tile>> CheckForChi(Tile tile)
    {
        List<List<Tile>> possibleSequences = new List<List<Tile>>();
        string suit = tile.Suit;
        int num = int.Parse(tile.Rank);

        List<Tile> GetTiles(int number)
        {
            return Hand.Where(t => t.Suit == suit && int.Parse(t.Rank) == number).ToList();
        }


        if (num <= 7)
        {
        var tiles1 = GetTiles(num + 1);
        var tiles2 = GetTiles(num + 2);
            foreach (var t1 in tiles1)
            {
                foreach (var t2 in tiles2)
                {
                    possibleSequences.Add(new List<Tile> { tile, t1, t2 });
                }
            }
        }
        if (num >= 2 && num <= 8)
        {
        var tiles1 = GetTiles(num - 1);
        var tiles2 = GetTiles(num + 1);
            foreach (var t1 in tiles1)
            {
                foreach (var t2 in tiles2)
                {
                    possibleSequences.Add(new List<Tile> { t1, tile, t2 });
                }
            }
        }
        if (num >= 3)
        {
        var tiles1 = GetTiles(num - 2);
        var tiles2 = GetTiles(num - 1);
            foreach (var t1 in tiles1)
            {
                foreach (var t2 in tiles2)
                {
                    possibleSequences.Add(new List<Tile> { t1, t2, tile });
                }
            }
        }

        return possibleSequences;
    }


}


