namespace AnpanatorCluster
{
    public class Member
    {
        public string Name { get; set; }
        public double[] Scores { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
