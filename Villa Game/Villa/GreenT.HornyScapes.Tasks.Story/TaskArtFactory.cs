using System;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Tasks.Story;

public class TaskArtFactory : IFactory<TaskArtMapper, TaskArt>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public TaskArtFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.lockerFactory = lockerFactory;
	}

	public TaskArt Create(TaskArtMapper mapper)
	{
		ILocker[] array = new ILocker[mapper.unlock_type.Length];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.TaskStory);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't create locker for TaskStoryID " + mapper.id);
		}
		TaskArt taskArt = null;
		try
		{
			return new TaskArt(mapper.id, array);
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException($"Can't create TaskStory with ID: {mapper.id}");
		}
	}
}
