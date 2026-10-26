using System;
using System.Collections.Generic;
using System.Linq;

public static class MapGenerator
{
    // Nodes per step (0-based): Start, 2 events, Fight, 2 events, Fight, 2 events, Fight.
    private static readonly int[] Widths = { 1, 2, 1, 2, 1, 2, 1 };

    // Forced fights, 0-based. These are your steps 3, 5, 7 counted from 1.
    private static readonly HashSet<int> FightSteps = new() { 2, 4, 6 };

    public static MapData Generate(int seed, NodeData start, NodeData fight, IReadOnlyList<NodeData> eventPool)
    {
        if (start == null) throw new ArgumentNullException(nameof(start));
        if (fight == null) throw new ArgumentNullException(nameof(fight));

        var events = eventPool?.Where(e => e != null).ToList();
        if (events == null || events.Count == 0)
            throw new ArgumentException("Event pool needs at least one NodeData with weight > 0.", nameof(eventPool));

        // 1. Structure: empty nodes, no content yet (seed-independent).
        var steps = new List<List<MapNode>>();
        for (int s = 0; s < Widths.Length; s++)
        {
            var column = new List<MapNode>();
            for (int lane = 0; lane < Widths[s]; lane++)
                column.Add(new MapNode(s, lane));
            steps.Add(column);
        }

        // 2. Connections: every node links to every node in the next step.
        // Correct for 1 -> 2 (split) and 2 -> 1 (converge). Wide -> wide would need real lane rules.
        for (int s = 0; s < steps.Count - 1; s++)
            foreach (var from in steps[s])
                foreach (var to in steps[s + 1])
                    from.Connect(to);

        // 3. Content: fixed steps first by rule, free steps from the seeded RNG.
        // Own RNG, never shared with the Deck. Fixed iteration order keeps it deterministic.
        var rng = new Random(seed);

        for (int s = 0; s < steps.Count; s++)
        {
            if (s == 0)
            {
                foreach (var node in steps[s]) node.Data = start;
            }
            else if (FightSteps.Contains(s))
            {
                foreach (var node in steps[s]) node.Data = fight;
            }
            else
            {
                var used = new List<NodeData>();
                foreach (var node in steps[s])
                {
                    // Avoid duplicates within a step when the pool allows it.
                    var candidates = events.Where(e => !used.Contains(e)).ToList();
                    if (candidates.Count == 0) candidates = events;

                    node.Data = PickWeighted(rng, candidates);
                    used.Add(node.Data);
                }
            }
        }

        // 4. Output + sanity check.
        var map = new MapData(seed, steps);
        if (!map.IsValid())
            throw new InvalidOperationException("Generated map failed validation.");
        return map;
    }

    private static NodeData PickWeighted(Random rng, List<NodeData> candidates)
    {
        int total = candidates.Sum(c => 1);
        int roll = rng.Next(total);

        foreach (var c in candidates)
        {
            if (roll < 1) return c;
            roll -= 1;
        }
        return candidates[candidates.Count - 1];   // unreachable, keeps the compiler happy
    }
}