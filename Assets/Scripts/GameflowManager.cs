using UnityEngine;

public class GameflowManager : MonoBehaviour
{
    public static GameflowManager Instance { get; private set; }

    public int kills = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
