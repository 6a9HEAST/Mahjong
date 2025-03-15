using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerHandView PlayerHandView;
    public PlayerDiscardView Player1DiscardView;
    public PlayerDiscardView Player2DiscardView;
    public PlayerDiscardView Player3DiscardView;
    public PlayerDiscardView Player4DiscardView;
    public List<IPlayer> Players { get; private set; }
    public List<Tile> Wall { get; private set; }
    public int ActivePlayer {  get; private set; }
    public int Dealer { get; private set; }
    public string RoundWind {  get; private set; }
    public int RoundNumber { get; private set; }
    public int RepeatCounter { get; private set; }
    private void Start()
    {
        Debug.Log("Игра началась");
        TileSpriteManager.Initialize();

        RoundWind = "East";
        RoundNumber = 1;
        RepeatCounter = 0;

        Players = new List<IPlayer>()
        {
            new RealPlayer("Player 1",this, Player1DiscardView),
            new AiPlayer("Player 2",this, Player2DiscardView),
            new AiPlayer("Player 3", this,Player3DiscardView),
            new AiPlayer("Player 4", this,Player4DiscardView)
        };

        //Players.Add(new RealPlayer("Player 1",this));
        //Players.Add(new AiPlayer("Player 2",this));
        //Players.Add(new AiPlayer("Player 3", this));
        //Players.Add(new AiPlayer("Player 4", this));

        ActivePlayer = -1;

        PrepareRound();

        
    }

    public void StartTurn()
    {
        var currentPlayer = Players[ActivePlayer];
        currentPlayer.AddTile(Wall[0]);
        Wall.RemoveAt(0);
        PlayerHandView.Draw(Players[0].Hand);
        currentPlayer.StartTurn();
    }

    public void EndTurn()
    {
        if (Wall.Count == 0)
        { 
            ExhaustiveDraw();
            return;
        }
        ActivePlayer = (ActivePlayer + 1) % 4;
        StartTurn();
    }
    public void PrepareRound() //TODO вариант без смены ветров
    {
        SetWinds();

        Wall = TileFactory.CreateWall();

        for (int j=0;j<4;j++)
            for (int i = 0; i < 13; i++)
            {
                Players[j].AddTile(Wall[0]);
                Wall.RemoveAt(0);
            }
        PlayerHandView.Draw(Players[0].Hand);
        StartTurn();
    }

    public void ExhaustiveDraw()
    {
        
        Debug.Log("ExhaustiveDraw");
        NextRound();
    }

    public void NextRound() //TODO вариант без смены ветров
    {
        foreach (var player in Players)
        {
            player.Clear();
        }

        
        RepeatCounter = 0;
        RoundNumber++;

        Dealer = (Dealer + 1) % 4;
        ActivePlayer = Dealer;
        PrepareRound();
    }

    public void SetWinds()
    {
        List<string> winds = new List<string>() { "East", "South", "West", "North" };

        if (ActivePlayer==-1) Dealer=ActivePlayer = Random.Range(0,4);

        int x = Dealer;
        for (int i = 0; i < 4; i++)
        {
            Players[x].Wind = winds[i];
            x = (x + 1) % 4;
        }

    }

    public void HandleTileClick(Tile clickedTile) //TODO отключение клика
    {
        //Debug.Log("HandleTileCLick");
        Players[0].DiscardTile(clickedTile);
        Players[0].PlayerDiscardView.Draw(Players[0].Discard);
        //Player1DiscardView.Draw(Players[0].Discard);
        PlayerHandView.Draw(Players[0].Hand);
    }

}

