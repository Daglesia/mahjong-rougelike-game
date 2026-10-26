using System;
using System.Collections.Generic;

public class Deck
{
    private readonly List<CardType> tiles = new(136);
    private readonly Random rng;

    public int Count => tiles.Count;

    public Deck(int? seed = null)
    {
        rng = seed.HasValue ? new Random(seed.Value) : new Random();
        Build();
        Shuffle();
    }

    private void Build()
    {
        tiles.Clear();

        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            if (type == CardType.UltimateJoker) tiles.Add(type);
            else if (type == CardType.WindJoker || type == CardType.DragonJoker) {
            for (int i = 0; i < 2; i++)    
                tiles.Add(type);
            } else
            {
                for (int i = 0; i < 4; i++)
                tiles.Add(type);   
            }
        }
    }

    public void Shuffle()
    {
        for (int i = tiles.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
        }
    }

    public CardType Draw()
    {
        if (tiles.Count == 0)
            throw new InvalidOperationException("Deck is empty.");

        int last = tiles.Count - 1;
        CardType t = tiles[last];
        tiles.RemoveAt(last);
        return t;
    }
}