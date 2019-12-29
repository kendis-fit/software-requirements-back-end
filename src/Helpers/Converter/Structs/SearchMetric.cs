namespace SoftwareRequirements.Helpers.Converter.Structs
{
    public readonly struct SearchMetric
    {
        public readonly string Coefficient;
        public readonly string Index;
        public readonly string Metric;

        public SearchMetric(string coefficient, string index, string metric)
        {
            this.Coefficient = coefficient;
            this.Index = index;
            this.Metric = metric;
        }
    }
}