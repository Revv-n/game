using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialInitializer : StructureInitializer<TutorialConfigSO>
{
	private readonly TutorialGroupManager tutorManager;

	private readonly IFactory<TutorialGroupSO, TutorialGroupSteps> factory;

	public TutorialInitializer(IFactory<TutorialGroupSO, TutorialGroupSteps> factory, TutorialGroupManager tutorManager)
		: base((IEnumerable<IStructureInitializer>)null)
	{
		this.factory = factory;
		this.tutorManager = tutorManager;
	}

	public override IObservable<bool> Initialize(TutorialConfigSO config)
	{
		try
		{
			if (!tutorManager.Collection.Any())
			{
				foreach (TutorialGroupSO group in config.Groups)
				{
					TutorialGroupSteps entity = factory.Create(group);
					tutorManager.Add(entity);
				}
				tutorManager.Initialize();
			}
			return Observable.Do<bool>(Observable.Return(true).Debug("TutorialConfigSO has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}
