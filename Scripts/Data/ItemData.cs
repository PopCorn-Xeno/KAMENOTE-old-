namespace Kamenote
{
    [System.Serializable]
    public class ItemData
    {
        #region フィールド

        public string name;

        public int value;

        public int id;

        #endregion

        #region メソッド

        public ItemData(string name, int value, int id)
        {
            this.name = name;
            this.value = value;
            this.id = id;
        }

        #endregion
    }
}
