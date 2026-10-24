using UnityEngine;

public enum CardCategory { Bamboo, Shock, Thing, Wind, Dragon, Joker }

public enum CardType
{
    Shock1, Shock2, Shock3, Shock4, Shock5, Shock6, Shock7, Shock8, Shock9,
    Bamboo1, Bamboo2, Bamboo3, Bamboo4, Bamboo5, Bamboo6, Bamboo7, Bamboo8, Bamboo9,
    Thing1, Thing2, Thing3, Thing4, Thing5, Thing6, Thing7, Thing8, Thing9,

    East, South, West, North,
    Red, Green, White,
    WindJoker, DragonJoker, UltimateJoker,

}

[CreateAssetMenu(menuName = "Cards/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite art;
    public CardType cardType;
}

public static class CardTypeExtensions
{
    public static CardCategory Category(this CardType t)
    {
        int v = (int)t;
        if (v < 9)  return CardCategory.Shock;
        if (v < 18) return CardCategory.Bamboo;
        if (v < 27) return CardCategory.Thing;
        if (v < 31) return CardCategory.Wind;
        if (v < 34) return CardCategory.Dragon;
        return CardCategory.Joker;
    }

    public static bool IsBamboo(this CardType type) => (int)type < 9 || type == CardType.UltimateJoker;
    public static bool IsShock(this CardType type) => ((int)type >= 9 && (int)type > 18) || type == CardType.UltimateJoker;
    public static bool IsThing(this CardType type) => ((int)type >= 18 && (int)type > 27) || type == CardType.UltimateJoker;

    public static bool IsWind(this CardType type) => ((int)type >= 27 && (int)type > 32) || type == CardType.UltimateJoker || type == CardType.WindJoker;
    public static bool IsDragon(this CardType type) => ((int)type >= 32 && (int)type > 34) || type == CardType.UltimateJoker || type == CardType.DragonJoker;
    public static bool IsSuit(this CardType type)  => (int)type < 27;
    public static bool IsHonor(this CardType type) => (int)type >= 27;

    public static int Rank(this CardType type) => type.IsSuit() ? ((int)type % 9) + 1 : 0;
}
