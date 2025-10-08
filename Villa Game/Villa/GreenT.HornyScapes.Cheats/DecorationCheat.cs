using System.Linq;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using Merge.Meta.RoomObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class DecorationCheat : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _getDecorationButton;

	private DecorationManager _decorationManager;

	private Decoration _decoration;

	private IHouseBuilder _builder;

	[Inject]
	public void Init(DecorationManager decorationManager, IHouseBuilder builder)
	{
		_decorationManager = decorationManager;
		_builder = builder;
	}

	private void Awake()
	{
		OnEnterValue(_inputField.text);
		_inputField.onValueChanged.AddListener(OnEnterValue);
		_getDecorationButton.onClick.AddListener(BuildDecoration);
	}

	private void OnEnterValue(string value)
	{
		if (int.TryParse(value, out var id) && _decorationManager.Collection.Any((Decoration item) => item.ID == id))
		{
			_decoration = _decorationManager.GetItem(id);
			_getDecorationButton.interactable = true;
		}
		else
		{
			_decoration = null;
			_getDecorationButton.interactable = false;
		}
	}

	private void BuildDecoration()
	{
		if (_decoration != null && _decoration.State != EntityStatus.Rewarded)
		{
			_builder.BuildRoomObject(_decoration.HouseObjectID);
		}
	}
}
