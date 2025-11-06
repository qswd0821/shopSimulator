using Customer;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pos : MonoBehaviour
{
    List<GameObject> ListProductCategory = new List<GameObject>();

    // 결제 대기라인 및 결제 대기중인 손님 리스트
    [SerializeField]
    List<GameObject> ListWaitingforPayMentLine = new List<GameObject>();
    [SerializeField]
    List<Customer.Customer> ListCustomer = new List<Customer.Customer>();

    [SerializeField]
    GameObject ParentProductCategory;
    [SerializeField]
    GameObject ProductCategory;
    [SerializeField]
    Text TotalNumText;
    [SerializeField]
    Shelf CheckPointShelf;

    int TotalNum;
    Customer.Customer CurrCustomer;

    void Start()
    {
        Shared.GameManager.Pos = this;
        TotalNum = 0;
    }

    public void AddProductCategory(Product _product)
    {
        // 상품 목록 Ui 생성
        GameObject obj = Instantiate(ProductCategory);
        obj.transform.SetParent(ParentProductCategory.transform,false);
        obj.GetComponent<Ui_ProductCategory>().Init(_product);

        ListProductCategory.Add(obj);

        TotalNum += _product.GetPrice();
        TotalNumText.text = TotalNum.ToString();
    }

    //결제
    public void OnPayMent()
    {
        //손님이 없을 경우 결제하지 않음
        if (ListCustomer.Count <= 0) return;

        Shared.GameManager.Player.AddMoney(TotalNum);
        PosReset();

        // 계산대 위 물건 제거
        CheckPointShelf.DeleteAll();

        // 맨 앞 손님 제거
        Customer.Customer front = ListCustomer[0];
        ListCustomer.RemoveAt(0);
        front.OnFinishPayMent();

        // 남은 손님 줄 재정렬
        StartCoroutine(IDelayRefresh());
    }

    void PosReset()
    {
        TotalNum = 0;
        TotalNumText.text = TotalNum.ToString();
        RemoveAllProductCategory();
    }

    void RemoveAllProductCategory()
    {
        for (int i = ListProductCategory.Count - 1; i >= 0; i--)
        {
            Destroy(ListProductCategory[i]);
        }
        ListProductCategory.Clear();
    }

    public void AddProductToCheckPoint(Product _product)
    {
        _product.SetBodyActive(true);
        CheckPointShelf.AddProduct(_product);
    }

    public void AddWaitingForPayMentLine(Customer.Customer _custormer)
    {
        Vector3 WaitLinePos = GetWiteLinePosition();
        _custormer.Movement.MoveTo(WaitLinePos,(succeed) =>
        {
            OnCustomerArrivedAtLine(_custormer);
        });
        ListCustomer.Add(_custormer);
    }

    Vector3 GetWiteLinePosition()
    {
        int Idx = ListCustomer.Count;
        return ListWaitingforPayMentLine[Idx].transform.position;
    }

    public int GetMyWaitNumber(Customer.Customer _custormer)
    {
        return ListCustomer.IndexOf(_custormer);
    }


    IEnumerator IDelayRefresh()
    {
        yield return new WaitForSeconds(0.2f);
        RefreshWaitingLine();
    }
    // 줄 재정렬 (맨 앞 손님 빠질때 호출)
    void RefreshWaitingLine()
    {
        for (int i = 0; i < ListCustomer.Count; i++)
        {
            Vector3 nextPos = ListWaitingforPayMentLine[i].transform.position;
            ListCustomer[i].Movement.MoveTo(nextPos);
        }
    }
    void OnCustomerArrivedAtLine(Customer.Customer customer)
    {
        // 인덱스 찾기
        int myIdx = ListCustomer.IndexOf(customer);
        if (myIdx == -1) return; // 리스트에 없으면 무시

        // 뒤에 사람이 있어야 진행
        int nextIdx = myIdx + 1;
        if (nextIdx >= ListCustomer.Count) return;

        var next = ListCustomer[nextIdx];
        if (next == null) return;

        // 이미 도착한 상태면(HasArrivedAtDestination == true) 다시 이동시킬 필요 없음
        if (next.Movement.HasArrivedAtDestination()) return;

        // 뒤사람은 '도착한 사람 자리'로 한 칸 앞으로 당겨야 함
        // (myIdx 위치가 앞사람의 자리)
        if (myIdx < ListWaitingforPayMentLine.Count)
        {
            Vector3 targetPos = ListWaitingforPayMentLine[myIdx].transform.position;
            next.Movement.MoveTo(targetPos);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for(int i =0;i< ListWaitingforPayMentLine.Count;++i)
        {
            Gizmos.DrawCube(ListWaitingforPayMentLine[i].transform.position,Vector3.one * 5f);
        }
    }
}
