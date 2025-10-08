using System;
using System.Collections.Generic;
using GreenT.HornyScapes.DebugInfo;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreSectionView : MonoView<StoreSection>
{
	[SerializeField]
	private MergeStoreItemView[] _items;

	[SerializeField]
	private MonoTimer _refreshTimer;

	[SerializeField]
	private RefreshSectionButtonView _refreshSectionButton;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	private readonly CompositeDisposable _mainStream = new CompositeDisposable();

	private TimeHelper _timeHelper;

	[Inject]
	public void Init(TimeHelper timeHelper)
	{
		_timeHelper = timeHelper;
	}

	public override void Set(StoreSection source)
	{
		Dispose();
		base.Set(source);
		CreatRefreshStream();
		Refresh(base.Source);
		UpdateRefreshButton(source);
		CreatClearStream();
		TryDebug();
	}

	private void UpdateRefreshButton(StoreSection source)
	{
		_refreshSectionButton.Set(source);
	}

	private void CreatRefreshStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StoreSection>(base.Source.OntRefresh, (Action<StoreSection>)Refresh), (ICollection<IDisposable>)_mainStream);
	}

	private void Refresh(StoreSection storeSection)
	{
		SetItems();
		_refreshTimer.Init(base.Source.RefreshTimer, _timeHelper.UseCombineFormat);
	}

	private void SetItems()
	{
		MergeStoreItemView[] items = _items;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].gameObject.SetActive(value: false);
		}
		int num = Math.Min(_items.Length, base.Source.ShopItems.Count);
		for (int j = 0; j < num; j++)
		{
			_items[j].gameObject.SetActive(value: true);
			_items[j].Set(base.Source.GetItem(j));
		}
	}

	private void CreatClearStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StoreSection>(base.Source.OnClear, (Action<StoreSection>)delegate
		{
			Dispose();
		}), (ICollection<IDisposable>)_mainStream);
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
