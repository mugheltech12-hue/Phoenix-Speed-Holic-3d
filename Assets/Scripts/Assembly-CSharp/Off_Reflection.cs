using UnityEngine;
using UnityEngine.Rendering;

public class Off_Reflection : MonoBehaviour
{
	public void Action()
	{
		OFF(base.transform);
	}

	public void Remove()
	{
		Object.DestroyImmediate(GetComponent<Off_Reflection>(), true);
	}

	private void OFF(Transform _mTransform)
	{
		if (_mTransform.GetComponent<MeshRenderer>() != null)
		{
			_mTransform.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		foreach (Transform item in _mTransform)
		{
			OFF(item);
		}
	}
}
