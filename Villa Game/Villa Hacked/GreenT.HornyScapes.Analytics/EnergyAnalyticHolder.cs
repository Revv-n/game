using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Saves;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class EnergyAnalyticHolder : IInitializable, IDisposable
{
	private SaveController saveController;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	private CompositeDisposable compositeDisposable = new CompositeDisposable();

	private EnergySpendAnalyticData mainSpendEnergyAnalyticData;

	private EnergyRecievedAnalyticData mainRecievedEnergyAnalyticData;

	private EnergySpendAnalyticData eventSpendEnergyAnalyticData;

	private EnergyRecievedAnalyticData eventRecievedEnergyAnalyticData;

	private EnergyAnalyticHolder(SaveController saveController, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.saveController = saveController;
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>((IObservable<Unit>)saveController.OnStartSavingEvent, (Action<Unit>)delegate
		{
			SendAllAnalytic();
		}), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EnergySpendAnalyticData>(currencyAmplitudeAnalytic.OnEnergySpend, (Action<EnergySpendAnalyticData>)AddEnergySpendAnalytic), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EnergyRecievedAnalyticData>(currencyAmplitudeAnalytic.OnEnergyRecieved, (Action<EnergyRecievedAnalyticData>)AddEnergyRecievedAnalytic), (ICollection<IDisposable>)compositeDisposable);
	}

	private void AddEnergyRecievedAnalytic(EnergyRecievedAnalyticData newAnalyticData)
	{
		if (newAnalyticData.Type == CurrencyType.Energy)
		{
			AddAnalyticData(newAnalyticData, ref mainRecievedEnergyAnalyticData);
		}
		else
		{
			AddAnalyticData(newAnalyticData, ref eventRecievedEnergyAnalyticData);
		}
	}

	private void AddEnergySpendAnalytic(EnergySpendAnalyticData newAnalyticData)
	{
		if (newAnalyticData.Type == CurrencyType.Energy)
		{
			AddAnalyticData(newAnalyticData, ref mainSpendEnergyAnalyticData);
		}
		else
		{
			AddAnalyticData(newAnalyticData, ref eventSpendEnergyAnalyticData);
		}
	}

	private void SendRecievedEnergyAnalytic(EnergyRecievedAnalyticData energyRecievedAnalyticData)
	{
		currencyAmplitudeAnalytic.SendReceivedEventDefault(energyRecievedAnalyticData.Type, energyRecievedAnalyticData.Diff, energyRecievedAnalyticData.Source, energyRecievedAnalyticData.Identificator);
	}

	private void SendSpendEnergyAnalytic(EnergySpendAnalyticData energySpendAnalyticData)
	{
		currencyAmplitudeAnalytic.SendSpendEventDefault(energySpendAnalyticData.Type, energySpendAnalyticData.Diff, energySpendAnalyticData.Source, energySpendAnalyticData.ContentType, energySpendAnalyticData.Identificator);
	}

	private void SendAllAnalytic()
	{
		SendAnalyticAndReset(ref mainSpendEnergyAnalyticData, SendSpendEnergyAnalytic);
		SendAnalyticAndReset(ref mainRecievedEnergyAnalyticData, SendRecievedEnergyAnalytic);
		SendAnalyticAndReset(ref eventSpendEnergyAnalyticData, SendSpendEnergyAnalytic);
		SendAnalyticAndReset(ref eventRecievedEnergyAnalyticData, SendRecievedEnergyAnalytic);
	}

	private void AddAnalyticData<T>(T newAnalyticData, ref T actualAnalyticData) where T : BaseEnergyAnalyticData
	{
		if (actualAnalyticData == null)
		{
			actualAnalyticData = newAnalyticData;
			return;
		}
		if (actualAnalyticData.IsComparative(newAnalyticData))
		{
			actualAnalyticData.AddDiff(newAnalyticData.Diff);
			return;
		}
		if (actualAnalyticData is EnergySpendAnalyticData energySpendAnalyticData)
		{
			SendSpendEnergyAnalytic(energySpendAnalyticData);
		}
		else
		{
			SendRecievedEnergyAnalytic(actualAnalyticData as EnergyRecievedAnalyticData);
		}
		actualAnalyticData = newAnalyticData;
	}

	private void SendAnalyticAndReset<T>(ref T analyticData, Action<T> sendAnalytic) where T : class
	{
		if (analyticData != null)
		{
			sendAnalytic(analyticData);
			analyticData = null;
		}
	}

	public void Dispose()
	{
		CompositeDisposable obj = compositeDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
