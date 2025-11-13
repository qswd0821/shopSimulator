using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public Camera Cam;
    public float MouseSpeed;
    public float MoveSpeed;
    public float RayDistance;

    [SerializeField]
    GameObject AttachPoint;
    [SerializeField]
    Phone Phone;

    float YRotation;
    float XRotation;
    float h;
    float v;

    Item AttachItem;
    int Money;

    [SerializeField]
    Ui_MoneyText Ui_MoneyText;
    private void Start()
    {
        Shared.GameManager.Player = this;

        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 화면 안에서 고정
        Cursor.visible = false;                     // 마우스 커서를 보이지 않도록 설정
        
        Money = 0;
    }

    private void Update()
    {
        Ray();
        KeyInput();
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void KeyInput()
    {
        if(Input.GetKeyUp(KeyCode.Q))
        {

        }
    }
    void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * MouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * MouseSpeed * Time.deltaTime;

        YRotation += mouseX;    // 마우스 X축 입력에 따라 수평 회전 값을 조정
        XRotation -= mouseY;    // 마우스 Y축 입력에 따라 수직 회전 값을 조정

        XRotation = Mathf.Clamp(XRotation, -90f, 90f);  // 수직 회전 값을 -90도에서 90도 사이로 제한

        Cam.transform.rotation = Quaternion.Euler(XRotation, YRotation, 0); // 카메라의 회전
        transform.rotation = Quaternion.Euler(0, YRotation, 0);             // 플레이어 캐릭터의 회전
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // 수평 이동 입력 값
        v = Input.GetAxisRaw("Vertical");   // 수직 이동 입력 값

        Vector3 moveVec = transform.forward * v + transform.right * h;
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
            //Debug.Log("충돌한 오브젝트: " + hit.collider.name);

            // GameObject 자체
            GameObject obj = hit.collider.gameObject;

            if (Input.GetMouseButtonDown(0)) // 좌클릭
            {
                WorldSpaceUiClick(hit);

                if(AttachItem == null)
                {
                    // Item 스크립트를 가진 오브젝트인지 확인
                    Item attachitem = obj.GetComponent<Item>();
                    if (attachitem != null)
                    {
                        // Item 타입으로 캐스팅 성공 - 아이템 장착 시도
                        if(attachitem.TryAttach(this))
                        {
                            AttachItem = attachitem;
                        }
                    }
                }
                else
                {
                    AttachItem.AttachUse(obj);
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

    void WorldSpaceUiClick(RaycastHit _Hit)
    {
        // 1. UI 클릭 위치를 화면 좌표로 변환
        Vector3 screenPos = Cam.WorldToScreenPoint(_Hit.point);

        // 2. UI 시스템용 포인터 이벤트 생성
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPos;

        // 3. UI 전용 레이캐스트
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // 4. 클릭 처리
        foreach (var result in results)
        {
            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }

    public void AddMoney(int _Money)
    {
        Money += _Money;
        Ui_MoneyText.RefreshMoenyText(Money);
    }

    public GameObject GetAttachPoint() { return AttachPoint; }
}
