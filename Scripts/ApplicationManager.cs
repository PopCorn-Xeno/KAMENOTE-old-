using Kamenote.Contents;
using Kamenote.Menu;
using Kamenote.Save;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kamenote
{
    /// <summary>
    /// アプリケーション全体を管理する。
    /// </summary>
    /// <remarks>
    /// このクラスへのアクセスには<see cref="Instance"/>を使用する
    /// </remarks>
    public class ApplicationManager : MonoBehaviour
    {
        #region フィールド

        /// <summary>
        /// 注文履歴を保持する。
        /// </summary>
        /// <remarks>
        /// <b>（セーブ・ロード可能）</b>
        /// </remarks>
        public List<OrderArchive> orderArchives;

        /// <summary>
        /// 登録された商品情報を保持する。
        /// </summary>
        /// <remarks>
        /// <b>（セーブ・ロード可能）</b>
        /// </remarks>
        public List<ItemData> itemDatas;

        /// <summary>
        /// アプリ設定を保持する。
        /// </summary>
        /// <remarks>
        /// <b>（セーブ・ロード可能）</b>
        /// </remarks>
        public SettingData setting;

        /// <summary>
        /// 模擬店設定を保持する。
        /// </summary>
        /// <remarks>
        /// <b>（セーブ・ロード可能）</b>
        /// </remarks>
        public ShopData shop;

        /// <summary>
        /// 売上記録を保持する。
        /// </summary>
        /// <remarks>
        /// <b>（セーブ・ロード可能）</b>
        /// </remarks>
        public SalesData[] sales = new SalesData[2];

        /// <summary>
        /// コンテンツを格納し、画面に表示するウィンドウ。
        /// </summary>
        public Window window;

        /// <summary>
        /// メイン画面に表示するコンテンツ。
        /// </summary>
        public Contents contents;

        /// <summary>
        /// メニューウィンドウ。
        /// </summary>
        public Menu menu;

        /// <summary>
        /// ヘッダー等その他の要素。
        /// </summary>
        public Other other;

        #endregion

	    #region プロパティ

        /// <summary>
        /// このクラスの静的インスタンス。フィールドやメソッドへアクセスする。
        /// </summary>
        /// <remarks>
        /// <b>(DontDestroyOnLoad)</b>
        /// </remarks>
        public static ApplicationManager Instance { get; private set; }

        /// <summary>
        /// 現在時刻。（HH:mm）
        /// </summary>
        public string Time => DateTime.Now.ToString("HH:mm");

	    #endregion

        #region クラス

        /// <summary>
        /// メニュー画面に表示する、それぞれ機能を持った複数のコンテンツ。
        /// </summary>
        [Serializable]
        public class Contents
        {
            /// <summary>
            /// 売上管理
            /// </summary>
            public Sales sales;

            /// <summary>
            /// 整理券番号の待機確認
            /// </summary>
            public Reservation reservation;

            /// <summary>
            /// 注文管理
            /// </summary>
            public Order order;

            /// <summary>
            /// 注文受け渡し管理
            /// </summary>
            public Receive receive;

            /// <summary>
            /// 直近の注文ビューア
            /// </summary>
            public History history;

            /// <summary>
            /// <see cref="sales"/>以外のコンテンツのアクティブ状態を設定する。
            /// </summary>
            /// <param name="isActive">アクティブか</param>
            public void SetActives(bool isActive)
            {
                reservation.SetActive(isActive);
                order.SetActive(isActive);
                receive.SetActive(isActive);
                history.SetActive(isActive);
            }
        }

        /// <summary>
        /// メニューウィンドウ。
        /// </summary>
        [Serializable]
        public class Menu
        {
            /// <summary>
            /// 注文履歴一覧。
            /// </summary>
            public OrderArchiveManager orderArchive;
        }

        /// <summary>
        /// その他の情報を表示・管理する要素。
        /// </summary>
        [Serializable]
        public class Other
        {
            /// <summary>
            /// 現在の模擬店・日程情報を表示する。
            /// </summary>
            public Information information;

            /// <summary>
            /// 現在時刻を表示する。
            /// </summary>
            [SerializeField] private TMPro.TextMeshProUGUI timeView;

            /// <summary>
            /// チュートリアルのテキストを表示する。
            /// </summary>
            [SerializeField] private TMPro.TextMeshProUGUI tutorial;

            public const string PREPARE = "営業を開始する際は、商品情報等を入力してから「売り上げ」の「1日目を開始」ボタンをタップしてください。";
            
            public const string FINISH = "お疲れさまでした。メニュー「売上集計」から利益を確認できます。";

            /// <summary>
            /// 毎フレームで時間を更新する。
            /// </summary>
            public IEnumerator UpdateTime()
            {
                while (true)
                {
                    timeView.text = DateTime.Now.ToString("H:mm");
                    yield return null;
                }
            }

            /// <summary>
            /// チュートリアルテキスト（<see cref="tutorial"/>）のアクティブ状態、表示テキストを設定する。
            /// </summary>
            /// <param name="isActive">アクティブか</param>
            /// <param name="message">表示テキスト</param>
            public void SetTutorial(bool isActive, string message = null)
            {
                tutorial.gameObject.SetActive(isActive);
                tutorial.text = message;
            }
        }

        #endregion

	    #region MonoBehaviourメソッド

        void Awake()
        {
            // どこからでもアクセス可能なゲームオブジェクトにする
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
            
            // セーブデータをロードする。
            Instance.Load();
        }

        void Start()
        {
            // コルーチンの始動
            StartCoroutine(Instance.other.UpdateTime());
            StartCoroutine(Instance.AutoSave(3));

            // チュートリアルテキストの初期化
            if (!Instance.shop.day1Started)
                Instance.other.SetTutorial(true, Other.PREPARE);
            else if (Instance.shop.day1Finished || Instance.shop.day2Finished)
                Instance.other.SetTutorial(true, Other.FINISH);
            else
                Instance.other.SetTutorial(false);
        }

        void Update()
        {
            // Androidで戻るキー判定にならない
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Instance.window.attention.Initialize("アプリを終了しますか？\nデータは自動保存されます。", false, true)
                                         .SetSelect
                                         (
                                            Instance.Save,
                                            Application.Quit
                                         )
                                         .Open(0.5f);
            }
        }
        
	    #endregion

        #region メソッド

        /// <summary>
        /// 指定された整理券番号に該当する直近の注文履歴を取得する。
        /// <b>※何度か整理券番号のカウントがリセットされていた場合、今のリセット回数で注文されていた履歴を取得する。</b>
        /// </summary>
        /// <param name="reservation">指定する整理券番号</param>
        /// <returns>該当する注文履歴</returns>
        public List<OrderArchive> GetRecentOrders(int reservation)
        {
            return Instance.orderArchives.Where(archive => archive.reservation == reservation && archive.period == setting.currentReset)
                                .ToList();
        }
        
        /// <summary>
        /// 指定された整理券番号とリセット回数に該当する注文履歴を取得する。
        /// </summary>
        /// <param name="reservation">指定した整理券番号</param>
        /// <param name="resetCount">指定したリセット回数</param>
        /// <returns>該当する注文履歴</returns>
        public List<OrderArchive> GetRecentOrders(int reservation, int resetCount)
        {
            return Instance.orderArchives.Where(archive => archive.reservation == reservation && archive.period == resetCount)
                                .ToList();
        }

        /// <summary>
        /// 指定した日程での完了した注文を全取得する。
        /// </summary>
        /// <param name="day">指定した日程</param>
        /// <returns>該当する注文履歴</returns>
        public List<OrderArchive> GetOrdersDay(int day)
            => Instance.orderArchives.Where(archive => archive.day == day && archive.isReceived).ToList();

        /// <summary>
        /// データを全てセーブする。
        /// </summary>
        public void Save()
        {
            Settings.Instance.Save(Instance.setting);
            Shop.Instance.Save(Instance.shop);
            ItemDatas.Instance.Save(Instance.itemDatas);
            OrderArchives.Instance.Save(Instance.orderArchives);
        }

        /// <summary>
        /// データを全てロードする。
        /// </summary>
        public void Load()
        {
            Settings.Instance.Load(out Instance.setting);
            ItemDatas.Instance.Load(out Instance.itemDatas);
            OrderArchives.Instance.Load(out Instance.orderArchives);
            Shop.Instance.Load(out Instance.shop);
        }

        /// <summary>
        /// 指定した分ごとにオートセーブを実行する。
        /// </summary>
        /// <param name="minute">指定した分</param>
        private IEnumerator AutoSave(float minute)
        {
            float seconds = minute * 60;
            while (true)
            {
                yield return new WaitForSecondsRealtime(seconds);
                Instance.Save();
            }
        }

        #endregion
    }
}
