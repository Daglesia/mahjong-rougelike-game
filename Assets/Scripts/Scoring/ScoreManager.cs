using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text breakdownText;   // optional: shows "Triplet +30 / Wind / Dragon +30"

    private readonly List<IScoreClause> clauses = new()
    {
        new TripletClause(),
        new HonorTripletClause(),
        new EdgeSequenceClause(),
        new PlainSequenceClause(),
    };

    public int Score { get; private set; }
    public event Action<int> ScoreChanged;

    private void Start() => Refresh(null);

    // Future hook for relics / power-ups / etc.
    public void AddClause(IScoreClause clause) => clauses.Add(clause);
    public void RemoveClause(IScoreClause clause) => clauses.Remove(clause);

    public int Award(IReadOnlyList<CardType> cards)
    {
        var entries = new List<ScoreEntry>();
        foreach (var clause in clauses)
            if (clause.TryScore(cards, out var entry))
                entries.Add(entry);

        int gained = entries.Sum(e => e.Points);
        Score += gained;

        Refresh(entries);
        ScoreChanged?.Invoke(Score);
        return gained;
    }

    private void Refresh(List<ScoreEntry> entries)
    {
        if (scoreText != null) scoreText.text = $"Score: {Score}";

        if (breakdownText != null)
            breakdownText.text = entries == null || entries.Count == 0
                ? ""
                : string.Join("\n", entries.Select(e => $"{e.Label} +{e.Points}"));
    }
}