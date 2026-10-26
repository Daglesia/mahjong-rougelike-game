using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// UI representation of one MapNode. Lives on the node prefab, same pattern as Card.
public class MapNodeView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image art;

    public MapNode Node { get; private set; }
    public event Action<MapNodeView> Clicked;

    private void Awake()
    {
        if (art == null) art = GetComponent<Image>();
    }

    public void Setup(MapNode node)
    {
        Node = node;
        name = $"Node {node.Step}-{node.Lane} ({node.Data.id})";

        if (node.Data.art != null)
        {
            art.sprite = node.Data.art;
            art.color = Color.white;
        }
        else
        {
            // No art assigned yet: tint by kind so the map is readable straight away.
            art.sprite = null;
            art.color = node.Data.kind switch
            {
                NodeKind.Start => new Color(0.3f, 0.8f, 0.4f),
                NodeKind.Combat => new Color(0.85f, 0.3f, 0.3f),
                _              => new Color(0.4f, 0.6f, 0.9f),
            };
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Clicked?.Invoke(this);
    }
}