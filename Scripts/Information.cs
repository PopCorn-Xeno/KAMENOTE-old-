using UnityEngine;
using TMPro;

namespace Kamenote
{
    public class Information : MonoBehaviour
    {
        #region フィールド

        [SerializeField] private TextMeshProUGUI className;

        [SerializeField] private TextMeshProUGUI shopName;

        [SerializeField] private TextMeshProUGUI day;

        #endregion

	    #region MonoBehaviourメソッド

        void Start() => UpdateValue();

	    #endregion

        #region メソッド

        public void UpdateValue()
        {
            if (ApplicationManager.Instance.shop != null)
            {
                if ((ApplicationManager.Instance.shop.className == null || ApplicationManager.Instance.shop.className == "")
                     && (ApplicationManager.Instance.shop.shopName == null || ApplicationManager.Instance.shop.shopName == ""))
                {
                    className.text = "模擬店情報が設定されていません。メニューから設定を行ってください。";
                    shopName.text = "未設定";
                }
                else
                {
                    className.text = ApplicationManager.Instance.shop.className;
                    shopName.text = ApplicationManager.Instance.shop.shopName;
                }
            }

            day.text = ApplicationManager.Instance.setting?.currentDay.ToString();
        }

        #endregion
    }
}
