using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {
    public static TileManager Instance { get; private set; }

    [System.Serializable]
    public class TileInfo {
        public Vector3Int position;
        public GameObject flower;

        public TileInfo(Vector3Int pos, GameObject flowerObj = null) {
            position = pos;
            flower = flowerObj;
        }
    }

    private Dictionary<Vector3Int, TileInfo> tiles = new Dictionary<Vector3Int, TileInfo>();
    private Dictionary<Vector3Int, Color> flowerColors = new Dictionary<Vector3Int, Color>();

    [Header("Flower spawning")]
    [SerializeField] private Tilemap tilemap;
    public TileBase grassTile;
    public TileBase floweredTile;
    public TileBase dirtTile;
    public TileBase waterTile;
    public TileBase wateredTile;
    public TileBase bloodiedTile;
    public TileBase pentagramTile;
    public TileBase spiritFlowerTile;
    [SerializeField] private int maxFlowers = 20;

    private int currentFlowers = 0;
    private bool spawnStarted;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (PreGameManager.Instance != null) {
            if (!PreGameManager.Instance.gameStarted) return;
        }
        if (!spawnStarted) {
            InvokeRepeating(nameof(TrySpawnFlower), 2f, 2f);
            spawnStarted = true;
        }
    }

    // ----------------- TILE INFO -----------------

    public void AddTile(Vector3Int pos, GameObject flower = null) {
        if (!tiles.ContainsKey(pos)) {
            tiles[pos] = new TileInfo(pos, flower);
        }
    }

    public void RemoveTile(Vector3Int pos) {
        if (tiles.ContainsKey(pos)) {
            tiles.Remove(pos);
        }
        if (flowerColors.ContainsKey(pos)) {
            flowerColors.Remove(pos);
        }
    }

    public bool HasTile(Vector3Int pos) {
        return tiles.ContainsKey(pos);
    }

    public GameObject GetFlower(Vector3Int pos) {
        return tiles.ContainsKey(pos) ? tiles[pos].flower : null;
    }

    public void SetFlower(Vector3Int pos, GameObject flowerObj) {
        if (tiles.ContainsKey(pos)) {
            tiles[pos].flower = flowerObj;
        } else {
            tiles[pos] = new TileInfo(pos, flowerObj);
        }
    }

    public void RemoveFlower(Vector3Int pos) {
        if (tiles.ContainsKey(pos)) {
            tiles[pos].flower = null;
        }
    }

    // ----------------- FLOWER COLOR -----------------

    public void SetFlowerColor(Vector3Int pos, Color color) {
        flowerColors[pos] = color;
    }

    public bool HasFlowerColor(Vector3Int pos) {
        return flowerColors.ContainsKey(pos);
    }

    public Color? GetFlowerColor(Vector3Int pos) {
        return flowerColors.ContainsKey(pos) ? flowerColors[pos] : null;
    }

    public void RemoveFlowerColor(Vector3Int pos) {
        if (flowerColors.ContainsKey(pos)) {
            flowerColors.Remove(pos);
        }
    }

    // ----------------- SPAWNING -----------------

    private void TrySpawnFlower() {
        if (currentFlowers >= maxFlowers) return;

        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int pos = new Vector3Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax),
            0
        );

        TileBase tile = tilemap.GetTile(pos);
        if (tile == grassTile) {
            tilemap.SetTile(pos, floweredTile);
            currentFlowers++;

        }
    }

    public void DecreaseFlowerCount() {
        if (currentFlowers > 0) currentFlowers--;
    }

    public void DestroySpawnArea(float worldXThreshold = -25) {
        if (tilemap == null) {
            Debug.LogWarning("Tilemap is null in TileManager.");
            return;
        }

        List<Vector3Int> toClear = new List<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(cellPos)) continue;

                Vector3 worldCenter = tilemap.GetCellCenterWorld(cellPos);
                if (worldCenter.x < worldXThreshold) {
                    toClear.Add(cellPos);
                }
            }
        }

        foreach (var pos in toClear) {
            if (tiles.ContainsKey(pos) && tiles[pos].flower != null) {
                Destroy(tiles[pos].flower);
                tiles[pos].flower = null;
                DecreaseFlowerCount();
            }

            if (tiles.ContainsKey(pos)) tiles.Remove(pos);
            if (flowerColors.ContainsKey(pos)) flowerColors.Remove(pos);

            tilemap.SetTile(pos, null);
            tilemap.SetTransformMatrix(pos, Matrix4x4.identity);
        }
    }

    public void ReplaceRandomGrassWithSpiritFlower() {
        if (tilemap == null || grassTile == null || spiritFlowerTile == null) {
            return;
        }
        BoundsInt bounds = tilemap.cellBounds;

        for (int i = 0; i < 250; i++) {
            int randX = Random.Range(bounds.xMin, bounds.xMax);
            int randY = Random.Range(bounds.yMin, bounds.yMax);
            Vector3Int pos = new Vector3Int(randX, randY, 0);

            TileBase current = tilemap.GetTile(pos);

            if (current == grassTile) {
                tilemap.SetTile(pos, spiritFlowerTile);
              
                return;
            }
        }
    }
}
