using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIUnityExample.NGramsFight
{
    public interface IView
    {
        event Action<KnownInput> PressedInput;
    }
}