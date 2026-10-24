using System.Collections.Generic;
using System.Linq;

public readonly struct ScoreEntry
{
    public readonly string Label;
    public readonly int Points;
    public ScoreEntry(string label, int points) { Label = label; Points = points; }
}

public interface IScoreClause
{
    /// Return true and fill 'entry' if this clause applies to the combo.
    bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry);
}

public class TripletClause : IScoreClause
{
    public int points = 30;

    public bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry)
    {
        entry = new ScoreEntry("Triplet", points);
        return ComboCalculator.IsTriplet(cards);
    }
}

/// Stacks on top of the triplet clause for winds/dragons (30 + 30 = 60 by default).
public class HonorTripletClause : IScoreClause
{
    public int points = 30;

    public bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry)
    {
        entry = new ScoreEntry("Wind / Dragon", points);
        if (!ComboCalculator.IsTriplet(cards)) return false;

        // Only real winds/dragons or Wind/Dragon jokers prove an honor triplet.
        // UltimateJokers are neutral, so Bamboo5 + Bamboo5 + Ultimate stays a plain 30.
        return cards.Any(c =>
            c.Category() == CardCategory.Wind ||
            c.Category() == CardCategory.Dragon ||
            c == CardType.WindJoker ||
            c == CardType.DragonJoker);
            }
}

/// Sequence that starts on 1 or ends on 9.
public class EdgeSequenceClause : IScoreClause
{
    public int points = 20;

    public bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry)
    {
        entry = new ScoreEntry("Edge Sequence", points);
        return SequenceInfo.IsPureSequence(cards) && SequenceInfo.CanTouchEdge(cards);
    }
}

public class PlainSequenceClause : IScoreClause
{
    public int points = 10;

    public bool TryScore(IReadOnlyList<CardType> cards, out ScoreEntry entry)
    {
        entry = new ScoreEntry("Sequence", points);
        return SequenceInfo.IsPureSequence(cards) && !SequenceInfo.CanTouchEdge(cards);
    }
}

internal static class SequenceInfo
{
    // A combo like Bamboo5 + 2 UltimateJokers is both a triplet and a sequence;
    // triplet wins so the two don't double-count.
    public static bool IsPureSequence(IReadOnlyList<CardType> cards) =>
        ComboCalculator.IsSequence(cards) && !ComboCalculator.IsTriplet(cards);

    // With UltimateJokers the window can slide, so we pick the best-scoring one:
    // true if any valid 3-wide window containing all naturals starts at 1 or ends at 9.
    public static bool CanTouchEdge(IReadOnlyList<CardType> cards)
    {
        var ranks = cards.Where(c => c != CardType.UltimateJoker).Select(c => c.Rank()).ToList();
        if (ranks.Count == 0) return true;

        int min = ranks.Min(), max = ranks.Max();
        for (int start = 1; start <= 7; start++)
        {
            if (min < start || max > start + 2) continue;
            if (start == 1 || start + 2 == 9) return true;
        }
        return false;
    }
}