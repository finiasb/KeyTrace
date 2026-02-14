namespace WpfApp1
{
    public class KeyCalculator
    {
        public double CalculateKPM(int totalKeys, int activeMinutes)
        {
            if (activeMinutes <= 0) return 0;
            return (double)totalKeys / activeMinutes;
        }

        public int CalculateWPM(int KPM)
        {
            return KPM / 5;
        }
    }
}