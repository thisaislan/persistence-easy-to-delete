using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Styles
{
    internal static class PedInspectorStyles
    {
        private const float DefaultControlHeight = 24f;
        private const float SectionSpaceHeight = 8f;
        private const char IconCacheSeparator = '|';
        private const int SolidTextureSize = 1;
        private const float LineHeight = 1f;

        private static readonly Dictionary<Color, GUIStyle> CachedCardStyles = new Dictionary<Color, GUIStyle>();
        private static readonly Dictionary<Color, GUIStyle> CachedRowStyles = new Dictionary<Color, GUIStyle>();
        private static readonly Dictionary<Color, GUIStyle> CachedLabelStyles = new Dictionary<Color, GUIStyle>();
        private static readonly Dictionary<Color, GUIStyle> CachedDataSectionStyles = new Dictionary<Color, GUIStyle>();
        private static readonly Dictionary<Color, Texture2D> CachedSolidTextures = new Dictionary<Color, Texture2D>();
        private static readonly Dictionary<string, GUIContent> CachedIconContents = new Dictionary<string, GUIContent>();
        private static GUIStyle cachedValueAreaStyle;
        private static GUIStyle cachedBoldFoldoutStyle;
        private static GUIStyle cachedTextFieldStyle;

        internal enum ButtonColorStyle
        {
            Neutral,
            Calm,
            Growth,
            Alert,
            Urgent,
            Quiet
        }

        internal static readonly Color RowBackgroundColorA = new Color(0.19f, 0.19f, 0.19f);
        internal static readonly Color RowBackgroundColorB = new Color(0.22f, 0.22f, 0.22f);
        internal static readonly Color SectionBackgroundColor = new Color(0.17f, 0.17f, 0.17f);
        internal static readonly Color LineColor = new Color(0.28f, 0.28f, 0.28f);
        internal static readonly Color RowIndexLabelColor = new Color(0.45f, 0.45f, 0.45f);
        internal static readonly Color EmptyListLabelColor = new Color(0.5f, 0.5f, 0.5f);

        internal static readonly Color CalmButtonColor = new Color(0.60f, 0.66f, 0.78f);
        internal static readonly Color GrowthButtonColor = new Color(0.60f, 0.73f, 0.60f);
        internal static readonly Color AlertButtonColor = new Color(0.80f, 0.73f, 0.60f);
        internal static readonly Color UrgentButtonColor = new Color(0.80f, 0.60f, 0.60f);
        internal static readonly Color QuietButtonColor = new Color(0.66f, 0.71f, 0.78f);
        internal static readonly Color NeutralButtonColor = new Color(0.75f, 0.75f, 0.75f);

        internal static GUIStyle ValueAreaStyle
        {
            get
            {
                if (cachedValueAreaStyle == null)
                {
                    cachedValueAreaStyle = new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true
                    };
                }

                return cachedValueAreaStyle;
            }
        }

        internal static GUIStyle BoldFoldoutStyle
        {
            get
            {
                if (cachedBoldFoldoutStyle == null)
                {
                    cachedBoldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                    {
                        fontStyle = EditorStyles.boldLabel.fontStyle
                    };
                }

                return cachedBoldFoldoutStyle;
            }
        }

        internal static GUIStyle TextFieldStyle
        {
            get
            {
                if (cachedTextFieldStyle == null)
                {
                    cachedTextFieldStyle = new GUIStyle(EditorStyles.textField)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }

                return cachedTextFieldStyle;
            }
        }

        internal static Color GetButtonColor(ButtonColorStyle style)
        {
            switch (style)
            {
                case ButtonColorStyle.Calm: return CalmButtonColor;
                case ButtonColorStyle.Growth: return GrowthButtonColor;
                case ButtonColorStyle.Alert: return AlertButtonColor;
                case ButtonColorStyle.Urgent: return UrgentButtonColor;
                case ButtonColorStyle.Quiet: return QuietButtonColor;
                default: return NeutralButtonColor;
            }
        }

        internal static GUIStyle GetCardStyle(Color color)
        {
            if (!CachedCardStyles.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle
                {
                    padding = new RectOffset(CardPaddingHorizontal, CardPaddingHorizontal, CardPaddingVertical, CardPaddingVertical),
                    margin = new RectOffset(0, 0, CardMarginTopBottom, CardMarginTopBottom),
                    normal = { background = GetSolidTexture(color) }
                };
                CachedCardStyles.Add(color, style);
            }

            return style;
        }

        internal static GUIStyle GetDataSectionStyle(Color color)
        {
            if (!CachedDataSectionStyles.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle
                {
                    padding = new RectOffset(
                        DataSectionPaddingLeft,
                        DataSectionPaddingRight,
                        CardPaddingVertical,
                        CardPaddingVertical),
                    margin = new RectOffset(0, 0, CardMarginTopBottom, CardMarginTopBottom),
                    normal = { background = GetSolidTexture(color) }
                };
                CachedDataSectionStyles.Add(color, style);
            }

            return style;
        }

        internal static GUIStyle GetRowStyle(Color color)
        {
            if (!CachedRowStyles.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle
                {
                    padding = new RectOffset(RowPaddingHorizontal, RowPaddingHorizontal, RowPaddingTop, RowPaddingBottom),
                    margin = new RectOffset(0, 0, RowMarginTopBottom, RowMarginTopBottom),
                    normal = { background = GetSolidTexture(color) }
                };
                CachedRowStyles.Add(color, style);
            }

            return style;
        }

        internal static GUIStyle GetLabelStyle(Color color)
        {
            if (!CachedLabelStyles.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = color }
                };
                CachedLabelStyles.Add(color, style);
            }

            return style;
        }

        internal static Texture2D GetSolidTexture(Color color)
        {
            if (!CachedSolidTextures.TryGetValue(color, out Texture2D texture))
            {
                texture = new Texture2D(SolidTextureSize, SolidTextureSize, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                texture.SetPixel(0, 0, color);
                texture.Apply();
                CachedSolidTextures.Add(color, texture);
            }

            return texture;
        }

        internal static bool DrawTextButton(
            string label,
            ButtonColorStyle style,
            Action onAction,
            float height = DefaultControlHeight,
            string tooltip = null,
            bool enabled = true,
            float? width = null
        )
        {
            Color previousColor = GUI.backgroundColor;
            bool previousEnabled = GUI.enabled;

            GUI.backgroundColor = GetButtonColor(style);
            GUI.enabled = enabled;

            List<GUILayoutOption> options = new List<GUILayoutOption> { GUILayout.Height(height) };
            if (width.HasValue)
            {
                options.Add(GUILayout.Width(width.Value));
            }

            bool pressed = GUILayout.Button(new GUIContent(label, tooltip), options.ToArray());

            GUI.backgroundColor = previousColor;
            GUI.enabled = previousEnabled;

            if (pressed)
            {
                onAction?.Invoke();
            }

            return pressed;
        }

        internal static bool DrawIconButton(
            string iconName,
            ButtonColorStyle style,
            Action onAction,
            float width = DefaultControlWidth,
            float height = DefaultControlWidth,
            string tooltip = null,
            bool enabled = true
        )
        {
            Color previousColor = GUI.backgroundColor;
            bool previousEnabled = GUI.enabled;

            GUI.backgroundColor = GetButtonColor(style);
            GUI.enabled = enabled;

            string cacheKey = tooltip == null ? iconName : iconName + IconCacheSeparator + tooltip;

            if (!CachedIconContents.TryGetValue(cacheKey, out GUIContent content))
            {
                content = EditorGUIUtility.IconContent(iconName, tooltip);
                CachedIconContents.Add(cacheKey, content);
            }

            bool pressed = GUILayout.Button(
                content,
                GUILayout.Width(width),
                GUILayout.Height(height)
            );

            GUI.backgroundColor = previousColor;
            GUI.enabled = previousEnabled;

            if (pressed)
            {
                onAction?.Invoke();
            }

            return pressed;
        }

        internal static void DrawLine(Color color)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, LineHeight);
            rect.height = LineHeight;
            EditorGUI.DrawRect(rect, color);
        }

        internal static void DrawSectionSpace()
        {
            EditorGUILayout.Space(SectionSpaceHeight);
        }

        private const float DefaultControlWidth = 24f;

        private const int CardPaddingHorizontal = 10;
        private const int CardPaddingVertical = 8;
        private const int CardMarginTopBottom = 2;

        private const int DataSectionPaddingLeft = 16;
        private const int DataSectionPaddingRight = 10;

        private const int RowPaddingHorizontal = 6;
        private const int RowPaddingTop = 4;
        private const int RowPaddingBottom = 4;
        private const int RowMarginTopBottom = 1;

    }
}