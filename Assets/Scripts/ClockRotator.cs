using UnityEngine;

public class ClockRotator : MonoBehaviour
{
    private Transform hourArrow;
    private Transform minuteArrow;

    private float hourRotation = 0;
    private float minuteRotation = 0;

    private void Awake() {
        hourArrow = transform.GetChild(0);
        minuteArrow = transform.GetChild(1);
    }

    private void Update() {
        if (PentagramManager.Instance.timerActive) {
            hourRotation = (hourRotation + 30 * Time.deltaTime) % 360;
            minuteRotation = (minuteRotation + 360 * Time.deltaTime) % 360;
            hourArrow.localRotation = Quaternion.Euler(0, 0, -hourRotation);
            minuteArrow.localRotation = Quaternion.Euler(0, 0, -minuteRotation);
        }
    }
}
