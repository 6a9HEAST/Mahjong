using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<IHandView> HandViews;

    public List<PlayerDiscardView> playerDiscardViews;

    public List<CallContainerView> playerCallContainerViews;

    public List<CallTextView> callTextViews;

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

    public bool TEST_HAND = true;
    public List<Tile> TEST_TILES=new();
    public List<Tile> TEST_TILES2=new();

    public WaitForSeconds WAIT_TIME = new(0.3f);

    
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
            new RealPlayer("Player 1", this, playerDiscardViews[0],HandViews[0],playerCallContainerViews[0],0),
            new AiPlayer("Player 2", this, playerDiscardViews[1],HandViews[1],playerCallContainerViews[1], 1),
            new AiPlayer("Player 3", this, playerDiscardViews[2],HandViews[2], playerCallContainerViews[2], 2),
            new AiPlayer("Player 4", this, playerDiscardViews[3],HandViews[3], playerCallContainerViews[3], 3)
        };
        foreach (var player in Players)
            player.Score = 25000;

        foreach (var text in callTextViews)
            text.Clear();
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
            if (TEST_HAND && TEST_TILES.Count > 0 && ActivePlayer==1)
            {
                currentPlayer.AddTile(TEST_TILES[0]);
                tile = TEST_TILES[0];
                TEST_TILES.RemoveAt(0);
            }
            else

            {
                tile = new(Wall[0]);
                currentPlayer.AddTile(tile);
                //tile = Wall[0];
                Wall.RemoveAt(0);
            }
        }

        //ПОСЛЕ ВЗЯТИЯ ТАЙЛА ЗАПУСК АНАЛИЗА РУКИ (ЕСЛИ НЕ РИИЧИ И РУКА НЕ СОБРАНА)
        if (!Players[ActivePlayer].Riichi && !Players[ActivePlayer].HasWaitOn(tile)) 
            Players[ActivePlayer].HandAnalyzer.AnalyzeHand();

        Players[ActivePlayer].DrawHand();

        CenterView.UpdateTilesRemaining(Wall.Count);
        currentPlayer.StartTurn();
    }

    public void EndTurn(int newActivePlayer=-1)
    {        
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
        ClearOtherCalls();
        VictoryScreen=false;

        for (int j = 0; j < 4; j++)
            if (!(TEST_HAND&&j==1))//Если тестовая рука то не набираем игроку руку из стены
            for (int i = 0; i < 13; i++)
            {
                Tile tile=new(Wall[0]);
                Players[j].AddTile(tile,true);
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
                new("Man", "2"),
                new("Man", "3"),
                new("Man", "4"),
                new("Pin", "5"),
                new("Pin", "6"),
                new("Pin", "6"),
                new("Pin", "7"),
                new("Pin", "7"),
                new("Pin", "8"),
                new("Sou", "2"),
                new("Sou", "4"),
                new("Sou", "6"),
                new("Sou", "6"),
            };

            foreach (var tile in hand)
            {
                Players[1].AddTile(tile, true);
            }

                TEST_TILES = new List<Tile>() //СПИСОК ТАЙЛОВ КОТОРЫЕ БУДУТ ВЫДАВАТЬСЯ ИГРОКУ
            {
                new("Sou", "1"),
                //new("Dragon", "Red"),
                new("Sou", "3")
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
       // Players[0].DrawHand();

        CenterView.UpdatePlayerBetView(Players);
        CenterView.UpdateWinds(Players[0].Wind, Players[1].Wind, Players[2].Wind, Players[3].Wind);
        CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
        CenterView.UpdateRoundWind(RoundWind);
        CenterView.UpdateBet(Bet);
        foreach (var player in Players)
        {
            player.SortHand();
            player.DrawHand();
        }
        StartTurn();
    }

    public void ExhaustiveDraw()
    {
        bool[]pays= new bool[4];

        for (int i = 0; i < pays.Length; i++) 
        {
            if (Players[i].WaitCosts.Count >0)
            {
                pays[i] = false;
                Players[i].PlayerHandView.DrawOpenHand(Players[i].Hand);
            }
            else
            {
                pays[i] = true;
                Players[i].PlayerHandView.DrawCloseHand(Players[i].Hand);
            }
        }

        int[] score_change = new int[4] {0,0,0,0};
        var count1=pays.Count(t=>t);
        var count2=pays.Count(t=>!t);

        if (count1!=0&&count1!=Players.Count)
        {
            int loses = 3000 / count1;
            int gains = 3000 / count2;
            
            for (int i = 0;i < 4;i++) 
                if (pays[i]) score_change[i] = -loses;
                else score_change[i] = gains;
            
        }

        VictoryScreen = true;
       StartCoroutine(VictoryScreenView.Draw(this, score_change));
        //NextRound();
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
        if (!Players[0].IsActive|| VictoryScreen) return;

        if (Players[0].Riichi) //если игрок объявил риичи то если есть тайлы со свойством то это тайл которым он объявляет риичи
            if (clickedTile.IsDiscardable())
                clickedTile.Properties.Add("Riichi");
            else return;//если таких нет то он уже объявил риичи и ждет последнего тайла

        CallsButtonsView.Clear();
        if (Players[0].Riichi)
        {
            Players[0].UpgradeWaits(clickedTile);
            ProcessRiichi(Players[0]);
        }
        StartCoroutine(Players[0].DiscardTile(clickedTile));
        if (Players[0].Riichi) Players[0].Ippatsu = true;
        Players[0].DrawDiscard();
    }

    public void ProcessRiichi(IPlayer player)
    {
        Bet++;
        player.Score -= 1000;
        CenterView.UpdateBet(Bet);
        CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
        CenterView.UpdatePlayerBetView(Players);
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
        if (callsSent == 0) 
        {
            EndTurn();
            return;
        }
        foreach (var player in Players)
        {
            if (player.HasCalls())
            {
                player.ProceedCalls();
            }
            
        }
        
    }

    public IEnumerator Executecalls()
    {
        callsSent--;
        if (callsSent > 0) yield break ;

        foreach (var player in Players)
        {
            if (player.ron!=(null,null))
            {
                //callTextViews[player.index].Draw("РОН!");
                //yield return new WaitForSeconds(0.5f);
                //callTextViews[player.index].Clear();

                player.ExecuteRon();
                ClearOtherCalls();
                //EndTurn();
                yield break;
            }
        }

        foreach (var player in Players)
        {
            if (player.pon != (null, null))
            {
                callTextViews[player.index].Draw("ПОН!");
                yield return new WaitForSeconds(0.5f);
                callTextViews[player.index].Clear();

                player.ExecutePon();
                ClearOtherCalls();
                //EndTurn();
                yield break;
            }
            if (player.kan != (null, null))
                {
                callTextViews[player.index].Draw("КАН!");
                yield return new WaitForSeconds(0.5f);
                callTextViews[player.index].Clear();

                player.ExecuteKan();
                ClearOtherCalls();
                //EndTurn();
                yield break;
            }

        }
        
        foreach (var player in Players)
        {
            if (player.chi != (null, null))                
            {
                callTextViews[player.index].Draw("ЧИ!");
                yield return new WaitForSeconds(0.5f);
                callTextViews[player.index].Clear();

                player.ExecuteChi(player.chi.Item1[0]);
                ClearOtherCalls();
                //EndTurn();
                yield break;
            }
        }
        EndTurn();

    }

    public void ClearOtherCalls()
    {
        foreach (var player in Players)
        {
            player.ClearCalls();
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
        StartCoroutine(VictoryScreenView.Draw(player, score_change, han, yakus));
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
