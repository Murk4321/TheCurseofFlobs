using UnityEngine;
using TMPro;

public class TMPAnimator : MonoBehaviour {
    private TextMeshProUGUI tmp;
    private Mesh mesh;
    private Vector3[] vertices;

    public float amplitude = 5f;          
    public float frequency = 2f;      
    public float speed = 2f;          

    void Awake() {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        tmp.ForceMeshUpdate();
        mesh = tmp.mesh;
        vertices = mesh.vertices;

        float time = Time.time * speed;


        for (int i = 0; i < vertices.Length; i++) {
            Vector3 orig = vertices[i];
            orig.y += Mathf.Sin(orig.x * frequency * 0.01f + time) * amplitude;
            vertices[i] = orig;

        }

        mesh.vertices = vertices;
        tmp.canvasRenderer.SetMesh(mesh);
    }
}
