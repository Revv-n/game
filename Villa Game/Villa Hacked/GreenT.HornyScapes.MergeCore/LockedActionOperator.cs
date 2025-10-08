using System;
using GreenT.HornyScapes.MergeCore.GameItemBox;
using Merge;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class LockedActionOperator : ModuleActionOperatorSimple
{
	private IDisposable currencyStream;

	public override GIModuleType Type => GIModuleType.Locked;

	public Locked LockedBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return LockedBox;
	}

	protected override void Start()
	{
		base.Start();
		currencyStream = ObservableExtensions.Subscribe<int>((IObservable<int>)CurrencyProcessor.GetCountReactiveProperty(CurrencyType.Hard), (Action<int>)SetState);
	}

	private void OnDestroy()
	{
		currencyStream?.Dispose();
	}

	public override void SetBox(GIBox.Base box)
	{
		LockedBox = box as Locked;
		SetState(LockedBox.Data.UnlockPrice);
		bool flag = !LockedBox.Data.BlocksMerge;
		bool flag2 = LockedBox.Data.UnlockPrice >= 0;
		base.gameObject.SetActive(flag && flag2);
		block.SetButtonLabelText(LockedBox.Data.UnlockPrice.ToString());
		base.IsActive = true;
	}
}
