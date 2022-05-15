using System;
using System.Collections.Generic;

namespace AIUnityExample.NGramsFight
{
    public interface IView
    {
        void SetValidInputs(ISet<string> validInputs);
        event Action<string> OnPressedInput;
    }
}