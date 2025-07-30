using UnityEngine;

/// <summary>
/// 기본 도형으로 정적인 배 모양을 생성하는 스크립트
/// </summary>
public class SimpleFloatingShip : MonoBehaviour
{
    public Material hullMaterial;   // 선체 재질
    public Material deckMaterial;   // 갑판 재질
    public Material sailMaterial;   // 돛 재질

    void Start()
    {
        BuildShip();
    }

    void BuildShip()
    {
        // 배 전체를 담는 부모 오브젝트 생성
        GameObject shipRoot = new GameObject("SimpleShip");

        // 부모 먼저 지정 (지역좌표계 기준 위치 설정 위해)
        shipRoot.transform.SetParent(transform, false);

        // 지역좌표계 위치 지정 (부모 오브젝트 기준으로 공중에 띄움)
        shipRoot.transform.localPosition = new Vector3(0, 1, 0);

        // 1. 선체 - 긴 큐브 1개
        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(shipRoot.transform, false);
        hull.transform.localScale = new Vector3(8, 1.5f, 3);
        hull.transform.localPosition = Vector3.zero;
        if (hullMaterial != null) hull.GetComponent<Renderer>().material = hullMaterial;

        // 2. 갑판 - 선체 위 얇은 큐브
        GameObject deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deck.transform.SetParent(shipRoot.transform, false);
        deck.transform.localScale = new Vector3(7.5f, 0.3f, 2.8f);
        deck.transform.localPosition = new Vector3(0, 1f, 0);
        if (deckMaterial != null) deck.GetComponent<Renderer>().material = deckMaterial;

        // 3. 돛대 - 실린더 하나
        GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mast.transform.SetParent(shipRoot.transform, false);
        mast.transform.localScale = new Vector3(0.3f, 3.5f, 0.3f);
        mast.transform.localPosition = new Vector3(0, 3f, 0);
        if (deckMaterial != null) mast.GetComponent<Renderer>().material = deckMaterial;

        // 4. 돛 - 얇은 큐브 하나 (돛 형태), 돛대보다 조금 낮고 앞쪽으로 조정
        GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sail.transform.SetParent(shipRoot.transform, false);
        sail.transform.localScale = new Vector3(3f, 3f, 0.1f);
        sail.transform.localPosition = new Vector3(0, 5f, 0f);
        if (sailMaterial != null) sail.GetComponent<Renderer>().material = sailMaterial;

        // 5. 뱃머리 장식 - 작은 큐브
        GameObject frontDecoration = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontDecoration.transform.SetParent(shipRoot.transform, false);
        frontDecoration.transform.localScale = new Vector3(1, 1, 1);
        frontDecoration.transform.localPosition = new Vector3(4.5f, 0.5f, 0);
        if (hullMaterial != null) frontDecoration.GetComponent<Renderer>().material = hullMaterial;
    }
}
