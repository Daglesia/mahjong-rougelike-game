using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComboCalculator
{
    private static bool AreConsecutive(int a, int b, int c)
    {
        var uniqueNumbers = new HashSet<int> { a, b, c };
        if (uniqueNumbers.Count < 3) return false;

        int min = Math.Min(a, Math.Min(b, c));
        int max = Math.Max(a, Math.Max(b, c));

        return (max - min) == 2;
    }

public static bool IsSequence(IReadOnlyList<CardType> cards)
{
    // Wind/Dragon jokers can only represent honors, which never form sequences
    if (cards.Any(c => c == CardType.WindJoker || c == CardType.DragonJoker))
        return false;

    var naturals = cards.Where(c => c != CardType.UltimateJoker).ToList();

    if (naturals.Any(c => !c.IsSuit())) return false;
    if (naturals.Select(c => c.Category()).Distinct().Count() > 1) return false;

    var ranks = naturals.Select(c => c.Rank()).ToList();
    if (ranks.Distinct().Count() != ranks.Count) return false;

    // Zero or one natural: the jokers can fill in the rest
    if (ranks.Count < 2) return true;

    // UltimateJokers fill the gaps, so the naturals only need to fit in a window of 3
    return ranks.Max() - ranks.Min() <= 2;
}

public static bool IsTriplet(IReadOnlyList<CardType> cards)
{
    var naturals = cards.Where(c => !c.IsJoker()).ToList();
    var jokers = cards.Where(c => c.IsJoker()).ToList();

    if (naturals.Count == 0)
    {
        // All jokers: they must be able to stand for one common tile
        bool hasWind = jokers.Contains(CardType.WindJoker);
        bool hasDragon = jokers.Contains(CardType.DragonJoker);
        return !(hasWind && hasDragon);
    }

    var target = naturals[0];
    if (naturals.Any(c => c != target)) return false;

    return jokers.All(j => j.CanReplace(target));
}
    public static bool ComboValid(IReadOnlyList<CardType> cards)
    {
        return IsTriplet(cards) || IsSequence(cards);
    }

    public static void Evaluate(IReadOnlyList<CardType> cards)
    {
        Debug.Log("Combo check: " + string.Join(", ", cards));
        Debug.Log("Combo valid: " + ComboValid(cards));
    }
}