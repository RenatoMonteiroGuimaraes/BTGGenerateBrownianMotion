namespace BTGGenerateBrownianMotion.Models
{
    public class BrownianMotionParams
    {
        public double InitialPrice { get; set; }
        public double Volatility { get; set; }
        public double Drift { get; set; }
        public int Time { get; set; }
        public int Simulations { get; set; } = 1;
    }
}
