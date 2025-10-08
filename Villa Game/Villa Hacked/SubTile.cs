public class SubTile : AbstractTile
{
	public bool IsOff => !base.gameObject.activeInHierarchy;
}
