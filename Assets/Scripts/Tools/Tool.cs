using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Tool : MonoBehaviour {
    protected Animator anim;
    protected Tilemap tilemap;
    protected Camera cam;
    protected PlayerMovement player;

    protected bool canUse = true;
    [SerializeField] protected float useDistance = 2f;

    protected Vector3Int? storedTilePos;
    protected TileBase pendingFromTile;
    protected TileBase pendingToTile;
    protected Sounds soundToPlay;
    protected float soundVolume;

    protected virtual void Start() {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        player = GetComponentInParent<PlayerMovement>();
        tilemap = GameObject.Find("Map").GetComponent<Tilemap>();
    }

    protected virtual void Update() {
        bool usable = CheckToolUsage();
        TilemapHighlighter.Instance.SetHighlightTile(
            usable ? HighlightTileType.Yes : HighlightTileType.No
        );
    }

    public abstract void PrimaryAction();

    public abstract void SecondaryAction();

    protected bool PrepareTileAction(TileBase fromTile, TileBase toTile, string animTrigger, Sounds sound = Sounds.No, float volume = 0.5f) {
        if (!canUse) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);

        if (dist <= useDistance) {
            TileBase targetedTile = tilemap.GetTile(tilePos);
            if (targetedTile == fromTile) {
                soundToPlay = sound;
                soundVolume = volume;
                anim.enabled = true;
                anim.SetTrigger(animTrigger);
                canUse = false;
                player.canMove = false;

                storedTilePos = tilePos;
                pendingFromTile = fromTile;
                pendingToTile = toTile;
                return true;
            }
        }

        return false;
    }

    public virtual void ApplyTileChange() {
        if (storedTilePos.HasValue) {
            tilemap.SetTile(storedTilePos.Value, pendingToTile);
            AudioManager.Instance.PlaySound(soundToPlay, soundVolume);
            storedTilePos = null;
        }
    }

    public virtual void AllowUsage() {
        canUse = true;
        player.canMove = true;
    }

    protected virtual bool CheckToolUsage() {
        if (!canUse || !player.canMove) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);

        if (dist > useDistance) return false;

        return true;
    }
}
