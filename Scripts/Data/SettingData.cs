namespace Kamenote
{
    [System.Serializable]
    public class SettingData
    {
        /// <summary>
        /// 現在選択中の整理券番号。
        /// </summary>
        public int currentReservation = 1;

        /// <summary>
        /// 現在の整理券番号のリセット回数。
        /// </summary>
        public int currentReset = 0;

        /// <summary>
        /// 現在の来客数。(整理券番号の通りでカウント)
        /// </summary>
        public int currentCustomerCount = 0;

        /// <summary>
        /// 現在の日程。
        /// </summary>
        public int currentDay = 1;

        public SettingData(int reservation, int reset, int count, int day)
        {
            currentReservation = reservation;
            currentReset = reset;
            currentCustomerCount = count;
            currentDay = day;
        }
    }
}