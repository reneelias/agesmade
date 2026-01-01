using System;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private PolygonCollider2D cameraBounds;

        [SerializeField] private GameObject PastWorldGO;
        [SerializeField] private GameObject PresentWorldGO;
        [SerializeField] private GameObject FutureWorldGO;

        [SerializeField] List<Portal> portals = new List<Portal>();
        public PolygonCollider2D CameraBounds => cameraBounds;


        private void Start()
        {
            //if (WorldStateManager.Instance.CurrentRoom != this)
            //    gameObject.SetActive(false);

            //Transform[] transforms = PastWorldGO.GetComponentsInChildren<Transform>(true);

            //foreach (Transform transform in transforms)
            //{
            //    if (transform.gameObject.CompareTag("Ground") && transform.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            //    {

            //        Debug.Log(spriteRenderer.gameObject.name + " color is now " + spriteRenderer.color);
            //    }
            //}

            // TODO: Patch portals!
            //portals.Clear();
            //foreach (var portal in transform.GetComponentsInChildren<Portal>())
            //{
            //    portals.Add(portal);
            //}
        }

        public void ChangeWorldState(EWorldState currentWorldState)
        {
            switch (currentWorldState)
            {
                
                case EWorldState.PAST:
                    PastWorldGO.SetActive(true);
                    PresentWorldGO.SetActive(false);
                    FutureWorldGO.SetActive(false);
                    break;
                case EWorldState.PRESENT:
                    PastWorldGO.SetActive(false);
                    PresentWorldGO.SetActive(true);
                    FutureWorldGO.SetActive(false);
                    break;
                case EWorldState.FUTURE:
                    PastWorldGO.SetActive(false);
                    PresentWorldGO.SetActive(false);
                    FutureWorldGO.SetActive(true);              
                    break;
                default:
                    break;
            }
        }

        public Portal GetPortal(int portalIndex)
        {
            foreach (var portal in portals)
            {
                if (portal.PortalIndex == portalIndex)
                {
                    return portal;
                }
            }

            Debug.LogError($"Portal index {portalIndex} does not exist");
            return null;
        }
    }
}
