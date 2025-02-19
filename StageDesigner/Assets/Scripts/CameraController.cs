using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform CameraTarget; // Target to orbit around
	public Transform DefaultCameraTarget;
	public float Dist = 10f; // Default zoom distance
	public float RotationSpeed = 25f; // Rotation speed
	public float ZoomSpeed = 2f; // Zoom speed (sensitivity)
	public float PanSpeed = 2f; // Panning speed
	public float SmoothTime = 0.1f; // Smooth transition time

	private Vector2 rotation;
	private Vector2 targetRotation;
	private Vector2 rotationVelocity;
	private float targetDist;
	private float zoomVelocity;
	private Vector3 panOffset = Vector3.zero; // Offset for panning
	private bool _isRotating = false;
	private bool _isZooming = false;
	private bool _isPanning = false;

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

		if (Input.GetKey(KeyCode.LeftShift))
		{
			HandleRotation();
			isCursor = true;
		}

		if (Input.GetKey(KeyCode.LeftAlt))
		{
			HandleZoom();
			isCursor = true;
		}

		if (Input.GetMouseButton(2))
		{
			HandlePanning();
			isCursor = true;
		}

		// Smooth rotation
		rotation.x = Mathf.SmoothDamp(rotation.x, targetRotation.x, ref rotationVelocity.x, SmoothTime);
		rotation.y = Mathf.SmoothDamp(rotation.y, targetRotation.y, ref rotationVelocity.y, SmoothTime);

		// Smooth zoom
		Dist = Mathf.SmoothDamp(Dist, targetDist, ref zoomVelocity, SmoothTime);

		// Apply final camera transformation
		ApplyCameraTransformation();

		// Stop rotating, zooming, or panning when input stops
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			_isRotating = false;
			isCursor = false;
		}

		if (Input.GetKeyUp(KeyCode.LeftAlt))
		{
			_isZooming = false;
			isCursor = false;
		}

		if (Input.GetMouseButtonUp(2))
		{
			_isPanning = false;
			isCursor = false;
		}

		Cursor.visible = !isCursor;
	}

	private void HandlePanning()
	{
		_isPanning = true;

		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		// Move the camera along its local right and up vectors
		Vector3 right = transform.right;
		Vector3 up = transform.up;
		panOffset -= (right * mouseX + up * mouseY) * PanSpeed * Time.deltaTime;
	}

	private void HandleRotation()
	{
		//if (!_isRotating) // Only set target when rotation starts
		//{
		//	Ray objectRay = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		//	if (Physics.Raycast(objectRay, out RaycastHit objectHit))
		//	{
		//		CameraTarget = objectHit.transform;
		//	}
		//	else
		//	{
		//		CameraTarget = DefaultCameraTarget;
		//	}
		//}

		_isRotating = true;

		float mouseX = -Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		targetRotation.x += -mouseX * RotationSpeed * Time.deltaTime;
		targetRotation.y += -mouseY * RotationSpeed * Time.deltaTime;
		targetRotation.y = Mathf.Clamp(targetRotation.y, -90f, 90f);
	}

	private void HandleZoom()
	{
		_isZooming = true;

		float mouseY = Input.GetAxis("Mouse Y");
		if (mouseY != 0)
		{
			targetDist -= mouseY * ZoomSpeed * 0.1f;
		}
	}

	private void ApplyCameraTransformation()
	{
		// Calculate rotation
		Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
		Vector3 offset = rotationQuat * new Vector3(0, 0, -Dist);

		// Apply position and keep pan offset
		transform.position = CameraTarget.position + offset + panOffset;
		transform.LookAt(CameraTarget.position + panOffset); // Apply pan offset to focus point
	}
}