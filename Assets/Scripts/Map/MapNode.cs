using System.Collections.Generic;

/// Runtime node of a generated map. Plain class, not a ScriptableObject:
/// it is created per run from a seed and never edited as an asset.
public class MapNode
{
    private readonly List<MapNode> next = new();

    public int Step { get; }
    public int Lane { get; }

    public NodeData Data { get; set; }

    public IReadOnlyList<MapNode> Next => next;

    public MapNode(int step, int lane)
    {
        Step = step;
        Lane = lane;
    }

    public void Connect(MapNode to)
    {
        if (to == null || next.Contains(to)) return;
        next.Add(to);
    }

    public override string ToString() => $"Node({Step},{Lane}) {(Data != null ? Data.id : "empty")}";
}