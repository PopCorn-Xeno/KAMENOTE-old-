namespace Kamenote
{
    [System.Serializable]
    public class SalesData
    {
        public int sales;

        public int customerCount;

        public SalesData(int sales, int customerCount)
        {
            this.sales = sales;
            this.customerCount = customerCount;
        }
    }
}