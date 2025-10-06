using UnityEngine;

public class MouseSettings : MonoBehaviour {
    public static MouseSettings Instance { get; private set; }

    private Camera cam;
    private SpriteRenderer sr;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ApplyCursorSettings();
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (cam != null) {
            transform.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void SetCursorSprite(Sprite cursor) {
        if (sr != null) sr.sprite = cursor;
    }

    private void ApplyCursorSettings() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            ApplyCursorSettings();
        }
    }

    private void OnApplicationPause(bool isPaused) {
        if (!isPaused) {
            ApplyCursorSettings();
        }
    }
}
