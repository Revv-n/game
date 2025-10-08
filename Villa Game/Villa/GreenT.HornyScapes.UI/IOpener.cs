namespace GreenT.HornyScapes.UI;

public interface IOpener<in T>
{
	void Open(T @object);
}
