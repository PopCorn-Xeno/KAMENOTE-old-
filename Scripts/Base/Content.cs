using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kamenote
{
    /// <summary>
    /// メインウィンドウに表示するコンテンツの抽象基底クラス。
    /// </summary>
    public abstract class Content : MonoBehaviour
    {
        #region フィールド

        /// <summary>
        /// <c>OnValidate</c>メソッドを実行するか。
        /// </summary>
        [SerializeField] protected bool onValidate = false;

        /// <summary>
        /// コンテンツのタイトル。
        /// </summary>
        [SerializeField] protected string titleText;

        /// <summary>
        /// コンテンツのアイコン。
        /// </summary>
        [SerializeField] protected Sprite iconSprite;

        /// <summary>
        /// コンテンツのタイトルを表示するテキスト。
        /// </summary>
        [SerializeField] protected TextMeshProUGUI title;

        /// <summary>
        /// コンテンツのアイコンを表示するイメージ。
        /// </summary>
        [SerializeField] protected Image icon;

        /// <summary>
        /// コンテンツの可視状態・インタラクトの可否に使う。
        /// </summary>
        [SerializeField] protected CanvasGroup canvasGroup;

        #endregion

	    #region プロパティ

        /// <summary>
        /// <see cref="ApplicationManager.Instance"/>のショートカット。 
        /// </summary>
        public static ApplicationManager Manager => ApplicationManager.Instance;

	    #endregion

        #region メソッド

        /// <summary>
        /// 初期化する。
        /// </summary>
        public virtual void Initialize()
        {
            title.text = titleText;
            icon.sprite = iconSprite;
        }

        /// <summary>
        /// コンテンツの値を更新する。
        /// </summary>
        public abstract void UpdateValue();

        /// <summary>
        /// コンテンツのアクティブ状態を設定する。
        /// </summary>
        /// <param name="isActive">アクティブか</param>
        public virtual void SetActive(bool isActive)
        {
            switch (isActive)
            {
                case true:
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    break;

                case false:
                    canvasGroup.alpha = 0.5f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    break;
            }
        }

        #endregion
    }
}
