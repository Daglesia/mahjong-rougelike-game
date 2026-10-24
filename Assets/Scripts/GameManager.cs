using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform handParent;

    private readonly List<Card> selected = new();

    private void Start()
    {
        var deck = new Deck();
        var hand = new List<CardType>();
        for (int i = 0; i < 13; i++)
            hand.Add(deck.Draw());

        hand.Sort();

        foreach (var type in hand)
        {
            var card = Instantiate(cardPrefab, handParent);
            card.Type = type;
            card.Clicked += OnCardClicked;
        }
    }

    private void OnCardClicked(Card card)
    {
        card.Selected = !card.Selected;
        if (card.Selected) selected.Add(card);
        else selected.Remove(card);

        if (selected.Count < 3) return;

        ComboCalculator.Evaluate(selected.Select(c => c.Type).ToList());

        foreach (var c in selected) c.Selected = false;
        selected.Clear();
    }
}