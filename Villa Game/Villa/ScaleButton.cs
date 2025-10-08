using System;
using DG.Tweening;
using GreenT.HornyScapes.Sounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(ScaleButtonSettings))]
public class ScaleButton : Button, IPointerExitHandler, IEventSystemHandler, IPointerEnterHandler
{
	[Inject]
	protected IAudioPlayer audioPlayer;

	public float scaleCoef = 1.05f;

	public float duration = 0.1f;

	private Vector3 startScale;

	[SerializeField]
	protected ScaleButtonSettings settings;

	[SerializeField]
	protected Transform targetToScale;

	private Vector3 scale;

	private Tween doScale;

	public bool IsPointerOver { get; private set; }

	public event Action OnEnter;

	public event Action OnExit;

	protected override void Awake()
	{
		base.Awake();
		base.onClick.AddListener(PlayClick);
		OnEnter += PlayHover;
		startScale = base.transform.localScale;
		scale = scaleCoef * startScale;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (doScale.IsActive())
		{
			doScale.Kill(complete: true);
		}
		targetToScale.localScale = startScale;
		IsPointerOver = false;
		this.OnExit?.Invoke();
	}

	private void PlayHover()
	{
		audioPlayer?.PlayOneShotAudioClip2D(settings.Hover.Sound);
	}

	private void PlayClick()
	{
		audioPlayer?.PlayOneShotAudioClip2D(settings.Click.Sound);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (IsInteractable())
		{
			doScale = targetToScale.DOScale(scale, duration).OnStart(delegate
			{
				targetToScale.localScale = Vector3.zero;
			});
		}
		IsPointerOver = true;
		this.OnEnter?.Invoke();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (doScale.IsActive())
		{
			doScale.Kill(complete: true);
		}
		targetToScale.localScale = startScale;
		IsPointerOver = false;
		this.OnExit?.Invoke();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.onClick.RemoveAllListeners();
	}
}
