using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using DG.Tweening;

namespace Kamenote
{
    /// <summary>
    /// ウィンドウ
    /// </summary>
    public class Window : MonoBehaviour
    {
        #region フィールド

        /// <summary>
        /// ウィンドウを管理するキャンバスグループ。
        /// </summary>
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary>
        /// ウィンドウ内に表示するコンテンツ。
        /// </summary>
        [SerializeField] private RectTransform content;

        /// <summary>
        /// ウィンドウ名を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI windowNameText;

        /// <summary>
        /// ウィンドウを閉じるボタン。
        /// </summary>
        [SerializeField] private Button closeButton;

        /// <summary>
        /// ウィンドウ内で1つ前のコンテンツに戻るボタン。
        /// </summary>
        [SerializeField] private Button backButton;

        /// <summary>
        /// アテンションウィンドウ。
        /// </summary>
        public Attention attention;

        /// <summary>
        /// ウィンドウのトランスフォーム。
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// デフォルトのウィンドウ位置。
        /// </summary>
        private static readonly Vector2 position = new(-20, 0);

        /// <summary>
        /// デフォルトのウィンドウサイズ。
        /// </summary>
        private static readonly Vector2 size = new(960, 480);

        #endregion

        #region クラス

        /// <summary>
        /// ウィンドウの実体を保持する。
        /// </summary>
        /// <typeparam name="T">ウィンドウコンテンツのコンポーネントの種類</typeparam>
        public class Substance<T> where T : WindowContent<T>
        {
            /// <summary>
            /// ウィンドウ本体。
            /// </summary>
            public Window window;

            /// <summary>
            /// ウィンドウコンテンツのコンポーネント。
            /// </summary>
            public T component;

            public Substance(Window window, T component)
            {
                this.window = window;
                this.component = component;
            }
        }

        [Serializable]
        public class Attention
        {
            [SerializeField] private CanvasGroup window;

            [SerializeField] private TextMeshProUGUI message;

            [SerializeField] private Image condition;

            [SerializeField] private GameObject buttonsParent;

            [SerializeField] private Button enter;

            [SerializeField] private Button cancel;

            [SerializeField] private Sprite completed;

            [SerializeField] private Sprite warning;

            private bool selectable;

            /// <summary>
            /// アテンションウィンドウを初期化する。
            /// </summary>
            /// <param name="message">表示するメッセージ</param>
            /// <param name="isNormal">正常なら<c>true</c>、エラーなら<c>false</c></param>
            /// <returns></returns>
            public Attention Initialize(string message, bool isNormal, bool selectable = false)
            {
                window.interactable = selectable;
                window.blocksRaycasts = selectable;

                this.message.text = message;
                condition.sprite = isNormal ? completed : warning;
                
                this.selectable = selectable;
                buttonsParent.SetActive(selectable);

                return this;
            }

            /// <summary>
            /// 選択肢ボタンを設定する。
            /// </summary>
            /// <param name="onEnter">決定ボタンが押されたときに発火するイベント</param>
            /// <returns></returns>
            public Attention SetSelect(params Action[] onEnter)
            {
                // 選択肢の使用ができるとき
                if (selectable)
                {
                    enter.onClick.RemoveAllListeners();
                    cancel.onClick.RemoveAllListeners();

                    enter.onClick.AddListener(() => onEnter.ToList().ForEach(e => e.Invoke()));
                    enter.onClick.AddListener(() => Close(1.5f));
                    cancel.onClick.AddListener(() => Close(0.5f));
                }

                return this;
            }

            /// <summary>
            /// アテンションウィンドウを3秒間表示する。
            /// </summary>
            public void Open()
            {
                window.alpha = 0;
                window.DOFade(1f, 1.5f).SetLoops(2, LoopType.Yoyo);
            }

            /// <summary>
            /// 指定秒数間フェードインしながらアテンションウィンドウを表示する。
            /// </summary>
            /// <param name="duration">フェードインする時間</param>
            public void Open(float duration)
            {
                window.alpha = 0;
                window.DOFade(1f, duration);
            }

            /// <summary>
            /// 指定秒数間フェードアウトしながらアテンションウィンドウを表示する。
            /// </summary>
            /// <param name="duration">フェードアウトする時間</param>
            private void Close(float duration)
            {
                window.interactable = false;
                window.blocksRaycasts = false;
                window.DOFade(0f, duration);
            }
        }

        #endregion

        #region MonoBehaviourメソッド

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            closeButton.onClick.AddListener(Close);
            backButton.interactable = false;
            backButton.onClick.AddListener(MovePreviousContent);
            backButton.onClick.AddListener(() => backButton.interactable = content.childCount != 1);
            Initialize("");
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ウィンドウを初期化する。
        /// </summary>
        /// <param name="windowName">ウィンドウ名</param>
        /// <param name="position">ウィンドウ表示位置</param>
        /// <param name="size">ウィンドウサイズ</param>
        /// <returns>ウィンドウ本体</returns>
        public Window Initialize(string windowName, Vector2 position = default, Vector2 size = default)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            windowNameText.text = windowName;
            backButton.interactable = content.childCount > 1;

            rectTransform.anchoredPosition = position != default ? position : Window.position;
            rectTransform.sizeDelta = size != default ? size : Window.size;

            return this;
        }

        /// <summary>
        /// ウィンドウにコンテンツを設定する。
        /// </summary>
        /// <remarks>
        /// 戻り値からフィールドにアクセスすると参照先がプレハブになるためしないこと。<br/>
        /// メソッドを使う場合だと、その中で使用するフィールドは生成先を参照する。
        /// </remarks>
        /// <typeparam name="T">ウィンドウコンテンツの種類</typeparam>
        /// <param name="content">コンテンツとして生成するゲームオブジェクト</param>
        /// <returns>ウィンドウの実体</returns>
        public Substance<T> SetContent<T>(GameObject content) where T : WindowContent<T>
        {
            // 既に同じコンテンツが設定されていた場合は、古い方を消す
            var contents = Enumerable.Range(0, this.content.childCount)
                                  .Select(i => this.content.GetChild(i).GetComponent<T>())
                                  .Where(component => component != null);

            if (contents.Count() > 0)
                contents.ToList().ForEach(content => Destroy(content.gameObject));

            return new(this, Instantiate(content, this.content).GetComponent<T>());
        }

        /// <summary>
        /// コンテンツを1つ取得する。
        /// </summary>
        /// /// <remarks>
        /// 戻り値からフィールドにアクセスすると参照先がプレハブになるためしないこと。<br/>
        /// メソッドを使う場合だと、その中で使用するフィールドは生成先を参照する。
        /// </remarks>
        /// <typeparam name="T">コンテンツの種類</typeparam>
        /// <returns>ウィンドウの実体</returns>
        public Substance<T> GetContent<T>() where T : WindowContent<T>
        {
            var component = Enumerable.Range(0, content.childCount)
                              .Select(i => content.GetChild(i).GetComponent<T>())
                              .Where(component => component != null).ElementAt(0);
             return new(this, component);
        }

        /// <summary>
        /// ウィンドウを開く。
        /// </summary>
        public void Open()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1, 0.5f);

            if (content.childCount > 1)
            {
                backButton.interactable = true;
                Enumerable.Range(0, content.childCount - 1).ToList().ForEach(i => content.GetChild(i).gameObject.SetActive(false));
            }
                
        }

        /// <summary>
        /// ウィンドウを閉じる。
        /// </summary>
        public void Close()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0, 0.5f);

            // ウィンドウのコンテンツをすべて削除する
            Enumerable.Range(0, content.childCount).ToList().ForEach(i => Destroy(content.GetChild(i).gameObject));
        }

        /// <summary>
        /// 現在表示中のコンテンツを削除し、前に表示していたコンテンツを再表示する。
        /// </summary>
        public void MovePreviousContent()
        {
            if (content.childCount > 1)
            {
                int target = content.childCount - 1;
                Destroy(content.GetChild(target).gameObject);
                content.GetChild(target - 1).gameObject.SetActive(true);
            }
        }

        #endregion
    }
}
