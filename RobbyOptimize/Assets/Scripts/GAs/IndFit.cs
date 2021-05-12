namespace LibGameAI.GAs
{
    public struct IndFit<I, F>
    {
        public I Ind { get; }
        public F Fit { get; set; }

        public IndFit(I ind, F fit = default)
        {
            Ind = ind;
            Fit = fit;
        }
    }
}