using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace exploration
{
    public class GPButtonCustomDoor : GoPointerButton
    {
		public Transform importedActualBoat;
		public BoatEmbarkCollider embarkCol;
		private Transform walkCol;
		public float slideDistance;
		private bool open;
		private bool inMotion;
		public InteriorEffectsTrigger[] interiorAudios;
		public float angle;

		public void Setup()
		{
			transform.parent = importedActualBoat;
			BoxCollider component = GetComponent<BoxCollider>();
			BoxCollider boxCollider = new GameObject("trapdoor").AddComponent<BoxCollider>();
			boxCollider.center = component.center;
			boxCollider.size = component.size;
			boxCollider.transform.parent = embarkCol.walkCollider;
			boxCollider.transform.localPosition = transform.localPosition;
			boxCollider.transform.localRotation = transform.localRotation;
			boxCollider.transform.localScale = transform.localScale;
			walkCol = boxCollider.transform;
		}

		public bool IsOpen()
        {
            return open;
        }

		public override void OnActivate()
		{
			if (inMotion)
			{
				return;
			}
			if (open)
			{
				StartCoroutine(OpenOrClose(-1));
				return;
			}
			StartCoroutine(OpenOrClose(1));
		}

		private IEnumerator OpenOrClose(int direction)
		{
			inMotion = true;

			if (direction > 0)
			{
				open = true;
			}
			else
			{
				open = false;
			}

			//InteriorEffectsTrigger[] array = interiorAudios;

			//for (int i = 0; i < array.Length; i++)
			//{
			//	array[i].Refresh();
			//}

			float progress = 0f;

			while (progress < angle)
			{
				float num = Time.deltaTime * 125f;

				if (progress + num > angle)
				{
					num = angle - progress;
				}

				if (slideDistance != 0f)
				{
					transform.Translate(Vector3.up * (float)direction * num * slideDistance / angle, Space.Self);
					walkCol.localPosition = transform.localPosition;
				}
				else
				{
					transform.Rotate(Vector3.up, num * (float)(-(float)direction), Space.Self);
					walkCol.localRotation = transform.localRotation;
				}

				progress += num;

				yield return new WaitForEndOfFrame();
			}

			inMotion = false;

			yield break;
		}
	}
}
