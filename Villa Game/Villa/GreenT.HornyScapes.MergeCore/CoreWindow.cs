using GreenT.HornyScapes.Animations;
using GreenT.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class CoreWindow : Window
{
	public const string counter_key = "{0}/{1}";

	public ScaleButton uiButton;

	public TMP_Text inventoryCounter;

	[SerializeField]
	private RewardContainer rewardContainer;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private CompositeDisposable disposables = new CompositeDisposable();

	[Inject]
	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		starters.InitOpeners();
	}

	public override void Open()
	{
		disposables.Clear();
		base.Open();
		rewardContainer.Display(display: true);
		starters.Open().DoOnCancel(starters.InitClosers).Subscribe(delegate
		{
			starters.InitClosers();
		})
			.AddTo(disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		rewardContainer.Display(display: false);
		starters.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			base.Close();
		})
			.AddTo(disposables);
	}

	private void OnDisable()
	{
		disposables.Clear();
	}

	public void UpdateInventoryCounter(int storedItems, int maxPlace)
	{
		inventoryCounter.text = $"{storedItems}/{maxPlace}";
	}
}
