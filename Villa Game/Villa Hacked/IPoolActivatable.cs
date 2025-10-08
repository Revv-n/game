public interface IPoolActivatable
{
	bool IsActiveForPool { get; }

	void SetActiveForPool(bool active);
}
