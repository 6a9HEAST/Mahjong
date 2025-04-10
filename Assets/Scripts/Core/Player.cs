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
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public List<List<Tile>> Calls { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    // Для вызовов используем кортежи, содержащие информацию о тайле и игроке, с которым связан вызов
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }

    public abstract void AddTile(Tile tile);
    public abstract void StartTurn();
    public abstract void CallChi(List<Tile> tiles);
    public abstract void CallPon();
    public abstract void CallKan();
    public abstract void CallPass();

    void DiscardTile(Tile tile);
    void ClearCalls();

    // Проверка возможности вызовов
    public bool CheckForPon(Tile tile)
    {
        if (TileCounts.ContainsKey(tile.ToString()))
            if (TileCounts[tile.ToString()] >= 2) return true;
        return false;
    }

    /// <summary>
    /// Эта функция определяет может ли игрок объявить кан на посылаемый тайл.
    /// </summary>
    /// <returns>0 - нет кана. 1 - открытый кан. 2 - закрытый кан</returns>
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
        if (pon != (null, null) || kan != (null, null) || chi != (null, null))
            return true;
        else
            return false;
    }

    // Метод, который должен обработать вызовы (реализуется по-разному для ИИ и реального игрока)
    public abstract void ProceedCalls();
}




public class AiPlayer : IPlayer
{
    public string Name { get; set; }
    public int index { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpen { get; set; }
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }
    public List<List<Tile>> Calls { get; set; }

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
    }

    public void AddTile(Tile tile)
    {
        Hand.Add(tile);
    }

    public void DiscardTile(Tile tile)
    {
        Hand.Remove(tile);
        Discard.Add(tile);
        PlayerDiscardView.Draw(Discard);
      
        GameManager.CheckForCalls(tile, this);

        //GameManager.EndTurn();
    }

    public void StartTurn()
    {
        
        int index = Random.Range(0, Hand.Count);
        DiscardTile(Hand[index]);
    }

    public void ClearCalls()
    {
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
    }

    public void ProceedCalls()
    {
        // ИИ автоматически отказывает от вызова (можно добавить дополнительную логику)
        //Debug.Log(Name + " автоматически отказывается от вызова");
        //GameManager.OnCallDecisionMade(this);
        //ClearCalls();
    }

    public void CallChi(List<Tile> t) { }
    public void CallPon() { }
    public void CallKan() { }
    public void CallPass() { }

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
}




public class RealPlayer : IPlayer
{
    public string Name { get; set; }
    public int index { get; set; }
    public List<Tile> Hand { get; set; }
    public List<Tile> Discard { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpen { get; set; }
    public string Wind { get; set; }
    public GameManager GameManager { get; set; }
    public PlayerDiscardView PlayerDiscardView { get; set; }
    public CallContainerView CallContainerView { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> TileCounts { get; set; }
    public (Tile tile, IPlayer player) pon { get; set; }
    public (Tile tile, IPlayer player) kan { get; set; }
    public (List<List<Tile>>, IPlayer player) chi { get; set; }
    public List<List<Tile>> Calls { get; set; }

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
        Calls = new List<List<Tile>>();
        this.index = index;
        CallContainerView = callContainerView;
        IsOpen = false;
    }

    public void AddTile(Tile tile)
    {
        Hand.Add(tile);
        var kanCheck = CheckForKan(tile);

        if (kanCheck==2||kanCheck==1)
            { 
            kan = (tile, this);
            ProceedCalls();
            }
        if (TileCounts.ContainsKey(tile.ToString()))
            TileCounts[tile.ToString()]++;
        else
            TileCounts[tile.ToString()] = 1;
    }

    public void DiscardTile(Tile tile)
    {
        Hand.Remove(tile);
        Discard.Add(tile);
        TileCounts[tile.ToString()]--;
        GameManager.CheckForCalls(tile, this);
       // GameManager.CallsButtonsView.Clear();
    }

    public void StartTurn()
    {
        // В этом примере реальный игрок не сбрасывает автоматически
        // Логика для ожидания действий реализована через UI
    }

    public void ClearCalls()
    {
        pon = (null, null);
        kan = (null, null);
        chi = (null, null);
    }

    public void ProceedCalls()
    {
        // При наличии вызовов отображаем соответствующие кнопки на UI.
        if (pon != (null, null))
            { 
            GameManager.CallsButtonsView.CreatePonButton();
            //Debug.Log("Есть пон на " + pon.tile.ToString());
        }
        if (kan != (null, null))
            { GameManager.CallsButtonsView.CreateKanButton();
            //Debug.Log("Есть Кан на " + kan.tile.ToString());
        }
        if (chi != (null, null))
            { GameManager.CallsButtonsView.CreateChiButton();
            //Debug.Log("Есть чи");
        }
        GameManager.CallsButtonsView.CreatePassButton();
        // Также можно добавить кнопку "Пас", чтобы игрок мог отказаться от вызова.
    }

    // Методы, вызываемые из UI при нажатии на кнопки:

    public void CallPon()
    {
        IsOpen = true;
        //Удаление тайла из дискарда другого игрока и двух из руки
        pon.player.Discard.Remove(pon.tile);

        for (int i = 0; i < 2; i++)
        {
            Hand.Remove(pon.tile);
            //TileCounts[pon.tile.ToString()]--; не уменьшается счетчик чтобы можно было объявить кан по цумо
        }
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

    public void CallKan()
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
            AddTile(GameManager.KanTiles[0]);
            GameManager.KanTiles.RemoveAt(0);
            GameManager.RevealDora();
            GameManager.PlayerHandView.Draw(Hand);
            ClearCalls();
            GameManager.CallsButtonsView.Clear();
            return;


        }

        //Обработка ситуации если другой игрок сбросил тайл для кана
        IsOpen = true;
        kan.player.Discard.Remove(kan.tile);
        for (int i = 0; i < 3; i++)
        {
            Hand.Remove(kan.tile);
            TileCounts[kan.tile.ToString()]--;
        }

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
        GameManager.CallsButtonsView.Clear();
    }

    public void CallChi(List<Tile> tiles)
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

        //Debug.Log(Name + " объявляет ЧИ");
        //// Реализуйте оформление чи (при необходимости с выбором варианта)
        //GameManager.OnCallDecisionMade(this);
        //ClearCalls();
        //GameManager.CallsButtonsView.Clear();
    }

    // Этот метод можно вызвать при отказе от любого вызова (например, кнопка "Пас")
    public void CallPass()
    {
        ClearCalls();

        GameManager.EndTurn();
        //Debug.Log(Name + " отказывается от вызова");
        //GameManager.OnCallDecisionMade(this);
     
        GameManager.CallsButtonsView.Clear();
    }

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
}



