using UnityEngine;

public class SkyBoxScale : MonoBehaviour
{
	private Transform mTransform;

	private void Awake()
	{
		mTransform = base.transform;
		float num = 1.7777778f;
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = num2 / num;
		mTransform.localScale = new Vector3(mTransform.localScale.x * num3, mTransform.localScale.y * num3, mTransform.localScale.z);
		mTransform.localScale = new Vector3(mTransform.localScale.x, mTransform.localScale.y * (1f / num2), mTransform.localScale.z);
		base.enabled = false;
	}
}
