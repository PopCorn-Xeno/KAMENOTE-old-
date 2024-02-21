using System.Collections.Generic;
using System.Linq;

namespace Kamenote.Save
{
    [System.Serializable]
    public class ItemDatas : SaveData<ItemDatas>
    {
        public ItemData[] datas;

        public ItemDatas Save(List<ItemData> datas)
        {
            Instance.datas = datas?.ToArray();
            Save(Type.ItemDatas);
            return Instance;
        }

        public ItemDatas Load(out List<ItemData> datas)
        {
            Load(Type.ItemDatas);
            datas = Instance.datas?.ToList() ?? new();
            return Instance;
        }

        public void Delete() => Delete(Type.ItemDatas);
    }
}
