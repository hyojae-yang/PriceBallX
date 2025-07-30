using UnityEngine;

/// <summary>
/// �� ��� ť�긦 �����ϰ�, �۹� �������� ������� ��ġ�ϴ� ��ũ��Ʈ
/// </summary>
public class FarmBuilder : MonoBehaviour
{
    int rowCount = 15;               // ���(��) ��
    int blocksPerRow = 20;           // ����� ť�� ����
    float rowSpacing = 2f;           // ��� �� ����
    float blockLength = 1f;          // ť�� ���� (X �Ǵ� Z)
    float blockWidth = 1f;           // ť�� �ʺ� (Z �Ǵ� X)
    float blockHeight = 0.6f;        // ť�� ����
    public Material soilMaterial;           // �� ����
    public bool verticalRows = false;       // true�� Z������ ���η� ��ġ

    public GameObject[] cropPrefabs;        // �۹� ������ �迭 (��: ��, ���, ���� ��)
    public float cropYOffset = 0.5f;        // �۹��� ť�� ���� ��¦ ���� ���� ����

    void Start()
    {
        BuildFarm();
    }

    void BuildFarm()
    {
        int totalBlocks = rowCount * blocksPerRow;
        int prefabCount = cropPrefabs.Length;
        int blocksPerCrop = Mathf.FloorToInt((float)totalBlocks / prefabCount);

        int currentCropIndex = 0;
        int cropCounter = 0;

        for (int row = 0; row < rowCount; row++)
        {
            // ��� ���� ��ġ ���
            Vector3 rowOffset = verticalRows
                ? new Vector3(row * rowSpacing, 0, 0)
                : new Vector3(0, 0, row * rowSpacing);

            for (int col = 0; col < blocksPerRow; col++)
            {
                Vector3 blockPos = verticalRows
                    ? transform.position + rowOffset + new Vector3(0, 0, col * blockLength)
                    : transform.position + rowOffset + new Vector3(col * blockLength, 0, 0);

                // 1. ��� ��� ����
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.transform.position = blockPos;
                block.transform.localScale = new Vector3(blockLength, blockHeight, blockWidth);
                block.transform.parent = this.transform;

                // ���� ���� (���� ���)
                if (soilMaterial != null)
                    block.GetComponent<Renderer>().material = soilMaterial;

                // 2. �۹� ��ġ
                if (cropPrefabs.Length > 0)
                {
                    // �۹� ������ ����
                    GameObject cropPrefab = cropPrefabs[currentCropIndex];
                    Vector3 cropPos = blockPos + Vector3.up * (blockHeight / 2 + cropYOffset);
                    GameObject crop = Instantiate(cropPrefab, cropPos, Quaternion.identity, block.transform);

                    // ���� �й�
                    cropCounter++;
                    if (cropCounter >= blocksPerCrop && currentCropIndex < prefabCount - 1)
                    {
                        currentCropIndex++;
                        cropCounter = 0;
                    }
                }
            }
        }
    }
}
