using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerHandView PlayerHandView;
    public PlayerDiscardView Player1DiscardView;
    public PlayerDiscardView Player2DiscardView;
    public PlayerDiscardView Player3DiscardView;
    public PlayerDiscardView Player4DiscardView;

    public CallContainerView Player1CallContainerView;
    public CallContainerView Player2CallContainerView;
    public CallContainerView Player3CallContainerView;
    public CallContainerView Player4CallContainerView;

    public CenterView CenterView;
    public DoraIndicatorView DoraIndicatorView;
    public CallsButtonsView CallsButtonsView;
    public List<IPlayer> Players { get; private set; }
    public List<Tile> Wall { get; private set; }
    public List<Tile> DoraIndicator { get; private set; } = new List<Tile>();
    public List<Tile> UraDoraIndicator { get; private set; } = new List<Tile>();
    public List<Tile> KanTiles { get; private set; } = new List<Tile>();
    public int DorasShown { get; private set; }
    public int ActivePlayer { get; private set; }
    public int Dealer { get; private set; }
    public string RoundWind { get; private set; }
    public int RoundNumber { get; private set; }
    public int RepeatCounter { get; private set; }

    // Список игроков, от которых ждём решения по вызовам (пон, чи, кан)
    private List<IPlayer> pendingCallPlayers = new List<IPlayer>();

    private void Start()
    {
        TileSpriteManager.Initialize();
        RoundWind = "East";
        RoundNumber = 1;
        RepeatCounter = 0;

        Players = new List<IPlayer>()
        {
            new RealPlayer("Player 1", this, Player1DiscardView,Player1CallContainerView,0),
            new AiPlayer("Player 2", this, Player2DiscardView,Player2CallContainerView, 1),
            new AiPlayer("Player 3", this, Player3DiscardView,Player3CallContainerView, 2),
            new AiPlayer("Player 4", this, Player4DiscardView,Player4CallContainerView, 3)
        };
        foreach (var player in Players)
            player.Score = 25000;

        CallsButtonsView.player = Players[0];
        ActivePlayer = -1;
        PrepareRound();
    }

    public void StartTurn(int afterCall=-1)
    {
        var currentPlayer = Players[ActivePlayer];
        Players[ActivePlayer].IsActive = true;
        if (afterCall == -1)
        {
            currentPlayer.AddTile(Wall[0]);
            Wall.RemoveAt(0);
        }

        Players[ActivePlayer].HandAnalyzer.AnalyzeHand();

        PlayerHandView.Draw(Players[0].Hand);
        CenterView.UpdateTilesRemaining(Wall.Count);
        currentPlayer.StartTurn();
    }

    public void EndTurn(int newActivePlayer=-1)
    {
        //Debug.Log(Players[ActivePlayer].Name + " заканчивает ход");
        // Если остались игроки, от которых ждём решения по вызовам – не переходим к следующему ходу.
        if (pendingCallPlayers.Count > 0)
        {
            Debug.Log("Ожидание решений по вызовам, переход хода отложен.");
            return;
        }
        if (Wall.Count == 0)
        {
            ExhaustiveDraw();
            return;
        }
        Players[ActivePlayer].IsActive = false;
        if (newActivePlayer == -1)
            ActivePlayer = (ActivePlayer + 1) % 4;
        else ActivePlayer = newActivePlayer;
        StartTurn(newActivePlayer);
    }

    public void PrepareRound() //TODO вариант без смены ветров
    {
        SetWinds();

        Wall = TileFactory.CreateWall();
        DoraIndicator.Clear();
        UraDoraIndicator.Clear();
        KanTiles.Clear();

        for (int j = 0; j < 4; j++)
            for (int i = 0; i < 13; i++)
            {
                Players[j].AddTile(Wall[0],true);
                Wall.RemoveAt(0);
            }
        for (int j = 0; j < 5; j++)
        {
            DoraIndicator.Add(Wall[0]);
            Wall.RemoveAt(0);
        }
        for (int j = 0; j < 5; j++)
        {
            UraDoraIndicator.Add(Wall[0]);
            Wall.RemoveAt(0);
        }
        for (int j = 0; j < 4; j++)
        {
            KanTiles.Add(Wall[0]);
            Wall.RemoveAt(0);
        }

        DorasShown = 1;
        DoraIndicatorView.Draw(DoraIndicator, DorasShown);
        PlayerHandView.Draw(Players[0].Hand);
        CenterView.UpdateWinds(Players[0].Wind, Players[1].Wind, Players[2].Wind, Players[3].Wind);
        CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
        CenterView.UpdateRoundWind(RoundWind);
        PlayerHandView.Sort(Players[0].Hand);

        //for (int i = 0; i < 4; i++)
        //{
        //    Players[0].Hand.RemoveAt(0);
            
        //}
        //List<Tile> list = new List<Tile>() { new Tile("Man", "1", false, "Called"), new Tile("Man", "1", false, "Called"), new Tile("Man", "1"), new Tile("Man", "1") };
        //Players[0].Calls.Add(list);
        //Players[0].CallContainerView.Draw(Players[0].Calls);

        StartTurn();
    }

    public void ExhaustiveDraw()
    {
        // Обработка ситуация, когда тайлы закончились
        NextRound();
    }

    public void NextRound() //TODO вариант без смены ветров
    {
        foreach (var player in Players)
        {
            player.Clear();
            player.ClearCalls();
            player.PlayerDiscardView.Draw(player.Discard);
        }
        RepeatCounter = 0;
        RoundNumber++;
        Dealer = (Dealer + 1) % 4;
        ActivePlayer = Dealer;
        PrepareRound();
    }

    public void SetWinds() //Смена ветров
    {
        List<string> winds = new List<string>() { "East", "South", "West", "North" };
        if (ActivePlayer == -1)
        {
            Dealer = ActivePlayer = Random.Range(0, 4); // Если начало игры, выбираем дилера случайно
        }
        int x = Dealer;
        for (int i = 0; i < 4; i++)
        {
            Players[x].Wind = winds[i];
            x = (x + 1) % 4;
        }
    }

    public void HandleTileClick(Tile clickedTile) //TODO отключение клика, если ждём вызов
    {
        if (!Players[0].IsActive) return;
        Players[0].DiscardTile(clickedTile);
        Players[0].PlayerDiscardView.Draw(Players[0].Discard);
        PlayerHandView.Sort(Players[0].Hand);
        PlayerHandView.Draw(Players[0].Hand);
        // После сброса вызываем проверку вызовов
        // (для реального игрока кнопки вызовов отобразятся, и ход не перейдёт дальше до принятия решения)
        //EndTurn();
    }

    // Метод проверки возможности вызовов (пон/чи/кан) после сброса тайла
    public void CheckForCalls(Tile tile, IPlayer sender)
    {
        // Проверяем для всех игроков возможность пон или кан
        foreach (var player in Players)
        {
            if (player==sender) continue;
            if (player.CheckForPon(tile))
            {
                player.pon = (tile, sender);

            }
            if (player.CheckForKan(tile)==1)
            {
                player.kan = (tile, sender);
            }
        }

        // Проверка на возможность чи только для игрока справа от скидывающего
        int index = Players.IndexOf(sender);
        index = (index + 1) % 4;
        if (tile.Suit != "Dragon" && tile.Suit != "Wind")
        {
            var chis = Players[index].CheckForChi(tile);
            if (chis != null && chis.Count > 0)
            {
                Players[index].chi = (chis, sender);
            }
        }

        // Сбрасываем список ожидающих вызовов и заполняем его игроками, у которых есть вызовы
        //pendingCallPlayers.Clear();
        //foreach (var player in Players)
        //{
        //    if (player.HasCalls())
        //    {
        //        //pendingCallPlayers.Add(player);
        //        // Для ИИ сразу обрабатываем вызов (автоматически Pass)
        //        // У реального игрока эта функция покажет кнопки на UI для выбора
        //       // player.ProceedCalls();
        //    }
        //}
        // Если ни у кого вызовов нет – можно продолжать ход
        //if (pendingCallPlayers.Count == 0)
        if (Players[0].HasCalls()) 
        {
            Players[0].ProceedCalls();
        }
        else EndTurn();
    }

    // Вызывается из кнопок или у ИИ, когда игрок принял решение (вызвал пон/чи/кан или отказался)
    public void OnCallDecisionMade(IPlayer player)
    {
        if (pendingCallPlayers.Contains(player))
        {
            pendingCallPlayers.Remove(player);
            Debug.Log(player.Name + " принял решение по вызову.");
        }
        if (pendingCallPlayers.Count == 0)
        {
            EndTurn();
        }
    }

    public void RevealDora()
    {
        DorasShown++;
        DoraIndicatorView.Draw(DoraIndicator, DorasShown);
    }
}
