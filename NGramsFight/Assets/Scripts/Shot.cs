using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public class Shot : MonoBehaviour
    {
        [SerializeField]
        private float speed = 0.2f;

        private SpriteRenderer spriteRenderer;

        private Vector3 originalPosition;

        private Coroutine displace;

        private Agent shooter;

        private void Awake()
        {
            shooter = GetComponentInParent<Agent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            // Required for triggers to trigger
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        public void Fire(AttackDefenseDamage attDefDam)
        {
            originalPosition = transform.localPosition;

            float y = attDefDam.ProperDefense switch
            {
                DefenseType.High => 0.172f,
                DefenseType.Med => -0.010f,
                DefenseType.Low => -0.191f,
                _ => 0
            };

            transform.localPosition += new Vector3(0, y, 0);

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
            Debug.Log("SHOT TRIGGER!!");
            StopCoroutine(displace);
            spriteRenderer.enabled = false;
            transform.localPosition = originalPosition;
        }
    }
}