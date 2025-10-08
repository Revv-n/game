using System;
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
		this.saveController = saveController;
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	public void Initialize()
	{
		saveController.OnStartSavingEvent.Subscribe(delegate
		{
			SendAllAnalytic();
		}).AddTo(compositeDisposable);
		currencyAmplitudeAnalytic.OnEnergySpend.Subscribe(AddEnergySpendAnalytic).AddTo(compositeDisposable);
		currencyAmplitudeAnalytic.OnEnergyRecieved.Subscribe(AddEnergyRecievedAnalytic).AddTo(compositeDisposable);
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
		compositeDisposable?.Dispose();
	}
}
