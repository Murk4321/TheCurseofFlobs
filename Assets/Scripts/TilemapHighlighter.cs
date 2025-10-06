using UnityEngine;
using UnityEngine.Tilemaps;

public enum HighlightTileType {
    Default = 0,
    Yes = 1,
    No = 2,
    Disable = 3
}

public class TilemapHighlighter : MonoBehaviour {
    public static TilemapHighlighter Instance { get; private set; }

    private Tilemap highlightTilemap; 
    [SerializeField] private TileBase[] highlightTiles;

    HighlightTileType selectedTile = HighlightTileType.Disable;

    private Camera cam;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        highlightTilemap = GetComponent<Tilemap>();
    }

    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = highlightTilemap.WorldToCell(mouseWorldPos);

        highlightTilemap.ClearAllTiles();
        highlightTilemap.SetTile(cellPos, highlightTiles[(int)selectedTile]);
    }

    public void SetHighlightTile(HighlightTileType tileType) {
        selectedTile = tileType;
    }
}
