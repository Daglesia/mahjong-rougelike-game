using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform handParent;
    [SerializeField] private int maxDiscards = 3;

    [SerializeField] private ScoreManager scoreManager;

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

        var types = selected.Select(c => c.Type).ToList();
        bool valid = ComboCalculator.Evaluate(types);

        if (valid)
        {
            scoreManager.Award(types);
            ReplaceCards(selected.ToList());
        }
        else
            foreach (var c in selected) c.Selected = false;

        selected.Clear();
    }

    private void ReplaceCards(List<Card> used)
    {
        foreach (var card in used)
        {
            card.Selected = false;

            if (deck.Count > 0)
            {
                card.Type = deck.Draw();   // reuse the card object with a new face
            }
            else
            {
                // Deck is empty: the card is removed with no replacement
                card.Clicked -= OnCardClicked;
                card.RightClicked -= OnCardRightClicked;
                hand.Remove(card);
                Destroy(card.gameObject);
            }
        }

        SortHand();
    }
}