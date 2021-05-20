/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;

namespace LibGameAI.Util
{
    public static class Stats
    {

        // Public domain code from https://www.johndcook.com/blog/csharp_phi/
        public static double NormalCDF(double x, double mean = 0, double std = 1)
        {
            // constants
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;
            const double p = 0.3275911;

            // x after mean and std
            x = (x - mean) / std;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return 0.5 * (1.0 + sign * y);
        }

        // Quick and dirty Box-Mueller transform, ignoring z1 (probably need to improve this)
        public static double NextNormalDouble(Func<double> nextUnifDouble)
        {

            double u1, u2;

            do
            {
                u1 = nextUnifDouble();
                u2 = nextUnifDouble();
            }
            while (u1 <= double.Epsilon);

            double mag = Math.Sqrt(-2.0 * Math.Log(u1));
            double z0  = mag * Math.Cos(2 * Math.PI * u2);
            //double z1  = mag * Math.Sin(2 * Math.PI * u2);

            return z0;
        }
    }
}