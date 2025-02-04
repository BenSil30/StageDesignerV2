using UnityEngine;

namespace TransformGizmos
{
	public class GizmoController : MonoBehaviour
	{
		[SerializeField] private Rotation m_rotation;
		[SerializeField] private Translation m_translation;
		[SerializeField] private Scaling m_scaling;
		[SerializeField] private GameObject m_rotationAppendix;

		[SerializeField] private Material m_clickedMaterial;
		[SerializeField] private Material m_transparentMaterial;
		[SerializeField] private GameObject m_objectWithMeshes;
		[SerializeField] private GameObject m_degreesText;

		[Header("Adjustable Variables")]
		[SerializeField] public GameObject m_targetObject;

		[SerializeField] private float m_gizmoSize = 1;

		private Transformation m_transformation = Transformation.None;

		private enum Transformation
		{
			None,
			Rotation,
			Translation,
			Scale
		}

		private void Start()
		{
			transform.SetPositionAndRotation(m_targetObject.transform.position, m_targetObject.transform.rotation);
			transform.localScale = m_targetObject.transform.localScale;
			m_rotation.Initialization(m_targetObject, m_clickedMaterial, m_transparentMaterial, m_objectWithMeshes, m_degreesText, m_rotationAppendix);
			m_translation.Initialization(m_targetObject, m_clickedMaterial, m_transparentMaterial);
			m_scaling.Initialization(m_targetObject, m_clickedMaterial, m_transparentMaterial);

			//ChangeTransformationState(Transformation.None);
		}

		private void Update()
		{
			transform.SetPositionAndRotation(m_targetObject.transform.position, m_targetObject.transform.rotation);
			m_degreesText.transform.position = m_targetObject.transform.position;
			m_objectWithMeshes.transform.position = m_targetObject.transform.position;
			m_rotation.SetGizmoSize(m_gizmoSize);
			m_translation.SetGizmoSize(m_gizmoSize);
			m_scaling.SetGizmoSize(m_gizmoSize);

			//if (Input.GetKeyDown(KeyCode.E))
			//	ChangeTransformationState(Transformation.Rotation);

			//if (Input.GetKeyDown(KeyCode.W))
			//	ChangeTransformationState(Transformation.Translation);

			//if (Input.GetKeyDown(KeyCode.R))
			//	ChangeTransformationState(Transformation.Scale);
		}

		private void ChangeTransformationState(Transformation transformation)
		{
			m_rotation.gameObject.SetActive(false);
			m_translation.gameObject.SetActive(false);
			m_scaling.gameObject.SetActive(false);

			switch (transformation)
			{
				case Transformation.None:
					break;

				case Transformation.Rotation:
					if (m_transformation == Transformation.Rotation)
					{
						m_transformation = Transformation.None;
					}
					else
					{
						m_rotation.gameObject.SetActive(true);
						m_transformation = transformation;
					}
					break;

				case Transformation.Translation:
					if (m_transformation == Transformation.Translation)
					{
						m_transformation = Transformation.None;
					}
					else
					{
						m_translation.gameObject.SetActive(true);
						m_transformation = transformation;
					}
					break;

				case Transformation.Scale:
					if (m_transformation == Transformation.Scale)
					{
						m_transformation = Transformation.None;
					}
					else
					{
						m_scaling.gameObject.SetActive(true);
						m_transformation = transformation;
					}
					break;
			}
		}

		public void ToggleRotation()
		{
			ChangeTransformationState(Transformation.Rotation);
		}

		public void ToggleMovement()
		{
			ChangeTransformationState(Transformation.Translation);
		}

		public void ToggleScale()
		{
			ChangeTransformationState(Transformation.Scale);
		}
	}
}