using UnityEngine;

public class FlobAnimatorEvents : MonoBehaviour
{
    public void ApplyTileChange() {
        GetComponentInParent<CorpseTool>().ApplyTileChange();
    }

    public void AllowUsage() {
        GetComponentInParent<CorpseTool>().AllowUsage();
    }
}
