using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI.Promote;

public class PictureSwitcher : MonoBehaviour
{
	[SerializeField]
	private CardPictureSelector pictureSelector;

	[SerializeField]
	private ToggleWithLocker togglePrefab;

	[SerializeField]
	private Transform toggleContainer;

	[SerializeField]
	private ToggleGroup toggleGroup;

	[SerializeField]
	private List<ToggleWithLocker> toggles = new List<ToggleWithLocker>();

	protected CharacterSettings characterSettings;

	private IDisposable levelUpStream;

	private void OnEnable()
	{
		pictureSelector.Current.Where((CardPictures _pic) => _pic != null).SelectMany((CardPictures _pic) => _pic.CharacterSettings.OnUpdate.StartWith(_pic.CharacterSettings)).TakeUntilDisable(this)
			.Subscribe(InitToggles);
	}

	private void InitToggles(CharacterSettings characterSettings)
	{
		this.characterSettings = characterSettings;
		SetOnToggles(characterSettings);
		SetLevelUpStream(characterSettings);
		SetOffUnnecessaryToggle(characterSettings);
	}

	private void SetOnToggles(CharacterSettings characterSettings)
	{
		IDictionary<int, Sprite> avatars = characterSettings.Public.GetBundleData().Avatars;
		for (int i = 0; i < avatars.Count; i++)
		{
			ToggleWithLocker toggle = GetToggle(i);
			int level = avatars.Keys.ElementAt(i);
			bool flag = IsAvailableToggle(characterSettings.Promote, level);
			if (characterSettings.AvatarNumber == i)
			{
				toggle.SetIsOnWithoutNotify(value: true);
			}
			toggle.onValueChanged.AddListener(SelectPicture(i));
			toggle.SetLocker(!flag);
		}
	}

	private void SetLevelUpStream(CharacterSettings characterSettings)
	{
		levelUpStream?.Dispose();
		levelUpStream = characterSettings.Promote.Level.Skip(1).Where(IsUpToNeedLevel).TakeUntilDisable(this)
			.Subscribe(OnLevelUp);
		bool IsUpToNeedLevel(int currentLevel)
		{
			return characterSettings.Public.GetBundleData().Avatars.Keys.Any((int _key) => _key == currentLevel);
		}
	}

	private void SetOffUnnecessaryToggle(CharacterSettings characterSettings)
	{
		int num = 0;
		if (characterSettings.SkinID == 0)
		{
			num = characterSettings.Public.GetBundleData().Avatars.Count;
		}
		for (int i = num; i != toggles.Count; i++)
		{
			toggles[i].gameObject.SetActive(value: false);
		}
	}

	protected virtual void OnLevelUp(int currentLevel)
	{
		try
		{
			RefreshToggles(currentLevel);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	private void RefreshToggles(int currentLevel)
	{
		int num = 0;
		foreach (KeyValuePair<int, Sprite> avatar in characterSettings.Public.GetBundleData().Avatars)
		{
			ToggleWithLocker toggle = GetToggle(num);
			bool flag = IsAvailableToggle(characterSettings.Promote, avatar.Key);
			BeforeUnlockNewToggle(toggle, num, !flag);
			UnlockToggle(toggle, flag);
			toggle.isOn = currentLevel == avatar.Key;
			num++;
		}
	}

	protected virtual void BeforeUnlockNewToggle(ToggleWithLocker toggle, int toggleIndex, bool newState)
	{
		if (toggle.IsLock && !newState)
		{
			characterSettings.SetAvatar(toggleIndex);
		}
	}

	protected virtual void UnlockToggle(ToggleWithLocker toggle, bool unlock)
	{
		toggle.SetLocker(!unlock);
		int id = toggles.IndexOf(toggle);
		toggle.onValueChanged.RemoveListener(SelectPicture(id));
		toggle.onValueChanged.AddListener(SelectPicture(id));
	}

	private bool IsAvailableToggle(IPromote promote, int level)
	{
		return promote.Level.Value >= level;
	}

	private ToggleWithLocker GetToggle(int index)
	{
		ToggleWithLocker toggleWithLocker = toggles[index];
		if (toggleWithLocker != null)
		{
			toggleWithLocker.onValueChanged.RemoveAllListeners();
			toggleWithLocker.isOn = false;
			toggleWithLocker.gameObject.SetActive(value: true);
			return toggleWithLocker;
		}
		toggleWithLocker = UnityEngine.Object.Instantiate(togglePrefab, toggleContainer);
		toggleGroup.RegisterToggle(toggleWithLocker);
		toggles.Add(toggleWithLocker);
		return toggleWithLocker;
	}

	private UnityAction<bool> SelectPicture(int id)
	{
		return delegate(bool isActive)
		{
			if (isActive)
			{
				characterSettings.SetAvatar(id);
			}
		};
	}
}
