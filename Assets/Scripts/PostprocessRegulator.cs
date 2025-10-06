using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostprocessRegulator : MonoBehaviour
{
    public static PostprocessRegulator Instance { get; private set; }

    private PostProcessVolume postFX;
    private Vignette vignette;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        postFX = GetComponent<PostProcessVolume>();
    }

    private void Start() {
        postFX.profile.TryGetSettings(out vignette);
    }

    public void SetVignette(float intensity) {
        if (vignette != null)
            vignette.intensity.value = intensity;
    }
}
