using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace AIUnityExample.NGramsFight
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private GameObject viewGameObject;

        [SerializeField]
        [ReorderableList]
        private List<AttackPattern> patterns;

        private LinkedList<InputType> buffer;
        private IView view;
        private InputActionNode inputMatchingTreeRoot;

        private const int MIN_BUFFER_SIZE = 3;
        private const int MAX_BUFFER_SIZE = 5;

        // Start is called before the first frame update
        private void Awake()
        {
            buffer = new LinkedList<InputType>();
            view = viewGameObject.GetComponent<IView>();
        }

        private void OnEnable()
        {
            view.PressedInput += HandleInput;
        }

        private void OnDisable()
        {
            view.PressedInput -= HandleInput;
        }

        private void FixedUpdate()
        {
            if (buffer.Count >= MIN_BUFFER_SIZE && buffer.Count <= MAX_BUFFER_SIZE)
            {
                InputActionNode actionNode = inputMatchingTreeRoot.Match(buffer);
                if (actionNode is null)
                {
                    // Input didn't match anything
                }
                else if (actionNode.IsLeaf)
                {
                    // Action found, schedule it
                }
            }
        }

        private void HandleInput(InputType input)
        {
            buffer.AddLast(input);
            if (buffer.Count > MAX_BUFFER_SIZE)
            {
                buffer.RemoveFirst();
            }
        }
    }
}