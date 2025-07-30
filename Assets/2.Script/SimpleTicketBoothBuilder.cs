using UnityEngine;

/// <summary>
/// ������ ��ǥ��(Ƽ�� �ν�) ���� ��ũ��Ʈ (URP, ������ǥ�� ���)
/// </summary>
public class SimpleTicketBoothBuilder : MonoBehaviour
{
    public Vector3 size = new Vector3(4, 3, 3); // �ʺ�, ����, ����
    public float unit = 1f;

    Material wallMat, roofMat, windowMat, counterMat;

    void Start()
    {
        CreateMaterials();
        BuildBooth();
    }

    void CreateMaterials()
    {
        string urpLitShader = "Universal Render Pipeline/Lit";

        wallMat = new Material(Shader.Find(urpLitShader));
        wallMat.color = new Color(0.8f, 0.6f, 0.4f); // ������ ��

        roofMat = new Material(Shader.Find(urpLitShader));
        roofMat.color = new Color(0.3f, 0.3f, 0.3f); // ���� ȸ�� ����

        windowMat = new Material(Shader.Find(urpLitShader));
        windowMat.color = new Color(0.4f, 0.7f, 0.9f); // â���� ���� �Ķ�

        counterMat = new Material(Shader.Find(urpLitShader));
        counterMat.color = new Color(0.5f, 0.3f, 0.1f); // ī���Ϳ� ���� ����
    }

    void BuildBooth()
    {
        GameObject root = new GameObject("TicketBooth");
        root.transform.SetParent(transform, false);

        Vector3 offset = new Vector3(size.x / 2, 0, size.z / 2) * unit;

        for (int y = 0; y < size.y; y++)
        {
            for (int z = 0; z < size.z; z++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    bool isOuterWall = y == 0 || y == size.y - 1 || x == 0 || x == size.x - 1 || z == 0 || z == size.z - 1;

                    // ���� â���� ����α�
                    bool isFrontCenter = (z == 0) && (x == size.x / 2 || x == size.x / 2 - 1) && (y == 1 || y == 2);
                    if (!isOuterWall || isFrontCenter)
                        continue;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * unit;
                    cube.transform.SetParent(root.transform, false);
                    cube.transform.localPosition = new Vector3(x, y, z) * unit - offset;
                    cube.GetComponent<Renderer>().material = wallMat;
                }
            }
        }

        // â�� â��
        GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
        window.transform.localScale = new Vector3(2f, 1f, 0.1f) * unit;
        window.transform.SetParent(root.transform, false);
        window.transform.localPosition = new Vector3(0, 2f, -offset.z - 0.01f); // �ո鿡 ��¦ ����
        window.GetComponent<Renderer>().material = windowMat;

        // â�� ī����
        GameObject counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
        counter.transform.localScale = new Vector3(2.2f, 0.4f, 0.5f) * unit;
        counter.transform.SetParent(root.transform, false);
        counter.transform.localPosition = new Vector3(0, 1f, -offset.z + 0.25f);
        counter.GetComponent<Renderer>().material = counterMat;

        // ���� ���� �߰�
        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.localScale = new Vector3(size.x + 0.5f, 0.3f, size.z + 0.5f) * unit;
        roof.transform.SetParent(root.transform, false);
        roof.transform.localPosition = new Vector3(0, size.y * unit + 0.15f, 0);
        roof.GetComponent<Renderer>().material = roofMat;
    }
}
