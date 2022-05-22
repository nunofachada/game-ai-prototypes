using UnityEngine;
using TMPro;

namespace AIUnityExample.NGramsFight.UI
{
    public class LevelLabelUpdater : MonoBehaviour
    {
        [SerializeField]
        private GameController gameController;

        private TextMeshProUGUI text;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            UpdateLevelLabel();
        }

        private void OnEnable()
        {
            gameController.OnNextLevel += UpdateLevelLabel;
        }

        private void OnDisable()
        {
            gameController.OnNextLevel -= UpdateLevelLabel;
        }

        private void UpdateLevelLabel()
        {
            text?.SetText($"Level {gameController.Level:d3}");
        }
    }
}