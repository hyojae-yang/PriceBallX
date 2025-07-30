using UnityEngine;

/// <summary>
/// 지지대에 여러 줄기를 붙이고 각 줄기에 고추 열매와 잎을 생성하는 스크립트 (URP 대응)
/// </summary>
public class ChiliPlantCreator : MonoBehaviour
{
    int chiliPerLine = 5;        // 줄기 한 줄마다 생성할 고추 수
    int stemCount = 3;           // 줄기 줄 수
    float poleHeight = 2.0f;     // 지지대 전체 높이
    float chiliSpacing = 0.2f;   // 고추 간 간격

    Material brownMat;
    Material greenMat;
    Material redMat;

    void Start()
    {
        InitializeMaterials();
        CreateChiliPlant();
    }

    // URP용 머티리얼 초기화
    void InitializeMaterials()
    {
        // URP용 Lit 셰이더 불러오기
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");

        if (urpShader == null)
        {
            Debug.LogError("URP Lit 셰이더를 찾을 수 없습니다. 프로젝트에 URP가 적용되어 있는지 확인하세요.");
            return;
        }

        // 각 색상별 머티리얼 생성
        brownMat = new Material(urpShader);
        brownMat.color = new Color(0.4f, 0.2f, 0.1f);  // 지지대용 갈색

        greenMat = new Material(urpShader);
        greenMat.color = Color.green;                 // 줄기/잎용 녹색

        redMat = new Material(urpShader);
        redMat.color = Color.red;                     // 고추 열매용 빨간색
    }

    void CreateChiliPlant()
    {
        GameObject chiliPlant = new GameObject("ChiliPlant");
        chiliPlant.transform.parent = this.transform;

        // 지지대 생성 (Cylinder)
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(chiliPlant.transform);
        pole.transform.localScale = new Vector3(0.1f, poleHeight / 2f, 0.1f);
        pole.transform.localPosition = new Vector3(0, poleHeight / 2f, 0);
        pole.GetComponent<Renderer>().material = brownMat;

        // 줄기 위치 분산: 위, 중간, 아래
        for (int s = 0; s < stemCount; s++)
        {
            float yPos = Mathf.Lerp(poleHeight * 0.8f, poleHeight * 0.3f, (float)s / (stemCount - 1));

            // 줄기 (Cube)
            GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stem.transform.SetParent(chiliPlant.transform);
            stem.transform.localScale = new Vector3(1.2f, 0.05f, 0.05f);
            stem.transform.localPosition = new Vector3(0, yPos, 0);
            stem.GetComponent<Renderer>().material = greenMat;

            // 고추 열매들 (Capsule)
            for (int i = 0; i < chiliPerLine; i++)
            {
                GameObject chili = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                chili.transform.SetParent(chiliPlant.transform);

                float x = -0.5f + i * chiliSpacing;
                float z = Random.Range(-0.05f, 0.05f);
                chili.transform.localPosition = new Vector3(x, yPos - 0.1f, z);
                chili.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
                chili.GetComponent<Renderer>().material = redMat;
            }

            // 잎 두 개 추가 (Sphere)
            for (int j = 0; j < 2; j++)
            {
                GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaf.transform.SetParent(chiliPlant.transform);
                float x = Random.Range(-0.5f, 0.5f);
                float z = Random.Range(-0.1f, 0.1f);
                leaf.transform.localPosition = new Vector3(x, yPos - 0.05f, z);
                leaf.transform.localScale = Vector3.one * 0.2f;
                leaf.GetComponent<Renderer>().material = greenMat;
            }
        }

        chiliPlant.transform.localPosition = Vector3.zero;
    }
}
