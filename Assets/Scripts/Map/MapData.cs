using System.Collections.Generic;

/// A generated map: the seed it came from plus the grid of nodes (steps -> lanes).
/// Pure data, no MonoBehaviour. Regenerable from the seed, so a save only needs the seed and visited path.
public class MapData
{
    private readonly List<List<MapNode>> steps;

    public int Seed { get; }
    public IReadOnlyList<IReadOnlyList<MapNode>> Steps => steps;
    public int StepCount => steps.Count;
    public MapNode Start => steps[0][0];

    public MapData(int seed, List<List<MapNode>> steps)
    {
        Seed = seed;
        this.steps = steps;
    }

    public MapNode GetNode(int step, int lane) => steps[step][lane];

    /// Every node is reachable from Start, and every node except the last step has a way forward.
    public bool IsValid()
    {
        var reached = new HashSet<MapNode>();
        var stack = new Stack<MapNode>();
        stack.Push(Start);
        reached.Add(Start);

        while (stack.Count > 0)
            foreach (var n in stack.Pop().Next)
                if (reached.Add(n)) stack.Push(n);

        int total = 0;
        for (int s = 0; s < steps.Count; s++)
        {
            foreach (var node in steps[s])
            {
                total++;
                if (s < steps.Count - 1 && node.Next.Count == 0) return false;
            }
        }

        return reached.Count == total;
    }
}