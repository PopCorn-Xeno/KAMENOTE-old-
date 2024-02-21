namespace Kamenote.Save
{
    [System.Serializable]
    public class Settings : SaveData<Settings>
    {
        public SettingData data;

        public Settings Save(SettingData data)
        {
            Instance.data = data;
            Save(Type.Setting);
            return Instance;
        }

        public Settings Load(out SettingData data)
        {
            Load(Type.Setting);
            data = Instance.data ?? new(1, 0, 0, 1);
            return Instance;
        }

        public void Delete() => Delete(Type.Setting);
    }
}
