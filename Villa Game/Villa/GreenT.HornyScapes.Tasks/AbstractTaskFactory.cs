using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Quest;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public abstract class AbstractTaskFactory<TTaskMapper, TTask> : IFactory<TTaskMapper, TTask>, IFactory where TTaskMapper : TaskMapper where TTask : Task
{
	protected readonly LockerFactory lockerFactory;

	protected readonly IFactory<Task, Dictionary<StateType, TaskStateBase>> taskStateFactory;

	protected readonly LinkedContentFactory contentFactory;

	protected readonly GoalFactory goalFactory;

	protected readonly ISaver saver;

	protected readonly TaskContentTypeEnumConverter taskContentTypeEnumConverter;

	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	public AbstractTaskFactory(LockerFactory lockerFactory, LinkedContentFactory contentFactory, GoalFactory goalFactory, ISaver saver, LinkedContentAnalyticDataFactory analyticDataFactory, TaskContentTypeEnumConverter taskContentTypeEnumConverter, IFactory<Task, Dictionary<StateType, TaskStateBase>> taskStateFactory)
	{
		this.lockerFactory = lockerFactory;
		this.taskStateFactory = taskStateFactory;
		this.contentFactory = contentFactory;
		this.goalFactory = goalFactory;
		this.saver = saver;
		this.analyticDataFactory = analyticDataFactory;
		this.taskContentTypeEnumConverter = taskContentTypeEnumConverter;
	}

	public TTask Create(TTaskMapper mapper)
	{
		CompositeLocker locker = CreateLocker(mapper);
		TTask val = null;
		try
		{
			ContentType contentType = taskContentTypeEnumConverter.Convert(mapper.content_type);
			Goal goal = CreateGoal(mapper, contentType);
			LinkedContent content = GetContent(mapper);
			val = CreateTask(mapper, goal, content, locker, contentType);
			val.SetStates(taskStateFactory.Create(val));
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create task with ID:  {mapper.task_id}");
		}
		val.Initialize();
		saver.Add(val);
		return val;
	}

	public abstract TTask CreateTask(TTaskMapper mapper, IGoal goal, LinkedContent content, ILocker locker, ContentType contentType);

	public abstract Goal CreateGoal(TTaskMapper mapper, ContentType contentType);

	private CompositeLocker CreateLocker(TTaskMapper mapper)
	{
		if (mapper is TaskActivityMapper)
		{
			return new CompositeLocker(new List<ILocker>());
		}
		ILocker[] array = new ILocker[mapper.unlock_type.Length];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.Task);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": Don't equal count of unlock type-value in ID: " + mapper.task_id);
		}
		return new CompositeLocker(array);
	}

	private LinkedContent GetContent(TaskMapper mapper)
	{
		LinkedContent linkedContent = null;
		for (int i = 0; i < mapper.rew_id.Length; i++)
		{
			Selector selector = SelectorTools.CreateSelector(mapper.rew_id[i]);
			LinkedContentAnalyticData analyticData = analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Task);
			LinkedContent linkedContent2 = contentFactory.Create(mapper.rew_type[i], selector, mapper.rew_qty[i], 0, ContentType.Main, analyticData);
			if (linkedContent == null)
			{
				linkedContent = linkedContent2;
			}
			else
			{
				linkedContent.Insert(linkedContent2);
			}
		}
		return linkedContent;
	}
}
