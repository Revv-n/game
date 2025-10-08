using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public interface ICameraChanger
{
	Camera MergeCamera { get; }

	Camera MainCamera { get; }

	void ChangeCamera(IWindow window);

	void AddCameraToWindow(IWindow window, Camera camera);

	void RemovewCameraByWindow(IWindow window);
}
