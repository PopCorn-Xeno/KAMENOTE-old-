using System.Collections.Generic;
using System.Linq;

namespace Kamenote.Save
{
    [System.Serializable]
    public class OrderArchives : SaveData<OrderArchives>
    {
        public OrderArchive[] datas;

        public OrderArchives Save(List<OrderArchive> datas)
        {
            Instance.datas = datas?.ToArray();
            Save(Type.OrderArchives);
            return Instance;
        }

        public OrderArchives Load(out List<OrderArchive> datas)
        {
            Load(Type.OrderArchives);
            datas = Instance.datas?.ToList() ?? new();
            return Instance;
        }

        public void Delete() => Delete(Type.OrderArchives);
    }
}
