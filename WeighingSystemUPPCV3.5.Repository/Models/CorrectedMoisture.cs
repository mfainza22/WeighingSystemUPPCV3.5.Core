namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class CorrectedMoisture
    {
        public CorrectedMoisture()
        {
            Corrected10 = 0;
            Corrected12 = 0;
            Corrected14 = 0;
            Corrected15 = 0;
        }
        public decimal Corrected10 { get; set; }

        public decimal Corrected12 { get; set; }

        public decimal Corrected14 { get; set; }

        public decimal Corrected15 { get; set; }

        public int MCStatus { get; set; }
    }
}
