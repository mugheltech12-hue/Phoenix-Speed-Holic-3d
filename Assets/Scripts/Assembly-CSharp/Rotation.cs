using UnityEngine;

public class Rotation : MonoBehaviour
{
	private Transform mTransform;

	public float degree;

	private void Awake()
	{
		mTransform = base.transform;
	}

	private void Update()
	{
		mTransform.Rotate(new Vector3(0f, 0f, degree * Time.deltaTime));
	}
}
