using System;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

[ExecuteAlways]
public sealed class DynamicHoleMask : MonoBehaviour
{
	[SerializeField]
	private RawImage _rawImage;

	[SerializeField]
	private MaskSettings[] _currentSettings;

	[Header("Размер текстуры маски")]
	[SerializeField]
	private Vector2Int _textureSize = new Vector2Int(1024, 1024);

	private Texture2D _maskTexture;

	public MaskSettings[] CurrentSettings => _currentSettings;

	public void Show(MaskSettings[] maskSettings)
	{
		_currentSettings = maskSettings;
		RebuildMask();
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
		_currentSettings = Array.Empty<MaskSettings>();
	}

	private void OnEnable()
	{
		RebuildMask();
	}

	private void OnValidate()
	{
		RebuildMask();
	}

	private void RebuildMask()
	{
		if (_maskTexture != null)
		{
			UnityEngine.Object.DestroyImmediate(_maskTexture);
		}
		_maskTexture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.ARGB32, mipChain: false);
		_maskTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] array = new Color32[_textureSize.x * _textureSize.y];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		_maskTexture.SetPixels32(array);
		MaskSettings[] currentSettings = _currentSettings;
		foreach (MaskSettings settings in currentSettings)
		{
			DrawHole(settings);
		}
		_maskTexture.Apply();
		_rawImage.texture = _maskTexture;
		_rawImage.SetNativeSize();
		_rawImage.rectTransform.anchorMin = Vector2.zero;
		_rawImage.rectTransform.anchorMax = Vector2.one;
		_rawImage.rectTransform.offsetMin = Vector2.zero;
		_rawImage.rectTransform.offsetMax = Vector2.zero;
	}

	private void DrawHole(MaskSettings settings)
	{
		Vector2 holeSize = settings.HoleSize;
		Vector2 holePosition = settings.HolePosition;
		int num = Mathf.RoundToInt(holeSize.x);
		int num2 = Mathf.RoundToInt(holeSize.y);
		int num3 = Mathf.RoundToInt((float)_textureSize.x * 0.5f + holePosition.x - (float)num * 0.5f);
		int num4 = Mathf.RoundToInt((float)_textureSize.y * 0.5f + holePosition.y - (float)num2 * 0.5f);
		for (int i = num4; i < num4 + num2; i++)
		{
			if (i < 0 || _textureSize.y <= i)
			{
				continue;
			}
			for (int j = num3; j < num3 + num; j++)
			{
				if (j >= 0 && _textureSize.x > j)
				{
					_maskTexture.SetPixel(j, i, new Color(1f, 1f, 1f, 0f));
				}
			}
		}
	}
}
