using System;
using UnityEngine;

namespace NPC
{
    public class NpcDialogue : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayerMask;

        [SerializeField] private RectTransform dialogueBox;

        void OnTriggerEnter2D(Collider2D col)
        {
            // If player
            if ((playerLayerMask.value & (1 << col.gameObject.layer)) > 0)
            {
                ShowDialogue();
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            // If player
            if ((playerLayerMask.value & (1 << col.gameObject.layer)) > 0)
            {
                HideDialogue();
            }
        }

        private void ShowDialogue()
        {
            dialogueBox?.gameObject.SetActive(true);
        }

        private void HideDialogue()
        {
            dialogueBox?.gameObject.SetActive(false);
        }
    }
}
