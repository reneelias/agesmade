using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private List<Transform> Points = new List<Transform>();

        float lerpDuration = 3; 
        Vector3 startValue; 
        Vector3 endValue; 
        float timeElapsed = 0;

        private int pointIndex = 0;
        void Start()
        {
            startValue = Points[0].transform.position;
            endValue = Points[1].transform.position;
            transform.position = startValue;
        }

        private void OnEnable()
        {
            StartCoroutine(Lerp());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator Lerp()
        {

            while (timeElapsed < lerpDuration)
            {
                transform.position = Vector3.Lerp(startValue, endValue, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            
            timeElapsed = 0;
            transform.position = endValue;
            startValue = endValue;
            pointIndex = pointIndex + 1 >= Points.Count ? 0 : pointIndex + 1;
            endValue = Points[pointIndex].transform.position;

            yield return new WaitForSeconds(1.5f);
         
            StartCoroutine(Lerp());
        }
    }
}
