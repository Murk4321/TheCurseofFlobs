using UnityEngine;

public static class ColorUtils {
    public static float CompareColors(Color a, Color b) {
        float distance = Mathf.Sqrt(
            Mathf.Pow(a.r - b.r, 2) +
            Mathf.Pow(a.g - b.g, 2) +
            Mathf.Pow(a.b - b.b, 2)
        );

        float similarity = 1f - (distance / 1.732f);
        return Mathf.Clamp01(similarity);
    }

    public static Color RandomColor() {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
}
