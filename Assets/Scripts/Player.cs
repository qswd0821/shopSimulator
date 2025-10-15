using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera Cam;
    public float MouseSpeed;
    public float MoveSpeed;
    public float RayDistance;

    [SerializeField]
    GameObject AttachPoint;

    float YRotation;
    float XRotation;
    float h;
    float v;

    Item AttachItem;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                     // 마우스 커서를 보이지 않도록 설정
    }

    private void Update()
    {
        Ray();
        Rotate();
    }

    private void FixedUpdate()
    {
        
        Move();
    }

    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * MouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * MouseSpeed * Time.deltaTime;

        YRotation += mouseX;    // 마우스 X축 입력에 따라 수평 회전 값을 조정
        XRotation -= mouseY;    // 마우스 Y축 입력에 따라 수직 회전 값을 조정

        XRotation = Mathf.Clamp(XRotation, -90f, 90f);  // 수직 회전 값을 -90도에서 90도 사이로 제한

        Cam.transform.rotation = Quaternion.Euler(XRotation, YRotation, 0); // 카메라의 회전을 조절
        transform.rotation = Quaternion.Euler(0, YRotation, 0);             // 플레이어 캐릭터의 회전을 조절
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // 수평 이동 입력 값
        v = Input.GetAxisRaw("Vertical");   // 수직 이동 입력 값

        // 입력에 따라 이동 방향 벡터 계산
        Vector3 moveVec = transform.forward * v + transform.right * h;

        // 이동 벡터를 정규화하여 이동 속도와 시간 간격을 곱한 후 현재 위치에 더함
        transform.position += moveVec.normalized * MoveSpeed * Time.deltaTime;
    }

    void Ray()
    {
        // 화면 중앙 좌표 계산
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // 레이 생성
        Ray ray = Cam.ScreenPointToRay(screenCenter);

        // 레이 시각화 (Scene 뷰에서만 보임)
        Debug.DrawRay(ray.origin, ray.direction * RayDistance, Color.red);

        // 실제 레이캐스트
        if (Physics.Raycast(ray, out RaycastHit hit, RayDistance))
        {
            Debug.Log("충돌한 오브젝트: " + hit.collider.name);

            if(Input.GetMouseButtonDown(0)) // 좌클릭
            {
                if(AttachItem == null)
                {
                    // GameObject 자체
                    GameObject obj = hit.collider.gameObject;

                    // Item 스크립트를 가진 오브젝트인지 확인
                    Item attachitem = obj.GetComponent<Item>();
                    if (attachitem != null)
                    {
                        // Item 타입으로 캐스팅 성공 - 아이템 장착
                        attachitem.Attach(AttachPoint);
                        AttachItem = attachitem;
                    }
                }
                else
                {
                    AttachItem.AttachUse();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭
        {
            if (AttachItem != null)
            {
                AttachItem.Detach(); // 장착된 아이템 해제
                AttachItem = null;
            }
        }
    }
}
