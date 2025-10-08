using GreenT.HornyScapes.Animations;

namespace GreenT.HornyScapes.Info.UI;

public class InfoMergeWindow : PopupWindow
{
	public override void Open()
	{
		base.Open();
		fadeCloser.gameObject.SetActive(value: true);
	}

	public override void Close()
	{
		base.Close();
		fadeCloser.gameObject.SetActive(value: false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
