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
        if (cards.All(card => card.IsBamboo()))
        {
            return AreConsecutive((int)cards[0], (int)cards[1], (int)cards[2]);
        }
        if (cards.All(card => card.IsThing()))
        {
            return AreConsecutive((int)cards[0], (int)cards[1], (int)cards[2]);
        }
        if (cards.All(card => card.IsShock()))
        {
            return AreConsecutive((int)cards[0], (int)cards[1], (int)cards[2]);
        }
        return false;
    }

    public static bool IsTriplet(IReadOnlyList<CardType> cards)
    {
        if (cards[0] == cards[1] && cards[1] == cards[2])
        {
            return true;
        }
        if (cards.Any(card => card.IsSuit()))
        {
            // return cards.All(card => card == CardType.UltimateJoker || Ca)
        }

        return false;
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