using UnityEngine;

public class RoboLift : MonoBehaviour
{
    public CharacterController controller; // ������ �� Character Controller
    public float liftSpeed = 2f;           // �������� �������
    public float maxHeight = 10f;          // ������������ ������ �������
    public float minHeight = 0f;           // ����������� ������
    private Vector3 moveDirection;         // ����������� ��������
    private float startY;                  // ��������� ������� �� Y

    void Start()
    {
        // ��������� ��������� ������
        startY = transform.position.y;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            // ��������� �����, ���� �� ���������� ������������ ������
            if (transform.position.y < startY + maxHeight)
            {
                moveDirection.y = liftSpeed;
            }
            else
            {
                moveDirection.y = 0;
            }
        }
        else if (Input.GetKey(KeyCode.F))
        {
            // ��������� ����, ���� �� ���������� ����������� ������
            if (transform.position.y > startY + minHeight)
            {
                moveDirection.y = -liftSpeed;
            }
            else
            {
                moveDirection.y = 0;
            }
        }
        else
        {
            // ���� ������� �� ������, ������������� ��������
            moveDirection.y = 0;
        }

        // ��������� �������� ����� Character Controller
        controller.Move(moveDirection * Time.deltaTime);
    }
}
