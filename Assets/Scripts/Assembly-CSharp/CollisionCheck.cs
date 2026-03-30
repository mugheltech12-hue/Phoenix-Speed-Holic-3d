using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
	public GameManager GM;

	private void OnTriggerEnter(Collider other)
	{
		print(other.gameObject.name);

		int layer = other.gameObject.layer;
		if (layer == 9)
		{
			GM.SendMessage("SetOver");
		}
	}
}
