using GreenT.HornyScapes.Sounds;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public abstract class BaseObjectConfig : ScriptableObject
{
	[SerializeField]
	private Merge.Meta.RoomObjects.Behaviour behavior;

	[SerializeField]
	private string objectName;

	[Tooltip("Иконка прогресса объекта для ивента (маленькая)")]
	[SerializeField]
	private Sprite _previewIcon;

	[Tooltip("Иконка для карточки награды")]
	[SerializeField]
	private Sprite _cardViewIcon;

	[SerializeField]
	private IntSpriteDictionary iconsBySkin = new IntSpriteDictionary();

	[SerializeField]
	private int room;

	[SerializeField]
	private int serialNumber;

	[SerializeField]
	private Sprite taskIcon;

	[SerializeField]
	private Vector2 bordPosition;

	[SerializeField]
	private RoomObjectSoundSO changeStateSound;

	public RoomObjectSoundSO ChangeStateSound
	{
		get
		{
			return changeStateSound;
		}
		set
		{
			changeStateSound = value;
		}
	}

	public Merge.Meta.RoomObjects.Behaviour Behaviour
	{
		get
		{
			return behavior;
		}
		set
		{
			behavior = value;
		}
	}

	public string ObjectName
	{
		get
		{
			return objectName;
		}
		set
		{
			objectName = value;
		}
	}

	public Sprite TaskIcon
	{
		get
		{
			return taskIcon;
		}
		set
		{
			taskIcon = value;
		}
	}

	public Vector3 BordPosition
	{
		get
		{
			return bordPosition;
		}
		set
		{
			bordPosition = value;
		}
	}

	public int RoomID
	{
		get
		{
			return room;
		}
		set
		{
			room = value;
		}
	}

	public int Number
	{
		get
		{
			return serialNumber;
		}
		set
		{
			serialNumber = value;
		}
	}

	public CompositeIdentificator ID => new CompositeIdentificator(RoomID, Number);

	public IntSpriteDictionary Icons => iconsBySkin;

	public Sprite PreviewIcon => _previewIcon;

	public Sprite CardViewIcon => _cardViewIcon;

	public bool NormalBehaviour => behavior == Merge.Meta.RoomObjects.Behaviour.Normal;

	public bool TrashBehaviour => behavior == Merge.Meta.RoomObjects.Behaviour.Trash;

	public bool PresetBehaviour => behavior == Merge.Meta.RoomObjects.Behaviour.Preset;

	public bool StaticBehaviour => behavior == Merge.Meta.RoomObjects.Behaviour.Static;

	public override string ToString()
	{
		return base.ToString() + " ID:" + ID.ToString();
	}
}
