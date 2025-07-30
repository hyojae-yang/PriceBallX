using UnityEngine;

/// <summary>
/// 밭 옆에 둘 간단한 큐브 기반 창고 생성기 (URP 셰이더 기반)
/// </summary>
public class SimpleWarehouseBuilder : MonoBehaviour
{
    [Header("창고 크기 설정")]
    public int width = 6;     // 가로
    public int height = 3;    // 높이
    public int depth = 10;    // 세로
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

        // 벽체 색: 회갈색
        wallMat = new Material(Shader.Find(urpLitShader));
        wallMat.color = new Color(0.6f, 0.4f, 0.2f);

        // 지붕: 짙은 회색
        roofMat = new Material(Shader.Find(urpLitShader));
        roofMat.color = new Color(0.2f, 0.2f, 0.2f);

        // 문: 청회색
        doorMat = new Material(Shader.Find(urpLitShader));
        doorMat.color = new Color(0.3f, 0.5f, 0.6f);

        // 기둥: 진한 갈색
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
                    // 바닥, 벽, 천장만 만들기 (속은 비워두기)
                    bool isWall = y == 0 || y == height - 1 || x == 0 || x == width - 1 || z == 0 || z == depth - 1;
                    if (!isWall) continue;

                    // 문 위치 (앞면 중앙 2칸)
                    if (y == 0 && z == 0 && (x == width / 2 || x == width / 2 - 1))
                        continue;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * unitSize;

                    Vector3 localPos = new Vector3(x, y, z) * unitSize - offset;
                    cube.transform.SetParent(root.transform, false);
                    cube.transform.localPosition = localPos;

                    Renderer renderer = cube.GetComponent<Renderer>();

                    // 지붕
                    if (y == height - 1)
                        renderer.material = roofMat;
                    // 기둥 (4 모서리)
                    else if ((x == 0 || x == width - 1) && (z == 0 || z == depth - 1))
                        renderer.material = supportMat;
                    // 벽
                    else
                        renderer.material = wallMat;
                }
            }
        }

        // 문은 따로 앞에 배치
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.localScale = new Vector3(unitSize * 2f, unitSize * 2f, unitSize * 0.3f);
        door.transform.SetParent(root.transform, false);
        door.transform.localPosition = new Vector3(0, unitSize, -offset.z - 0.1f); // 앞에 살짝 돌출
        door.GetComponent<Renderer>().material = doorMat;

    }
}
