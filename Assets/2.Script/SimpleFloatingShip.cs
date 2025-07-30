using UnityEngine;

/// <summary>
/// �⺻ �������� ������ �� ����� �����ϴ� ��ũ��Ʈ
/// </summary>
public class SimpleFloatingShip : MonoBehaviour
{
    public Material hullMaterial;   // ��ü ����
    public Material deckMaterial;   // ���� ����
    public Material sailMaterial;   // �� ����

    void Start()
    {
        BuildShip();
    }

    void BuildShip()
    {
        // �� ��ü�� ��� �θ� ������Ʈ ����
        GameObject shipRoot = new GameObject("SimpleShip");

        // �θ� ���� ���� (������ǥ�� ���� ��ġ ���� ����)
        shipRoot.transform.SetParent(transform, false);

        // ������ǥ�� ��ġ ���� (�θ� ������Ʈ �������� ���߿� ���)
        shipRoot.transform.localPosition = new Vector3(0, 1, 0);

        // 1. ��ü - �� ť�� 1��
        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(shipRoot.transform, false);
        hull.transform.localScale = new Vector3(8, 1.5f, 3);
        hull.transform.localPosition = Vector3.zero;
        if (hullMaterial != null) hull.GetComponent<Renderer>().material = hullMaterial;

        // 2. ���� - ��ü �� ���� ť��
        GameObject deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deck.transform.SetParent(shipRoot.transform, false);
        deck.transform.localScale = new Vector3(7.5f, 0.3f, 2.8f);
        deck.transform.localPosition = new Vector3(0, 1f, 0);
        if (deckMaterial != null) deck.GetComponent<Renderer>().material = deckMaterial;

        // 3. ���� - �Ǹ��� �ϳ�
        GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mast.transform.SetParent(shipRoot.transform, false);
        mast.transform.localScale = new Vector3(0.3f, 3.5f, 0.3f);
        mast.transform.localPosition = new Vector3(0, 3f, 0);
        if (deckMaterial != null) mast.GetComponent<Renderer>().material = deckMaterial;

        // 4. �� - ���� ť�� �ϳ� (�� ����), ���뺸�� ���� ���� �������� ����
        GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sail.transform.SetParent(shipRoot.transform, false);
        sail.transform.localScale = new Vector3(3f, 3f, 0.1f);
        sail.transform.localPosition = new Vector3(0, 5f, 0f);
        if (sailMaterial != null) sail.GetComponent<Renderer>().material = sailMaterial;

        // 5. ��Ӹ� ��� - ���� ť��
        GameObject frontDecoration = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontDecoration.transform.SetParent(shipRoot.transform, false);
        frontDecoration.transform.localScale = new Vector3(1, 1, 1);
        frontDecoration.transform.localPosition = new Vector3(4.5f, 0.5f, 0);
        if (hullMaterial != null) frontDecoration.GetComponent<Renderer>().material = hullMaterial;
    }
}
