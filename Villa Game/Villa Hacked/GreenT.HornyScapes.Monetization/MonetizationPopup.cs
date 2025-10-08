using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Settings.UI;
using GreenT.UI;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Monetization;

public class MonetizationPopup : PopupWindow
{
	public Button AbortButton;

	public Button SupportButton;

	public URLOpener supportUrlOpener;

	public Button CloseButton;

	public LocalizedTextMeshPro Description;

	public Image Loader;

	public Image FinalImage;

	[SerializeField]
	protected string pendingDescriptionLocalizeKey;

	[SerializeField]
	protected string failDescriptionLocalizeKey;

	protected IDisposable stream;

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Canvas = GetComponentInParent<Canvas>();
		base.Init(windowsOpener);
	}

	protected override void Awake()
	{
		base.Awake();
		CloseButton.onClick.AddListener(Close);
	}

	public override void Open()
	{
		base.Open();
		SetDescriptionLocalization(pendingDescriptionLocalizeKey);
		SetImage(isPending: true);
		SetButton(isPending: true);
	}

	public virtual void SetFailedView()
	{
		SetDescriptionLocalization(failDescriptionLocalizeKey);
		SetImage(isPending: false);
		SetButton(isPending: false);
	}

	protected virtual void SetImage(bool isPending)
	{
		Loader.gameObject.SetActive(isPending);
		FinalImage.gameObject.SetActive(!isPending);
	}

	protected virtual void SetButton(bool isPending)
	{
		AbortButton.gameObject.SetActive(isPending);
		CloseButton.gameObject.SetActive(!isPending);
		SupportButton.gameObject.SetActive(!isPending);
	}

	protected void SetDescriptionLocalization(string key)
	{
		Description.Init(key);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		AbortButton?.onClick.RemoveAllListeners();
		CloseButton?.onClick.RemoveAllListeners();
		SupportButton?.onClick.RemoveAllListeners();
		stream?.Dispose();
	}
}
