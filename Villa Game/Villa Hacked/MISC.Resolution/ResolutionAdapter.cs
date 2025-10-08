using UnityEngine;

namespace MISC.Resolution;

public abstract class ResolutionAdapter : MonoBehaviour, IResolutionAdapter
{
	protected void Start()
	{
		ResolutionType resolution = ResolutionAdapterMaster.Resolution;
		Adaptate(resolution);
	}

	public abstract void Adaptate(ResolutionType type);

	protected abstract void WritePreset(ResolutionType type);

	[ContextMenu("WriteAll")]
	private void WriteAll()
	{
		WriteIPad();
		WriteOld();
		WriteWide();
	}

	[ContextMenu("WriteIPad")]
	private void WriteIPad()
	{
		WriteAndSave(ResolutionType.IPad);
	}

	[ContextMenu("WriteOld")]
	private void WriteOld()
	{
		WriteAndSave(ResolutionType.Old);
	}

	[ContextMenu("WriteWide")]
	private void WriteWide()
	{
		WriteAndSave(ResolutionType.Wide);
	}

	private void WriteAndSave(ResolutionType type)
	{
		WritePreset(type);
		SafeEditor.SaveUndoAction(this, "WritePreset");
	}
}
