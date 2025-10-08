using System;
using System.Collections.Generic;
using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialGroupFactory : IFactory<TutorialGroupSO, TutorialGroupSteps>, IFactory
{
	private readonly ISaver saver;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private readonly IFactory<TutorialStepSO, TutorialStep> stepFactory;

	public TutorialGroupFactory(ISaver saver, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, IFactory<TutorialStepSO, TutorialStep> stepFactory)
	{
		this.saver = saver;
		this.lockerFactory = lockerFactory;
		this.stepFactory = stepFactory;
	}

	public TutorialGroupSteps Create(TutorialGroupSO groupData)
	{
		TutorialGroupSteps tutorialGroupSteps = null;
		ILocker[] lockers = CreateLockers(groupData);
		List<TutorialStep> steps = CreateSteps(groupData);
		try
		{
			tutorialGroupSteps = new TutorialGroupSteps(groupData.GroupID, steps, lockers);
		}
		catch (Exception innerException)
		{
			innerException.SendException($"{GetType().Name}: Can't create {groupData.GetType().Name} with GroupID {groupData.GroupID}");
		}
		saver.Add(tutorialGroupSteps);
		return tutorialGroupSteps;
	}

	private List<TutorialStep> CreateSteps(TutorialGroupSO groupData)
	{
		List<TutorialStep> list = new List<TutorialStep>();
		if (groupData.Steps.Count == 0)
		{
			new Exception().SendException($"{GetType().Name}: id {groupData.GroupID} has empty Steps");
			return list;
		}
		try
		{
			foreach (TutorialStepSO step in groupData.Steps)
			{
				list.Add(stepFactory.Create(step));
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Steps for {groupData.GroupID}");
		}
	}

	private ILocker[] CreateLockers(TutorialGroupSO groupData)
	{
		ILocker[] array = new ILocker[groupData.LockerList.Count];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(groupData.LockerList[i].UnlockType, groupData.LockerList[i].UnlockValue, LockerSourceType.TutorialGroup);
			}
			return array;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create locker for {groupData.GetType()} with ID " + groupData.GroupID);
		}
	}
}
