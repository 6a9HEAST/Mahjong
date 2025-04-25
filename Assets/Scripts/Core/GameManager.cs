using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public VictoryScreenView VictoryScreenView;

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
    public int Bet { get; private set; }

    public bool VictoryScreen { get; private set; } = false;

    public bool TEST_HAND = false;
    public List<Tile> TEST_TILES=new List<Tile>();
    public List<Tile> TEST_TILES2=new List<Tile>();

    public WaitForSeconds WAIT_TIME = new WaitForSeconds(0.3f);

    // Список игроков, от которых ждём решения по вызовам (пон, чи, кан)
    private List<IPlayer> pendingCallPlayers = new List<IPlayer>();
    public int callsSent = 0;

    private void Start()
    {
        Bet = 0;
        TileSpriteManager.Initialize();
        RoundWind = "East";
        RoundNumber = 1;
        RepeatCounter = 0;
        VictoryScreenView.Itself.SetActive(false);

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
        Tile tile=null;
        if (afterCall == -1)
        {
            if (TEST_HAND && TEST_TILES.Count > 0 && ActivePlayer==0)
            {
                currentPlayer.AddTile(TEST_TILES[0]);
                tile = TEST_TILES[0];
                TEST_TILES.RemoveAt(0);
            }
            else

            {
                currentPlayer.AddTile(Wall[0]);
                tile = Wall[0];
                Wall.RemoveAt(0);
            }
        }

        //ПОСЛЕ ВЗЯТИЯ ТАЙЛА ЗАПУСК АНАЛИЗА РУКИ (ЕСЛИ НЕ РИИЧИ И РУКА НЕ СОБРАНА)
        if (!Players[ActivePlayer].Riichi && !Players[ActivePlayer].HasWaitOn(tile)) 
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

        if (newActivePlayer == -1)//После объявления посылается индекс игрока который сделал объявление
            ActivePlayer = (ActivePlayer + 1) % 4;
        else ActivePlayer = newActivePlayer;
        StartTurn(newActivePlayer);
    }

    public void PrepareRound() 
    {
        SetWinds();

        Wall = TileFactory.CreateWall();
        DoraIndicator.Clear();
        UraDoraIndicator.Clear();
        KanTiles.Clear();
        VictoryScreen=false;

        for (int j = 0; j < 4; j++)
            if (!(TEST_HAND&&j==0))//Если тестовая рука то не набираем игроку руку из стены
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

        if (TEST_HAND) //Замена руки на тестовую
        {

            var hand = new List<Tile>() //13 тайлов
            { 
                new("Man", "1"),
                new("Man", "2"),
                new("Man", "3"),
                new("Sou", "1"),
                new("Sou", "2"),
                new("Sou", "3"),
                new("Pin", "1"),
                new("Pin", "2"),
                new("Man", "8"),
                new("Dragon", "Green"),
                new("Dragon", "Green"),
                new("Dragon", "Green"),
                new("Wind", "East"),
            };

            foreach (var tile in hand)
            {
                Players[0].AddTile(tile, true);
            }

                TEST_TILES = new List<Tile>() //СПИСОК ТАЙЛОВ КОТОРЫЕ БУДУТ ВЫДАВАТЬСЯ ИГРОКУ
            {
                new("Pin", "3"),
                //new("Dragon", "Red"),
                new("Wind", "East")
            };

            TEST_TILES2 = new List<Tile>() //СПИСОК ТАЙЛОВ КОТОРЫЕ БУДЕТ СБРАСЫВАТЬ БОТ
            {
                //new("Pin", "7"),
                //new("Dragon", "Red"),
                //new("Wind", "East")
            };
        }

        DorasShown = 1;
        DoraIndicatorView.Draw(DoraIndicator, DorasShown);
        PlayerHandView.Draw(Players[0].Hand);

        CenterView.UpdatePlayerBetView(Players);
        CenterView.UpdateWinds(Players[0].Wind, Players[1].Wind, Players[2].Wind, Players[3].Wind);
        CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
        CenterView.UpdateRoundWind(RoundWind);
        CenterView.UpdateBet(Bet);

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

    public void NextRound(int _Dealer=-1)
    {
        foreach (var player in Players)
        {
            CallsButtonsView.Clear();
            player.Clear();
            player.ClearCalls();
            player.PlayerDiscardView.Draw(player.Discard);
            player.Riichi = false;
        }
        
        if (_Dealer==-1)
        {
            RepeatCounter = 0;
            if (RoundNumber<4)
            RoundNumber++;
            else
            {
                //Логика конца игры
            }
            Dealer = (Dealer + 1) % 4;
        }
        else
        { 
            Dealer = _Dealer;
            RepeatCounter++;
        }
        ActivePlayer = Dealer;
        PrepareRound();
    }
    /// <summary>
    /// Расстановка ветров в зависимости от того кто дилер
    /// </summary>
    public void SetWinds() 
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

    public void HandleTileClick(Tile clickedTile) 
    {
        //int[] x=new int[4];
        //VictoryScreenView.Draw(Players[0],x , 0, new List<(string Yakus,int Cost)>());

        if (!Players[0].IsActive|| VictoryScreen) return;

        if (Players[0].Riichi) //если игрок объявил риичи то если есть тайлы со свойством то это тайл которым он объявляет риичи
            if (clickedTile.IsDiscardable())
                clickedTile.Properties.Add("Riichi");
            else return;//если таких нет то он уже объявил риичи и ждет последнего тайла

        CallsButtonsView.Clear();
        if (Players[0].Riichi)
        {
            Players[0].UpgradeWaits(clickedTile);
            Bet++;
            Players[0].Score -= 1000;
            CenterView.UpdateBet(Bet);
            CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
            CenterView.UpdatePlayerBetView(Players);
        }
        StartCoroutine(Players[0].DiscardTile(clickedTile));
        if (Players[0].Riichi) Players[0].Ippatsu = true;
            Players[0].PlayerDiscardView.Draw(Players[0].Discard);
        
        
        

    }

    /// <summary>
    /// Проверяет, может ли кто то из игроков сделать объявлеие на этот (сброшенный) тайл</summary>
    /// <param name="tile"></param>
    /// <param name="sender"></param>
    public void CheckForCalls(Tile tile, IPlayer sender)
    {
        // Проверяем для всех игроков возможность пон или кан
        foreach (var player in Players)
        {
            if (player==sender) continue;

            if (player.CheckForRon(tile))
            {
                player.ron = (tile, sender);

            }

            if (player.Riichi) continue;


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
        if (tile.Suit != "Dragon" && tile.Suit != "Wind" && !Players[index].Riichi)
        {
            var chis = Players[index].CheckForChi(tile);
            if (chis != null && chis.Count > 0)
            {
                Players[index].chi = (chis, sender);
            }
        }
        
        callsSent=Players.Count(x=>x.HasCalls());

        foreach (var player in Players)
        {
            if (player.HasCalls())
            {
                player.ProceedCalls();
            }
            
        }
        if (callsSent == 0) EndTurn();
    }

    public void Executecalls()
    {
        callsSent--;
        if (callsSent > 0) return;

        foreach (var player in Players)
        {
            if (player.ron!=(null,null))
            {
                player.ExecuteRon();
                //EndTurn();
                return;
            }
        }

        foreach (var player in Players)
        {
            if (player.pon != (null, null))
            {
                player.ExecutePon();
                //EndTurn();
                return;
            }
            if (player.kan != (null, null))
                {
                player.ExecuteKan();
                //EndTurn();
                return;
            }

        }
        
        foreach (var player in Players)
        {
            if (player.chi != (null, null))                
            {
                player.ExecuteChi(player.chi.Item1[0]);
                //EndTurn();
                return;
            }
        }
        EndTurn();

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

    public void RoundWin(IPlayer player, int[] score_change,int han,List<(string Yaku,int Cost)> yakus)
    {
        CallsButtonsView.Clear();
        VictoryScreen = true;
        VictoryScreenView.Draw(player, score_change, han, yakus);
    }

    public List<Tile> GetDoras()
    {
        List<Tile> result=new List<Tile>();
        for (int i = 0; i < DorasShown; i++)
        {
            result.Add(GetNextTile(DoraIndicator[i]));
        }
        return result;
    }

    public List<Tile> GetUraDoras()
    {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i < DorasShown; i++)
        {
            result.Add(GetNextTile(UraDoraIndicator[i]));
        }
        return result;
    }

    private Tile GetNextTile(Tile tile)
    {
        Dictionary<string, string> Dragons = new Dictionary<string, string>()
        {
            {"Green", "Red"},
            {"Red", "White"},
            {"White", "Green"}
        };
        Dictionary<string, string> Winds = new Dictionary<string, string>()
        {
            {"East", "South"},
            {"South", "West"},
            {"West", "North"},
            {"North", "East"}
        };

        if ( tile.Suit=="Dragon")
        {
            return new Tile(tile.Suit, Dragons[tile.Rank]);
        }
        if (tile.Suit == "Wind")
        {
            return new Tile(tile.Suit, Winds[tile.Rank]);
        }
        if (tile.TryGetRankAsInt()<9)
        return new Tile(tile.Suit, (int.Parse(tile.Rank) + 1).ToString());
        else return new Tile(tile.Suit, 1.ToString());
    }

    public int GetThenClearBet()
    {
        int x = Bet;
        Bet = 0;
        return x*1000;
    }
}
