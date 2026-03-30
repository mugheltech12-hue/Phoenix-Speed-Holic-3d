using UnityEngine;

public class UIDynamicPosition : MonoBehaviour
{
	private float standard_Aspect = 1.7777778f;

	private float device_Aspect = (float)Screen.width / (float)Screen.height;

	private void Awake()
	{
		Transform mTransform = base.transform;
		CheckChild(mTransform);
	}

	private void CheckChild(Transform _mTransform)
	{
		foreach (Transform item in _mTransform)
		{
			item.localPosition = new Vector3(device_Aspect / standard_Aspect * item.localPosition.x, item.localPosition.y, item.localPosition.z);
			if (item.GetComponent<TextMesh>() != null)
			{
				item.GetComponent<TextMesh>().characterSize *= device_Aspect / standard_Aspect;
			}
			CheckChild(item);
		}
	}
}
