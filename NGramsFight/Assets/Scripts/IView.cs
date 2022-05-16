using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIUnityExample.NGramsFight
{
    public interface IView
    {
        void SetValidInputs(ISet<KeyCode> validInputs);
        event Action<KeyCode> OnPressedInput;
    }
}