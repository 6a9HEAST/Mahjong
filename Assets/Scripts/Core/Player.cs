using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public string Name { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive {  get; set; }
    public string Wind {  get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public abstract void AddTile(Tile tile);

    public abstract void StartTurn();

    public abstract void DiscardTile(Tile tile);

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
    }

    public  void StartTurn()
    {
        int tile=Random.Range(0, Hand.Count);
        DiscardTile(Hand[tile]);
        GameManager.EndTurn();
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
    public RealPlayer(string name, GameManager gameManager, PlayerDiscardView playerDiscardView)
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
        Hand.Remove(tile);
        Discard.Add(tile);
        
        
    }
    public void StartTurn()
    {
        

    }
}


