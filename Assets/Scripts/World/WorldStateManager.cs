using System;
using Cinemachine;
using UnityEngine;

namespace World
{
    [Flags]
    public enum EWorldState
    {
        PAST = 0,
        PRESENT = 1,
        FUTURE = 2
    }

    public class WorldStateManager : MonoBehaviour
    {
        [SerializeField] public EWorldState currentWorldState = EWorldState.PAST;
        
        [Header("References")]
        [SerializeField] private CinemachineConfiner2D cameraConfiner;

            
        [Header("Testing stuff, removing this eventually")]
        [SerializeField] private Room testRoom1;
        [SerializeField] private Room testRoom2;
        [SerializeField] private GameObject player; //TESTING

        private static WorldStateManager _worldStateManager;
        [SerializeField] private Room currentRoom;

        [SerializeField] private AudioManager audioManager;

        // TODO: Reconcile HWorldStateChange with ONWorldStateChange
        public delegate void WorldStateChangeHandler( EWorldState state);
        public static event WorldStateChangeHandler HWorldStateChange;

        public Action<EWorldState> OnWorldStateChange;
        public Action<Room> OnRoomTransition;
        private Portal fromPortal;

        public static WorldStateManager Instance => _worldStateManager;
        public Room CurrentRoom => currentRoom;
        public PhysicsMaterial2D groundMaterial;
        
        private void Awake()
        {
            if (_worldStateManager == null)
            {
                _worldStateManager = this;
            }
        }

        private void Start()
        {
            
            Init();
            
            // TESTING: Remove this eventually
            // RoomTransition(testRoom1, 0);

        }

        private void Update()
        {
            // TESTING: Remove this eventually
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     if (currentRoom == testRoom1)
            //     {
            //         RoomTransition(testRoom2, 0);
            //     }
            //     else
            //     {
            //         RoomTransition(testRoom1, 0);
            //     }
            // }

            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeWorldState(currentWorldState == EWorldState.FUTURE ? 0 : currentWorldState + 1);

            }
        }

        private void OnDestroy()
        {
            _worldStateManager = null;
        }


        private void Init()
        {
            RoomTransition(currentRoom);
            audioManager.ChangeAudio(currentWorldState);
            // ChangeWorldState(currentWorldState);
        }
        public void ChangeWorldState(EWorldState newWorldState)
        {
            currentWorldState = newWorldState;
            
            currentRoom?.ChangeWorldState(newWorldState);
            OnWorldStateChange?.Invoke(currentWorldState);

            HWorldStateChange?.Invoke(currentWorldState);

            audioManager.ChangeAudio(newWorldState);
        }

        public void RoomTransition(Room room, int portalIndex = -1)
        {
            // Previous Room
            currentRoom?.gameObject.SetActive(false);

            // New Room
            currentRoom = room;
            currentRoom?.ChangeWorldState(currentWorldState);
            currentRoom?.gameObject.SetActive(true);
            cameraConfiner.m_BoundingShape2D = currentRoom.CameraBounds;

            // TESTING: Move player to new spawn point
            if (portalIndex != -1)
            {
                fromPortal = currentRoom.GetPortal(portalIndex);
                if (fromPortal.SpawnPoint == null)
                {
                    Debug.LogError("Portal does not have a spawn point!");
                    return;
                }
                Debug.Log("Portaling player...");
                player.transform.position = fromPortal.SpawnPoint.position;
            }

            OnRoomTransition?.Invoke(currentRoom);
        }

        public void HandlePlayerDeath()
        {
            Debug.Log("Handling player death");
            player.transform.position = fromPortal.SpawnPoint.position;
        }
    }
}