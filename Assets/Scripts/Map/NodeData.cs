using UnityEngine;

public enum NodeKind { Start, Combat, BlackHole, Shop, Stone, FreeItem, FreeCard, EquivalentExchange, Boss }

[CreateAssetMenu(menuName = "Map/Node")]
public class NodeData : ScriptableObject
{
    public string id;

    public string displayName;
    public Sprite art;
    public NodeKind kind;
}