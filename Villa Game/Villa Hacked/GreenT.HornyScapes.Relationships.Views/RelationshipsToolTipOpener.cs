using System;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.ToolTips;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class RelationshipsToolTipOpener : MonoView<RelationshipsToolTip>, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private Transform _container;

	[SerializeField]
	private TailedToolTipSettings _settings;

	private IPointerPress _pointerPress;

	private RelationshipsToolTip _toolTip;

	private IDisposable _timerDisposable;

	private readonly Subject<RelationshipsToolTipOpener> _requested = new Subject<RelationshipsToolTipOpener>();

	public ReactiveProperty<bool> IsPlaying { get; } = new ReactiveProperty<bool>();


	public IObservable<RelationshipsToolTipOpener> Requested => Observable.AsObservable<RelationshipsToolTipOpener>((IObservable<RelationshipsToolTipOpener>)_requested);

	[Inject]
	private void Init(IPointerPress pointerPress)
	{
		_pointerPress = pointerPress;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!IsPlaying.Value)
		{
			Request();
		}
	}

	public void Request()
	{
		_requested?.OnNext(this);
	}

	public void ShowToolTip(RelationshipsToolTip toolTip, RectTransform parent, int count)
	{
		if (!IsPlaying.Value)
		{
			_settings.KeyText = $"{count}";
			_settings.Parent = parent;
			_timerDisposable?.Dispose();
			IsPlaying.Value = true;
			_toolTip = toolTip;
			_toolTip.Set(_settings);
			_toolTip.Display(display: true);
			_timerDisposable = ObservableExtensions.Subscribe<Unit>(Observable.DoOnCancel<Unit>(ShutDownObservable(), (Action)HideToolTip), (Action<Unit>)delegate
			{
				HideToolTip();
			});
		}
	}

	public void HideToolTip()
	{
		if (IsPlaying.Value)
		{
			_timerDisposable?.Dispose();
			if (_toolTip != null)
			{
				_toolTip.Display(display: false);
			}
			IsPlaying.Value = false;
		}
	}

	private void Awake()
	{
		if (_settings != null)
		{
			InitSettings(_settings);
		}
	}

	private void OnDisable()
	{
		_timerDisposable?.Dispose();
	}

	private void OnDestroy()
	{
		_timerDisposable?.Dispose();
		IsPlaying?.Dispose();
	}

	private void OnValidate()
	{
		if (_container == null)
		{
			_container = base.transform;
		}
	}

	private void InitSettings(TailedToolTipSettings settings)
	{
		_settings = UnityEngine.Object.Instantiate(settings);
		_settings.Parent = _container;
	}

	private IObservable<Unit> ShutDownObservable()
	{
		return Observable.Take<Unit>(Observable.AsUnitObservable<Vector2>(Observable.TakeUntil<Vector2, bool>(Observable.Skip<Vector2>(_pointerPress.OnPointerClick(), 1), Observable.First<bool>((IObservable<bool>)IsPlaying, (Func<bool, bool>)((bool x) => !x)))), 1);
	}
}
