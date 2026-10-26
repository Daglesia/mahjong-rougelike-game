using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Lays a MapData out horizontally (step = x, lane = y) and draws a line for every connection.
/// Put this on a RectTransform under a Canvas. Display only: it knows nothing about run progress.
public class MapView : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private MapNodeView nodePrefab;
    [SerializeField] private float stepSpacing = 220f;
    [SerializeField] private float laneSpacing = 160f;

    [Header("Lines")]
    [SerializeField] private float lineThickness = 6f;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.6f);

    [Header("Generate on start (temporary, until a run controller owns this)")]
    [SerializeField] private bool generateOnStart = true;
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 12345;
    [SerializeField] private NodeData startData;
    [SerializeField] private NodeData fightData;
    [SerializeField] private List<NodeData> eventPool = new();

    private RectTransform linesRoot;
    private RectTransform nodesRoot;
    private readonly Dictionary<MapNode, MapNodeView> views = new();

    public event Action<MapNode> NodeClicked;

    private void Start()
    {
        if (!generateOnStart) return;

        int usedSeed = useRandomSeed ? Environment.TickCount : seed;
        Debug.Log($"Map seed: {usedSeed}");
        Build(MapGenerator.Generate(usedSeed, startData, fightData, eventPool));
    }

    public void Build(MapData map)
    {
        Clear();

        // Lines root is created first so it renders behind the nodes.
        linesRoot = CreateRoot("Lines");
        nodesRoot = CreateRoot("Nodes");

        var positions = new Dictionary<MapNode, Vector2>();

        for (int s = 0; s < map.Steps.Count; s++)
        {
            var column = map.Steps[s];
            for (int lane = 0; lane < column.Count; lane++)
            {
                var node = column[lane];
                var pos = GetPosition(s, lane, map.StepCount, column.Count);
                positions[node] = pos;

                var view = Instantiate(nodePrefab, nodesRoot);
                var rt = (RectTransform)view.transform;
                SetCenterAnchored(rt);
                rt.anchoredPosition = pos;

                view.Setup(node);
                view.Clicked += OnNodeViewClicked;
                views[node] = view;
            }
        }

        foreach (var column in map.Steps)
            foreach (var node in column)
                foreach (var next in node.Next)
                    CreateLine(positions[node], positions[next]);
    }

    public MapNodeView GetView(MapNode node) => views.TryGetValue(node, out var v) ? v : null;

    private void Clear()
    {
        foreach (var view in views.Values)
            if (view != null) view.Clicked -= OnNodeViewClicked;
        views.Clear();

        if (linesRoot != null) Destroy(linesRoot.gameObject);
        if (nodesRoot != null) Destroy(nodesRoot.gameObject);
    }

    private void OnNodeViewClicked(MapNodeView view)
    {
        Debug.Log($"Clicked {view.Node}");
        NodeClicked?.Invoke(view.Node);
    }

    // Horizontal map: step grows to the right, lane 0 is the top row, everything centered on the container.
    private Vector2 GetPosition(int step, int lane, int stepCount, int laneCount)
    {
        float x = (step - (stepCount - 1) * 0.5f) * stepSpacing;
        float y = ((laneCount - 1) * 0.5f - lane) * laneSpacing;
        return new Vector2(x, y);
    }

    private void CreateLine(Vector2 from, Vector2 to)
    {
        var go = new GameObject("Line", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(linesRoot, false);

        var img = go.GetComponent<Image>();
        img.color = lineColor;
        img.raycastTarget = false;

        // Pivot on the left edge, then stretch to length and rotate toward the target.
        var rt = img.rectTransform;
        SetCenterAnchored(rt);
        rt.pivot = new Vector2(0f, 0.5f);

        Vector2 d = to - from;
        rt.anchoredPosition = from;
        rt.sizeDelta = new Vector2(d.magnitude, lineThickness);
        rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
    }

    private RectTransform CreateRoot(string rootName)
    {
        var go = new GameObject(rootName, typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.SetParent(transform, false);
        SetCenterAnchored(rt);
        rt.sizeDelta = Vector2.zero;
        return rt;
    }

    private static void SetCenterAnchored(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
    }
}