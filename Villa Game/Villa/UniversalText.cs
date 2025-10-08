using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UniversalText
{
	public static object GetTextObject(GameObject go)
	{
		if (go.TryGetComponent<Text>(out var component))
		{
			return component;
		}
		if (go.TryGetComponent<TMP_Text>(out var component2))
		{
			return component2;
		}
		if (go.TryGetComponent<TextMesh>(out var component3))
		{
			return component3;
		}
		return null;
	}

	public static bool GetAllowBestFit(object target)
	{
		if (target is GameObject go)
		{
			return GetAllowBestFit(GetTextObject(go));
		}
		if (target is Text text)
		{
			return text.resizeTextForBestFit;
		}
		if (target is TMP_Text tMP_Text)
		{
			return tMP_Text.enableAutoSizing;
		}
		return false;
	}

	public static void SetAllowBestFit(object target, bool value)
	{
		if (target is GameObject go)
		{
			SetAllowBestFit(GetTextObject(go), value);
		}
		else if (target is Text text)
		{
			text.resizeTextForBestFit = value;
		}
		else if (target is TMP_Text tMP_Text)
		{
			tMP_Text.enableAutoSizing = value;
		}
	}

	public static float GetFontSize(object target)
	{
		if (target is GameObject go)
		{
			return GetFontSize(GetTextObject(go));
		}
		if (target is Text text)
		{
			return text.fontSize;
		}
		if (target is TMP_Text tMP_Text)
		{
			return tMP_Text.fontSize;
		}
		if (target is TextMesh textMesh)
		{
			return textMesh.fontSize;
		}
		return 0f;
	}

	public static void SetFontSize(object target, float value)
	{
		if (target is GameObject go)
		{
			SetFontSize(GetTextObject(go), value);
		}
		else if (target is Text text)
		{
			text.fontSize = (int)value;
		}
		else if (target is TMP_Text tMP_Text)
		{
			tMP_Text.fontSize = value;
		}
		else if (target is TextMesh textMesh)
		{
			textMesh.fontSize = (int)value;
		}
	}

	public static float GetMaxAutoSize(object target)
	{
		if (target is GameObject go)
		{
			return GetMaxAutoSize(GetTextObject(go));
		}
		if (target is Text text)
		{
			return text.resizeTextMaxSize;
		}
		if (target is TMP_Text tMP_Text)
		{
			return tMP_Text.fontSizeMax;
		}
		if (target is TextMesh textMesh)
		{
			return textMesh.fontSize;
		}
		return 0f;
	}

	public static void SetMaxAutoSize(object target, float value)
	{
		if (target is GameObject go)
		{
			SetMaxAutoSize(GetTextObject(go), value);
		}
		else if (target is Text text)
		{
			text.resizeTextMaxSize = (int)value;
		}
		else if (target is TMP_Text tMP_Text)
		{
			tMP_Text.fontSizeMax = value;
		}
	}

	public static float GetMinAutoSize(object target)
	{
		if (target is GameObject go)
		{
			return GetMinAutoSize(GetTextObject(go));
		}
		if (target is Text text)
		{
			return text.resizeTextMinSize;
		}
		if (target is TMP_Text tMP_Text)
		{
			return tMP_Text.fontSizeMin;
		}
		if (target is TextMesh textMesh)
		{
			return textMesh.fontSize;
		}
		return 0f;
	}

	public static void SetMinAutoSize(object target, float value)
	{
		if (target is GameObject go)
		{
			SetMinAutoSize(GetTextObject(go), value);
		}
		else if (target is Text text)
		{
			text.resizeTextMinSize = (int)value;
		}
		else if (target is TMP_Text tMP_Text)
		{
			tMP_Text.fontSizeMin = value;
		}
	}

	public static TextAnchor GetAligment(object target)
	{
		if (target is GameObject go)
		{
			return GetAligment(GetTextObject(go));
		}
		if (target is Text text)
		{
			return text.alignment;
		}
		if (target is TMP_Text tMP_Text)
		{
			int num = 1;
			int num2 = 1;
			if (tMP_Text.alignment == TextAlignmentOptions.Bottom || tMP_Text.alignment == TextAlignmentOptions.BottomFlush || tMP_Text.alignment == TextAlignmentOptions.BottomLeft || tMP_Text.alignment == TextAlignmentOptions.BottomRight || tMP_Text.alignment == TextAlignmentOptions.BottomJustified || tMP_Text.alignment == TextAlignmentOptions.BottomGeoAligned)
			{
				num2 = 2;
			}
			if (tMP_Text.alignment == TextAlignmentOptions.Top || tMP_Text.alignment == TextAlignmentOptions.TopFlush || tMP_Text.alignment == TextAlignmentOptions.TopGeoAligned || tMP_Text.alignment == TextAlignmentOptions.TopJustified || tMP_Text.alignment == TextAlignmentOptions.TopLeft || tMP_Text.alignment == TextAlignmentOptions.TopRight)
			{
				num2 = 0;
			}
			if (tMP_Text.alignment == TextAlignmentOptions.Right || tMP_Text.alignment == TextAlignmentOptions.BaselineRight || tMP_Text.alignment == TextAlignmentOptions.BottomRight || tMP_Text.alignment == TextAlignmentOptions.CaplineRight || tMP_Text.alignment == TextAlignmentOptions.MidlineRight || tMP_Text.alignment == TextAlignmentOptions.TopRight)
			{
				num = 2;
			}
			if (tMP_Text.alignment == TextAlignmentOptions.Left || tMP_Text.alignment == TextAlignmentOptions.BaselineLeft || tMP_Text.alignment == TextAlignmentOptions.BottomLeft || tMP_Text.alignment == TextAlignmentOptions.CaplineLeft || tMP_Text.alignment == TextAlignmentOptions.MidlineLeft || tMP_Text.alignment == TextAlignmentOptions.TopLeft)
			{
				num2 = 0;
			}
			return (TextAnchor)(num2 * 3 + num);
		}
		if (target is TextMesh textMesh)
		{
			return textMesh.anchor;
		}
		return TextAnchor.MiddleLeft;
	}

	public static void SetAligment(object target, TextAnchor value)
	{
		if (target is GameObject go)
		{
			SetAligment(GetTextObject(go), value);
		}
		else if (target is Text text)
		{
			text.alignment = value;
		}
		else if (target is TMP_Text tMP_Text)
		{
			switch (value)
			{
			case TextAnchor.UpperLeft:
				tMP_Text.alignment = TextAlignmentOptions.TopLeft;
				break;
			case TextAnchor.UpperCenter:
				tMP_Text.alignment = TextAlignmentOptions.Top;
				break;
			case TextAnchor.UpperRight:
				tMP_Text.alignment = TextAlignmentOptions.TopRight;
				break;
			case TextAnchor.MiddleLeft:
				tMP_Text.alignment = TextAlignmentOptions.Left;
				break;
			case TextAnchor.MiddleCenter:
				tMP_Text.alignment = TextAlignmentOptions.Center;
				break;
			case TextAnchor.MiddleRight:
				tMP_Text.alignment = TextAlignmentOptions.Right;
				break;
			case TextAnchor.LowerLeft:
				tMP_Text.alignment = TextAlignmentOptions.BottomLeft;
				break;
			case TextAnchor.LowerCenter:
				tMP_Text.alignment = TextAlignmentOptions.Bottom;
				break;
			case TextAnchor.LowerRight:
				tMP_Text.alignment = TextAlignmentOptions.BottomRight;
				break;
			}
		}
		else if (target is TextMesh textMesh)
		{
			textMesh.anchor = value;
		}
	}
}
