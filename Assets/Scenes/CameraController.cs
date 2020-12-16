using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 2;

    public float rotationUpperLimit = 20;

    public float rotationLowerLimit = -20;

    private float m_RotationLowerLimit = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            m_RotationLowerLimit = 360 + rotationLowerLimit;

            if (transform.localEulerAngles.x > rotationUpperLimit && transform.localEulerAngles.x < m_RotationLowerLimit)
                if (transform.localEulerAngles.x - rotationUpperLimit <= m_RotationLowerLimit - transform.localEulerAngles.x)
                    transform.localEulerAngles = new Vector3(rotationUpperLimit, transform.localEulerAngles.y, transform.localEulerAngles.z);
                else
                    transform.localEulerAngles = new Vector3(m_RotationLowerLimit, transform.localEulerAngles.y, transform.localEulerAngles.z);

            float y = -Input.GetAxis("Mouse Y");

            if ((transform.localEulerAngles.x == rotationUpperLimit && y > 0) || (transform.localEulerAngles.x == m_RotationLowerLimit && y < 0))
                return;

            transform.Rotate(y * rotationSpeed, 0, 0);
        }
    }
}
