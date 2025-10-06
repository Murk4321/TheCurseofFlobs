using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private Animator bodyAnim;
    private Rigidbody2D rb;
    private ParticleSystem stepParticles;
    public bool canMove = true;
    public bool inDialog = false;

    [SerializeField] private float moveSpeed = 5f;

    private Vector2 inputDir;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        stepParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update() {
        ManageStepParticles();

        if (!canMove || inDialog) {
            bodyAnim.SetFloat("speed", 0f);
            inputDir = Vector2.zero;
            return;
        }

        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        bodyAnim.SetFloat("speed", inputDir.magnitude);

        if (inputDir.x != 0) {
            if (PreGameManager.Instance != null) {
                if (PreGameManager.Instance.isSmall) transform.localScale = new Vector3(inputDir.x > 0 ? -0.6f : 0.6f, 0.6f, 0.6f);
                else transform.localScale = new Vector3(inputDir.x > 0 ? -1 : 1, 1, 1);
            } else transform.localScale = new Vector3(inputDir.x > 0 ? -1 : 1, 1, 1);
        }
    }

    private void ManageStepParticles() {
        var psMain = stepParticles.main;
        var psEmission = stepParticles.emission;

        if (inputDir.sqrMagnitude <= Mathf.Epsilon) {
            psEmission.rateOverTime = 0.2f;
        } else {
            psEmission.rateOverTime = 5;
        }

        float angle = Mathf.Atan2(inputDir.x, inputDir.y);
        psMain.startRotation = angle;
    }

    private void FixedUpdate() {
        if (!canMove) return;

        Vector2 targetPos = rb.position + inputDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }
}
