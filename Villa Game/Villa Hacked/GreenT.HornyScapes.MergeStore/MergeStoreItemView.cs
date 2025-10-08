using System;
using System.Collections.Generic;
using GreenT.HornyScapes.DebugInfo;
using GreenT.HornyScapes.Events;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreItemView : MonoView<IItem>
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _count;

	[SerializeField]
	private SaleItemButtonView _saleItemButtonView;

	[SerializeField]
	private SaleView[] _sales;

	[SerializeField]
	private GameObject _saleStateRoot;

	[SerializeField]
	private GameObject _soldStateRoot;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	private readonly CompositeDisposable _mainStream = new CompositeDisposable();

	private MergeIconService _mergeIconService;

	[Inject]
	public void Init(MergeIconService mergeIconService)
	{
		_mergeIconService = mergeIconService;
	}

	public override void Set(IItem source)
	{
		Dispose();
		base.Set(source);
		SetIcon();
		CreatPurchasedStream();
		CreatClearStream();
		SetInfo();
		SetSaleIcon();
		SetState();
		TryDebug();
	}

	private void SetIcon()
	{
		_icon.sprite = _mergeIconService.GetSprite(base.Source.ItemKey);
	}

	private void SetInfo()
	{
		_count.text = base.Source.Amount.ToString();
		_saleItemButtonView.Set(base.Source);
	}

	private void CreatPurchasedStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)base.Source.Purchased, (Action<bool>)delegate
		{
			SetState();
		}), (ICollection<IDisposable>)_mainStream);
	}

	private void SetState()
	{
		_saleStateRoot.SetActive(!base.Source.Purchased.Value);
		_soldStateRoot.SetActive(base.Source.Purchased.Value);
	}

	private void CreatClearStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IItem>(base.Source.OnClear, (Action<IItem>)delegate
		{
			Dispose();
		}), (ICollection<IDisposable>)_mainStream);
	}

	private void SetSaleIcon()
	{
		SaleView[] sales = _sales;
		for (int i = 0; i < sales.Length; i++)
		{
			sales[i].gameObject.SetActive(value: false);
		}
		if (base.Source.Sale != 0)
		{
			_sales[base.Source.SaleID].gameObject.SetActive(value: true);
			_sales[base.Source.SaleID].Set(base.Source.Sale);
		}
	}

	private void Dispose()
	{
		_mainStream.Clear();
	}

	private void OnDestroy()
	{
		_mainStream.Dispose();
	}

	private void TryDebug()
	{
	}
}
