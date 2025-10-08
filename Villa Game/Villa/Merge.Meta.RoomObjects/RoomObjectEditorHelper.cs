using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta.RoomObjects;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class RoomObjectEditorHelper : MonoBehaviour
{
	[SerializeField]
	private RoomObjectConfig initedConfig;

	public int room;

	private Transform root;

	[SerializeField]
	private List<RoomObjectEditorView> views = new List<RoomObjectEditorView>();

	[SerializeField]
	private List<GreenT.HornyScapes.Animations.Animation> beforeChangeAnimations = new List<GreenT.HornyScapes.Animations.Animation>();

	[SerializeField]
	private List<GreenT.HornyScapes.Animations.Animation> afterChangeAnimations = new List<GreenT.HornyScapes.Animations.Animation>();

	[SerializeField]
	private RoomObjectAnimationGroup roomObjectBeforeChangeAnimationGroup;

	[SerializeField]
	private RoomObjectAnimationGroup roomObjectAfterChangeAnimationGroup;

	public IEnumerable<RoomObjectEditorView> Views => views.AsEnumerable();

	public bool IsVisible
	{
		get
		{
			return root.gameObject.activeSelf;
		}
		set
		{
			SetVisible(value);
		}
	}

	protected RoomObjectEditorView HelperPrefab { get; private set; }

	public void Init(RoomObjectEditorView prefab)
	{
		HelperPrefab = prefab;
	}

	public void Init(RoomObjectConfig config)
	{
		Debug.Log("Init on " + base.gameObject.name);
		room = config.RoomID;
		if (root == null)
		{
			root = new GameObject("Root").transform;
			root.SetParent(base.transform);
			root.localPosition = Vector3.zero;
		}
		int count = views.Count;
		for (int i = 0; i != config.Views.Count; i++)
		{
			RoomObjectEditorView roomObjectEditorView;
			if (i < count)
			{
				roomObjectEditorView = views[i];
			}
			else
			{
				roomObjectEditorView = CreateView();
				views.Add(roomObjectEditorView);
			}
			roomObjectEditorView.Set(config.Views[i]);
			roomObjectEditorView.SetActiveSkin((config.Behaviour == Behaviour.Normal) ? 1 : 0);
			roomObjectEditorView.Display(isOn: true);
			CreateAnimations(roomObjectEditorView);
		}
		for (int j = config.Views.Count; j < count; j++)
		{
			views[j].Display(isOn: false);
		}
		initedConfig = config;
		roomObjectBeforeChangeAnimationGroup = base.gameObject.AddComponent<RoomObjectAnimationGroup>();
		roomObjectAfterChangeAnimationGroup = base.gameObject.AddComponent<RoomObjectAnimationGroup>();
		roomObjectBeforeChangeAnimationGroup.SetAnimations(beforeChangeAnimations);
		roomObjectAfterChangeAnimationGroup.SetAnimations(afterChangeAnimations);
	}

	public RoomObjectEditorView CreateView()
	{
		RoomObjectEditorView roomObjectEditorView = Object.Instantiate(HelperPrefab, root);
		roomObjectEditorView.transform.localScale = Vector3.one;
		return roomObjectEditorView;
	}

	public void RemoveView(int index)
	{
		Object.DestroyImmediate(views[index].gameObject);
		views.RemoveAt(index);
	}

	public void SelectSkin(int skin)
	{
		foreach (RoomObjectEditorView view in views)
		{
			view.SetActiveSkin(skin);
		}
	}

	public void Select(int index = -1)
	{
		if (views.Count != 0)
		{
			Mathf.Clamp(index, 0, views.Count - 1);
		}
	}

	public void SetVisible(bool visible)
	{
		root.gameObject.SetActive(visible);
	}

	public void CreateAnimations(RoomObjectEditorView view)
	{
		if (view.AfterChangeAnimation != null)
		{
			GreenT.HornyScapes.Animations.Animation animation = Object.Instantiate(view.AfterChangeAnimation, view.transform);
			animation.name = "ShowAnimation";
			TransformAnimation[] components = animation.GetComponents<TransformAnimation>();
			foreach (TransformAnimation item in components)
			{
				beforeChangeAnimations.Add(item);
			}
		}
		if (view.BeforeChangeAnimation != null)
		{
			GreenT.HornyScapes.Animations.Animation animation2 = Object.Instantiate(view.BeforeChangeAnimation, view.transform);
			animation2.name = "ChangeAnimation";
			TransformAnimation[] components = animation2.GetComponents<TransformAnimation>();
			foreach (TransformAnimation transformAnimation in components)
			{
				afterChangeAnimations.Add(transformAnimation);
				transformAnimation.Transform = view.transform;
				transformAnimation.Renderer = view.transform.GetChild(0).GetComponent<Renderer>();
			}
		}
		Debug.Log("On " + base.gameObject.name + "ChangeAnimationsCount: " + afterChangeAnimations.Count);
	}

	public void PlayAnim()
	{
		Debug.Log("Try play editor anim! ChangeCount: " + afterChangeAnimations.Count);
		if (roomObjectAfterChangeAnimationGroup.Count != 0)
		{
			Debug.Log("Play anim!");
			roomObjectAfterChangeAnimationGroup.Play();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlayAnim();
		}
	}
}
