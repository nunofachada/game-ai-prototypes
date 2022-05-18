using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class MoveConfig : MonoBehaviour
    {
        [SerializeField]
        [Expandable]
        private AttackPatternSet patterns;

        private InputHandler inputHandler;

        public AttackPatternSet Patterns => patterns;

        private void Awake()
        {
            inputHandler = GetComponentInParent<InputHandler>();
            inputHandler.SetKnownInputs(patterns.KnownInputs);
        }
    }
}