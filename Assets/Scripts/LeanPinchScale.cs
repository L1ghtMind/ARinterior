using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to scale the current GameObject relative to the specified camera using the pinch gesture.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanPinchScale")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Pinch Scale")]
	public class LeanPinchScale : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera that will be used to calculate the zoom.
		/// None = MainCamera.</summary>
		[Tooltip("The camera that will be used to calculate the zoom.\n\nNone = MainCamera.")]
		public Camera Camera;

		/// <summary>Should the scaling be performanced relative to the finger center?</summary>
		[Tooltip("Should the scaling be performanced relative to the finger center?")]
		public bool Relative;
		
		/// <summary>The sensitivity of the scaling.
		/// 1 = Default.
		/// 2 = Double.</summary>
		[Tooltip("The sensitivity of the scaling.\n\n1 = Default.\n2 = Double.")]
		public float Sensitivity = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		[FSA("Dampening")] public float Damping = -1.0f;

		[HideInInspector]
		[SerializeField]
		private Vector3 remainingScale;

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
			
			var oldScale = transform.localPosition;

			
			var fingers = Use.GetFingers();

			
			var pinchScale = LeanGesture.GetPinchScale(fingers);

			if (pinchScale != 1.0f)
			{
				pinchScale = Mathf.Pow(pinchScale, Sensitivity);

				
				if (Relative == true)
				{
					var pinchScreenCenter = LeanGesture.GetScreenCenter(fingers);

					if (transform is RectTransform)
					{
						TranslateUI(pinchScale, pinchScreenCenter);
					}
					else
					{
						Translate(pinchScale, pinchScreenCenter);
					}
				}

				transform.localScale *= pinchScale;

				remainingScale += transform.localPosition - oldScale;
			}

			
			var factor = LeanHelper.GetDampenFactor(Damping, Time.deltaTime);

			
			var newRemainingScale = Vector3.Lerp(remainingScale, Vector3.zero, factor);

			
			transform.localPosition = oldScale + remainingScale - newRemainingScale;

			
			remainingScale = newRemainingScale;
		}

		protected virtual void TranslateUI(float pinchScale, Vector2 pinchScreenCenter)
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

			
			var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, transform.position);

			
			screenPoint.x = pinchScreenCenter.x + (screenPoint.x - pinchScreenCenter.x) * pinchScale;
			screenPoint.y = pinchScreenCenter.y + (screenPoint.y - pinchScreenCenter.y) * pinchScale;

			// перетворення
			var worldPoint = default(Vector3);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, camera, out worldPoint) == true)
			{
				transform.position = worldPoint;
			}
		}

		protected virtual void Translate(float pinchScale, Vector2 screenCenter)
		{
			// Make sure the camera exists
			var camera = LeanHelper.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				// Екранне положення перетворення
				var screenPosition = camera.WorldToScreenPoint(transform.position);

				// зсув контрольної точки
				screenPosition.x = screenCenter.x + (screenPosition.x - screenCenter.x) * pinchScale;
				screenPosition.y = screenCenter.y + (screenPosition.y - screenCenter.y) * pinchScale;

				// перетворення до звичних координат
				transform.position = camera.ScreenToWorldPoint(screenPosition);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}