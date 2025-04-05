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
    public CenterView CenterView;
    public DoraIndicatorView DoraIndicatorView;
    public List<IPlayer> Players { get; private set; }
    public List<Tile> Wall { get; private set; }
    public List<Tile> DoraIndicator { get; private set; }=new List<Tile>();
    public List<Tile> UraDoraIndicator { get; private set; } =new List<Tile>();
    public List<Tile> KanTiles { get; private set; } = new List<Tile>();
    public int DorasShown { get; private set; }
    public int ActivePlayer {  get; private set; }
    public int Dealer { get; private set; }
    public string RoundWind {  get; private set; }
    public int RoundNumber { get; private set; }
    public int RepeatCounter { get; private set; }
    private void Start()
    {
        Debug.Log("���� ��������");
        TileSpriteManager.Initialize();
        //CenterView.OnCenterClicked.AddListener(HandleCenterClick);
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
        foreach (var player in Players) 
            player.Score = 25000;


        ActivePlayer = -1;

        PrepareRound();

        
    }

    public void StartTurn()
    {
        var currentPlayer = Players[ActivePlayer];
        currentPlayer.AddTile(Wall[0]);
        Wall.RemoveAt(0);
        PlayerHandView.Draw(Players[0].Hand);
        CenterView.UpdateTilesRemaining(Wall.Count);
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
    public void PrepareRound() //TODO ������� ��� ����� ������
    {
        SetWinds();

        Wall = TileFactory.CreateWall();
        DoraIndicator.Clear();
        UraDoraIndicator.Clear();
        KanTiles.Clear();

        for (int j=0;j<4;j++)
            for (int i = 0; i < 13; i++)
            {
                Players[j].AddTile(Wall[0]);
                Wall.RemoveAt(0);
            }
        for (int j=0;j<5; j++)
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
        DoraIndicatorView.Draw(DoraIndicator,DorasShown);
        PlayerHandView.Draw(Players[0].Hand);
        CenterView.UpdateWinds(Players[0].Wind, Players[1].Wind, Players[2].Wind, Players[3].Wind);
        CenterView.UpdateScore(Players[0].Score, Players[1].Score, Players[2].Score, Players[3].Score);
        CenterView.UpdateRoundWind(RoundWind);
        StartTurn();
    }

    public void ExhaustiveDraw()
    {
        //Players[0].Score += 1500; �����
        //Players[1].Score -= 2000;
        //Players[2].Score += 8000;
        //RoundWind = "South";
        NextRound();
    }

    public void NextRound() //TODO ������� ��� ����� ������
    {
        //Debug.Log("Next Round");
        foreach (var player in Players)
        {
            player.Clear();
            player.PlayerDiscardView.Draw(player.Discard);
        }

        
        RepeatCounter = 0;
        RoundNumber++;

        Dealer = (Dealer + 1) % 4;
        ActivePlayer = Dealer;
        PrepareRound();
    }

    public void SetWinds() //����� ������
    {
        List<string> winds = new List<string>() { "East", "South", "West", "North" };

        if (ActivePlayer==-1) Dealer=ActivePlayer = Random.Range(0,4); // ���� ������ ���� �� ��������
                                                                       // �������� ������

        int x = Dealer;
        for (int i = 0; i < 4; i++)
        {
            Players[x].Wind = winds[i];
            x = (x + 1) % 4;
        }

    }

    public void HandleTileClick(Tile clickedTile) //TODO ���������� �����
    {
        //Debug.Log("HandleTileCLick");
        Players[0].DiscardTile(clickedTile);
        Players[0].PlayerDiscardView.Draw(Players[0].Discard);

        PlayerHandView.Sort(Players[0].Hand);
        PlayerHandView.Draw(Players[0].Hand);
        EndTurn();
    }

    public void CheckForCalls(Tile tile,IPlayer sender)
    {
        foreach (var player in Players)
        {
            if ( player.CheckForPon(tile)) //�������� ����� �� ��� �� �� ������� �������� ���
            {
                Debug.Log(player.Name + " has pon of " + tile.ToString());
            }

            if (player.CheckForKan(tile)) //�������� ����� �� ��� �� �� ������� �������� ���
            {
                Debug.Log(player.Name + " has kan of " + tile.ToString());
            }
        }

        //�������� ����� �� ��� �� �� ������� �������� ��

        int index = Players.IndexOf(sender); //�������� ������ ������ ������� ������ ���
        index = (index + 1) % 4;// �������� ������ ������ ������ �� ����

        if (tile.Suit == "Dragon" || tile.Suit == "Wind") return;
        var chis = Players[index].CheckForChi(tile);//�������� ������ ��������� ��
        if (chis == null) return;
        foreach (var chi in chis)
        {
            Debug.Log(Players[index].Name + " has chi of " + chi[0].ToString() + " " + chi[1].ToString() + " " + chi[2].ToString());
        }

    }

    //public void HandleCenterClick()
    //{

    //}

}

