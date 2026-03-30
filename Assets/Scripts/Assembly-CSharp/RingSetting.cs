using UnityEngine;

public class RingSetting : MonoBehaviour
{
	public int IndexDegree;

	private Transform[] cTransform;

	private Transform mTransform;

	public bool isLightedRing = false;

	public GameObject lightedRing;
	private void OnEnable()
	{
		lightedRing.SetActive(isLightedRing);
	}

    public void Init(int _IndexDegree)
	{
		mTransform.rotation = Quaternion.identity;
		mTransform.Rotate(new Vector3(0f, 0f, 10 * _IndexDegree));
		IndexDegree = _IndexDegree;
	}

	private void Awake()
	{
		mTransform = base.transform;
		cTransform = new Transform[mTransform.childCount];
		for (int i = 0; i < mTransform.childCount; i++)
		{
			cTransform[i] = mTransform.GetChild(i);
		}
	}

    // RingSetting.cs mein add karo
    public Vector3 GetRoadCenter()
    {
        // Ring ka center world position return karo
        return mTransform.position;
    }
}
