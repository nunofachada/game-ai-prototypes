﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections.Generic;
using UnityEngine;

namespace GameAIPrototypes.Movement.Paths
{
    public abstract class AbstractPathGenerator : MonoBehaviour
    {
        public abstract IList<Vector2> GeneratePath();
    }
}