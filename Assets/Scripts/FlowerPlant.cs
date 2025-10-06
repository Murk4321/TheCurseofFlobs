using UnityEngine;

public enum FlowerType {
    Normal,
    Devil,
    Spirit
}

public class FlowerPlant : MonoBehaviour
{
    [SerializeField] private float growthTime = 10;
    [SerializeField] private float maxSize = 1;
    [SerializeField] private Sprite activeCenterSprite;
    public SpriteRenderer petalSprite;
    public SpriteRenderer flowerSprite;
    public SpriteRenderer centerSprite;

    [SerializeField] private Sprite deadFlower;
    [SerializeField] private Sprite deadPetals;

    public Vector3Int growingOn;
    public bool isGrown;
    public bool isDead;
    public FlowerType flowerType;
    public Color flowerColor;

    private int rotationDir;
    private Animator anim;

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start() {
        transform.localScale = Vector3.zero;
        float startRotationDir = Random.Range(-1f, 1f);

        rotationDir = startRotationDir > 0 ? 1 : -1;

        if (flowerType == FlowerType.Normal)
        {
            flowerColor = ColorUtils.RandomColor();
            petalSprite.color = flowerColor;
        }
        maxSize += Random.Range(-0.2f, 0.6f);
    }

    private void Update() {
        Vector3 maxSizeVector = new Vector3(maxSize, maxSize, maxSize);

        if(PentagramManager.Instance != null)
        {
            if (flowerType == FlowerType.Normal && transform.localScale.x > maxSizeVector.x - 0.2f && (PentagramManager.Instance.stage == 1 || PentagramManager.Instance.stage == 2)) {
                isDead = true;
            }
        }

        if (!isGrown && !isDead) {
            if (transform.localScale.x < maxSizeVector.x - 0.1f && !isGrown) {
                transform.localScale = Vector3.Lerp(transform.localScale, maxSizeVector, (1 / growthTime) * Time.deltaTime);
           
            } else isGrown = true;

        } else {
            if ((flowerType == FlowerType.Normal || flowerType == FlowerType.Spirit) && !isDead) {
                petalSprite.transform.Rotate(0, 0, rotationDir * 100 * Time.deltaTime);
            } else if (flowerType == FlowerType.Normal && isDead) {
                petalSprite.sprite = deadPetals;
                flowerSprite.sprite = deadFlower;
                centerSprite.sprite = null;
                petalSprite.transform.localRotation = Quaternion.identity;
                anim.enabled = false;
            } else if (flowerType == FlowerType.Devil && centerSprite.sprite != activeCenterSprite) {
                centerSprite.sprite = activeCenterSprite;
            }
        }
    }
}
