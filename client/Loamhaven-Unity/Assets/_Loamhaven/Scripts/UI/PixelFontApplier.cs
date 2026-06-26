using UnityEngine;
using TMPro;

namespace Loamhaven.UI
{
    public class PixelFontApplier : MonoBehaviour
    {
        [Tooltip("Load and apply Resources/Fonts/m5x7 TMP asset to all text.")]
        [SerializeField] bool _applyCustomFont;

        [Tooltip("Override font size (0 = keep per-component size).")]
        [SerializeField] int _fontSizeOverride;

        TMP_FontAsset _font;

        void Awake()
        {
            if (_applyCustomFont)
                _font = Resources.Load<TMP_FontAsset>("Fonts/m5x7");

            ApplyAll();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Application.isPlaying) ApplyAll();
        }
#endif

        void ApplyAll()
        {
            foreach (var t in GetComponentsInChildren<TMP_Text>(includeInactive: true))
                Apply(t);
        }

        void Apply(TMP_Text t)
        {
            if (_font != null) t.font = _font;
            if (_fontSizeOverride > 0) t.fontSize = _fontSizeOverride;

            t.enableAutoSizing = false;

            if (t.font?.atlasTexture != null)
                t.font.atlasTexture.filterMode = FilterMode.Point;
        }
    }
}