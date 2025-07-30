using UnityEngine;

/// <summary>
/// �� ���� �� ������ ť�� ��� â�� ������ (URP ���̴� ���)
/// </summary>
public class SimpleWarehouseBuilder : MonoBehaviour
{
    [Header("â�� ũ�� ����")]
    public int width = 6;     // ����
    public int height = 3;    // ����
    public int depth = 10;    // ����
    public float unitSize = 1f;

    Material wallMat, roofMat, doorMat, supportMat;

    void Start()
    {
        CreateMaterials();
        BuildWarehouse();
    }

    void CreateMaterials()
    {
        string urpLitShader = "Universal Render Pipeline/Lit";

        // ��ü ��: ȸ����
        wallMat = new Material(Shader.Find(urpLitShader));
        wallMat.color = new Color(0.6f, 0.4f, 0.2f);

        // ����: £�� ȸ��
        roofMat = new Material(Shader.Find(urpLitShader));
        roofMat.color = new Color(0.2f, 0.2f, 0.2f);

        // ��: ûȸ��
        doorMat = new Material(Shader.Find(urpLitShader));
        doorMat.color = new Color(0.3f, 0.5f, 0.6f);

        // ���: ���� ����
        supportMat = new Material(Shader.Find(urpLitShader));
        supportMat.color = new Color(0.3f, 0.2f, 0.1f);
    }

    void BuildWarehouse()
    {
        GameObject root = new GameObject("Warehouse");
        root.transform.SetParent(transform, false);

        Vector3 offset = new Vector3(width * 0.5f, 0, depth * 0.5f) * unitSize;

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // �ٴ�, ��, õ�常 ����� (���� ����α�)
                    bool isWall = y == 0 || y == height - 1 || x == 0 || x == width - 1 || z == 0 || z == depth - 1;
                    if (!isWall) continue;

                    // �� ��ġ (�ո� �߾� 2ĭ)
                    if (y == 0 && z == 0 && (x == width / 2 || x == width / 2 - 1))
                        continue;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * unitSize;

                    Vector3 localPos = new Vector3(x, y, z) * unitSize - offset;
                    cube.transform.SetParent(root.transform, false);
                    cube.transform.localPosition = localPos;

                    Renderer renderer = cube.GetComponent<Renderer>();

                    // ����
                    if (y == height - 1)
                        renderer.material = roofMat;
                    // ��� (4 �𼭸�)
                    else if ((x == 0 || x == width - 1) && (z == 0 || z == depth - 1))
                        renderer.material = supportMat;
                    // ��
                    else
                        renderer.material = wallMat;
                }
            }
        }

        // ���� ���� �տ� ��ġ
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.localScale = new Vector3(unitSize * 2f, unitSize * 2f, unitSize * 0.3f);
        door.transform.SetParent(root.transform, false);
        door.transform.localPosition = new Vector3(0, unitSize, -offset.z - 0.1f); // �տ� ��¦ ����
        door.GetComponent<Renderer>().material = doorMat;

    }
}
