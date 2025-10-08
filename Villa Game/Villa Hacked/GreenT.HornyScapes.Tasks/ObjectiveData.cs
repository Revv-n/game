namespace GreenT.HornyScapes.Tasks;

public class ObjectiveData
{
	public int Required { get; protected set; }

	public ObjectiveData(int required)
	{
		Required = required;
	}
}
