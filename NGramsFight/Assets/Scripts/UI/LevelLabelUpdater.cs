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
            gameController.OnChangeLevel += UpdateLevelLabel;
        }

        private void OnDisable()
        {
            gameController.OnChangeLevel -= UpdateLevelLabel;
        }

        private void UpdateLevelLabel()
        {
            text?.SetText($"Level {gameController.Level:d3}");
        }
    }
}