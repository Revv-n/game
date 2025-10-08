using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleToggle : Toggle
{
	public float scaleCoef = 1.05f;

	public float duration = 0.1f;

	public float unScaleCoef = 0.95f;

	private Vector3 startScale;

	[SerializeField]
	private ScaleButtonSettings settings;

	private Vector3 scale;

	private Tween doScale;

	public bool IsPointerOver { get; private set; }

	public event Action OnEnter;

	public event Action OnExit;

	protected override void Awake()
	{
		base.Awake();
		startScale = base.transform.localScale;
		scale = scaleCoef * startScale;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (!base.isOn)
		{
			if (doScale.IsActive())
			{
				doScale.Kill(complete: true);
			}
			doScale = base.transform.DOScale(scale, duration).OnStart(delegate
			{
				base.transform.localScale = Vector3.zero;
			});
		}
		IsPointerOver = true;
		this.OnEnter?.Invoke();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (!base.isOn)
		{
			if (doScale.IsActive())
			{
				doScale.Kill(complete: true);
			}
			base.transform.localScale = startScale;
		}
		IsPointerOver = false;
		this.OnExit?.Invoke();
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (!base.isOn)
		{
			if (doScale.IsActive())
			{
				doScale.Kill();
			}
			doScale = base.transform.DOScale(unScaleCoef, duration).OnStart(delegate
			{
				base.transform.localScale = Vector3.zero;
			});
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (!base.isOn)
		{
			if (doScale.IsActive())
			{
				doScale.Kill();
			}
			doScale = base.transform.DOScale(startScale, duration).OnStart(delegate
			{
				base.transform.localScale = Vector3.zero;
			});
		}
	}
}
