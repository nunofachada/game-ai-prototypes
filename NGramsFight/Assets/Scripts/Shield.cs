using System.Collections;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Shield : MonoBehaviour
    {
        private HitAnimate hitAnimate;

        private void Awake()
        {
            hitAnimate = GetComponent<HitAnimate>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            hitAnimate?.Animate();
        }
    }
}