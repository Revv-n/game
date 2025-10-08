using GreenT.Data;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;

namespace GreenT.HornyScapes.Events;

public class FieldCleaner
{
	private readonly ISaver saver;

	private readonly MergeFieldProvider mergeFieldProvider;

	private readonly GameItemController gameItemController;

	public FieldCleaner(ISaver saver, MergeFieldProvider mergeFieldProvider)
	{
		this.saver = saver;
		this.mergeFieldProvider = mergeFieldProvider;
	}

	public void Clean(GreenT.HornyScapes.MergeCore.MergeField field)
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		ContentType type = field.Type;
		instance.OpenField(type);
		field.ResetField();
		saver.Delete(field.Data);
		mergeFieldProvider.TryRemove(field);
		instance.CurrentField.FieldMediator.Container.SetActive(value: false);
		instance.OpenField((type != ContentType.Event) ? ContentType.Event : ContentType.Main);
	}
}
