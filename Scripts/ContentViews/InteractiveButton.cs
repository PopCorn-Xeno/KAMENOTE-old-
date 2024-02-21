using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kamenote.ContentViews
{
    /// <summary>
    /// 開閉状態の識別を行って他スクリプトからのイベントを実行するボタン。
    /// </summary>
    [RequireComponent(typeof(Button), typeof(Image))]
    public class InteractiveButton : MonoBehaviour, IContentView
    {
        #region フィールド

        /// <summary>
        /// ボタンが閉じているときのアイコン。
        /// </summary>
        [SerializeField] private Sprite closingIcon;

        /// <summary>
        /// ボタンが開いているときのアイコン。
        /// </summary>
        [SerializeField] private Sprite openingIcon;

        /// <summary>
        /// ボタン。
        /// </summary>
        [SerializeField] private Button button;

        /// <summary>
        /// ボタンに表示するイメージ。
        /// </summary>
        [SerializeField] private Image icon;

        /// <summary>
        /// 「開く」ときに発火するイベント。
        /// </summary>
        public event Action OnOpened;

        /// <summary>
        /// 「閉じる」ときに発火するイベント。
        /// </summary>
        public event Action OnClosed;

        /// <summary>
        /// 開いたときだけ表示する場合。
        /// </summary>
        [SerializeField] private bool onlyOpening;

        #endregion

        #region プロパティ

        /// <summary>
        /// 現在開かれているか。
        /// </summary>
        public bool IsOpen { get; set; } = false;

        #endregion

        #region MonoBehaviourメソッド

        void Awake() => Initialize();

        #endregion

        #region メソッド

        public void Initialize()
        {
            if (!onlyOpening) icon.sprite = closingIcon;
            else
            {
                icon.color = new(0, 0, 0, 0);
                icon.sprite = openingIcon;
            }

            button.onClick.AddListener(Interact);
        }

        /// <summary>
        /// ボタンがインタラクトされたとき行う。
        /// </summary>
        private void Interact()
        {
            // 開いていないとき
            if (!IsOpen) Open();
            // 開いているとき
            else Close();
        }

        public void Open()
        {
            OnOpened?.Invoke();
            icon.sprite = openingIcon;
            icon.color = new Color32(245, 245, 245, 255);
            IsOpen = true;
        }

        public void Close()
        {
            OnClosed?.Invoke();
            if (!onlyOpening)
            {
                icon.sprite = closingIcon;
                icon.color = new Color32(100, 100, 100, 255);
            }
            else icon.color = new(0, 0, 0, 0);

            IsOpen = false;
        }

        /// <summary>
        /// ボタンを有効化し、可視状態にする。
        /// </summary>
        public void Enable()
        {
            icon.color = new(icon.color.r, icon.color.g, icon.color.b, 1);
            button.interactable = true;
        }

        /// <summary>
        /// ボタンを無効化し、隠す。
        /// </summary>
        public void Disable()
        {
            icon.color = new(icon.color.r, icon.color.g, icon.color.b, 0);
            button.interactable = false;
        }

        #endregion
    }
}
