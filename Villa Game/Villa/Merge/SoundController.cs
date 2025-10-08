using System;
using System.Collections;
using System.Collections.Generic;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Sounds;
using StripClub.Messenger;
using StripClub.Model;
using UniRx;
using UnityEngine;
using Zenject;

namespace Merge;

public class SoundController : Controller<SoundController>, IAudioPlayer
{
	public bool Debug;

	[SerializeField]
	private AudioSource oneShotSource;

	[SerializeField]
	private SoundNode prefab;

	[SerializeField]
	private MusicNode musicNodePrefab;

	[SerializeField]
	private FadingSoundNode fadingSoundNode;

	[SerializeField]
	private SoundSO getNewMessage;

	private SmartPool<SoundNode> pool;

	[SerializeField]
	private SoundSettingsSO data;

	private List<string> thisFrameSoundsStarted = new List<string>();

	private bool erasingInProgress;

	private bool onGameLoad;

	private CompositeDisposable audioChangedStream = new CompositeDisposable();

	private readonly Dictionary<AudioClip, SoundNode> _sounds = new Dictionary<AudioClip, SoundNode>(64);

	private GameSettings gameSettings;

	private MusicNode MusicNode
	{
		get
		{
			return MusicNode.Instance;
		}
		set
		{
			MusicNode.Instance = value;
		}
	}

	public SoundSettingsSO Data => data;

	private void Awake()
	{
		Preload();
	}

	public void PlayCurrencySound(CurrencyType currencyType)
	{
		if (gameSettings.CurrencySettings.TryGetValue(currencyType, out var currencySettings))
		{
			AudioClip getSound = currencySettings.GetSound;
			if (getSound != null)
			{
				PlayAudioClip2D(getSound);
			}
		}
	}

	protected override void OnDestroy()
	{
		data.SaveAllData();
		audioChangedStream.Dispose();
		base.OnDestroy();
	}

	public override void Init()
	{
		data.MusicVolume.Subscribe(delegate
		{
			AtMusicChange();
		}).AddTo(audioChangedStream);
		data.SoundVolume.Subscribe(delegate
		{
			AtSoundChange();
		}).AddTo(audioChangedStream);
		MusicNode.SetPlaylist(MusicNode.ClipPaths, MusicNode.CurrentClipId, MusicNode.CurrentTime);
		MusicNode.Resume();
	}

	[Inject]
	private void InnerInit(GameSettings gameSettings, GameStarter gameStarter, IMessengerManager messenger)
	{
		this.gameSettings = gameSettings;
		pool = new SmartPool<SoundNode>(prefab, base.transform);
		data.LoadAllData();
		if (MusicNode == null)
		{
			MusicNode = UnityEngine.Object.Instantiate(musicNodePrefab);
			UnityEngine.Object.DontDestroyOnLoad(MusicNode.gameObject);
		}
		gameStarter.IsDataLoaded.Subscribe(delegate(bool x)
		{
			Init();
			onGameLoad = x;
		}).AddTo(this);
		messenger.OnUpdate.Where(GetNewMessage).Subscribe(delegate
		{
			PlayMessageDelivered();
		});
		static bool GetNewMessage(MessengerUpdateArgs _args)
		{
			if (_args.Message != null)
			{
				return _args.Message.State == ChatMessage.MessageState.Delivered;
			}
			return false;
		}
		void PlayMessageDelivered()
		{
			PlayAudioClip2D(getNewMessage.Sound);
		}
	}

	public void PlaySound(AudioClip clip, bool isLooped = false)
	{
		if (onGameLoad)
		{
			SoundNode soundNode = Controller<SoundController>.Instance.pool.Pop();
			soundNode.Play(clip, data.SoundVolume.Value, Debug, isLooped);
			_sounds[clip] = soundNode;
		}
	}

	public void PlaySound(string name, bool isLooped = false)
	{
		if (onGameLoad && data.SoundVolume.Value != 0f && !thisFrameSoundsStarted.Contains(name))
		{
			if (!erasingInProgress)
			{
				StartCoroutine(CRT_Erase());
			}
			thisFrameSoundsStarted.Add(name);
			AudioClip audioClip = Resources.Load<AudioClip>("Sounds/" + name);
			SoundNode soundNode = Controller<SoundController>.Instance.pool.Pop();
			soundNode.Play(audioClip, data.SoundVolume.Value, Debug, isLooped);
			_sounds[audioClip] = soundNode;
		}
	}

	public void StopSound(AudioClip clip)
	{
		if (onGameLoad && _sounds.TryGetValue(clip, out var value))
		{
			_sounds.Remove(clip);
			value.Stop();
		}
	}

	private IEnumerator CRT_Erase()
	{
		erasingInProgress = true;
		yield return new WaitForEndOfFrame();
		thisFrameSoundsStarted.Clear();
		erasingInProgress = false;
	}

	private void AtSoundChange()
	{
	}

	private void AtMusicChange()
	{
		MusicNode.Volume = data.MusicVolume.Value;
	}

	public void PlayAudioClip2D(AudioClip clip, bool isLooped = false)
	{
		if (onGameLoad)
		{
			_ = Debug;
			PlaySound(clip, isLooped);
		}
	}

	public void PlayAudioClip2D(SoundSO soundSo, bool isLooped = false)
	{
		PlayAudioClip2D(soundSo.Sound, isLooped);
	}

	public void StopAudioClip2D(AudioClip clip)
	{
		if (onGameLoad)
		{
			_ = Debug;
			StopSound(clip);
		}
	}

	public void StopPlayingClip()
	{
		throw new NotImplementedException();
	}

	public void PlayOneShotAudioClip2D(AudioClip clip)
	{
		if (onGameLoad)
		{
			_ = Debug;
			oneShotSource.volume = data.SoundVolume.Value;
			oneShotSource.PlayOneShot(clip);
		}
	}

	public void PlayFadingAudioClip2D(AudioClip clip)
	{
		fadingSoundNode.Play(clip, data.SoundVolume.Value);
	}

	public void MuteAll(bool mute)
	{
		ChangeSound(mute ? 0f : data.DefaultSoundVolume);
		ChangeMusic(mute ? 0f : data.DefaultMusicVolume);
	}

	public void ChangeSound(float value)
	{
		data.SoundVolume.Value = value;
	}

	public void ChangeSound(bool mute)
	{
		ChangeSound(mute ? 0f : data.DefaultSoundVolume);
	}

	public void ChangeMusic(float value)
	{
		data.MusicVolume.Value = value;
	}

	public void ChangeMusic(bool mute)
	{
		ChangeMusic(mute ? 0f : data.DefaultMusicVolume);
	}

	public SoundNode CreateSoundNode()
	{
		SoundNode soundNode = pool.Pop();
		soundNode.SetVolume(data.SoundVolume.Value);
		soundNode.gameObject.SetActive(value: true);
		return soundNode;
	}

	public void ReleaseSoundNode(SoundNode node)
	{
		node.Stop();
	}
}
