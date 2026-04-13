using UnityEngine;

public class AirWallCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            BottomMessageBox.ShowCustom("前面的区域以后再来探索吧");
        }
    }
}
