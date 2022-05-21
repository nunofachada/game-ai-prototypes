using UnityEngine;
using TMPro;

namespace AIUnityExample.NGramsFight.UI
{
    public class HealthUpdater : MonoBehaviour
    {
        [SerializeField]
        private Agent agent;

        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateHealth();
        }

        private void OnEnable()
        {
            agent.OnHealthChange += UpdateHealth;
        }

        private void OnDisable()
        {
            agent.OnHealthChange -= UpdateHealth;
        }

        private void UpdateHealth()
        {
            text?.SetText($"{agent.Health:f2}");
        }
    }
}