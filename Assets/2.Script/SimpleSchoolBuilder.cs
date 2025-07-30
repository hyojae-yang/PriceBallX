using UnityEngine;

/// <summary>
/// ť��� ������ �����ϴ� ������ �б� ���� ��ũ��Ʈ (URP ���̴� ����)
/// </summary>
public class SimpleSchoolBuilder : MonoBehaviour
{
    [Header("�б� ũ�� ����")]
    public int width = 10;     // x ���� ����
    public int height = 5;     // y ���� ����
    public int depth = 20;     // z ���� ����
    public float unitSize = 1f; // ť�� �ϳ��� ũ��

    // ���ο��� ������ ��Ƽ�����
    Material wallMat, roofMat, windowMat, doorMat;

    void Start()
    {
        CreateMaterials();  // ��Ƽ���� ����
        BuildSchool();      // �б� ����
    }

    void CreateMaterials()
    {
        // URP ���̴� ���
        string urpLitShader = "Universal Render Pipeline/Lit";

        // ��: ���� ������
        wallMat = new Material(Shader.Find(urpLitShader));
        wallMat.color = new Color(0.9f, 0.85f, 0.7f);

        // ����: ��ο� ȸ��
        roofMat = new Material(Shader.Find(urpLitShader));
        roofMat.color = new Color(0.2f, 0.2f, 0.2f);

        // â��: ������ �Ķ�
        windowMat = new Material(Shader.Find(urpLitShader));
        windowMat.color = new Color(0.4f, 0.6f, 1f, 0.5f);
        windowMat.SetFloat("_Surface", 1); // Transparent
        windowMat.SetFloat("_Blend", 0);   // Alpha
        windowMat.SetFloat("_ZWrite", 0);  // ZWrite ��
        windowMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        windowMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // ���Թ�: ���� ����
        doorMat = new Material(Shader.Find(urpLitShader));
        doorMat.color = new Color(0.3f, 0.2f, 0.1f);
    }

    void BuildSchool()
    {
        // �б� ��ü�� ��� ��Ʈ ������Ʈ
        GameObject root = new GameObject("School");
        root.transform.SetParent(transform, false); // �θ� ������Ʈ ���� ������ǥ�� ����

        // �߾� ���� ������ ���� ������
        Vector3 centerOffset = new Vector3(width * 0.5f, 0, depth * 0.5f) * unitSize;

        // ť�� ��ġ
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // ���Ա� ���� ����α�
                    if (y == 0 && z == 0 && (x >= width / 2 - 1 && x <= width / 2))
                        continue;

                    // ť�� ����
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * unitSize;

                    // ������ǥ�� ��ġ ����
                    Vector3 localPos = new Vector3(x, y, z) * unitSize - centerOffset;
                    cube.transform.SetParent(root.transform, false);
                    cube.transform.localPosition = localPos;

                    // ��Ƽ���� ����
                    Renderer renderer = cube.GetComponent<Renderer>();

                    // ����
                    if (y == height - 1)
                    {
                        renderer.material = roofMat;
                    }
                    // â�� (2�� �̻�, ���� ����)
                    else if (y >= 2 && x % 3 == 0 && z % 4 == 0)
                    {
                        renderer.material = windowMat;
                    }
                    // ���Ա� �ֺ��� �� ��
                    else if (y == 0 && z == 0 && (x >= width / 2 - 2 && x <= width / 2 + 2))
                    {
                        renderer.material = doorMat;
                    }
                    // �Ϲ� ��
                    else
                    {
                        renderer.material = wallMat;
                    }
                }
            }
        }
    }
}
