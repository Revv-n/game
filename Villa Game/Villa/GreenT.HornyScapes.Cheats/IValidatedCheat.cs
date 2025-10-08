namespace GreenT.HornyScapes.Cheats;

public interface IValidatedCheat : ICheat
{
	bool IsValid();

	void Validate();
}
public interface IValidatedCheat<T> : ICheat
{
	bool IsValid(T param);

	void Validate(T param);
}
