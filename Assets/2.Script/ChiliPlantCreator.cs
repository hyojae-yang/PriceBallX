using UnityEngine;

/// <summary>
/// �����뿡 ���� �ٱ⸦ ���̰� �� �ٱ⿡ ���� ���ſ� ���� �����ϴ� ��ũ��Ʈ (URP ����)
/// </summary>
public class ChiliPlantCreator : MonoBehaviour
{
    int chiliPerLine = 5;        // �ٱ� �� �ٸ��� ������ ���� ��
    int stemCount = 3;           // �ٱ� �� ��
    float poleHeight = 2.0f;     // ������ ��ü ����
    float chiliSpacing = 0.2f;   // ���� �� ����

    Material brownMat;
    Material greenMat;
    Material redMat;

    void Start()
    {
        InitializeMaterials();
        CreateChiliPlant();
    }

    // URP�� ��Ƽ���� �ʱ�ȭ
    void InitializeMaterials()
    {
        // URP�� Lit ���̴� �ҷ�����
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");

        if (urpShader == null)
        {
            Debug.LogError("URP Lit ���̴��� ã�� �� �����ϴ�. ������Ʈ�� URP�� ����Ǿ� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // �� ���� ��Ƽ���� ����
        brownMat = new Material(urpShader);
        brownMat.color = new Color(0.4f, 0.2f, 0.1f);  // ������� ����

        greenMat = new Material(urpShader);
        greenMat.color = Color.green;                 // �ٱ�/�ٿ� ���

        redMat = new Material(urpShader);
        redMat.color = Color.red;                     // ���� ���ſ� ������
    }

    void CreateChiliPlant()
    {
        GameObject chiliPlant = new GameObject("ChiliPlant");
        chiliPlant.transform.parent = this.transform;

        // ������ ���� (Cylinder)
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(chiliPlant.transform);
        pole.transform.localScale = new Vector3(0.1f, poleHeight / 2f, 0.1f);
        pole.transform.localPosition = new Vector3(0, poleHeight / 2f, 0);
        pole.GetComponent<Renderer>().material = brownMat;

        // �ٱ� ��ġ �л�: ��, �߰�, �Ʒ�
        for (int s = 0; s < stemCount; s++)
        {
            float yPos = Mathf.Lerp(poleHeight * 0.8f, poleHeight * 0.3f, (float)s / (stemCount - 1));

            // �ٱ� (Cube)
            GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stem.transform.SetParent(chiliPlant.transform);
            stem.transform.localScale = new Vector3(1.2f, 0.05f, 0.05f);
            stem.transform.localPosition = new Vector3(0, yPos, 0);
            stem.GetComponent<Renderer>().material = greenMat;

            // ���� ���ŵ� (Capsule)
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

            // �� �� �� �߰� (Sphere)
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
