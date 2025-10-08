using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;

public class IndicatorDisplayService : IInitializable, IDisposable
{
	private readonly SignalBus _signalBus;

	private readonly MainScreenIndicator _mainScreenIndicator;

	private readonly Dictionary<FilteredIndicatorType, Subject<bool>> _subjectMap = new Dictionary<FilteredIndicatorType, Subject<bool>>();

	private readonly Dictionary<FilteredIndicatorType, bool> _receiverMap = new Dictionary<FilteredIndicatorType, bool>();

	private readonly FilteredIndicatorType[] _indicatorTypes;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private IDisposable _mergeStream;

	private IndicatorDisplayService(MainScreenIndicator mainScreenIndicator, SignalBus signalBus)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		_signalBus = signalBus;
		_mainScreenIndicator = mainScreenIndicator;
		EnumExtension.ForeachEnum<FilteredIndicatorType>(InitializeStructure);
		_receiverMap[FilteredIndicatorType.None] = true;
		_indicatorTypes = EnumExtension.GetTargetValue<FilteredIndicatorType>();
	}

	private void InitializeStructure(FilteredIndicatorType type)
	{
		_subjectMap.Add(type, new Subject<bool>());
		_receiverMap.Add(type, value: false);
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(bool, FilteredIndicatorType)>(Observable.Select<IndicatorSignals.PushRequest, (bool, FilteredIndicatorType)>(_signalBus.GetStream<IndicatorSignals.PushRequest>(), (Func<IndicatorSignals.PushRequest, (bool, FilteredIndicatorType)>)((IndicatorSignals.PushRequest signal) => (Status: signal.Status, Type: signal.Type))), (Action<(bool, FilteredIndicatorType)>)SetReceiverState), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(bool, FilteredIndicatorType)>(Observable.Select<bool, (bool, FilteredIndicatorType)>(Observable.Where<bool>((IObservable<bool>)_mainScreenIndicator.IsVisible, (Func<bool, bool>)((bool visible) => visible)), (Func<bool, (bool, FilteredIndicatorType)>)((bool value) => (value: value, DecideToShow()))), (Action<(bool, FilteredIndicatorType)>)PushIndicator), (ICollection<IDisposable>)_compositeDisposable);
	}

	public IObservable<bool> OnIndicatorPush(FilteredIndicatorType type)
	{
		return Observable.Where<bool>((IObservable<bool>)_subjectMap[type], (Func<bool, bool>)((bool visible) => visible));
	}

	private void UpdateReceiverState()
	{
		bool value = _mainScreenIndicator.IsVisible.Value;
		PushIndicator((status: value || _receiverMap.FirstOrDefault((KeyValuePair<FilteredIndicatorType, bool> receiver) => receiver.Value).Key == FilteredIndicatorType.None, type: DecideToShow()));
	}

	private FilteredIndicatorType DecideToShow()
	{
		return _indicatorTypes.FirstOrDefault((FilteredIndicatorType type) => _receiverMap[type]);
	}

	private void PushIndicator((bool status, FilteredIndicatorType type) tuple)
	{
		_subjectMap[tuple.type].OnNext(tuple.status);
	}

	private void SetReceiverState((bool status, FilteredIndicatorType type) tuple)
	{
		_receiverMap[tuple.type] = tuple.status;
		UpdateReceiverState();
	}

	public void Dispose()
	{
		_mergeStream?.Dispose();
		_compositeDisposable.Dispose();
	}
}
