using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to translate the current GameObject relative to the camera using the finger drag gesture.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTranslate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate")]
	public class LeanDragTranslate : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera the translation will be calculated using.\n\nNone = MainCamera.</summary>
		[Tooltip("The camera the translation will be calculated using.\n\nNone = MainCamera.")]
		public Camera Camera;

		/// <summary>The sensitivity of the translation.
		/// 1 = Default.
		/// 2 = Double.</summary>
		[Tooltip("The sensitivity of the translation.\n\n1 = Default.\n2 = Double.")]
		public float Sensitivity = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		[FSA("Dampening")] public float Damping = -1.0f;

		/// <summary>This allows you to control how much momenum is retained when the dragging fingers are all released.
		/// NOTE: This requires <b>Dampening</b> to be above 0.</summary>
		[Tooltip("This allows you to control how much momenum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Dampening</b> to be above 0.")]
		[Range(0.0f, 1.0f)]
		public float Inertia;

		[HideInInspector]
		[SerializeField]
		private Vector3 remainingTranslation;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			
			var oldPosition = transform.localPosition;

			
			var fingers = Use.GetFingers();

			
			var screenDelta = LeanGesture.GetScreenDelta(fingers);

			if (screenDelta != Vector2.zero)
			{
				
				if (transform is RectTransform)
				{
					TranslateUI(screenDelta);
				}
				else
				{
					Translate(screenDelta);
				}
			}

			
			remainingTranslation += transform.localPosition - oldPosition;

			
			var factor = LeanHelper.GetDampenFactor(Damping, Time.deltaTime);

			
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);

			
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;

			if (fingers.Count == 0 && Inertia > 0.0f && Damping > 0.0f)
			{
				newRemainingTranslation = Vector3.Lerp(newRemainingTranslation, remainingTranslation, Inertia);
			}

			
			remainingTranslation = newRemainingTranslation;
		}

		private void TranslateUI(Vector2 screenDelta)
		{
			var camera = Camera;

			if (camera == null)
			{
				var canvas = transform.GetComponentInParent<Canvas>();

				if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
				{
					camera = canvas.worldCamera;
				}
			}

			// трансформування позиції екрану
			var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, transform.position);

			// додаємо дельту
			screenPoint += screenDelta * Sensitivity;

			// перетворення до звичних координат
			var worldPoint = default(Vector3);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, camera, out worldPoint) == true)
			{
				transform.position = worldPoint;
			}
		}

		private void Translate(Vector2 screenDelta)
		{
			// перевіряємо камеру
			var camera = LeanHelper.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// трансформування позиції екрану
				var screenPoint = camera.WorldToScreenPoint(transform.position);

				//дельта
				screenPoint += (Vector3)screenDelta * Sensitivity;

				//перетворення до звичних координат
				transform.position = camera.ScreenToWorldPoint(screenPoint);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
			}
		}
	}
}