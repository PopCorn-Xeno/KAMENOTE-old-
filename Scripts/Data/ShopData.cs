using System;

namespace Kamenote
{
    [Serializable]
    public class ShopData
    {
        public string className;

        public string year;

        public string shopName;

        public int maxReservation = 1;

        public bool day1Started;

        public bool day1Finished;

        public bool day2Started;

        public bool day2Finished;

        public string GetYear()
        {
            year = DateTime.Now.ToString("yyyy");
            return year;
        }

        public ShopData(string className, string shopName, int maxReservation, string year)
        {
            this.className = className;
            this.shopName = shopName;
            this.maxReservation = maxReservation;
            this.year = year ?? GetYear();
        }
    }
}
