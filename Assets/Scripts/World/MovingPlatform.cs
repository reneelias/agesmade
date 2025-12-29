using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private List<Transform> Points = new List<Transform>();
        
        private float timeElapsed = 0;
        private float lerpDuration = 1;

        private int pointIndex = 0;
        private Vector3 positionToMoveTo;

        void Start()
        {
        }

        private void OnEnable()
        {
            transform.position = Points[0].transform.position;
            positionToMoveTo = Points[pointIndex+1].transform.position;
            StartCoroutine(LerpPosition(positionToMoveTo, 5));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator LerpPosition(Vector3 targetPosition, float duration)
        {
            float time = 0;
            Vector3 startPosition = transform.position;

            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            pointIndex = pointIndex + 1 >= Points.Count ? 0 : pointIndex + 1;
            positionToMoveTo = Points[pointIndex].transform.position;

            yield return new WaitForSeconds(1.5f);
         
            StartCoroutine(LerpPosition(positionToMoveTo, 5));
        }
    }
}
