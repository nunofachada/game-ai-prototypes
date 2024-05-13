/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Authors: Diogo de Andrade, Nuno Fachada
 * */

using System;
using UnityEngine;

[Serializable]
public class FileSelector
{
    [SerializeField]
    private string filename;

    public string Filename => filename;
}


[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class OnPathChangedAttribute : Attribute
{
    public string Callback { get; }

    public OnPathChangedAttribute(string callback)
    {
        Callback = callback;
    }
}
