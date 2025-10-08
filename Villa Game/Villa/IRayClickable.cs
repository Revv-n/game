public interface IRayClickable
{
	bool IsRayClickEnable { get; }

	RayClickOrder RayClickOrder { get; }

	void AtRayClick();
}
