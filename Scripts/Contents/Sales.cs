using System;
using Kamenote.ContentViews;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamenote.Contents
{
    public class Sales : Content
    {
        #region フィールド

        [SerializeField] private SimpleValueView soldCount;

        [SerializeField] private Button manageDay;

        [SerializeField] private TextMeshProUGUI dayCondition;

        [SerializeField] private Button showSales;

        #endregion

        #region MonoBehaviourメソッド

        void Start()
        {
            Initialize();
            SetActive(true);
        }

        void OnValidate()
        {
            if (onValidate) Initialize();
        }

        #endregion

        #region メソッド

        public override void Initialize()
        {
            base.Initialize();
            soldCount.Initialize("注文回数", Manager.setting.currentCustomerCount, "組");
            showSales.onClick.AddListener(Manager.window.Initialize("売上集計", size: new(960, 700)).Open);
            manageDay.onClick.AddListener(ManageDay);

            if (Manager.shop.day2Started && Manager.shop.day2Finished)
            {
                dayCondition.text = "全日程終了";
            }
            else if (Manager.shop.day2Started && Manager.shop.day1Finished)
            {
                dayCondition.text = "2日目を終了";

            }
            else if (Manager.shop.day1Finished && Manager.shop.day1Started)
            {
                dayCondition.text = "2日目を開始";
            }
            else if (!Manager.shop.day1Finished && Manager.shop.day1Started)
            {
                dayCondition.text = "1日目を終了";
            }
            else
            {
                dayCondition.text = "1日目を開始";
            }
        }

        public override void UpdateValue()
        {
            Manager.setting.currentCustomerCount = soldCount.UpdateValue(Manager.setting.currentCustomerCount + 1);
            Manager.sales[Manager.setting.currentDay - 1].customerCount = Manager.setting.currentCustomerCount;
        }

        private void ManageDay()
        {
            string message = null;
            Action<bool> callback = null;

            if (Manager.shop.day1Finished && Manager.shop.day2Finished && Manager.shop.day1Started && Manager.shop.day2Started)
            {
                manageDay.interactable = false;
                Manager.window.attention.Initialize("全日程が終了しました。お疲れさまでした。", true).Open();
                return;
            }
            else
            {
                if (Manager.shop.day2Started && !Manager.shop.day2Finished)
                {
                    message = "全日程終了";
                    callback += flag => Manager.shop.day2Finished = flag;
                    callback += flag => Manager.sales[1].customerCount = Manager.setting.currentCustomerCount;
                }
                else if (!Manager.shop.day2Started && Manager.shop.day1Finished)
                {
                    message = "2日目を終了";
                    callback += flag => Manager.shop.day2Started = flag;
                    callback += flag => Manager.setting.currentDay = 2;
                    callback += flag => Manager.sales[0].customerCount = Manager.setting.currentCustomerCount;
                    callback += flag => Manager.setting.currentCustomerCount = 0;
                    callback += flag => soldCount.Value = 0;
                }
                else if (!Manager.shop.day1Finished && Manager.shop.day1Started)
                {
                    message = "2日目を開始";
                    callback = flag => Manager.shop.day1Finished = flag;
                }
                else
                {
                    message = "1日目を終了";
                    callback += flag => Manager.shop.day1Started = flag;
                    callback += Manager.contents.SetActives;
                }
            }

            Manager.window.attention.Initialize($"{dayCondition.text}しますか？", true, true)
                                    .SetSelect
                                    (
                                        () => dayCondition.text = message,
                                        () => callback?.Invoke(true),
                                        Manager.other.information.UpdateValue,
                                        SetTutorial
                                    )
                                    .Open(0.5f);

        }

        private void SetTutorial()
        {
            if (!Manager.shop.day1Started)
                Manager.other.SetTutorial(true, ApplicationManager.Other.PREPARE);
            else if ((!Manager.shop.day2Started && Manager.shop.day1Finished) || Manager.shop.day2Finished)
                Manager.other.SetTutorial(true, ApplicationManager.Other.FINISH);
            else
                Manager.other.SetTutorial(false);
        }

        #endregion
    }
}
