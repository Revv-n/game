namespace GreenT.HornyScapes;

public interface ISelector<in T>
{
	void Select(T selector);
}
