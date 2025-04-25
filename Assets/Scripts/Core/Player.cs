using static System.Math;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;



public interface IPlayer
{
    public string Name { get; set; }
    public int index { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsOpen { get; set; }
    public bool IsActive { get; set; }
    public bool Ippatsu { get; set; }
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public List<List<Tile>> Calls { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (Tile tile, IPlayer player) ron { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }
    public HandAnalyzer HandAnalyzer { get; set; }
    public List<DiscardWaitCost> DiscardWaitCosts { get; set; }
    public List<WaitCost> WaitCosts { get; set; }
    public bool TsumoAwailable { get; set; }
    public bool PermanentFuriten { get; set; }
    public bool TemporaryFuriten { get; set; }
    public bool Riichi { get; set; }
    public abstract void AddTile(Tile tile, bool isRoundStart = false);
    public abstract void StartTurn();
    public abstract void ExecuteChi(List<Tile> tiles);
    public abstract void ExecutePon();
    public abstract void ExecuteKan();
    public abstract IEnumerator ExecutePass();
    public abstract void ExecuteTsumo();
    public abstract void ExecuteRon();
    public abstract void TryAddDiscardWaitCost(DiscardWaitCost x);
    public abstract void UpgradeWaits(Tile tile);
    public abstract bool HasWaitOn(Tile tile);

    //public abstract void CallPon();
    //public abstract void CallKan();
    //public abstract void CallRon();
    //public abstract void CallChi();

    IEnumerator DiscardTile(Tile tile);
    void ClearCalls();

    // Проверка возможности вызовов
    public bool CheckForPon(Tile tile)
    {
        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 2) return true;
        return false;
    }
    public abstract int CheckForKan(Tile tile, bool isTsumo = false);
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
                    possibleSequences.Add(new List<Tile> { tile, t1, t2 });
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
                    possibleSequences.Add(new List<Tile> { tile, t1, t2 });
                }
            }
        }
        return possibleSequences;
    }
    public abstract bool CheckForRon(Tile tile);

    public void Clear()
    {
        Hand.Clear();
        Discard.Clear();
        TileCounts.Clear();
        CallContainerView.Clear();
        ClearCalls();
        Calls.Clear();
        IsOpen = false;
    }

    // Если какой-либо вызов объявлен, возвращается true
    public bool HasCalls()
    {
        if (pon != (null, null) || kan != (null, null) || chi != (null, null)||ron!=(null,null))
            return true;
        else
            return false;
    }

    // Метод, который должен обработать вызовы (реализуется по-разному для ИИ и реального игрока)
    public abstract void ProceedCalls();

    public void CallPon()
    {
        var copy_pon = pon;
        ClearCalls();
        pon = copy_pon;
        GameManager.Executecalls();
    }

    public void CallChi()
    {
        var copy_chi = chi;
        ClearCalls();
        chi = copy_chi;
        GameManager.Executecalls();
    }

    public void CallKan()
    {
        var copy_kan = kan;
        ClearCalls();
        kan = copy_kan;
        GameManager.Executecalls();
    }

    public void CallRon()
    {
        var copy_ron = ron;
        ClearCalls();
        ron = copy_ron;
        GameManager.Executecalls();
    }
}




public class AiPlayer : IPlayer
{
    public string Name { get; set; }
    public int index { get; set; }
    public string Wind { get; set; }
    public int Score { get; set; }
    ////////////////////////////////////////////////////////
    public bool IsOpen { get; set; }
    public List<Tile> Hand { get; set; }
    public List<List<Tile>> Calls { get; set; }
    public List<Tile> Discard { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    ////////////////////////////////////////////////////////      
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }    
    ////////////////////////////////////////////////////////
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }
    public (Tile tile, IPlayer player) ron { get; set; }
    ////////////////////////////////////////////////////////    
    public HandAnalyzer HandAnalyzer { get; set; }
    public List<DiscardWaitCost> DiscardWaitCosts { get; set; }
    public List<WaitCost> WaitCosts { get; set; }
    ////////////////////////////////////////////////////////
    public bool IsActive { get; set; }
    public bool Riichi { get; set; }=false;
    public bool Ippatsu { get; set; } = false;
    public bool TsumoAwailable { get; set; }=false ;
    public bool PermanentFuriten { get; set; } = false;
    public bool TemporaryFuriten { get; set; } = false;


    public AiPlayer(string name, GameManager gameManager, PlayerDiscardView playerDiscardView,
        CallContainerView callContainerView,int index)
    {
        Name = name;
        Hand = new List<Tile>();
        Discard = new List<Tile>();
        IsActive = false;
        GameManager = gameManager;
        PlayerDiscardView = playerDiscardView;
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
        TileCounts = new Dictionary<string, int>();
        Calls = new List<List<Tile>>();
        this.index = index;
        CallContainerView = callContainerView;
        IsOpen=false;
        HandAnalyzer = new HandAnalyzer(this);
        DiscardWaitCosts = new List<DiscardWaitCost>();
        WaitCosts = new List<WaitCost>();
    }

    public void AddTile(Tile tile,bool isRoundStart=false)
    {
        Hand.Add(tile);
    }

    public IEnumerator DiscardTile(Tile tile)
    {
        Hand.Remove(tile);
        Discard.Add(tile);
        PlayerDiscardView.Draw(Discard);

        //GameManager.CheckForCalls(tile, this);
        yield return GameManager.WAIT_TIME;
        GameManager.CheckForCalls(tile, this);
    }


    public void StartTurn()
    {
        if (GameManager.TEST_TILES2.Count>0&&index==3&&GameManager.TEST_HAND)
        {
            GameManager.StartCoroutine(DiscardTile(GameManager.TEST_TILES2[0]));
            GameManager.TEST_TILES2.RemoveAt(0);
        }
        else
        {
            int index = Random.Range(0, Hand.Count);
            GameManager.StartCoroutine(DiscardTile(Hand[index]));
        }
    }

    public void ClearCalls()
    {
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
    }


    public void ProceedCalls()
    {
        GameManager.StartCoroutine(ExecutePass());
    }

    public void ExecuteChi(List<Tile> t) { }
    public void ExecutePon() { }
    public void ExecuteKan() { }
    public IEnumerator ExecutePass()
    {
        if (TsumoAwailable) TsumoAwailable = false;
        if (ron != (null, null))
            if (Riichi) PermanentFuriten = true;
            else TemporaryFuriten = true;

        ClearCalls();
        if (Riichi)
        {
            yield return GameManager.WAIT_TIME;
            GameManager.EndTurn();
        }
        else
        if (!IsActive)
            GameManager.EndTurn();
    }
    public void ExecuteTsumo() { }
    public void ExecuteRon() { }
    public void UpgradeWaits(Tile tile) { }
    public int CheckForKan(Tile tile, bool isTsumo = false)
    {
        //0= нет кана
        //1=открытый кан
        //2=закрытый кан


        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 3)
            {
                foreach (var call in Calls)
                    if (call.Contains(tile) && isTsumo) return 1;
                if (isTsumo) return 2;
                else return 1;
            }
        return 0;

    }
    public bool CheckForRon(Tile tile)
    {
        return false;
    }
    public  void TryAddDiscardWaitCost(DiscardWaitCost x)
    {

    }

    public  bool HasWaitOn(Tile tile)
    {
        if (tile != null)
            if (WaitCosts.Any(t => t.Wait.Equals(tile)))
                return true;
        return false;
    }

    //public void CallPon() { }
    //public void CallKan() { }
    //public void CallChi() { }
    //public void CallRon() { }
}




public class RealPlayer : IPlayer
{
    public string Name { get; set; }
    public int index { get; set; }
    public string Wind { get; set; }
    public int Score { get; set; }
    ////////////////////////////////////////////////////////
    public bool IsOpen { get; set; }
    public List<Tile> Hand { get; set; }
    public List<List<Tile>> Calls { get; set; }
    public List<Tile> Discard { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    ////////////////////////////////////////////////////////      
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }
    ////////////////////////////////////////////////////////
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }
    public (Tile tile, IPlayer player) ron { get; set; }
    ////////////////////////////////////////////////////////    
    public HandAnalyzer HandAnalyzer { get; set; }
    public List<DiscardWaitCost> DiscardWaitCosts { get; set; }
    public List<WaitCost> WaitCosts { get; set; }
    ////////////////////////////////////////////////////////
    public bool IsActive { get; set; }
    public bool Riichi { get; set; } = false;
    public bool Ippatsu { get; set; } = false;
    public bool TsumoAwailable { get; set; } = false;
    public bool PermanentFuriten { get; set; } = false;
    public bool TemporaryFuriten { get; set; } = false;
    public bool DiscardFuriten { get; set; } = false;



    public RealPlayer(string name, GameManager gameManager, PlayerDiscardView playerDiscardView,
        CallContainerView callContainerView,int index)
    {
        Name = name;
        Hand = new List<Tile>();
        Discard = new List<Tile>();
        IsActive = false;
        GameManager = gameManager;
        PlayerDiscardView = playerDiscardView;
        TileCounts = new Dictionary<string, int>();

        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
        ron = (null, null);

        Calls = new List<List<Tile>>();
        this.index = index;
        CallContainerView = callContainerView;
        IsOpen = false;
        HandAnalyzer = new HandAnalyzer(this);
        DiscardWaitCosts = new List<DiscardWaitCost>();
        WaitCosts = new List<WaitCost>();
    }

    public void AddTile(Tile tile,bool isRoundStart=false)
    {
        Hand.Add(tile);
        if (!isRoundStart)
        {
            var kanCheck = CheckForKan(tile);
            if (kanCheck==2||kanCheck==1)
                { 
                kan = (tile, this);
                //ProceedCalls();
                }
            
        }
        if (TileCounts.ContainsKey(tile.ToString()))
            TileCounts[tile.ToString()]++;
        else
            TileCounts[tile.ToString()] = 1;
    }

    public IEnumerator DiscardTile(Tile tile)
    {
        if (!Riichi)
            UpgradeWaits(tile);
        TemporaryFuriten = false;
        Ippatsu = false;
        Hand.Remove(tile);
        GameManager.PlayerHandView.Sort(Hand);
        GameManager.PlayerHandView.Draw(Hand);

        Discard.Add(tile);
        PlayerDiscardView.Draw(Discard);

        TileCounts[tile.ToString()]--;

        yield return GameManager.WAIT_TIME;
        GameManager.CheckForCalls(tile, this);

        //GameManager.CheckForCalls(tile, this);

    }


    public void StartTurn()
    {
        GameManager.StartCoroutine(CheckForTsumo());
        ProceedCalls();
    }

    public void ClearCalls()
    {
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
        ron = (null, null);
        TsumoAwailable = false;
    }

    /// <summary>
    /// Добавляет кнопки для возможных вызовов и кнопку для пропуска
    /// </summary>
    public void ProceedCalls()
    {
        bool buttoncreated = false;
        // При наличии вызовов отображаем соответствующие кнопки на UI.
        if (!IsOpen && DiscardWaitCosts.Count > 0 && GameManager.Wall.Count>4 &&!Riichi&&IsActive)
            {
            GameManager.CallsButtonsView.CreateRiichiButton();
            buttoncreated = true;
            }

        if (TsumoAwailable)
            {
            GameManager.CallsButtonsView.CreateTsumoButton();
            buttoncreated = true;
            }

        if (pon != (null, null))
            { 
            GameManager.CallsButtonsView.CreatePonButton();
            buttoncreated = true;
        }
        if (kan != (null, null))
            { 
            GameManager.CallsButtonsView.CreateKanButton();
            buttoncreated = true;
            }
        if (chi != (null, null))
            { 
            GameManager.CallsButtonsView.CreateChiButton();
            buttoncreated = true;
            }
        if ( ron!= (null, null))
        {
            GameManager.CallsButtonsView.CreateRonButton();
            buttoncreated = true;
        }
        if (buttoncreated)GameManager.CallsButtonsView.CreatePassButton();
    }

    

    public void ExecutePon()
    {
        IsOpen = true;
        //Удаление тайла из дискарда другого игрока и двух из руки
        pon.player.Discard.Remove(pon.tile);

        for (int i = 0; i < 2; i++)
        {
            Hand.Remove(pon.tile);
                    }
        TileCounts[pon.tile.ToString()] -= 3;
        //Создание новой тройки открытых тайлов
        Calls.Add(new List<Tile>());

        for (int i = 0;i < 3; i++)
        {
            Calls[^1].Add(pon.tile.Clone());
        }
        
        int left = (index - 1+4) % 4;//индекс игрока слева
        int middle = (index + 2+4) % 4;//индекс игрока напротив
        int right = (index + 1+4) % 4;//индекс игрока справа

        if (pon.player.index == left)//в зависимости от того у котого взят тайл, переворачиваем на 90 градусов соответствующий тайл
        {
            Calls[^1][0].Properties.Add("Called");
        }
        else if (pon.player.index == middle) 
        {
            Calls[^1][1].Properties.Add("Called");
        }
        else if (pon.player.index == right)
        {
            Calls[^1][2].Properties.Add("Called");
        }
        CallContainerView.Draw(Calls);
        GameManager.PlayerHandView.Draw(Hand);
        pon.player.PlayerDiscardView.Draw(pon.player.Discard);
        ClearCalls();
        GameManager.EndTurn(0);
        GameManager.CallsButtonsView.Clear();

        //// Здесь можно реализовать дополнительную логику оформления пон
        //GameManager.OnCallDecisionMade(this);
        //ClearCalls();
        //GameManager.CallsButtonsView.Clear();
    }

    public void ExecuteKan()
    {
        if (kan.player == this) //если тайл для кана - цумо тайл
        {
            foreach (var call in Calls) // проверка если уже был объявлен пон из этого тайла, то кан открытый
                if (call.Contains(kan.tile))
                {
                    IsOpen = true;
                    var tile = kan.tile.Clone();
                    tile.Properties.Add("Called");
                    if (call[0].Properties.Contains("Called"))
                        call.Insert(0, tile);
                    else if (call[1].Properties.Contains("Called"))
                        call.Insert(1, tile);
                    else if (call[2].Properties.Contains("Called"))
                        call.Insert(2, tile);
                    Hand.Remove(kan.tile);
                    TileCounts[kan.tile.ToString()]--;
                    CallContainerView.Draw(Calls);
                    AddTile(GameManager.KanTiles[0]);
                    GameManager.KanTiles.RemoveAt(0);
                    GameManager.RevealDora();
                    GameManager.PlayerHandView.Draw(Hand);
                    ClearCalls();
                    GameManager.CallsButtonsView.Clear();
                    return;
                }

            //Обработка ситуации если пона не было и кан закрытый
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < 4; i++)
            {
                var tile = kan.tile.Clone();
                tile.Properties.Add("Called");
                tiles.Add(tile);
                Hand.Remove(kan.tile);
            }
            Calls.Add(tiles);
            CallContainerView.Draw(Calls);
            TileCounts[kan.tile.ToString()] -= 4;
            AddTile(GameManager.KanTiles[0]);
            GameManager.KanTiles.RemoveAt(0);
            GameManager.RevealDora();
            GameManager.PlayerHandView.Draw(Hand);

            ClearCalls();
            GameManager.CallsButtonsView.Clear();

            GameManager.StartCoroutine(CheckForTsumo());

            return;


        }

        //Обработка ситуации если другой игрок сбросил тайл для кана
        IsOpen = true;
        kan.player.Discard.Remove(kan.tile);
        for (int i = 0; i < 3; i++)
        {
            Hand.Remove(kan.tile);
            
        }
        TileCounts[kan.tile.ToString()] -= 3;
        List<Tile> tiles_ = new List<Tile>();
        for (int i = 0; i < 4; i++)
        {
            var tile = kan.tile.Clone();
            tiles_.Add(tile);
        }

        Calls.Add(tiles_);

        int left = (index - 1 + 4) % 4;//индекс игрока слева
        int middle = (index + 2 + 4) % 4;//индекс игрока напротив
        int right = (index + 1 + 4) % 4;//индекс игрока справа

        if (kan.player.index == left)//в зависимости от того у котого взят тайл, переворачиваем на 90 градусов соответствующий тайл
        {
            Calls[^1][0].Properties.Add("Called");
        }
        else if (kan.player.index == middle)
        {
            Calls[^1][1].Properties.Add("Called");
        }
        else if (kan.player.index == right)
        {
            Calls[^1][2].Properties.Add("Called");
        }
        kan.player.Discard.Remove(kan.tile);
        kan.player.PlayerDiscardView.Draw(kan.player.Discard);

        Hand.Remove(kan.tile);
        Hand.Remove(kan.tile);
        Hand.Remove(kan.tile);

        CallContainerView.Draw(Calls);

        AddTile(GameManager.KanTiles[0]);
        GameManager.KanTiles.RemoveAt(0);

        GameManager.RevealDora();
        GameManager.PlayerHandView.Draw(Hand);
        ClearCalls();
        GameManager.EndTurn(0);
        GameManager.CallsButtonsView.Clear();
    }

    public void ExecuteChi(List<Tile> tiles)
    {
        IsOpen=true;
        chi.player.Discard.Remove(tiles[0]);
        Hand.Remove(tiles[1]);
        Hand.Remove(tiles[2]);
        TileCounts[tiles[1].ToString()]--;
        TileCounts[tiles[2].ToString()]--;

        Calls.Add(new List<Tile>());

        for (int i = 0; i < 3; i++)
        {
            Calls[^1].Add(tiles[i].Clone());
        }
        Calls[^1][0].Properties.Add("Called");

        CallContainerView.Draw(Calls);
        GameManager.PlayerHandView.Draw(Hand);
        chi.player.PlayerDiscardView.Draw(chi.player.Discard);
        ClearCalls();
        GameManager.EndTurn(0);
        GameManager.CallsButtonsView.Clear();
    }

    public IEnumerator ExecutePass()
    {
        if (TsumoAwailable) TsumoAwailable = false;
        if (ron != (null, null))
            if (Riichi) PermanentFuriten = true;
            else TemporaryFuriten = true;

            ClearCalls();
        if (Riichi)
        {
            yield return GameManager.WAIT_TIME;
            GameManager.EndTurn();
        }
        else
        if (!IsActive)
         GameManager.EndTurn();        
    }

    public void CallRiichi()
    {

    }

    public void ExecuteTsumo() 
    {
        var wait_cost = WaitCosts.FirstOrDefault(t => t.Wait.Equals(Hand[^1]));//находим стоимость руки с цумо тайлом

        var fu = wait_cost.Costs.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов

        fu.Cost += 2; //надбавка за цумо

        fu.Cost += 20; //минимальная базовая стоимость

        if (wait_cost.Costs.Any(z => z.Yaku == "Семь пар"))
            fu.Cost = 25;//фиксированная стоимость 7 пар
        else
        fu.Cost= (int)Ceiling(fu.Cost / 10.0) * 10;//округление очков фу в большую сторону


        if (!IsOpen) wait_cost.Costs.Insert(0, ("Цумо", 1));

        if (Riichi)
        {
            if (Ippatsu) wait_cost.Costs.Insert(0, ("Иппацу", 1));
            wait_cost.Costs.Insert(0, ("Риичи", 1));
        }
        

        var round_end = new RoundEndCalculator();
        round_end.CountTsumoPoints(this, wait_cost.Costs);
    }

    public void ExecuteRon()
    {
        Debug.Log("Ron");
        var wait_cost = WaitCosts.FirstOrDefault(t => t.Wait.Equals(ron.tile));

        var fu = wait_cost.Costs.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов
        
        fu.Cost += 20; //минимальная базовая стоимость
        if (IsOpen) fu.Cost += 10; //надбавка за рон с закрытой рукой

        if (wait_cost.Costs.Any(z => z.Yaku == "Семь пар"))
            fu.Cost = 25;//фиксированная стоимость 7 пар
        else
            fu.Cost = (int)Ceiling(fu.Cost / 10.0) * 10;//округление очков фу в большую сторону

        if (Riichi)
        {
            if (Ippatsu) wait_cost.Costs.Insert(0, ("Иппацу", 1));
            wait_cost.Costs.Insert(0, ("Риичи", 1));
            
        }

        Hand.Add(ron.tile);

        var round_end=new RoundEndCalculator();
        round_end.CountRonPoints(this, ron.player, wait_cost.Costs);
    }

    /// <summary>
    /// 0=нет кана,
    /// 1=открытый кан,
    /// 2=закрытый кан
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="isTsumo"></param>
    /// <returns></returns>
    public int CheckForKan(Tile tile, bool isTsumo = false)
    {
        //0= нет кана
        //1=открытый кан
        //2=закрытый кан


        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 3)
            {
                if (isTsumo) return 2;
                else return 1;
            }
        foreach (var call in Calls)
            if (isPon(call) && isTsumo) return 1;
        return 0;

    }

    public IEnumerator CheckForTsumo()
    {
       
        foreach (var waitcost in WaitCosts)
        {   // Если есть ожидание на взятый тайл 
            if (waitcost.Wait.Equals(Hand[^1]))
                //Если у руки есть стоимость ИЛИ если риичи ИЛИ если рука закрыта - можно объявить цумо
                if (waitcost.Costs.Count > 0||Riichi||!IsOpen) 
                    {
                    TsumoAwailable = true;
                    break;
                    }
        }
        if (TsumoAwailable) yield break;

        if (Riichi && kan==(null,null))
        {
            yield return GameManager.WAIT_TIME;
            GameManager.StartCoroutine(DiscardTile(Hand[^1])); //пауза, после сброс цумо тайла
        }
                                        
    }


    public void TryAddDiscardWaitCost(DiscardWaitCost x)
    {
        // Ищем существующий элемент с таким же Discard
        var existing = DiscardWaitCosts.FirstOrDefault(c => c.Discard.Equals(x.Discard));

        if (existing != null)
        {
            // Если элемент существует, добавляем новые WaitCost из x, которых ещё нет в списке
            foreach (var waitCost in x.WaitsCosts)
            {
                // Проверяем, есть ли WaitCost с таким же Wait (сравниваем по значению)
                if (!existing.WaitsCosts.Any(wc => wc.Wait.Equals(waitCost.Wait)))
                {
                    existing.WaitsCosts.Add(waitCost);
                }
            }
        }
        else
        {
            // Если элемент не найден, добавляем новый DiscardWaitCost и помечаем тайл как сбрасываемый
            DiscardWaitCosts.Add(x);
            foreach (var tile in Hand)
            {
                if (tile.Equals(x.Discard) && !tile.IsDiscardable())
                {
                    tile.AddDiscardable();
                }
            }
        }
    }
    /// <summary>
    /// Если сброшенный тайл есть в списке дискард-ожидания, то обновляем ожидания игрока  на тайлы
    /// </summary>
    public void UpgradeWaits(Tile tile)
    {
        
        WaitCosts.Clear();
        ClearDiscardableMarks();
        if (DiscardWaitCosts.Any(t => t.Discard.Equals(tile)))
        {
            WaitCosts = DiscardWaitCosts.Where(c => c.Discard.Equals(tile)).Select(c => c.WaitsCosts).FirstOrDefault();            
        }

        if (WaitCosts.Any(t => Discard.Contains(t.Wait))) DiscardFuriten = true;
        else DiscardFuriten = false;
        DiscardWaitCosts.Clear();
    }

    /// <summary>
    /// Убирает у всех тайлов пометку что их можно сбросить чтобы стать темпай
    /// </summary>
    public void ClearDiscardableMarks()
    {
        foreach (var t in Hand)
            t.RemoveDiscardable();
    }

    public bool CheckForRon(Tile tile)
    {
        if (TemporaryFuriten || PermanentFuriten||DiscardFuriten) return false;
        foreach (var waitcost in WaitCosts)
        {
            if (waitcost.Wait.Equals(tile))
                return true;
        }
        return false;
    }

    public bool HasWaitOn(Tile tile)
    {
        if (tile != null)
            if (WaitCosts.Any(t => t.Wait.Equals(tile)))
                return true;
        return false;
    }

    public bool isPon(List<Tile> block)
    {
        if (block.Count != 3) return false;
        return block[0].Equals(block[1]) && block[0].Equals(block[2]);
    }
}



