using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kamenote
{
    public class ContentsBlock : MonoBehaviour
    {
        [SerializeField] private string title = "タイトルを入力";

        [SerializeField] private Sprite icon;

        [SerializeField] private TextMeshProUGUI titleText;

        [SerializeField] private Image iconImage;

        [SerializeField] private CanvasGroup canvasGroup;
    }
}
