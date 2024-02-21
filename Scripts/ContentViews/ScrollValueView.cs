using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kamenote.ContentViews
{
    /// <summary>
    /// スクロールビューで複数の値を表示する。
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollValueView : MonoBehaviour, IContentView
    {
        #region フィールド

        /// <summary>
        /// キャプションテキストに表示するテキスト。
        /// </summary>
        [SerializeField] private string captionText;

        /// <summary>
        /// キャプションテキスト。
        /// </summary>
        [SerializeField] private TMPro.TextMeshProUGUI caption;

        /// <summary>
        /// スクロール方向。
        /// </summary>
        [SerializeField] private ScrollDirection scrollDirection = default;

        /// <summary>
        /// スクロール。
        /// </summary>
        [SerializeField] private ScrollRect scrollRect;

        public int contentsLimit = 10000;

        /// <summary>
        /// 表示するアイテム。
        /// </summary>
        [SerializeField] private List<GameObject> contents;

        public int ContentCount => contents.Count;

        [SerializeField] private bool onValidate;

        #endregion

        #region 列挙型

        /// <summary>
        /// スクロール方向を指定する。
        /// </summary>
        private enum ScrollDirection { Horizontal, Vertical }

        #endregion

        #region MonoBehaviourメソッド

        void Awake() => Initialize();

        void OnValidate()
        {
            #if UNITY_EDITOR
            if (onValidate) UnityEditor.EditorApplication.delayCall += Initialize;
            #endif
        }

        #endregion

        #region メソッド

        /// <summary>
        /// スクロールビューを初期化する。
        /// </summary>
        public void Initialize()
        {
            // コンテンツの上限を設定
            contents = new(contentsLimit - 1);

            if (captionText != null) caption.text = captionText;

            // スクロール方向を確認し、ScrollRectに適用する
            scrollRect.horizontal = scrollDirection == ScrollDirection.Horizontal;
            scrollRect.vertical = scrollDirection == ScrollDirection.Vertical;

            // コンテンツ部分の大きさを調整する
            // 現在どのレイアウトグループを持っているか取得する
            HorizontalLayoutGroup horizontal = null;
            VerticalLayoutGroup vertical = null;
            try
            {
                horizontal = scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                vertical = scrollRect.content.GetComponent<VerticalLayoutGroup>();
            }
            catch (MissingReferenceException) { }

            ContentSizeFitter contentSizeFitter = scrollRect.content.GetComponent<ContentSizeFitter>();

            switch (scrollDirection)
            {
                case ScrollDirection.Horizontal:

                    // 水平の場合、垂直を消してコンポーネントを再設定する（エディタ対応）
                    if (vertical != null)
                    {
#if UNITY_EDITOR

                        if (UnityEditor.EditorApplication.isPlaying) { Destroy(vertical); }
                        else { DestroyImmediate(vertical); }

#elif UNITY_STANDALONE || UNITY_ANDROID

                        Destroy(vertical);

#endif
                    }

                    if (horizontal == null) { horizontal = scrollRect.content.gameObject.AddComponent<HorizontalLayoutGroup>(); }

                    // コンテンツサイズを平行に合わせて調整
                    horizontal.childControlHeight = true;
                    horizontal.childControlHeight = false;
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                    break;
                case ScrollDirection.Vertical:

                    // 垂直の場合、水平を消してコンポーネントを再設定する
                    if (horizontal != null)
                    {
#if UNITY_EDITOR

                        if (UnityEditor.EditorApplication.isPlaying) { Destroy(horizontal); }
                        else { DestroyImmediate(horizontal); }

#elif UNITY_STANDALONE || UNITY_ANDROID

                        Destroy(horizontal);

#endif
                    }

                    if (vertical == null) { vertical = scrollRect.content.gameObject.AddComponent<VerticalLayoutGroup>(); }

                    // コンテンツサイズを垂直に合わせて調整
                    vertical.childControlHeight = false;
                    vertical.childControlWidth = true;
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                    break;
            }
        }

        /// <summary>
        /// コンテンツ内の指定されたインデックスのゲームオブジェクトを探し、
        /// それにアタッチされている取得したいコンポーネントを返す。
        /// </summary>
        /// <typeparam name="T">取得したいコンポーネント</typeparam>
        /// <param name="index">インデックス</param>
        /// <returns>コンポーネント</returns>
        public T ContentAt<T>(int index) where T : Component => contents[index].GetComponent<T>();

        /// <summary>
        /// スクロールビューにコンテンツを最後の要素として追加する。<br/>
        /// コンテンツにアタッチされたコンポーネントを1つ取得することもできる。
        /// </summary>
        /// <typeparam name="T">取得したいコンポーネントの種類</typeparam>
        /// <param name="prefab">ゲームオブジェクト（コンポーネントの設定が済んだプレハブが望ましい）</param>
        /// <param name="name">ゲームオブジェクトの名前</param>
        /// <param name="components">追加したいコンポーネント</param>
        /// <returns>取得したいコンポーネント</returns>
        public T Add<T>(GameObject prefab, string name, params System.Type[] components) where T : Component
        {
            var instance = Instantiate<T>(prefab, name, components);

            // コンテンツ数の上限を超えたら一番古いものから消す
            if (contents.Count >= contentsLimit)
            {
                var removed = contents[0];
                contents.Remove(removed);
                Destroy(removed);
            }
            contents.Add(instance);

            // 取得したいコンポーネントを１つ返す
            return instance.GetComponent<T>();
        }

        /// <summary>
        /// スクロールビューにコンテンツを先頭の要素として追加する。<br/>
        /// コンテンツにアタッチされたコンポーネントを1つ取得することもできる。
        /// </summary>
        /// <typeparam name="T">取得したいコンポーネントの種類</typeparam>
        /// <param name="prefab">ゲームオブジェクト（コンポーネントの設定が済んだプレハブが望ましい）</param>
        /// <param name="name">ゲームオブジェクトの名前</param>
        /// <param name="components">追加したいコンポーネント</param>
        /// <returns>取得したいコンポーネント</returns>
        public T Insert<T>(GameObject prefab, string name, params System.Type[] components) where T : Component
        {
            // 生成した後、ヒエラルキーの順番を変える
            var instance = Instantiate<T>(prefab, name, components);
            instance.transform.SetSiblingIndex(contents.Count - 1);

            // コンテンツ数の上限を超えたら一番新しい（一番後ろの）ものをnullにする
            if (contents.Count >= contentsLimit)
            {
                var removed = contents[^1];
                contents[^1] = null;
                Destroy(removed);
                
                // 最後以外の要素を1つ後ろに移し替える
                Enumerable.Range(1, contentsLimit - 1).Select(i => contents[^i] = contents[^(i + 1)]);
                // 空にした0番目を置き換える
                contents[0] = instance;
            }
            // 要素数が上限に達していなければ、そのまま挿入する
            else contents.Insert(0, instance);           

            return instance.GetComponent<T>();
        }

        /// <summary>
        /// ゲームオブジェクトをコンテンツとして生成する。
        /// </summary>
        /// <typeparam name="T">任意のコンポーネント</typeparam>
        /// <param name="prefab">生成するゲームオブジェクト（プレハブ）</param>
        /// <param name="name">ゲームオブジェクトの名前</param>
        /// <param name="components">追加するコンポーネント</param>
        /// <returns>生成したゲームオブジェクト（コンテンツ）</returns>
        private GameObject Instantiate<T>(GameObject prefab, string name, params System.Type[] components) where T : Component
        {
            // RectTransformがない場合、追加する
            if (prefab.GetComponent<RectTransform>() == null) prefab.AddComponent<RectTransform>();

            // 追加したいコンポーネントがある場合、全てアタッチする
            if (components != null)
            {
                var additions = components.Select(component => prefab.AddComponent(component)).ToArray();
            }

            // オブジェクトを生成する
            prefab = Instantiate(prefab, scrollRect.content);
            prefab.name = name;

            return prefab;
        }

        /// <summary>
        /// コンテンツ内のある名前のゲームオブジェクトを削除する。
        /// </summary>
        /// <param name="name">削除したいコンテンツの名前</param>
        public void Remove(string name)
        {
            GameObject removed = scrollRect.content.Find(name).gameObject;
            contents.Remove(removed);
            if (removed != null) Destroy(removed);
        }

        /// <summary>
        /// コンテンツを全て削除する。
        /// </summary>
        public void Clear()
        {
            contents.Clear();
            Enumerable.Range(0, scrollRect.content.childCount).ToList().ForEach(i => Destroy(scrollRect.content.GetChild(i).gameObject));
        }

        #endregion
    }
}
