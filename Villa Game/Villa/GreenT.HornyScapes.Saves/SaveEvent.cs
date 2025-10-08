using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public abstract class SaveEvent : MonoBehaviour, IDisposable
{
	protected CompositeDisposable saveStreams = new CompositeDisposable();

	private SaveController saver;

	[Inject]
	private void Init(SaveController saveController)
	{
		saver = saveController;
	}

	public abstract void Track();

	public virtual void StopTrack()
	{
		saveStreams.Clear();
	}

	protected void Save()
	{
		OnStartSaving();
		SaveToLocal();
		SaveToServer();
	}

	protected void SaveToLocal()
	{
		saver.SaveToLocal();
	}

	protected void SaveToServer()
	{
		saver.SaveToServer();
	}

	protected void OnStartSaving()
	{
		saver.OnStartSaving();
	}

	public void Dispose()
	{
		saveStreams.Dispose();
	}
}
