using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Level;

public class LevelWindow : Window
{
	[Serializable]
	public class InterfaceElementsDictionary : SerializableDictionary<InterfaceElements, InterfaceObject>
	{
	}

	[SerializeField]
	private InterfaceElementsDictionary interfaceElementsDictionary;

	[SerializeField]
	private List<GameObject> activateMetaUI;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private CompositeDisposable disposables = new CompositeDisposable();

	public LevelWindowArgs DisplayArgs { get; private set; }

	public override void Open()
	{
		disposables.Clear();
		base.Open();
		starters.Open().DoOnCancel(starters.InitClosers).Subscribe(delegate
		{
			starters.InitClosers();
		})
			.AddTo(disposables);
	}

	public override void Close()
	{
		disposables.Clear();
		starters.Close().DoOnCancel(base.Close).Subscribe(delegate
		{
			base.Close();
		})
			.AddTo(disposables);
	}

	protected override void SetActive(bool isActive)
	{
		base.SetActive(isActive);
		foreach (GameObject item in activateMetaUI)
		{
			item.SetActive(isActive);
		}
	}

	[Inject]
	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		starters.InitOpeners();
		SetView(new LevelWindowArgs());
	}

	public void SetView(LevelWindowArgs args)
	{
		DisplayArgs = args;
		foreach (KeyValuePair<InterfaceElements, InterfaceObject> item in interfaceElementsDictionary)
		{
			if (item.Value is IInteractable)
			{
				(item.Value as IInteractable).Interactable = args.interactableElements.HasFlag(item.Key);
			}
			if (args.visibleElements.HasFlag(item.Key))
			{
				item.Value.Show();
			}
			else
			{
				item.Value.Hide();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposables?.Dispose();
	}
}
