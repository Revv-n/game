using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class SettingsWindowSizeSetter : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField]
	private RectTransform window;

	[SerializeField]
	private RectTransform upper;

	[SerializeField]
	private RectTransform background;

	[SerializeField]
	private RectTransform girlArea;

	[Header("Window Positions")]
	[SerializeField]
	private float windowDefaultPosY;

	[SerializeField]
	private float windowBigSizePosY;

	[Header("Upper Positions")]
	[SerializeField]
	private float upperDefaultTopY;

	[SerializeField]
	private float upperBigSizeTopY;

	[Header("Background Positions")]
	[SerializeField]
	private float backDefaultTopY;

	[SerializeField]
	private float backBigSizeTopY;

	[Header("Girl Area Default Size")]
	[SerializeField]
	private float girlAreaWidthDefault;

	[SerializeField]
	private float girlAreaHeightDefault;

	[Header("Girl Area Big Size")]
	[SerializeField]
	private float girlAreaWidthBigSize;

	[SerializeField]
	private float girlAreaHeightBigSize;

	[Header("Registration button")]
	[SerializeField]
	private TextMeshProValueStates registrationButton;

	[SerializeField]
	private string registrationButtonLocalization;

	[SerializeField]
	private string erolabsBindButtonLocalization;

	public void SetDefaultSize()
	{
		SetAnchoredY(window, windowDefaultPosY);
		SetAnchoredY(upper, upperDefaultTopY);
		SetOffsetMaxY(background, backDefaultTopY);
		SetSize(girlArea, girlAreaWidthDefault, girlAreaHeightDefault);
		SetRegistrationButtonLocalization(registrationButtonLocalization);
	}

	public void SetBigSize()
	{
		SetAnchoredY(window, windowBigSizePosY);
		SetAnchoredY(upper, upperBigSizeTopY);
		SetOffsetMaxY(background, backBigSizeTopY);
		SetSize(girlArea, girlAreaWidthBigSize, girlAreaHeightBigSize);
		SetRegistrationButtonLocalization(erolabsBindButtonLocalization);
	}

	private void SetAnchoredY(RectTransform rt, float posY)
	{
		Vector2 anchoredPosition = rt.anchoredPosition;
		anchoredPosition.y = posY;
		rt.anchoredPosition = anchoredPosition;
	}

	private void SetOffsetMaxY(RectTransform rt, float offsetY)
	{
		Vector2 offsetMax = rt.offsetMax;
		offsetMax.y = offsetY;
		rt.offsetMax = offsetMax;
	}

	private void SetSize(RectTransform rt, float width, float height)
	{
		rt.sizeDelta = new Vector2(width, height);
	}

	private void SetRegistrationButtonLocalization(string localizationKey)
	{
		registrationButton.ForceSetElement(0, localizationKey);
	}
}
