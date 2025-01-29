using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform CameraTarget; // Target to orbit around
	public float Dist = 10f; // Default zoom distance
	public float RotationSpeed = 25f; // Rotation speed
	public float ZoomSpeed = 2f; // Zoom speed (sensitivity)
	public float SmoothTime = 0.1f; // Smooth transition time

	private Vector2 rotation;
	private Vector2 targetRotation;
	private Vector2 rotationVelocity;
	private float targetDist;
	private float zoomVelocity;
	private bool _isRotating = false;
	private bool _isZooming = false;

	private void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotation.x = angles.y;
		rotation.y = angles.x;
		targetRotation = rotation;
		targetDist = Dist; // Initialize zoom distance
	}

	private void Update()
	{
		bool isCursor = false;
		// Orbiting: Shift + Left Mouse Button
		if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(2))
		{
			_isRotating = true;
			isCursor = true;

			// todo set the target to the center object at the center of the camera position to dynamically rotate.
			// todo: also add panning
			float mouseX = -Input.GetAxis("Mouse X");
			float mouseY = -Input.GetAxis("Mouse Y");

			targetRotation.x += -mouseX * RotationSpeed * Time.deltaTime;
			targetRotation.y += -mouseY * RotationSpeed * Time.deltaTime;
			targetRotation.y = Mathf.Clamp(targetRotation.y, -90f, 90f);
		}

		// Zooming: Shift + Middle Mouse Button (Mouse Button 2)
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(2)) // Middle Mouse Button (button 2)
		{
			_isZooming = true;
			isCursor = true;
			float mouseY = Input.GetAxis("Mouse Y"); // Get vertical mouse movement

			if (mouseY != 0)
			{
				targetDist -= mouseY * ZoomSpeed * 0.1f; // Use vertical mouse movement for zoom
			}
		}

		// Smooth rotation
		rotation.x = Mathf.SmoothDamp(rotation.x, targetRotation.x, ref rotationVelocity.x, SmoothTime);
		rotation.y = Mathf.SmoothDamp(rotation.y, targetRotation.y, ref rotationVelocity.y, SmoothTime);

		// Smooth zoom
		Dist = Mathf.SmoothDamp(Dist, targetDist, ref zoomVelocity, SmoothTime);

		// Apply camera transformation
		Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
		Vector3 offset = rotationQuat * new Vector3(0, 0, -Dist);
		transform.position = CameraTarget.position + offset;
		transform.LookAt(CameraTarget);

		// Stop rotating when Shift or Mouse Button is released
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			_isRotating = false;
			_isZooming = false;
			isCursor = false;
		}

		// Hide cursor while rotating
		Cursor.visible = !isCursor;
	}
}