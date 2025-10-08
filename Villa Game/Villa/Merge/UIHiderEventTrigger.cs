namespace Merge;

public class UIHiderEventTrigger : UIHiderBase
{
	public override bool IsVisible
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		protected set
		{
			base.gameObject.SetActive(value);
		}
	}

	public override void DoVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public override void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}
}
