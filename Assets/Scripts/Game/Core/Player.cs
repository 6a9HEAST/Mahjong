using static System.Math;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Unity.Burst.Intrinsics;

public abstract class IPlayer
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
    public IHandView PlayerHandView { get; set; }
    public IDiscardView PlayerDiscardView { get; set; }
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
    public bool DiscardFuriten { get; set; } = false;


    public void AddTile(Tile tile, bool isRoundStart = false)
    {
        Hand.Add(tile);
        
        if (!isRoundStart)
        {
            DrawHand();
            //Debug.Log(Name + " " + Hand.Count);
            var kanCheck = CheckForKan(tile);
            if (kanCheck == 2 || kanCheck == 1)
            {
                kan = (tile, this);
                ProceedCalls();
            }


        }
        if (TileCounts.ContainsKey(tile.ToString()))
            TileCounts[tile.ToString()]++;
        else
            TileCounts[tile.ToString()] = 1;
    }
    public abstract void StartTurn();
    public void ExecuteChi(List<Tile> tiles)
    {
        IsOpen = true;
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

        CallContainerView.DrawCalls(Calls);
        DrawHand();
        chi.player.PlayerDiscardView.Draw(chi.player.Discard);
        ClearCalls();
        GameManager.EndTurn(index);
        GameManager.CallsButtonsView.Clear();
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

        for (int i = 0; i < 3; i++)
        {
            Calls[^1].Add(pon.tile.Clone());
        }

        int left = (index - 1 + 4) % 4;//индекс игрока слева
        int middle = (index + 2 + 4) % 4;//индекс игрока напротив
        int right = (index + 1 + 4) % 4;//индекс игрока справа

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
        CallContainerView.DrawCalls(Calls);
        DrawHand();
        pon.player.PlayerDiscardView.Draw(pon.player.Discard);
        ClearCalls();
        GameManager.EndTurn(index);
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
                    CallContainerView.DrawCalls(Calls);
                    AddTile(GameManager.KanTiles[0]);
                    GameManager.KanTiles.RemoveAt(0);
                    GameManager.RevealDora();
                    DrawHand();
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
            CallContainerView.DrawCalls(Calls);
            TileCounts[kan.tile.ToString()] -= 4;
            AddTile(GameManager.KanTiles[0]);
            GameManager.KanTiles.RemoveAt(0);
            GameManager.RevealDora();
            DrawHand();

            ClearCalls();
            GameManager.CallsButtonsView.Clear();

            CheckForTsumo();

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

        CallContainerView.DrawCalls(Calls);

        AddTile(GameManager.KanTiles[0]);
        GameManager.KanTiles.RemoveAt(0);

        GameManager.RevealDora();
        DrawHand();
        ClearCalls();
        GameManager.EndTurn(index);
        GameManager.CallsButtonsView.Clear();
    }

    public bool CheckForTsumo()
    {

        foreach (var waitcost in WaitCosts)
        {   // Если есть ожидание на взятый тайл 
            if (waitcost.Wait.Equals(Hand[^1]))
                //Если у руки есть стоимость ИЛИ если риичи ИЛИ если рука закрыта - можно объявить цумо
                if (waitcost.Costs.Count > 0 || Riichi || !IsOpen)
                {
                    TsumoAwailable = true;
                    ProceedCalls();
                    return true;
                }
        }

        if (Riichi && kan == (null, null))
        {
            //yield return GameManager.WAIT_TIME;
            GameManager.StartCoroutine(DiscardTile(Hand[^1])); //пауза, после сброс цумо тайла
            return true;
        }
        return false;

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
            GameManager.StartCoroutine(GameManager.Executecalls());
        }
        else if (!IsActive)
            GameManager.StartCoroutine(GameManager.Executecalls());
    }

    public void ExecuteTsumo()
    {
        
        var wait_cost = WaitCosts.FirstOrDefault(t => t.Wait.Equals(Hand[^1]));//находим стоимость руки с цумо тайлом

        var fu = wait_cost.Costs.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов
        wait_cost.Costs.Remove(fu);
        fu.Cost += 2; //надбавка за цумо

        fu.Cost += 20; //минимальная базовая стоимость

        if (wait_cost.Costs.Any(z => z.Yaku == "Семь пар"))
            fu.Cost = 25;//фиксированная стоимость 7 пар
        else
            fu.Cost = (int)Ceiling(fu.Cost / 10.0) * 10;//округление очков фу в большую сторону


        if (!IsOpen) wait_cost.Costs.Insert(0, ("Цумо", 1));

        if (Riichi)
        {
            if (Ippatsu) wait_cost.Costs.Insert(0, ("Иппацу", 1));
            wait_cost.Costs.Insert(0, ("Риичи", 1));
        }

        wait_cost.Costs.Add(fu);

        foreach (var tile in Hand) tile.RemoveDiscardable();
        GameManager.RonTsumoPopUpView.Draw("Цумо!");
        GameManager.AudioPlayer.StopMusic();
        GameManager.AudioPlayer.PlayRonTsumo();
        PlayerHandView.DrawOpenHand(Hand);
        var round_end = new RoundEndCalculator();
        round_end.CountTsumoPoints(this, wait_cost.Costs);
        

    }
    public void ExecuteRon()
    {
        Debug.Log("Ron");
        var wait_cost = WaitCosts.FirstOrDefault(t => t.Wait.Equals(ron.tile));

        var fu = wait_cost.Costs.FirstOrDefault(a => a.Yaku == "Fu");//находим количество минипоинтов
        wait_cost.Costs.Remove(fu);
        fu.Cost += 20; //минимальная базовая стоимость
        if (!IsOpen) fu.Cost += 10; //надбавка за рон с закрытой рукой

        if (wait_cost.Costs.Any(z => z.Yaku == "Семь пар"))
            fu.Cost = 25;//фиксированная стоимость 7 пар
        else
            fu.Cost = (int)Ceiling(fu.Cost / 10.0) * 10;//округление очков фу в большую сторону
        wait_cost.Costs.Add(fu);
        if (Riichi)
        {
            if (Ippatsu) wait_cost.Costs.Insert(0, ("Иппацу", 1));
            wait_cost.Costs.Insert(0, ("Риичи", 1));

        }

        Hand.Add(ron.tile);

        foreach (var tile in Hand) tile.RemoveDiscardable();

        GameManager.RonTsumoPopUpView.Draw("Рон!");
        GameManager.AudioPlayer.StopMusic();
        GameManager.AudioPlayer.PlayRonTsumo();

        PlayerHandView.DrawOpenHand(Hand);

        var round_end = new RoundEndCalculator();
        round_end.CountRonPoints(this, ron.player, wait_cost.Costs);
    }
    public void TryAddDiscardWaitCost(DiscardWaitCost x)
    {
        Debug.Log(this.Name + " Try add wait cost");
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
        string debug_message = "";
        if (Hand.Any(t => t.IsDiscardable())) debug_message = "найдены";
        else debug_message = "не найдены";
        Debug.Log("Метки после добавления: " + debug_message);
    }
    public abstract void UpgradeWaits(Tile tile);
    public bool HasWaitOn(Tile tile)
    {
        if (tile != null)
            if (WaitCosts.Any(t => t.Wait.Equals(tile)))
                return true;
        return false;
    }
    //public abstract IEnumerator DiscardTile(Tile tile);

    public  IEnumerator DiscardTile(Tile tile)
    {
        if (tile.Properties.Contains("Riichi"))
        {
            GameManager.callTextViews[index].Draw("РИИЧИ!");
            yield return new WaitForSeconds(0.5f);
            GameManager.callTextViews[index].Clear();
        }

        if (!Riichi)
            UpgradeWaits(tile);
        TemporaryFuriten = false;
        Ippatsu = false;
        tile.RemoveDiscardable();
        //Hand.Remove(tile);

        int idx = Hand.FindIndex(t => ReferenceEquals(t, tile));
        if (idx >= 0)
            Hand.RemoveAt(idx);

        SortHand();
        DrawHand();
        //GameManager.AudioPlayer.PlayTileDiscard();
        Discard.Add(tile);
        PlayerDiscardView.Draw(Discard);

        TileCounts[tile.ToString()]--;

        yield return 0;
        GameManager.CheckForCalls(tile, this);
    }
    public void ClearCalls()
    {
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
        ron = (null, null);
        TsumoAwailable = false;
    }

    // Проверка возможности вызовов
    public bool CheckForPon(Tile tile)
    {
        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 2) return true;
        return false;
    }
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
            if (IsPon(call) && isTsumo) return 1;
        return 0;

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
    public bool CheckForRon(Tile tile)
    {
        if (TemporaryFuriten || PermanentFuriten || DiscardFuriten) return false;
        foreach (var waitcost in WaitCosts)
        {
            if (waitcost.Wait.Equals(tile))
                if (waitcost.Costs.Count(t=>t.Yaku!="Fu")>0)
                return true;
                else
                {
                    TemporaryFuriten = true;
                    return false;
                }
        }
        return false;
    }

    public void Clear()
    {
        Hand.Clear();
        DrawHand();
        Discard.Clear();
        TileCounts.Clear();
        CallContainerView.Clear();
        ClearCalls();
        Calls.Clear();
        IsOpen = false;
        DiscardFuriten = false;
        TemporaryFuriten = false;
        PermanentFuriten = false;
        DiscardWaitCosts.Clear();
        WaitCosts.Clear();
    }

    // Если какой-либо вызов объявлен, возвращается true
    public bool HasCalls()
    {       
      return pon != (null, null) || kan != (null, null) || chi != (null, null) || ron != (null, null);       
    }

    // Метод, который должен обработать вызовы (реализуется по-разному для ИИ и реального игрока)
    public abstract void ProceedCalls();

    public void CallPon()
    {
        var copy_pon = pon;
        ClearCalls();
        pon = copy_pon;
        GameManager.StartCoroutine(GameManager.Executecalls());
    }

    public void CallChi()
    {
        var copy_chi = chi;
        ClearCalls();
        chi = copy_chi;
        GameManager.StartCoroutine(GameManager.Executecalls());
    }

    public void CallKan()
    {
        var copy_kan = kan;
        ClearCalls();
        kan = copy_kan;
        GameManager.StartCoroutine(GameManager.Executecalls());
    }

    public void CallRon()
    {
        var copy_ron = ron;
        ClearCalls();
        ron = copy_ron;
        GameManager.StartCoroutine(GameManager.Executecalls());
    }

    public bool IsPon(List<Tile> block)
    {
        if (block.Count != 3) return false;
        return block[0].Equals(block[1]) && block[0].Equals(block[2]);
    }

    public void DrawHand()
    {
        PlayerHandView.Draw(Hand);
    }

    public void DrawHand(int tilesToDraw)
    {
        PlayerHandView.Draw(Hand, tilesToDraw);
    }

    public void SortHand()
    {
        PlayerHandView.Sort(Hand);
        //PlayerHandView.Draw(Hand);
    }

    public void DrawDiscard()
    {
        PlayerDiscardView.Draw(Discard);
    }
}




public class AiPlayer : IPlayer
{
    private AiHandAnalyzer AiHandAnalyzer;

    public AiPlayer(string name, GameManager gameManager, IDiscardView playerDiscardView, IHandView playerHandView,
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
        PlayerHandView = playerHandView;
        AiHandAnalyzer = new AiHandAnalyzer(this);
    }
    public override void StartTurn()
    {
        GameManager.StartCoroutine(MakeMove());
    }

    public IEnumerator MakeMove()
    {
        yield return GameManager.WAIT_TIME;

        if (CheckForTsumo()) yield break ; //если есть цумо или автосброс в риичи то return

        if (TryDeclareRiichi()) yield break; //если можно объявить риичи то return

        if (FindEasyDiscard()) yield break; //если есть одиночный несредний то сбрасываем его

        var bestDiscard = AiHandAnalyzer.GetBestDiscard(); //поиск лушчего тайла для дискарда

        GameManager.StartCoroutine(DiscardTile(bestDiscard));
    }

    public override void ProceedCalls()
    {
        if (ron != (null, null))
        {
            CallRon();
            return;
        }

        if (kan != (null, null))
            if (kan.tile.Suit == "Dragon" || kan.tile.Rank == Wind || kan.tile.Rank == GameManager.RoundWind)
            {
                CallKan();
                return;
            }

        if (pon!=(null,null))
            if (pon.tile.Suit=="Dragon"||pon.tile.Rank==Wind||pon.tile.Rank==GameManager.RoundWind)
            { 
                CallPon();
                return;
            }

        //if (chi != (null,null))
        //{
        //    CallChi();
        //    return;
        //}

        if (TsumoAwailable)
        {
            ExecuteTsumo();
            return;
        }
        

        //if (chi!=(null,null))
        //{
        //    CallChi();
        //    return;
        //}
        GameManager.StartCoroutine(ExecutePass());
    }

    public bool TryDeclareRiichi()
    {
        if (!Riichi && DiscardWaitCosts.Count > 0 && !IsOpen)
        {
            Riichi = true;
            Ippatsu = true;

            int discardIndex= DiscardWaitCosts.Select((value, index) => new { value, index })
            .Aggregate((a, b) => a.value.WaitsCosts.Count > b.value.WaitsCosts.Count ? a : b)
            .index;//нахождение самого дорого ожидания

            Tile tile = new(DiscardWaitCosts[discardIndex].Discard);

            UpgradeWaits(tile);
            GameManager.ProcessRiichi(this);
            tile.Properties.Add("Riichi");
            
            GameManager.StartCoroutine(DiscardTile(tile));
            PlayerHandView.Sort(Hand);
            DrawDiscard();
            return true;

        }
        else return false;
    }

    public override void UpgradeWaits(Tile tile)// OVERRIDE
    {

        WaitCosts.Clear();        
        if (DiscardWaitCosts.Any(t => t.Discard.Equals(tile)))
        {
            WaitCosts = DiscardWaitCosts.Where(c => c.Discard.Equals(tile)).Select(c => c.WaitsCosts).FirstOrDefault();
        }

        if (WaitCosts.Any(t => Discard.Contains(t.Wait))) DiscardFuriten = true;
        else DiscardFuriten = false;
        DiscardWaitCosts.Clear();
    }

    //public override IEnumerator DiscardTile(Tile tile)
    //{
    //    if (!Riichi)
    //        UpgradeWaits(tile);
    //    TemporaryFuriten = false;
    //    Ippatsu = false;
    //    Hand.Remove(tile);
    //    //GameManager.PlayerHandView.Sort(Hand);
    //    //GameManager.PlayerHandView.Draw(Hand);

    //    Discard.Add(tile);
    //    PlayerDiscardView.Draw(Discard);

    //    TileCounts[tile.ToString()]--;

    //    yield return GameManager.WAIT_TIME;
    //    GameManager.CheckForCalls(tile, this);
    //}

    public bool FindEasyDiscard()
    {
        foreach (Tile tile in Hand)
        {
            if (IsSingle(tile) && IsPriority1(tile))
            {
                GameManager.StartCoroutine(DiscardTile(tile));
                return true;
            }
        }

        //foreach (Tile tile in Hand)
        //{
        //    if (IsSingle(tile) && IsPriority2(tile))
        //    {
        //        GameManager.StartCoroutine(DiscardTile(tile));
        //        return true;
        //    }
        //}

        foreach (Tile tile in Hand)
        {
            if (IsSingle(tile) && IsPriority3(tile))
            {
                GameManager.StartCoroutine(DiscardTile(tile));
                return true;
            }
        }

        foreach (Tile tile in Hand)
        {
            if (IsSingle(tile) && IsPriority4(tile))
            {
                GameManager.StartCoroutine(DiscardTile(tile));
                return true;
            }
        }

        return false;
    }

    private bool IsSingle(Tile tile)
    {
        return TileCounts[tile.ToString()] == 1;
    }

    private bool IsPriority1(Tile tile)
    {
        return tile.Suit == "Wind"
            && tile.Rank != Wind
            && tile.Rank != GameManager.RoundWind;
    } // Является ли ветер не ветром игрока и раунда

    private bool IsPriority2(Tile tile)
    {
        return (tile.Rank=="1"||tile.Rank=="9") && !HasNeighbor(tile);
    } // Является ли тайл изолированным терминалом


    private bool HasNeighbor(Tile tile)
    {        
        string neighborRank = null;

        if (tile.Rank == "1")
            neighborRank = "2";
        else if (tile.Rank == "9")
            neighborRank = "8";
        else
            return false;

        return Hand.Any(t => t.Suit == tile.Suit && t.Rank == neighborRank);
    }

    private bool IsPriority3(Tile tile)
    {
        if (tile.Suit == "Dragon")
            return true;

        if (tile.Suit == "Wind")
        {
            bool isPlayerOrRoundWind = tile.Rank == Wind || tile.Rank == GameManager.RoundWind;
            return isPlayerOrRoundWind && (Wind != GameManager.RoundWind);
        }

        return false;
    } // Является ли тайл драконом или ветром игрока или раунда

    private bool IsPriority4(Tile tile)
    {
        return tile.Suit == "Wind"
            && Wind == GameManager.RoundWind
            && tile.Rank == Wind;
    } //Двойной ветер
}




public class RealPlayer : IPlayer
{
    public RealPlayer(string name, GameManager gameManager, IDiscardView playerDiscardView, IHandView playerHandView,
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
        PlayerHandView = playerHandView;
    }
    public override void StartTurn()
    {
        CheckForTsumo();
        CheckForRiichi();       
    }


    public void CheckForRiichi()
    {
        bool buttoncreated = false;
        if (!IsOpen && DiscardWaitCosts.Count > 0 && GameManager.Wall.Count > 4 && !Riichi && IsActive)
        {
            GameManager.CallsButtonsView.CreateRiichiButton();
            buttoncreated = true;
        }
        if (buttoncreated) GameManager.CallsButtonsView.CreatePassButton();

    }
    /// <summary>
    /// Добавляет кнопки для возможных вызовов и кнопку для пропуска
    /// </summary>
    public override void ProceedCalls()
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
            GameManager.CallsButtonsView.CreateCallIndicator(pon.player);
            buttoncreated = true;
        }
        if (kan != (null, null))
            { 
            GameManager.CallsButtonsView.CreateKanButton();
            if (kan.player != this)
                GameManager.CallsButtonsView.CreateCallIndicator(kan.player);
            buttoncreated = true;
            }
        if (chi != (null, null))
            { 
            GameManager.CallsButtonsView.CreateChiButton();
            GameManager.CallsButtonsView.CreateCallIndicator(chi.player);
            buttoncreated = true;
            }
        if ( ron!= (null, null))
        {
            GameManager.CallsButtonsView.CreateRonButton();
            GameManager.CallsButtonsView.CreateCallIndicator(ron.player);
            buttoncreated = true;
        }
        if (buttoncreated)GameManager.CallsButtonsView.CreatePassButton();
    }        

    /// <summary>
    /// Если сброшенный тайл есть в списке дискард-ожидания, то обновляем ожидания игрока  на тайлы
    /// </summary>
    public override void UpgradeWaits(Tile tile)
    {
        
        WaitCosts.Clear();
        ClearDiscardableMarks();//Уникально для реальнго игрока - обновление меток сбросаемых тайлов
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

    
}



