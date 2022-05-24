using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class HitAnimate : MonoBehaviour
    {
        [SerializeField]
        private float maxScaleWhenHit = 1.2f;
        [SerializeField]
        private float hitAnimSeconds = 0.5f;

        private Vector3 baseScale;

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private IEnumerator Hit()
        {
            float timeStart = Time.time;
            float timeEllapsed;

            do
            {
                timeEllapsed = Time.time - timeStart;
                float scaleMult = Mathf.Lerp(1, maxScaleWhenHit, 2 * timeEllapsed / hitAnimSeconds);
                transform.localScale = baseScale * scaleMult;
                yield return null;
            } while (timeEllapsed < hitAnimSeconds / 2);

            do
            {
                timeEllapsed = Time.time - timeStart;
                float scaleMult = Mathf.Lerp(1, maxScaleWhenHit, 2 - 2 * timeEllapsed / hitAnimSeconds);
                transform.localScale = baseScale * scaleMult;
                yield return null;
            } while (timeEllapsed < hitAnimSeconds);

            transform.localScale = baseScale;
        }

        public void Animate()
        {
            StartCoroutine(Hit());
        }

    }
}