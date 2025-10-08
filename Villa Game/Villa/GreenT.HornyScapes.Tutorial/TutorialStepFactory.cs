using System;
using GreenT.Data;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialStepFactory : IFactory<TutorialStepSO, TutorialStep>, IFactory
{
	private readonly ISaver saver;

	private readonly ToolTipTutorialOpener toolTipTutorialOpener;

	private readonly TutorialLightningSystem lightningSystem;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public TutorialStepFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
	{
		this.saver = saver;
		this.toolTipTutorialOpener = toolTipTutorialOpener;
		this.lightningSystem = lightningSystem;
		this.lockerFactory = lockerFactory;
	}

	public TutorialStep Create(TutorialStepSO data)
	{
		TutorialStep tutorialStep = null;
		try
		{
			tutorialStep = CreateStep(data);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: step {data.GroupID}:{data.StepID} filled wrong");
		}
		saver.Add(tutorialStep);
		return tutorialStep;
	}

	private TutorialStep CreateStep(TutorialStepSO data)
	{
		if (!(data is TutorialEntityStepSO data2))
		{
			if (!(data is TutorialCompositeEntityStepSO data3))
			{
				if (!(data is TutorialClickCountStepSO data4))
				{
					if (data is TutorialLockerStepSO data5)
					{
						ILocker[] lockers = Create(data5);
						return new TutorialLockerStep(data5, lockers, toolTipTutorialOpener, lightningSystem);
					}
					return new TutorialStep(data, toolTipTutorialOpener, lightningSystem);
				}
				return new TutorialClickCountStep(data4, toolTipTutorialOpener, lightningSystem);
			}
			return new TutorialCompositeEntityStep(data3, toolTipTutorialOpener, lightningSystem);
		}
		return new TutorialEntityStep(data2, toolTipTutorialOpener, lightningSystem);
	}

	public ILocker[] Create(TutorialLockerStepSO data)
	{
		int i = 0;
		ILocker[] array = new ILocker[data.LockerList.Count];
		try
		{
			for (; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(data.LockerList[i].UnlockType, data.LockerList[i].UnlockValue, LockerSourceType.TutorialStep);
			}
			return array;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": Can't create locker: " + data.LockerList[i].UnlockType.ToString() + " with value: " + data.LockerList[i].UnlockValue);
		}
	}
}
