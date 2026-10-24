using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image art;
    [SerializeField] private RectTransform visual;
    [SerializeField] private float liftHeight = 20f;
    [SerializeField] private float speed = 15f;

    private static Dictionary<CardType, Sprite> sprites;

    private CardType type;

    public event Action<Card> Clicked;
    public bool Selected { get; set; }

    public CardType Type
    {
        get => type;
        set
        {
            type = value;
            art.sprite = Sprites[value];
        }
    }

    private void Update()
    {
        var target = new Vector2(0f, Selected ? liftHeight : 0f);
        visual.anchoredPosition = Vector2.Lerp(visual.anchoredPosition, target, speed * Time.deltaTime);
    }

    public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);

    private static Dictionary<CardType, Sprite> Sprites
    {
        get
        {
            if (sprites == null)
            {
                sprites = new Dictionary<CardType, Sprite>();
                foreach (var data in Resources.LoadAll<CardData>("Cards/ScriptableObjects"))
                    sprites[data.cardType] = data.art;
            }
            return sprites;
        }
    }
}