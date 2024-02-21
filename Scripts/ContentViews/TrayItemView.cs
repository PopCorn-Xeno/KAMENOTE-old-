using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;

namespace Kamenote.ContentViews
{
    public class TrayItemView : MonoBehaviour, IContentView
    {
        #region フィールド

        [SerializeField] private string itemName = "";

        [SerializeField] private Sprite icon;

        [SerializeField] private Button button;

        [SerializeField] private TextMeshProUGUI title;

        [SerializeField] private Image image;

        [SerializeField] private bool onValidate;

        #endregion

	    #region MonoBehaviourメソッド

        void Awake() => Initialize();

        void OnValidate()
        {
            if (onValidate) Initialize();
        }

	    #endregion

        #region メソッド

        public void Initialize()
        {
            if (title != null) title.text = itemName;
            if (image != null) image.sprite = icon;
        }

        public void AddListener(params UnityAction[] actions)
        {
            actions?.ToList()?.ForEach(action => button.onClick.AddListener(action));
        }

        #endregion
    }
}
