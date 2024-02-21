namespace Kamenote.Save
{
    public class Shop : SaveData<Shop>
    {
        public ShopData data;

        public Shop Save(ShopData data)
        {
            Instance.data = data;
            Save(Type.Store);
            return Instance;
        }

        public Shop Load(out ShopData data)
        {
            Load(Type.Store);
            data = Instance.data ?? new(null, null, 1, null);
            return Instance;
        }

        public void Delete() => Delete(Type.Store);
    }
}
