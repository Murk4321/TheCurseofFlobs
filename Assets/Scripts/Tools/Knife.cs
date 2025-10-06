using UnityEngine;
using UnityEngine.Tilemaps;

public class Knife : Tool {
    [SerializeField] private TileBase plantedTile;
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase bloodyPlantedTile;

    private bool pendingCut;
    private bool pendingWash;
    private bool pendingFlowerChange;
    private bool pendingCorpseChange;
    private Collider2D knifeCollider;
    private SpriteRenderer sr;
    public bool isBloody = false;  
    public bool canKill = false;

    private PlayerTools playerTools;

    [SerializeField] private Sprite knifeDefault;
    [SerializeField] private Sprite knifeBloody;

    private void Awake() {
        knifeCollider = GetComponent<Collider2D>();
        playerTools = FindObjectOfType<PlayerTools>();
        sr = GetComponent<SpriteRenderer>();
        knifeCollider.enabled = false;
    }

    public override void PrimaryAction() {
        if (!canUse) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;

        if (targetedTile == waterTile && isBloody) {
            anim.SetTrigger("cut");
            canUse = false;
            player.canMove = false;

            pendingWash = true;
            return;
        }

        GameObject flower = TileManager.Instance.GetFlower(tilePos);

        if ((targetedTile == plantedTile || targetedTile == grassTile || targetedTile == bloodyPlantedTile) 
            && flower != null && (flower.GetComponent<FlowerPlant>().isGrown || flower.GetComponent<FlowerPlant>().isDead) && playerTools.flowerPickedUp == null) {
            anim.SetTrigger("cut");
            canUse = false;
            player.canMove = false;

            storedTilePos = tilePos;
            pendingCut = true;
        }
    }

    public override void SecondaryAction() {
        if (!canUse || playerTools.corpsePickedUp != null || !canKill) return;

        anim.SetTrigger("kill");
        AudioManager.Instance.PlaySound(Sounds.Attack);
        knifeCollider.enabled = true;
        canUse = false;
        player.canMove = false;
    }

    public override void ApplyTileChange() {

        if (pendingCut && storedTilePos.HasValue) {
            Vector3Int pos = storedTilePos.Value;

            GameObject flower = TileManager.Instance.GetFlower(pos);
            FlowerPlant flowerPlant = flower.GetComponent<FlowerPlant>();
            if (flower != null && flowerPlant.isGrown && !flowerPlant.isDead) {
                flower.transform.parent = playerTools.toolHolder;
                flower.transform.localPosition = new Vector3(-1, -0.7f, 0);
                AudioManager.Instance.PlaySound(Sounds.Cut);

                var flowerTool = flower.AddComponent<FlowerTool>();
                flowerTool.flowerColor = flowerPlant.flowerColor;
                SpriteRenderer[] flowerRenderers = flower.GetComponentsInChildren<SpriteRenderer>();
                //foreach(var renderer in flowerRenderers) {
                //    renderer.sortingLayerName = "Tools";
                //}
                playerTools.flowerPickedUp = flower;
                playerTools.tools[(int)Tools.Flower] = flowerTool;
                pendingFlowerChange = true;

                TileManager.Instance.RemoveFlower(pos);
            } else if (flowerPlant.isDead) {
                AudioManager.Instance.PlaySound(Sounds.Cut);
                TileManager.Instance.RemoveFlower(pos);
                Destroy(flowerPlant.gameObject);
            }

            tilemap.SetTile(pos, dirtTile);
            storedTilePos = null;
            pendingCut = false;
        }

        if (pendingWash) {
            AudioManager.Instance.PlaySound(Sounds.Wash);
            isBloody = false;
            sr.sprite = knifeDefault;
            pendingWash = false;
        }

        knifeCollider.enabled = false;
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        GameObject flower = TileManager.Instance.GetFlower(tilePos);
        if (targetedTile == waterTile && isBloody) return true;
        if (flower == null) return false;
        FlowerPlant flowerPlant = flower.GetComponent<FlowerPlant>();
        if (targetedTile == bloodyPlantedTile && TileManager.Instance.GetFlower(tilePos) != null && flowerPlant.isGrown && playerTools.flowerPickedUp == null) return true;
        if (targetedTile == grassTile && TileManager.Instance.GetFlower(tilePos) != null && playerTools.flowerPickedUp == null) return true;
        return targetedTile == plantedTile && flowerPlant != null && (flowerPlant.isGrown || flowerPlant.isDead) && playerTools.flowerPickedUp == null;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Flob")) {
            knifeCollider.enabled = false;
            isBloody = true;
            sr.sprite = knifeBloody;
            GameflowManager.Instance.kills++;
            GameObject flob = collision.gameObject;
            Destroy(flob.GetComponent<Rigidbody2D>());
            Destroy(flob.GetComponent<Collider2D>());

            flob.transform.parent = playerTools.toolHolder;
            flob.transform.localPosition = new Vector3(-1.3f, -1.2f, 0);
             
            var corpseTool = flob.AddComponent<CorpseTool>();
            SpriteRenderer[] flobRenderers = flob.GetComponentsInChildren<SpriteRenderer>();
            //foreach (var renderer in flobRenderers) {
            //    renderer.sortingLayerName = "Tools";
            //}
            playerTools.corpsePickedUp = flob;
            playerTools.tools[(int)Tools.Corpse] = corpseTool;
            FindObjectOfType<FlobSpawner>().spawnedFlobs.Remove(flob);
            Destroy(flob.GetComponent<FlobCitizen>(), 0.2f);
            pendingCorpseChange = true;
        }
    }

    public override void AllowUsage() {
        base.AllowUsage();
        knifeCollider.enabled = false;
        if (pendingFlowerChange) {
            playerTools.SetTool(Tools.Flower);
            pendingFlowerChange = false;
        }

        if (pendingCorpseChange) {
            playerTools.SetTool(Tools.Corpse);
            pendingCorpseChange = false;
        }
    }
}
