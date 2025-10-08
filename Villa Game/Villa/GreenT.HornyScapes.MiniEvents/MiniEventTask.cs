using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTask : Task
{
	public CompositeIdentificator Identificator { get; set; }

	public MiniEventTask(TaskActivityMapper mapper, IGoal goal, LinkedContent reward, ILocker locker, ContentType contentType)
		: base(mapper.task_id, goal, reward, locker, contentType)
	{
		Identificator = new CompositeIdentificator(mapper.massive_id);
	}
}
