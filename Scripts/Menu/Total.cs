using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Kamenote.Menu
{
    public class Total : WindowContent<Total>
    {
        #region フィールド

        [SerializeField] private TMP_Dropdown dropdown;

        [SerializeField] private Image conditionImage;

        [SerializeField] private TextMeshProUGUI conditionText;

        [SerializeField] private TextMeshProUGUI customerCount;

        [SerializeField] private TextMeshProUGUI sales;

        [SerializeField] private ScrollValueView archiveView;

        [SerializeField] private GameObject archiveViewMiniPrefab;

        [SerializeField] private Sprite completed;

        [SerializeField] private Sprite uncompleted;

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("売上集計", size: new(960, 700)).SetContent<Total>(gameObject);
            substance.component.Initialize();
            substance.component.OnValueChanged();
            substance.window.Open();
        }

        public override void Initialize()
        {
            int result = GetTotal();
            Manager.sales[dropdown.value].sales = result;

            SetCondition();
            customerCount.text = Manager.sales[dropdown.value].customerCount.ToString();
            sales.text = $"￥{result}";
        }

        private void OnValueChanged()
            => dropdown.onValueChanged.AddListener(value => Initialize());
        

        private int GetTotal()
        {
            var result = Manager.GetOrdersDay(GetDay());

            archiveView.Clear();

            result.ForEach
                   (
                    archive => 
                        archiveView.Add<ArchiveView>(archiveViewMiniPrefab, $"{archive.name}-Day{archive.day}")
                                   .Initialize(false, false, false, false, true, false, false, false)
                                   .Information
                                   .SetText(archive)
                   );

            return result.Select(archive => archive.sales).Sum();
        }

        private int GetDay()
            => dropdown.value switch { 0 => 1, 1 => 2, _ => -1 };

        private void SetCondition()
        {
            if ((GetDay() == 1 && Manager.shop.day1Finished) || (GetDay() == 2 && Manager.shop.day2Finished))
            {
                conditionImage.sprite = completed;
                conditionText.text = "営業終了";
            }
            else if ((GetDay() == 1 && Manager.shop.day1Started) || (GetDay() == 2 && Manager.shop.day2Started))
            {
                conditionImage.sprite = uncompleted;
                conditionText.text = "営業中";
            }
            else if ((GetDay() == 1 && !Manager.shop.day1Started) || (GetDay() == 2 && !Manager.shop.day2Started))
            {
                conditionImage.sprite = uncompleted;
                conditionText.text = "準備中";
            }
        }

        #endregion
    }
}
