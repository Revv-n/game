using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using Merge;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreNotifiedButton : MonoBehaviour
{
	[SerializeField]
	private SaleItemButtonView _button;

	private ItemsCluster _assortment;

	private ContentSelectorGroup _contentSelectorGroup;

	private readonly CompositeDisposable _mergeStoreStream = new CompositeDisposable();

	public bool Use { get; private set; }

	[Inject]
	public void Init(ItemsCluster assortment, ContentSelectorGroup contentSelectorGroup, ICurrencyProcessor currencyProcessor, GameSettings gameSettings, NoCurrencyTabOpener noCurrencyTabOpener)
	{
		_assortment = assortment;
		_contentSelectorGroup = contentSelectorGroup;
		_button.Init(gameSettings, currencyProcessor, noCurrencyTabOpener);
	}

	public void Set(GIKey giKey, ButtonPosition buttonPosition = ButtonPosition.BankTab)
	{
		Set(giKey, _contentSelectorGroup.Current, buttonPosition);
	}

	public void Set(GIKey giKey, ContentType contentType, ButtonPosition buttonPosition = ButtonPosition.BankTab)
	{
		Use = false;
		_mergeStoreStream.Clear();
		_button.SetPosition(buttonPosition);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GIKey>(Observable.Where<GIKey>(_assortment.OnUpdate, IsValid(giKey)), (Action<GIKey>)delegate
		{
			UpdateMergeSale(giKey, contentType);
		}), (ICollection<IDisposable>)_mergeStoreStream);
		UpdateMergeSale(giKey, contentType);
	}

	private void SetButtonSettings(GIKey giKey)
	{
		if (_assortment.TryGet(giKey, out var result))
		{
			_button.Set(result);
		}
	}

	private void UpdateMergeSale(GIKey key, ContentType contentType)
	{
		Use = _assortment.Have(contentType, key);
		base.gameObject.SetActive(Use);
		if (Use)
		{
			SetButtonSettings(key);
		}
	}

	private Func<GIKey, bool> IsValid(GIKey key)
	{
		return (GIKey giKey) => giKey == key;
	}

	private void OnDisable()
	{
		_mergeStoreStream.Clear();
	}
}
