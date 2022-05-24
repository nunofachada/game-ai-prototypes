using System;
using System.Collections;
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

        private AttackDefenseDamage attDefDam;

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
            this.attDefDam = attDefDam;

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
            StopCoroutine(displace);
            spriteRenderer.enabled = false;
            transform.localPosition = originalPosition;

            if (collider.gameObject.name.Equals("Shield"))
            {
                Debug.Log($"{attDefDam.Attack} attack unsuccessful, enemy predicted proper {attDefDam.ProperDefense} defense!");
                shooter.TakeDamage(attDefDam.DamageToPlayerIfFail);
            }
            else if (collider.gameObject.name.Equals("Enemy"))
            {
                Debug.Log($"{attDefDam.Attack} attack SUCCESSFUL!");
                collider.gameObject.GetComponent<Agent>().TakeDamage(attDefDam.DamageToEnemyIfSuccess);
            }
            else
            {
                throw new InvalidProgramException("Shot hit something unexpected!");
            }
        }
    }
}