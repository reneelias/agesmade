using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrettsQuest.Data;

namespace World
{
    public class ColorableObject : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;

        private void OnEnable()
        {
            WorldStateManager.HWorldStateChange += OnWorldChange;
        }

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnDisable()
        {
            WorldStateManager.HWorldStateChange -= OnWorldChange;
        }

        void OnWorldChange(EWorldState newWorldState)
        {
            switch (newWorldState)
            {
                case EWorldState.PAST:
                    {
                        spriteRenderer.color = WorldColorData.pastColor;
                        break;
                    }
                case EWorldState.PRESENT:
                    {
                        spriteRenderer.color = WorldColorData.presentColor;
                        break;
                    }
                case EWorldState.FUTURE:
                    {
                        spriteRenderer.color = WorldColorData.futureColor;
                        break;
                    }

            }

            Debug.Log(spriteRenderer.color);
        }
    }
}

