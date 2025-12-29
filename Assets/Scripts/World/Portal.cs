using UnityEngine;

namespace World
{
    /// <summary>
    /// Component used to transition from room to room
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private int portalId;  // Used to determine which portal the player teleports to in a room transition
        [SerializeField] private Room nextRoom;

        [SerializeField] private Transform spawnPoint;

        public Transform SpawnPoint => spawnPoint;
        
        public int PortalIndex => portalId;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if ((playerLayerMask.value & (1 << col.gameObject.layer)) > 0)
            {
                WorldStateManager.Instance.RoomTransition(nextRoom, portalId);
            }
        }
    }
}