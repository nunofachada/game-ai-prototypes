using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Shot : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1;

        private SpriteRenderer spriteRenderer;

        private Vector3 originalPosition;

        private Coroutine displace;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            // Required for triggers to trigger
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        public void Fire()
        {
            originalPosition = transform.localPosition;
            displace = StartCoroutine(Displace());
        }

        // Update is called once per frame
        private IEnumerator Displace()
        {
            spriteRenderer.enabled = true;
            while (true)
            {
                transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.GetComponent<Shot>() is null)
            {
                Debug.Log("SHOT TRIGGER!!");
                StopCoroutine(displace);
                spriteRenderer.enabled = false;
                transform.localPosition = originalPosition;
            }
        }
    }
}