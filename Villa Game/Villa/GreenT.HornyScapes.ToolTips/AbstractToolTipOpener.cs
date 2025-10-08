using System;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public abstract class AbstractToolTipOpener<TSettings, TView> : MonoBehaviour, IToolTipOpener<TSettings> where TSettings : ToolTipSettings where TView : IView<TSettings>
{
	[SerializeField]
	private Transform container;

	[SerializeField]
	private TSettings settings;

	protected IViewManager<TSettings, TView> viewManager;

	protected TimeSpan showTime;

	protected IDisposable TimerDisposable;

	protected readonly Subject<TView> open = new Subject<TView>();

	protected readonly Subject<TView> close = new Subject<TView>();

	protected TView view;

	public TSettings Settings
	{
		get
		{
			return settings;
		}
		protected set
		{
			settings = value;
			settings.Parent = container;
		}
	}

	public ReactiveProperty<bool> IsPlaying { get; } = new ReactiveProperty<bool>();


	public IObservable<TView> OnOpen => open.AsObservable();

	public IObservable<TView> OnClose => close.AsObservable();

	[Inject]
	protected virtual void InitManager(IViewManager<TSettings, TView> manager)
	{
		viewManager = manager;
	}

	public virtual void Awake()
	{
		if (settings != null)
		{
			Init(settings);
		}
	}

	public virtual void Init(TSettings settings)
	{
		Settings = UnityEngine.Object.Instantiate(settings);
	}

	protected virtual IObservable<Unit> ShutDownObservable()
	{
		return Observable.Timer(showTime).AsUnitObservable();
	}

	public virtual void OpenToolTip(TSettings settings)
	{
		if (!IsPlaying.Value)
		{
			TimerDisposable?.Dispose();
			IsPlaying.Value = true;
			view = viewManager.Display(settings);
			open.OnNext(view);
			TimerDisposable = ShutDownObservable().DoOnCancel(ReleaseView).Subscribe(delegate
			{
				ReleaseView();
			});
		}
	}

	private void ReleaseView()
	{
		close.OnNext(view);
		ref TView reference = ref view;
		TView val = default(TView);
		if (val == null)
		{
			val = reference;
			reference = ref val;
			if (val == null)
			{
				goto IL_0046;
			}
		}
		reference.Display(isOn: false);
		goto IL_0046;
		IL_0046:
		IsPlaying.Value = false;
		view = default(TView);
	}

	private void OnDestroy()
	{
		TimerDisposable?.Dispose();
		open?.Dispose();
		close?.Dispose();
		IsPlaying?.Dispose();
	}

	private void OnDisable()
	{
		TimerDisposable?.Dispose();
	}

	protected virtual void OnValidate()
	{
		if (!container)
		{
			container = base.transform;
		}
	}
}
