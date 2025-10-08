using DG.Tweening;
using GreenT.HornyScapes.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialHighlighter : MonoBehaviour, ITutorialElement
{
	[SerializeField]
	protected Image highlightIcon;

	private LightningSystem lightningSystem;

	private Tween applyTween;

	private Canvas canvas;

	private GraphicRaycaster graphicRaycaster;

	private Material prevMaterial;

	private bool isActive;

	public bool IsLight { get; private set; }

	public bool BlockScreen { get; private set; }

	public void Init(TutorialLightningSystem lightningSystem, Image highlightImage)
	{
		this.lightningSystem = lightningSystem;
		highlightIcon = highlightImage;
	}

	public void Activate(bool blockScreen, bool isLight)
	{
		BlockScreen = blockScreen;
		IsLight = isLight;
		if (IsLight)
		{
			ActivateLightning();
		}
		if (BlockScreen)
		{
			ActivateIconOverlay();
		}
	}

	public void Deactivate()
	{
		if (IsLight)
		{
			DeactivateLightning();
		}
		if (BlockScreen)
		{
			DeactivateIconOverlay();
		}
	}

	public void Destroy()
	{
		Deactivate();
		Object.Destroy(this);
	}

	private void ActivateLightning()
	{
		prevMaterial = highlightIcon.material;
		applyTween = lightningSystem.Apply(highlightIcon);
	}

	private void DeactivateLightning()
	{
		lightningSystem.Undo(highlightIcon, applyTween);
		highlightIcon.material = prevMaterial;
	}

	private void ActivateIconOverlay()
	{
		isActive = true;
		if (!canvas && !TryGetComponent<Canvas>(out canvas))
		{
			canvas = base.gameObject.AddComponent<Canvas>();
		}
		if (!graphicRaycaster && !TryGetComponent<GraphicRaycaster>(out graphicRaycaster))
		{
			graphicRaycaster = base.gameObject.AddComponent<GraphicRaycaster>();
		}
		canvas.overrideSorting = true;
		canvas.sortingOrder = 9999;
	}

	private void OnEnable()
	{
		if ((bool)canvas)
		{
			canvas.overrideSorting = isActive;
		}
	}

	private void DeactivateIconOverlay()
	{
		isActive = false;
		canvas.overrideSorting = false;
	}

	private void OnDestroy()
	{
		Object.Destroy(graphicRaycaster);
		Object.Destroy(canvas);
		Deactivate();
	}
}
