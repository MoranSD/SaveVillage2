using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    public event Action<Guid> OnDestroyed;

    [field: SerializeField] public Guid UnicId { get; set; }
    public Sprite Sprite => spriteRenderer.sprite;

    public int Id;
    public int Health;
    public Vector2Int Size;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField, Range(0, 1)] private float debugAlpha = 0.5f;

    private bool showingStats;

    protected virtual void Awake()
    {
        var c = GetComponent<BoxCollider2D>();
        c.size = Size;
    }

    protected virtual void OnDestroy()
    {
        if(showingStats)
            G.StatsDrawer.HideStats(StatsDrawer.DrawStatsType.building);
    }

    protected virtual void OnMouseEnter()
    {
        G.StatsDrawer.DrawStats(this);
        showingStats = true;
    }

    protected virtual void OnMouseExit()
    {
        G.StatsDrawer.HideStats(StatsDrawer.DrawStatsType.building);
        showingStats = false;
    }

    public virtual string GetInfo()
    {
        return 
            $"UnicId {GameUtils.GetShortStringId(UnicId)}...\n" +//gets first 5 chars
            $"Id {Id}\n" +
            $"Health {Health}\n";
    }

    public void ApplyDamage(int damage)
    {
        Health -= damage;

        if (Health > 0)
            return;

        OnDestroyed?.Invoke(UnicId);
        Destroy(gameObject);
    }

    protected virtual void OnGetDestroyed() { }

    private void OnDrawGizmos()
    {
        bool isGray = false;
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                isGray = !isGray;
                Gizmos.color = isGray ? new(0.5f, 0.5f, 0.5f, debugAlpha) : new(0, 0, 0, debugAlpha);
                Gizmos.DrawCube(transform.position + new Vector3(x - Size.x / 2f + 0.5f, y - Size.y / 2f + 0.5f), Vector3.one);
            }
        }
    }
}
