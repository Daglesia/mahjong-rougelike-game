using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform handParent;
    [SerializeField] private int maxDiscards = 3;

    private readonly List<Card> selected = new();
    private readonly List<Card> hand = new();
    private Deck deck;
    private int discardsLeft;

    private void Start()
    {
        deck = new Deck();
        discardsLeft = maxDiscards;

        var types = new List<CardType>();
        for (int i = 0; i < 13; i++)
            types.Add(deck.Draw());

        foreach (var type in types)
        {
            var card = Instantiate(cardPrefab, handParent);
            card.Type = type;
            card.Clicked += OnCardClicked;
            card.RightClicked += OnCardRightClicked;
            hand.Add(card);
        }

        SortHand();
    }

    private void OnCardRightClicked(Card card)
    {
        if (discardsLeft <= 0 || deck.Count == 0) return;

        if (card.Selected)
        {
            card.Selected = false;
            selected.Remove(card);
        }

        card.Type = deck.Draw();   // swaps the sprite to the new card
        discardsLeft--;
        SortHand();

        Debug.Log($"Discards left: {discardsLeft}");
    }

    private void SortHand()
    {
        hand.Sort((a, b) => a.Type.CompareTo(b.Type));
        for (int i = 0; i < hand.Count; i++)
            hand[i].transform.SetSiblingIndex(i);
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