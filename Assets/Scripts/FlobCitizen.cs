using UnityEngine;
using UnityEngine.Tilemaps;

public class FlobCitizen : MonoBehaviour {
    public enum State {
        Passive,
        Interested,
        Scared,
        Dead
    }

    [SerializeField] private State currentState = State.Passive;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float stopDuration = 2f;
    [SerializeField] private float scaredDuration = 3f;
    private Transform player;
    private ParticleSystem stepParticles;

    private Vector3 targetPos;
    private float stopTimer = 0f;
    private bool isStopped = false;

    private float scaredTimer = 0f;
    private bool wasScaredOfPlayer = false;

    private Animator anim;
    [SerializeField] private SpriteRenderer bodySr;
    [SerializeField] private SpriteRenderer eyesSr;
    [SerializeField] private GameObject blood;

    private Tilemap tilemap;
    [SerializeField] private TileBase grassTile;

    [SerializeField] private Sprite defaultBody;
    [SerializeField] private Sprite defaultEyes;
    [SerializeField] private Sprite noMouthBody;
    [SerializeField] private Sprite deadEyes;
    [SerializeField] private Sprite interestedBody;
    [SerializeField] private Sprite interestedEyes;
    [SerializeField] private Sprite scaredBody;
    [SerializeField] private Sprite scaredEyes;

    public Color flobColor;
    private Vector3 lastPos;

    private void Awake() {
        player = FindObjectOfType<PlayerMovement>().transform;
        tilemap = GameObject.Find("Map").GetComponent<Tilemap>();
    }

    private void Start() {
        PickRandomTarget();
        anim = GetComponentInChildren<Animator>();
        stepParticles = GetComponentInChildren<ParticleSystem>();
        bodySr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        flobColor = ColorUtils.RandomColor();
        bodySr.color = flobColor;
    }

    private void FixedUpdate() {
        if (currentState == State.Dead) {
            DeadBehavior();
            return;
        }

        lastPos = transform.position;
        HandleParticles();

        if (currentState == State.Scared) {
            scaredTimer -= Time.fixedDeltaTime;
            if (scaredTimer <= 0f) {
                SetState(State.Passive);
            }
        }

        switch (currentState) {
            case State.Passive:
                PassiveBehavior();
                break;
            case State.Interested:
                InterestedBehavior();
                break;
            case State.Scared:
                ScaredBehavior();
                break;
        }
    }

    private void HandleParticles() {
        if (stepParticles == null) return;

        var psMain = stepParticles.main;
        var psEmission = stepParticles.emission;

        Vector3 moveDelta = transform.position - lastPos;
        float speed = moveDelta.magnitude / Time.deltaTime;

        if (speed <= 0.01f) {
            psEmission.rateOverTime = 0.2f;
        } else {
            psEmission.rateOverTime = 5;
        }

        if (moveDelta.sqrMagnitude > 0.001f) {
            float angle = Mathf.Atan2(moveDelta.y, moveDelta.x);
            psMain.startRotation = -angle + 90f * Mathf.Deg2Rad; 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Knife")) {
            SetState(State.Dead);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Trigger")) {
            if (currentState == State.Scared || currentState == State.Dead) return;
            if (wasScaredOfPlayer) {
                SetState(State.Scared);
                return;
            }

            PlayerTools tool = player.GetComponent<PlayerTools>();
            if (tool.equippedTool == Tools.Corpse || 
                (tool.equippedTool == Tools.Knife &&
                tool.tools[(int)tool.equippedTool].GetComponent<Knife>().isBloody)) {
                wasScaredOfPlayer = true;
                SetState(State.Scared);
                return;
            } 

            if (tool.equippedTool == Tools.Flower) {
                Color flowerColor = tool.tools[(int)tool.equippedTool].GetComponent<FlowerTool>().flowerColor;
                float colorSimilarity = ColorUtils.CompareColors(flowerColor, flobColor);

                if (colorSimilarity > 0.8f) {
                    SetState(State.Interested);
                }
            } else if (currentState != State.Passive) {
                SetState(State.Passive);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Trigger")) {
            if (currentState == State.Scared || currentState == State.Dead) return;
            SetState(State.Passive);
        }
    }

    // ------------------ STATES ------------------

    private void DeadBehavior() {
        anim.enabled = false;
        bodySr.sprite = noMouthBody;
        eyesSr.sprite = deadEyes;
        blood.SetActive(true);
    }

    private void PassiveBehavior() {
        bodySr.sprite = defaultBody;
        eyesSr.sprite = defaultEyes;
        if (isStopped) {
            stopTimer += Time.deltaTime;
            anim.SetFloat("speed", 0f);
            if (stopTimer >= stopDuration) {
                isStopped = false;
                PickRandomTarget();
            }
            return;
        }

        MoveTo(targetPos, moveSpeed);
        if (Vector3.Distance(transform.position, targetPos) < 0.2f) {
            isStopped = true;
            stopTimer = 0f;
        }
    }

    private void InterestedBehavior() {
        eyesSr.sprite = interestedEyes;
        bodySr.sprite = interestedBody;
        if (player != null && Vector2.Distance(player.position, transform.position ) > 1) {
            MoveTo(player.position, moveSpeed * 1.5f);
        }
    }

    private void ScaredBehavior() {
        bodySr.sprite = scaredBody;
        eyesSr.sprite = scaredEyes;

        if (player == null) return;

        Vector3Int bestTile = Vector3Int.zero;
        float bestScore = float.MinValue;

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        float currentPlayerDist = Vector3.Distance(player.position, transform.position);

        int searchRadius = 10;
        for (int x = -searchRadius; x <= searchRadius; x++) {
            for (int y = -searchRadius; y <= searchRadius; y++) {
                Vector3Int candidate = currentCell + new Vector3Int(x, y, 0);

                TileBase tile = tilemap.GetTile(candidate);
                if (tile != grassTile) continue;

                Vector3 worldPos = tilemap.CellToWorld(candidate) + new Vector3(0.5f, 0.5f, 0);
                float distToPlayer = Vector3.Distance(worldPos, player.position);
                float distToFlob = Vector3.Distance(worldPos, transform.position);

                if (distToPlayer > currentPlayerDist && distToFlob <= wanderRadius * 2f) {
                    float score = distToPlayer - distToFlob * 0.5f;
                    if (score > bestScore) {
                        bestScore = score;
                        bestTile = candidate;
                    }
                }
            }
        }

        if (bestScore > float.MinValue) {
            Vector3 fleeTarget = tilemap.CellToWorld(bestTile) + new Vector3(0.5f, 0.5f, 0);
            MoveTo(fleeTarget, moveSpeed * 2f);
        } else {

            Vector3 dir = (transform.position - player.position).normalized;
            Vector3 fallbackTarget = transform.position + dir * wanderRadius;
            MoveTo(fallbackTarget, moveSpeed * 2f);
        }
    }



    // ------------------ HELPERS ------------------

    private void PickRandomTarget() {
        for (int attempt = 0; attempt < 20; attempt++) {
            Vector2 randOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 worldCandidate = transform.position + new Vector3(randOffset.x, randOffset.y, 0f);

            Vector3Int cellPos = tilemap.WorldToCell(worldCandidate);
            TileBase tile = tilemap.GetTile(cellPos);

            if (tile == grassTile) {
                targetPos = tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
                return;
            }
        } 
        targetPos = transform.position;
    }

    private void MoveTo(Vector3 destination, float speed) {
        Vector3 oldPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        float velocity = (transform.position - oldPos).magnitude / Time.deltaTime;
        anim.SetFloat("speed", velocity);

        Vector3 dir = destination - transform.position;
        if (Mathf.Abs(dir.x) > 0.01f) {
            transform.localScale = new Vector3(dir.x > 0 ? -1 : 1, 1, 1);
        }
    }

    // ------------------ API ------------------

    public void SetState(State newState) {
        if (newState == State.Scared) {
            scaredTimer = scaredDuration;
        }

        currentState = newState;

        if (newState == State.Passive) {
            PickRandomTarget();
            isStopped = false;
            stopTimer = 0f;
        }
    }
}
