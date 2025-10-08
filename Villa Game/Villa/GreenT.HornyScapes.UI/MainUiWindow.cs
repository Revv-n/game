using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class MainUiWindow : Window
{
	[SerializeField]
	private AnimationSetOpenCloseController generalStarters;

	private CompositeDisposable disposables = new CompositeDisposable();

	public TaskBookNotify TaskNotify;

	protected override void Awake()
	{
		base.Awake();
		generalStarters.InitOpeners();
	}

	public override void Open()
	{
		disposables.Clear();
		base.Open();
		generalStarters.Open().DoOnCancel(generalStarters.InitClosers).Subscribe(delegate
		{
			generalStarters.InitClosers();
		})
			.AddTo(disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		generalStarters.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			base.Close();
		})
			.AddTo(disposables);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposables.Dispose();
	}
}
