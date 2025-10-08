namespace GreenT.HornyScapes.Tasks;

[Objective]
public abstract class SavableObjective : Objective<SavableObjectiveData>
{
	public override bool IsComplete => Data.Progress >= Data.Required;

	protected SavableObjective(SavableObjectiveData data)
		: base(data)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		Data.Progress = 0;
	}

	public override int GetTarget()
	{
		return Data.Required;
	}
}
