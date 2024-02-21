namespace Kamenote
{
    [System.Serializable]
    public class OrderArchive
    {
	    #region フィールド

        public string name;

        public int reservation;

        public int count;

        public bool isReceived;

        public int value;

        public int sales;

        public string time;

        public int day;

        public int period;

	    #endregion

        #region メソッド

        public void Receive() => isReceived = true;

        #endregion

        public OrderArchive(string name, int count, int reservation, bool isReceived, int value, string time, int day, int resetCount)
        {
            this.name = name;
            this.count = count;
            this.reservation = reservation;
            this.isReceived = isReceived;
            this.value = value;
            this.time = time;
            this.day = day;
            sales = value * count;
            period = resetCount;
        }
    }
}
