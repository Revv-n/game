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
		(from signal in _signalBus.GetStream<IndicatorSignals.PushRequest>()
			select (Status: signal.Status, Type: signal.Type)).Subscribe(SetReceiverState).AddTo(_compositeDisposable);
		(from visible in _mainScreenIndicator.IsVisible
			where visible
			select visible into value
			select (value: value, DecideToShow())).Subscribe(PushIndicator).AddTo(_compositeDisposable);
	}

	public IObservable<bool> OnIndicatorPush(FilteredIndicatorType type)
	{
		return _subjectMap[type].Where((bool visible) => visible);
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
