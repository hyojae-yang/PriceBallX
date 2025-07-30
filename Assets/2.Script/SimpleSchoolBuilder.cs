using UnityEngine;

/// <summary>
/// 큐브로 외형을 구성하는 심플한 학교 생성 스크립트 (URP 셰이더 적용)
/// </summary>
public class SimpleSchoolBuilder : MonoBehaviour
{
    [Header("학교 크기 설정")]
    public int width = 10;     // x 방향 길이
    public int height = 5;     // y 방향 층수
    public int depth = 20;     // z 방향 길이
    public float unitSize = 1f; // 큐브 하나의 크기

    // 내부에서 생성한 머티리얼들
    Material wallMat, roofMat, windowMat, doorMat;

    void Start()
    {
        CreateMaterials();  // 머티리얼 생성
        BuildSchool();      // 학교 생성
    }

    void CreateMaterials()
    {
        // URP 셰이더 경로
        string urpLitShader = "Universal Render Pipeline/Lit";

        // 벽: 연한 베이지
        wallMat = new Material(Shader.Find(urpLitShader));
        wallMat.color = new Color(0.9f, 0.85f, 0.7f);

        // 지붕: 어두운 회색
        roofMat = new Material(Shader.Find(urpLitShader));
        roofMat.color = new Color(0.2f, 0.2f, 0.2f);

        // 창문: 반투명 파랑
        windowMat = new Material(Shader.Find(urpLitShader));
        windowMat.color = new Color(0.4f, 0.6f, 1f, 0.5f);
        windowMat.SetFloat("_Surface", 1); // Transparent
        windowMat.SetFloat("_Blend", 0);   // Alpha
        windowMat.SetFloat("_ZWrite", 0);  // ZWrite 끔
        windowMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        windowMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // 출입문: 진한 갈색
        doorMat = new Material(Shader.Find(urpLitShader));
        doorMat.color = new Color(0.3f, 0.2f, 0.1f);
    }

    void BuildSchool()
    {
        // 학교 전체를 담는 루트 오브젝트
        GameObject root = new GameObject("School");
        root.transform.SetParent(transform, false); // 부모 오브젝트 기준 지역좌표로 설정

        // 중앙 기준 정렬을 위한 오프셋
        Vector3 centerOffset = new Vector3(width * 0.5f, 0, depth * 0.5f) * unitSize;

        // 큐브 배치
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 출입구 공간 비워두기
                    if (y == 0 && z == 0 && (x >= width / 2 - 1 && x <= width / 2))
                        continue;

                    // 큐브 생성
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * unitSize;

                    // 지역좌표로 위치 지정
                    Vector3 localPos = new Vector3(x, y, z) * unitSize - centerOffset;
                    cube.transform.SetParent(root.transform, false);
                    cube.transform.localPosition = localPos;

                    // 머티리얼 적용
                    Renderer renderer = cube.GetComponent<Renderer>();

                    // 지붕
                    if (y == height - 1)
                    {
                        renderer.material = roofMat;
                    }
                    // 창문 (2층 이상, 격자 간격)
                    else if (y >= 2 && x % 3 == 0 && z % 4 == 0)
                    {
                        renderer.material = windowMat;
                    }
                    // 출입구 주변은 문 색
                    else if (y == 0 && z == 0 && (x >= width / 2 - 2 && x <= width / 2 + 2))
                    {
                        renderer.material = doorMat;
                    }
                    // 일반 벽
                    else
                    {
                        renderer.material = wallMat;
                    }
                }
            }
        }
    }
}
