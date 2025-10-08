using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Tasks;

public class TasksWindowExpandButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private RectTransform arrows;

	[SerializeField]
	private float arrowsTweenTime;

	private Tween arrowsTween;

	public bool IsExpanded { get; private set; }

	public event Action OnClick;

	private void Start()
	{
		button.AddClickCallback(AtClick);
	}

	public TasksWindowExpandButton AddClickCallback(Action onClick)
	{
		if (onClick != null)
		{
			OnClick += onClick;
		}
		return this;
	}

	public void SetExpanded(bool expanded)
	{
		IsExpanded = expanded;
		arrowsTween?.Kill();
		arrows.transform.localScale = new Vector3((!expanded) ? 1 : (-1), 1f, 1f);
	}

	private void AtClick()
	{
		arrowsTween.Kill();
		arrowsTween = arrows.DOScaleX(IsExpanded ? 1 : (-1), arrowsTweenTime);
		IsExpanded = !IsExpanded;
		this.OnClick?.Invoke();
	}
}
