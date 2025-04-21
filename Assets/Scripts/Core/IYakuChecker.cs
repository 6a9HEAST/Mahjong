using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public interface IYakuChecker
{
    /// <summary>Возвращает количество ханов, если руку удовлетворяет условию, иначе 0.</summary>
    (string yaku,int cost) Check(List<List<Tile>> blocks,IPlayer player);
}


/// <summary>Таняо</summary>
public class Tanyao_Checker : IYakuChecker
{

    /// <summary>Таньяо</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            foreach (var tile in block)
                if (tile.Rank == "1" || tile.Rank == "9" || tile.Suit == "Dragon" || tile.Suit == "Wind")
                    return (null,0);
        return ("Таньяо",1);
    }
}

/// <summary>Красный дракон</summary>
public class RedDragon_Checker : IYakuChecker
{

    /// <summary>Якухай</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            if ((block.Count == 3 || block.Count == 4) && block[0].Rank == block[1].Rank&& block[1].Rank == block[2].Rank)
                if (block[0].Rank=="Red")
                    return ("Красный дракон", 1);
        return (null, 0);
    }
}

/// <summary>Зеленый дракон</summary>
public class GreenDragon_Checker : IYakuChecker
{

    /// <summary>Якухай</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            if ((block.Count == 3 || block.Count == 4) && block[0].Rank == block[1].Rank && block[1].Rank == block[2].Rank)
                if (block[0].Rank == "Green")
                    return ("Зеленый дракон", 1);
        return (null, 0);
    }
}

/// <summary>Белый дракон</summary>
public class WhteDragon_Checker : IYakuChecker
{

    /// <summary>Якухай</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            if ((block.Count == 3 || block.Count == 4) && block[0].Rank == block[1].Rank && block[1].Rank == block[2].Rank)
                if (block[0].Rank == "White")
                    return ("Белый дракон", 1);
        return (null, 0);
    }
}

/// <summary>Ветер раунда</summary>
public class RoundWind_Checker : IYakuChecker
{

    /// <summary>Ветер раунда</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            if ((block.Count == 3 || block.Count == 4) && block[0].Rank == block[1].Rank && block[1].Rank == block[2].Rank)
                if (block[0].Rank == player.GameManager.RoundWind)
                    return ("Ветер раунда", 1);
        return (null, 0);
    }
}

/// <summary>Ветер игрока</summary>
public class PlayerWind_Checker : IYakuChecker
{

    /// <summary>Ветер игрока</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        foreach (var block in blocks)
            if ((block.Count == 3 || block.Count == 4) && block[0].Rank == block[1].Rank && block[1].Rank == block[2].Rank)
                if (block[0].Rank == player.Wind)
                    return ("Ветер игрока", 1);
        return (null, 0);
    }
}

/// <summary>Пинфу</summary>
public class Pinfu_Checker : IYakuChecker
{

    /// <summary>Пинфу</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        if (player.IsOpen) return (null, 0);

        var melds = blocks.Where(b => b.Count == 3).ToList();
        if (melds.Count != 4)
            return (null, 0);

        // 5) Все сеты должны быть чи (последовательностями)
        if (!melds.All(IsChi))
            return (null, 0);

        // 6) Проверить, что выигрышный сет (последний в списке) образован двасторонним ожиданием
        var lastMeld = blocks.Last();
        var winningTile = lastMeld.Last();

        // Два тайла до выигрышного
        var firstTwo = lastMeld
            .Take(2)
            .OrderBy(t => t.TryGetRankAsInt())
            .ToList();

        if (!IsTwoSidedWait(firstTwo))
            return (null, 0);

        // Все условия пинфу выполнены
        return ("Пинфу", 1);

    }

    private bool IsTwoSidedWait(List<Tile> tiles)
    {
        int r1 = tiles[0].TryGetRankAsInt();
        int r2 = tiles[1].TryGetRankAsInt();
        // последовательные по рангу, и оба не на "краях" (1-2 или 8-9)
        return (r2 - r1 == 1) && (r1 > 1) && (r2 < 9);
    }

    private bool IsChi(List<Tile> block)
    {
        if (block[0].Suit == "Dragon" || block[0].Suit == "Wind") return false;
        var sorted = block
            .OrderBy(t => t.TryGetRankAsInt())
            .ToList();

        return sorted[0].Suit == sorted[1].Suit
            && sorted[1].Suit == sorted[2].Suit
            && sorted[1].TryGetRankAsInt() - sorted[0].TryGetRankAsInt() == 1
            && sorted[2].TryGetRankAsInt() - sorted[1].TryGetRankAsInt() == 1;
    }
}

/// <summary>Семь пар</summary>
public class SevenPairs_Checker : IYakuChecker
{

    /// <summary>Семь пар</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        if (player.IsOpen)
            return (null, 0);

        List<Tile> tiles=blocks.SelectMany(b => b).ToList();

        // 2) Должно быть ровно 14 тайлов
        if (tiles == null || tiles.Count != 14)
            return (null, 0);

        // 3) Группируем по одинаковым тайлам
        //    Счетчик пар: каждая пара (2) даёт 1, каждая каму (4 одинаковых) даёт 2
        var groups = tiles
            .GroupBy(t => (t.Suit, t.Rank))
            .Select(g => g.Count())
            .ToList();

        int pairCount = groups.Sum(count => count / 2);

        // 4) Все группы должны быть именно по 2 или по 4 тайла
        bool onlyPairsOrKans = groups.All(count => count == 2 || count == 4);

        if (onlyPairsOrKans && pairCount == 7)
        {
            // Семь пар — закрытый яку, 2 хан
            return ("Семь пар", 2);
        }

        return (null, 0);
    }
}

/// <summary>Две одинаковые последовательности(ипейко) или две по две (рянпейко)</summary>
public class Iipeikou_Ryanpeikou_Checker : IYakuChecker
{

    /// <summary>Две одинаковые последовательности(ипейко) или две по две (рянпейко)</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        if (player.IsOpen)
            return (null, 0);

        // 2) Собираем все блоки-чи (последовательности)
        var chiBlocks = blocks
            .Where(b => b.Count == 3 && IsChi(b))
            .ToList();

        // 3) Представляем каждую чи как паттерн "rank1,rank2,rank3-suit"
        var patterns = chiBlocks
            .Select(b =>
            {
                var ranks = b.Select(t => t.Rank).OrderBy(r => r);
                return string.Join(",", ranks) + "-" + b[0].Suit;
            })
            .ToList();

        // 4) Считаем, сколько паттернов встречаются по крайней мере по 2 раза
        int duplicatePairs = patterns
            .GroupBy(p => p)
            .Count(g => g.Count() >= 2);

        // 5) Если два или более, рянпэйко
        if (duplicatePairs >= 2)
            return ("Рянпэйко", 3);

        // 6) Если ровно одно — иипэйко
        if (duplicatePairs == 1)
            return ("Ипайко", 1);

        // 7) Иначе — нет яку
        return (null, 0);
    }

    // Вспомогательная проверка: блок — чи?
    private bool IsChi(List<Tile> block)
    {
        if (block[0].Suit == "Dragon" || block[0].Suit == "Wind") return false;
        var sorted = block.OrderBy(t => int.Parse(t.Rank)).ToList();
        return sorted[0].Suit == sorted[1].Suit
            && sorted[1].Suit == sorted[2].Suit
            && int.Parse(sorted[1].Rank) - int.Parse(sorted[0].Rank) == 1
            && int.Parse(sorted[2].Rank) - int.Parse(sorted[1].Rank) == 1;
    }
}

/// <summary>Саншоку</summary>
public class Sanshoku_Checker : IYakuChecker
{

    /// <summary>Саншоку</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var chiBlocks = blocks
       .Where(b => b.Count == 3 && IsChi(b))
       .ToList();

        // 2) Преобразуем каждую чи в паттерн по номерам: "1,2,3", "4,5,6" и т.д.
        var patterns = chiBlocks
            .Select(b =>
            {
                var sortedRanks = b
                    .Select(t => int.Parse(t.Rank))
                    .OrderBy(r => r);
                // Паттерн: например, "1,2,3"
                return string.Join(",", sortedRanks);
            })
            .Distinct();

        // 3) Для каждого паттерна проверяем, есть ли по одному сету в каждой масти
        bool hasSanshoku = patterns.Any(pattern =>
        {
            // Фильтруем чи с этим паттерном
            var samePatternBlocks = chiBlocks
                .Where(b =>
                {
                    var ranks = b.Select(t => int.Parse(t.Rank)).OrderBy(r => r);
                    return string.Join(",", ranks) == pattern;
                });
            // Собираем уникальные масти среди этих блоков
            var suits = samePatternBlocks
                .Select(b => b[0].Suit)
                .Distinct();
            // Если мастей ровно 3 — нашли саншоку
            return suits.Count() == 3;
        });

        if (!hasSanshoku)
            return (null, 0);

        // 4) Определяем хан в зависимости от открытости руки
        int han = player.IsOpen ? 1 : 2;
        return ("Саншоку", han);
    }

    // Вспомогательный метод: определяет, образуют ли три тайла чи
    private bool IsChi(List<Tile> block)
    {
        if (block[0].Suit == "Dragon" || block[0].Suit=="Wind") return false;
        var sorted = block
            .OrderBy(t => t.TryGetRankAsInt())
            .ToList();

        return sorted[0].Suit == sorted[1].Suit
            && sorted[1].Suit == sorted[2].Suit
            && int.Parse(sorted[1].Rank) - int.Parse(sorted[0].Rank) == 1
            && int.Parse(sorted[2].Rank) - int.Parse(sorted[1].Rank) == 1;
    }
}

/// <summary>Стрит</summary>
public class Ittsuu_Checker : IYakuChecker
{

    /// <summary>Стрит</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var chiBlocks = blocks
        .Where(b => b.Count == 3 && IsChi(b))
        .GroupBy(b => b[0].Suit);

        foreach (var group in chiBlocks)
        {
            var suit = group.Key;
            var sequences = group
                .Select(b => b.Select(t => int.Parse(t.Rank)).OrderBy(r => r).ToArray())
                .ToList();

            bool has123 = sequences.Any(seq => seq.SequenceEqual(new[] { 1, 2, 3 }));
            bool has456 = sequences.Any(seq => seq.SequenceEqual(new[] { 4, 5, 6 }));
            bool has789 = sequences.Any(seq => seq.SequenceEqual(new[] { 7, 8, 9 }));

            if (has123 && has456 && has789)
            {
                int han = player.IsOpen ? 1 : 2;
                return ("Иццу", han);
            }
        }

        return (null, 0);
    }

    // Вспомогательная функция для проверки, является ли блок чи
    private bool IsChi(List<Tile> block)
    {
        if (block[0].Suit == "Dragon" || block[0].Suit == "Wind") return false;
        var sorted = block.OrderBy(t => int.Parse(t.Rank)).ToList();
        return sorted[0].Suit == sorted[1].Suit &&
               sorted[1].Suit == sorted[2].Suit &&
               int.Parse(sorted[1].Rank) - int.Parse(sorted[0].Rank) == 1 &&
               int.Parse(sorted[2].Rank) - int.Parse(sorted[1].Rank) == 1;
    }
}

/// <summary>Тойтой/все тройки</summary>
public class Toitoi_Checker : IYakuChecker
{
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        // Должно быть именно 5 блоков: 4 сетa (пон/кан) и 1 пара
        if (blocks == null || blocks.Count != 5)
            return (null, 0);

        bool hasPair = blocks.Any(b => b.Count == 2);
        if (!hasPair)
            return (null, 0);

        // Проверяем каждый сетовый блок (всего 4) — он должен быть поун (3 тайла) или кан (4 тайла)
        foreach (var block in blocks.Where(b => b.Count >= 3))
        {
            // Для поуна (пон): 3 одинаковых тайла
            if (block.Count == 3)
            {
                if (!IsTriplet(block))
                    return (null, 0);
            }
            // Для кана: 4 одинаковых тайла
            else if (block.Count == 4)
            {
                if (!IsQuad(block))
                    return (null, 0);
            }
            else
            {
                // Блок неправильного размера
                return (null, 0);
            }
        }

        // Все проверки пройдены — това-тои выполнено
        return ("Тойтой", 2);
    }

    /// <summary>Проверяет, что три тайла образуют поун (пон).</summary>
    private bool IsTriplet(List<Tile> block)
    {
        return block.Count == 3
            && block.All(t => t.Suit == block[0].Suit && t.Rank == block[0].Rank);
    }

    /// <summary>Проверяет, что четыре тайла образуют кан.</summary>
    private bool IsQuad(List<Tile> block)
    {
        return block.Count == 4
            && block.All(t => t.Suit == block[0].Suit && t.Rank == block[0].Rank);
    }
}

/// <summary>Сананко/три закрытых тройки</summary>
public class Sanankou_Checker : IYakuChecker
{
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        int concealedSets = 0;

        foreach (var block in blocks)
        {
            if (block.Count == 3 && IsTriplet(block))
            {
                // Проверяем, не совпадает ли этот блок с открытым пон в Calls
                bool isOpenPon = player.Calls.Any(call =>
                    call.Count == 3 &&
                    call.All(t => block.Any(b => b.Suit == t.Suit && b.Rank == t.Rank))
                );
                if (!isOpenPon)
                    concealedSets++;
            }
        }

        // 2. Считаем закрытые каны (暗槓) в Calls
        foreach (var call in player.Calls)
        {
            if (call.Count == 4 && call.All(t => t.Properties.Contains("Called")))
            {
                concealedSets++;
            }
        }

        // 3. Если найдено ≥3 скрытых сетов — Sanankou
        if (concealedSets >= 3)
            return ("Сананко", 2);

        return (null, 0);
    }

    /// <summary>Проверяет, что блок из 3 тайлов — три одинаковых (暗刻/明刻).</summary>
    private bool IsTriplet(List<Tile> block)
    {
        return block.Count == 3
            && block.All(t => t.Suit == block[0].Suit && t.Rank == block[0].Rank);
    }
}

/// <summary>Санканцу/три кана</summary>
public class Sankantsu_Checker : IYakuChecker
{

    
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
    
        int kanCount = blocks.Count(block => block.Count == 4);

        
        if (kanCount >= 3)
        {
            
            return ("Санканцу", 2);
        }

        return (null, 0);
    }
}

/// <summary>Саншоку доко</summary>
public class Sanshoku_triplets_Checker : IYakuChecker
{

    
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var tripletsAndQuads = blocks
        .Where(b => (b.Count == 3 || b.Count == 4) && IsTripletOrQuad(b))
        .ToList();

        // Группируем по "номиналу" (rank), причём в каждой группе считаем уникальные масти
        var rankToSuits = tripletsAndQuads
            .GroupBy(b => b[0].Rank)
            .ToDictionary(
                g => g.Key,
                g => g.Select(b => b[0].Suit).Distinct().Count()
            );

        // Проверяем, есть ли ранг, у которого мастей ≥3
        bool hasSanShoku = rankToSuits.Values.Any(count => count >= 3);

        // Если да — это San shoku doko за 2 хан
        return hasSanShoku
            ? ("Саншоку доко", 2)
            : (null, 0);
    }

    /// <summary>Проверяет, что все тайлы в блоке одинаковы (пон или кан).</summary>
    private bool IsTripletOrQuad(List<Tile> block)
    {
        return block.All(t => t.Suit == block[0].Suit && t.Rank == block[0].Rank);
    }
}

/// <summary>Чанта/все сеты и пара содержат хотя бы один не средний тайл</summary>
public class Chanta_Checker : IYakuChecker
{

    
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        if (blocks == null || blocks.Count != 5)
            return (null, 0);

        bool hasPair = false;

        foreach (var block in blocks)
        {
            // 2) Определяем, является ли блок парой (2 тайла)
            if (block.Count == 2)
            {
                // Пара должна содержать терминал или почестный
                if (!block.Any(t => IsTerminal(t) || IsHonor(t)))
                    return (null, 0);
                hasPair = true;
            }
            // 3) Ментсу: чи, пон или кан
            else if (block.Count == 3 || block.Count == 4)
            {
                // Блок может быть чи (последовательность) или три/четыре одинаковых
                if (IsChi(block))
                {
                    // В чи обязательно должен быть терминал (крайний в ряд) или быть почестным в составе?
                    if (!block.Any(t => IsTerminal(t) || IsHonor(t)))
                        return (null, 0);
                }
                else
                {
                    // Для пон/кан: проверяем, что все тайлы одинаковы
                    if (!block.All(t => t.Suit == block[0].Suit && t.Rank == block[0].Rank))
                        return (null, 0);
                    // И один из тайлов — терминал или почестный (все четыре/три одинаковых, значит сразу условие выполнено)
                }
            }
            else
            {
                // Неверный размер блока
                return (null, 0);
            }
        }

        if (!hasPair)
            return (null, 0);

        // 4) Определяем хан с учётом открытости
        int han = player.IsOpen ? 1 : 2;
        return ("Чанта", han);
    }

    // Проверка чи (последовательности)
    private bool IsChi(List<Tile> block)
    {
        if (block[0].Suit == "Dragon" || block[0].Suit == "Wind") return false;
        if (block.Count != 3) return false;
        var sorted = block.OrderBy(t => int.Parse(t.Rank)).ToList();
        return sorted[0].Suit == sorted[1].Suit
            && sorted[1].Suit == sorted[2].Suit
            && int.Parse(sorted[1].Rank) - int.Parse(sorted[0].Rank) == 1
            && int.Parse(sorted[2].Rank) - int.Parse(sorted[1].Rank) == 1;
    }

    // Определяет, терминальный ли это тайл (1 или 9)
    private bool IsTerminal(Tile t)
    {
        return (t.Rank == "1" || t.Rank == "9");
    }

    // Определяет, почестный ли это тайл (ветр/дракон)
    private bool IsHonor(Tile t)
    {
        return t.Suit == "Dragon"||t.Suit=="Wind";
    }
}

/// <summary>Джунчан/все сеты и пара содержат хотя бы один терминал</summary>
public class Junchan_Checker : IYakuChecker
{

    
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        if (blocks == null || blocks.Count != 5)
            return (null, 0);

        bool hasNonTerminal = false;

        foreach (var block in blocks)
        {
            // Все тайлы в блоке не должны быть почестными
            if (block.Any(t => (t.Suit == "Dragon"||t.Suit=="Wind")))
                return (null, 0);

            // Блок-пара
            if (block.Count == 2)
            {
                // Пара должна содержать терминал 1 или 9
                if (!block.Any(t => IsTerminal(t)))
                    return (null, 0);
            }
            // Сет (чи, пон или кан)
            else if (block.Count == 3 || block.Count == 4)
            {
                // В любом случае в сете должен быть терминал
                if (!block.Any(t => IsTerminal(t)))
                    return (null, 0);
            }
            else
            {
                // Неправильный размер блока
                return (null, 0);
            }

            // Наблюдаем непредельный тайл
            if (block.Any(t => IsNonTerminal(t)))
                hasNonTerminal = true;
        }

        // Обязательно хотя бы один непредельный тайл
        if (!hasNonTerminal)
            return (null, 0);

        // Вычисляем хан с учётом открытости
        int han = player.IsOpen ? 2 : 3;
        return ("Джунчан", han);
    }

    // Проверяет терминал: числовая 1 или 9
    private bool IsTerminal(Tile t)
    {
        return t.Rank == "1" || t.Rank == "9";
    }

    // Проверяет непредельный: числовая 2–8
    private bool IsNonTerminal(Tile t)
    {
        if (t.Suit == "Dragon"||t.Suit=="Wind") return false;
        int r = int.Parse(t.Rank);
        return r >= 2 && r <= 8;
    }
}

/// <summary>Хоницу</summary>
public class Honitsu_Checker : IYakuChecker
{

    
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var allTiles = blocks.SelectMany(b => b).ToList();

        // 1) Определяем множество числовых мастей в руке
        var numericSuits = allTiles
            .Where(t => t.Suit == "Man" || t.Suit == "Pin" || t.Suit == "Sou")
            .Select(t => t.Suit)
            .Distinct()
            .ToList();

        // Должна быть ровно одна числовая масть
        if (numericSuits.Count != 1)
            return (null, 0);

        // 2) Проверяем наличие хотя бы одного почестного тайла
        bool hasHonor = allTiles.Any(t => t.Suit == "Wind"||t.Suit=="Dragon");
        if (!hasHonor)
            return (null, 0);

        // 3) Вычисляем хан с учётом открытости (куисагари)
        int han = player.IsOpen ? 2 : 3;

        return ("Хоницу", han);
    }
}

/// <summary>Чиницу</summary>
public class Chinitsu_Checker : IYakuChecker
{

    /// <summary>Якухай</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var allTiles = blocks.SelectMany(b => b).ToList();

        // 1) Если есть хоть один почестный тайл — не Chinitsu
        if (allTiles.Any(t => t.Suit == "Wind"||t.Suit=="Dragon"))
            return (null, 0);

        // 2) Определяем множество числовых мастей в руке
        var numericSuits = allTiles
            .Where(t => t.Suit == "Man" || t.Suit == "Pin" || t.Suit == "Sou")
            .Select(t => t.Suit)
            .Distinct()
            .ToList();

        // Должна быть ровно одна числовая масть
        if (numericSuits.Count != 1)
            return (null, 0);

        // 3) Вычисляем хан: 6 закрыто, 5 открыто (食い下がり)
        int han = player.IsOpen ? 5 : 6;

        return ("Чиницу", han);
    }
}

/// <summary>Хонрото</summary>
public class Honroto_Checker : IYakuChecker
{

    /// <summary>Якухай</summary>
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var allTiles = blocks.SelectMany(b => b).ToList();

        // Проверяем, что каждый тайл — терминал или почестный
        if (allTiles.Any(t =>
            !IsTerminal(t) && !IsHonor(t)))
            return (null, 0);

        // Проверяем, что есть хотя бы один терминал и хотя бы один почестный
        bool hasTerminal = allTiles.Any(IsTerminal);
        bool hasHonor = allTiles.Any(IsHonor);
        if (!hasTerminal || !hasHonor)
            return (null, 0);

        // Условие выполнено — 2 хан
        return ("Хонрото", 2);
    }

    // Терминал: числовая масть и ранг 1 или 9
    private bool IsTerminal(Tile t)
    {
        return (t.Rank == "1" || t.Rank == "9");
    }

    // Почестный: ветры или драконы
    private bool IsHonor(Tile t)
    {
        return t.Suit == "Wind"||t.Suit=="Dragon";
    }
}

/// <summary>Сёнсанген</summary>
public class Shosangen_Checker : IYakuChecker
{
    public (string yaku, int cost) Check(List<List<Tile>> blocks, IPlayer player)
    {
        var dragonTriplets = new List<string>();
        string pairDragon = null;

        foreach (var block in blocks)
        {
            // Проверка триплетов/квадов драконов
            if (block.Count >= 3 && block.All(t => t.Suit == "Dragon"))
            {
                var firstDragon = block[0].Rank;
                if (block.All(t => t.Rank == firstDragon))
                {
                    dragonTriplets.Add(firstDragon);
                }
            }
            // Проверка пары драконов
            else if (block.Count == 2 && block.All(t => t.Suit == "Dragon"))
            {
                var pairType = block[0].Rank;
                if (block.All(t => t.Rank == pairType))
                {
                    pairDragon = pairType;
                }
            }
        }

        // Должно быть ровно два уникальных дракона в триплетах
        var uniqueTriplets = dragonTriplets.Distinct().ToList();
        if (uniqueTriplets.Count != 2)
            return (null, 0);

        // Определяем отсутствующего дракона
        var allDragons = new List<string> { "Red", "Green", "White" };
        var missingDragon = allDragons.Except(uniqueTriplets).FirstOrDefault();

        // Проверка, что пара состоит из отсутствующего дракона
        if (pairDragon == missingDragon)
        {
            return ("Сёсанген", 2);
        }

        return (null, 0);
    }
}
