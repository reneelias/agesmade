using UnityEngine;

namespace World
{
    public class DeathZone : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayerMask;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if ((playerLayerMask.value & (1 << col.gameObject.layer)) > 0)
            {
                Debug.Log("Player triggered a death zone");
                WorldStateManager.Instance.HandlePlayerDeath();
            }
        }
    }
}
