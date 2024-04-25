/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

namespace GameAIPrototypes.ProceduralLandscape.GenConfig
{
    public class PostProcessingOnlyConfig : AbstractGenConfig
    {
        public override bool IsModifier => true;
        public override float[,] Generate(float[,] heights)
        {
            return heights;
        }
    }
}