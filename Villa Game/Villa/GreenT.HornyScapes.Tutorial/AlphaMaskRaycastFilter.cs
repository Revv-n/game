using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public sealed class AlphaMaskRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
	[SerializeField]
	private RawImage _rawImage;

	[SerializeField]
	private DynamicHoleMask _maskController;

	[Tooltip("Порог, ниже которого начинает проходить Raycast")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _alphaThreshold = 0.01f;

	private Texture2D _texture;

	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		if (!IsValidTexture())
		{
			return true;
		}
		if (!TryGetUV(screenPoint, eventCamera, out var uv))
		{
			return false;
		}
		float alphaAtUV = GetAlphaAtUV(in uv);
		if (_alphaThreshold <= alphaAtUV)
		{
			return true;
		}
		return IsInsideBlockingHole(in uv);
	}

	private void Awake()
	{
		_rawImage.raycastTarget = true;
	}

	private void OnEnable()
	{
		SetTexture();
	}

	private void OnValidate()
	{
		SetTexture();
	}

	private void SetTexture()
	{
		_texture = _rawImage.texture as Texture2D;
	}

	private bool IsValidTexture()
	{
		if (_rawImage != null)
		{
			return _rawImage.texture != null;
		}
		return false;
	}

	private bool TryGetUV(Vector2 screenPoint, Camera eventCamera, out Vector2 uv)
	{
		uv = Vector2.zero;
		RectTransform rectTransform = _rawImage.rectTransform;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var localPoint))
		{
			return false;
		}
		Rect rect = rectTransform.rect;
		float num = (localPoint.x - rect.x) / rect.width;
		float num2 = (localPoint.y - rect.y) / rect.height;
		float num3 = 0f;
		float num4 = 1f;
		if (num < num3 || num4 < num || num2 < num3 || num4 < num2)
		{
			return false;
		}
		uv = new Vector2(num, num2);
		return true;
	}

	private float GetAlphaAtUV(in Vector2 uv)
	{
		if (_texture == null)
		{
			SetTexture();
		}
		if (_texture == null)
		{
			return 1f;
		}
		return _texture.GetPixelBilinear(uv.x, uv.y).a;
	}

	private bool IsInsideBlockingHole(in Vector2 uv)
	{
		if (_maskController == null)
		{
			return false;
		}
		int width = _texture.width;
		float num = (float)_texture.width * 0.5f;
		int height = _texture.height;
		float num2 = (float)_texture.height * 0.5f;
		Vector2 point = new Vector2(uv.x * (float)width, uv.y * (float)height);
		MaskSettings[] currentSettings = _maskController.CurrentSettings;
		bool result = false;
		MaskSettings[] array = currentSettings;
		foreach (MaskSettings maskSettings in array)
		{
			Vector2 holeSize = maskSettings.HoleSize;
			Vector2 holePosition = maskSettings.HolePosition;
			Vector2 vector = new Vector2(num + holePosition.x, num2 + holePosition.y);
			if (new Rect(vector - holeSize * 0.5f, holeSize).Contains(point))
			{
				if (!maskSettings.RaycastTarget)
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}
}
