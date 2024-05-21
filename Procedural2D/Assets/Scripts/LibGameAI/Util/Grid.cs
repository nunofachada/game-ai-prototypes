namespace LibGameAI.Util
{
    public static class Grid
    {
        /// <summary>
        /// Helper function to convert off-grid positions, wrapping them
        /// around.
        /// </summary>
        /// <param name="pos">Grid position (width or height).</param>
        /// <param name="max">Maximum grid length (width or height).</param>
        /// <param name="wrap">
        /// Set to `true` if the specified position is off-grid, `false`
        /// otherwise.
        /// </param>
        /// <returns>
        /// The wrapped-around position if the given position is outside the
        /// given grid limit.
        /// </returns>
        public static (int pos, bool wrap) Wrap(int pos, int max)
        {
            bool wrap = false;
            while (pos < 0)
            {
                pos += max;
                wrap = true;
            }
            while (pos >= max)
            {
                pos -= max;
                wrap = true;
            }
            return (pos, wrap);
        }
    }
}