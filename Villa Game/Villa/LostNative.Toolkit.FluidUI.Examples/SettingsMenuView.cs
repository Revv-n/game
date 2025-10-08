using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LostNative.Toolkit.FluidUI.Examples;

public class SettingsMenuView : MonoBehaviour
{
	[SerializeField]
	private FluidToggle musicToggle;

	[SerializeField]
	private FluidToggle sfxToggle;

	[SerializeField]
	private FluidToggle notificationsToggle;

	[SerializeField]
	private FluidToggle hintsToggle;

	private void Start()
	{
		FluidToggle fluidToggle = musicToggle;
		fluidToggle.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Combine(fluidToggle.OnToggle, new FluidToggle.ToggleDelegate(MusicToggle_OnToggle));
		FluidToggle fluidToggle2 = sfxToggle;
		fluidToggle2.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Combine(fluidToggle2.OnToggle, new FluidToggle.ToggleDelegate(SfxToggle_OnToggle));
		FluidToggle fluidToggle3 = notificationsToggle;
		fluidToggle3.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Combine(fluidToggle3.OnToggle, new FluidToggle.ToggleDelegate(NotificationsToggle_OnToggle));
		FluidToggle fluidToggle4 = hintsToggle;
		fluidToggle4.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Combine(fluidToggle4.OnToggle, new FluidToggle.ToggleDelegate(HintsToggle_OnToggle));
	}

	private void MusicToggle_OnToggle(bool optionASelected)
	{
		Debug.Log(optionASelected ? "Music off" : "Music on");
	}

	private void SfxToggle_OnToggle(bool optionASelected)
	{
		Debug.Log(optionASelected ? "SFX off" : "SFX on");
	}

	private void NotificationsToggle_OnToggle(bool optionASelected)
	{
		Debug.Log(optionASelected ? "Notifications off" : "Notifications on");
	}

	private void HintsToggle_OnToggle(bool optionASelected)
	{
		Debug.Log(optionASelected ? "Hints off" : "Hints on");
	}

	private void RemoveListeners()
	{
		FluidToggle fluidToggle = musicToggle;
		fluidToggle.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Remove(fluidToggle.OnToggle, new FluidToggle.ToggleDelegate(MusicToggle_OnToggle));
		FluidToggle fluidToggle2 = sfxToggle;
		fluidToggle2.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Remove(fluidToggle2.OnToggle, new FluidToggle.ToggleDelegate(SfxToggle_OnToggle));
		FluidToggle fluidToggle3 = notificationsToggle;
		fluidToggle3.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Remove(fluidToggle3.OnToggle, new FluidToggle.ToggleDelegate(NotificationsToggle_OnToggle));
		FluidToggle fluidToggle4 = hintsToggle;
		fluidToggle4.OnToggle = (FluidToggle.ToggleDelegate)Delegate.Remove(fluidToggle4.OnToggle, new FluidToggle.ToggleDelegate(HintsToggle_OnToggle));
	}

	private void OnDestroy()
	{
		RemoveListeners();
	}

	[UsedImplicitly]
	public void OnOkClick()
	{
		Debug.Log("Ok clicked!");
	}

	[UsedImplicitly]
	public void OnCloseClick()
	{
		Debug.Log("Close clicked!");
	}
}
