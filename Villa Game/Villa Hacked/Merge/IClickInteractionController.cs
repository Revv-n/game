namespace Merge;

public interface IClickInteractionController : IModuleController
{
	ClickResult Interact(GameItem gameItem);
}
