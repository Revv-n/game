using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Merge.Core.Collection;

public class ContainerWindowItem : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private LocalizedTextMeshPro levelTitle;

	private IMergeIconProvider _iconProvider;

	private GameItemConfigManager _gameItemConfigManager;

	public GIData Data { get; private set; }

	[Inject]
	public void Init(IMergeIconProvider iconProvider, GameItemConfigManager gameItemConfigManager)
	{
		_iconProvider = iconProvider;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public ContainerWindowItem SetItem(GIData data, bool ClockAnabled)
	{
		if (Data == data)
		{
			return this;
		}
		Data = data;
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(data.Key);
		icon.sprite = _iconProvider.GetSprite(data.Key);
		levelTitle.SetArguments(configOrNull.Level);
		return this;
	}
}
