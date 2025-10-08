using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Tasks;
using Merge;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class ObjectiveView : MonoView<IObjective>, IDisposable
{
	[SerializeField]
	private InfoButton infoButton;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private LocalizedTextMeshPro count;

	[SerializeField]
	private CompletableView completableViewsOnlyText;

	[SerializeField]
	private Vector3 ForMergeItemPosition;

	[SerializeField]
	private Vector3 ForDescriptionPosition;

	[SerializeField]
	private GameObject _sale;

	protected CompositeDisposable onUpdateStream = new CompositeDisposable();

	public override void Set(IObjective source)
	{
		if (base.Source != null)
		{
			Unsubscribe();
		}
		base.Set(source);
		SetIcon(source);
		UpdateCount(source);
		UpdateText(source);
		SubscribeOnObjective();
		ChangeInfoButton(state: true);
	}

	public void SetIsSale(bool isSale)
	{
		_sale.SetActive(isSale);
	}

	public void ChangeInfoButton(bool state)
	{
		if (state && base.Source is MergeItemObjective mergeItemObjective)
		{
			icon.raycastTarget = true;
			infoButton.SetActive(active: true);
			infoButton.Set(mergeItemObjective.ItemKey);
		}
		else
		{
			icon.raycastTarget = false;
			infoButton.SetActive(active: false);
		}
	}

	public void SetPosition(bool isMergeItem)
	{
		if (isMergeItem)
		{
			count.transform.localPosition = ForMergeItemPosition;
		}
		else
		{
			count.transform.localPosition = ForDescriptionPosition;
		}
	}

	private void Unsubscribe()
	{
		onUpdateStream.Clear();
	}

	private void SubscribeOnObjective()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IObjective>(base.Source.OnUpdate, (Action<IObjective>)UpdateCount), (ICollection<IDisposable>)onUpdateStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IObjective>(base.Source.OnUpdate, (Action<IObjective>)UpdateText), (ICollection<IDisposable>)onUpdateStream);
	}

	private void SetIcon(IObjective source)
	{
		icon.sprite = base.Source.GetIcon();
	}

	private void UpdateCount(IObjective source)
	{
		count.SetArguments(base.Source.GetProgress(), base.Source.GetTarget());
	}

	private void UpdateText(IObjective source)
	{
		completableViewsOnlyText.SetComplete(base.Source.IsComplete);
	}

	public void Dispose()
	{
		CompositeDisposable obj = onUpdateStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
