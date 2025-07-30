using UnityEngine;

/// <summary>
/// 밭 고랑 큐브를 생성하고, 작물 프리팹을 순서대로 배치하는 스크립트
/// </summary>
public class FarmBuilder : MonoBehaviour
{
    int rowCount = 15;               // 고랑(줄) 수
    int blocksPerRow = 20;           // 고랑당 큐브 개수
    float rowSpacing = 2f;           // 고랑 간 간격
    float blockLength = 1f;          // 큐브 길이 (X 또는 Z)
    float blockWidth = 1f;           // 큐브 너비 (Z 또는 X)
    float blockHeight = 0.6f;        // 큐브 높이
    public Material soilMaterial;           // 흙 재질
    public bool verticalRows = false;       // true면 Z축으로 세로로 배치

    public GameObject[] cropPrefabs;        // 작물 프리팹 배열 (예: 무, 당근, 상추 등)
    public float cropYOffset = 0.5f;        // 작물을 큐브 위로 살짝 띄우기 위한 높이

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
            // 고랑 시작 위치 계산
            Vector3 rowOffset = verticalRows
                ? new Vector3(row * rowSpacing, 0, 0)
                : new Vector3(0, 0, row * rowSpacing);

            for (int col = 0; col < blocksPerRow; col++)
            {
                Vector3 blockPos = verticalRows
                    ? transform.position + rowOffset + new Vector3(0, 0, col * blockLength)
                    : transform.position + rowOffset + new Vector3(col * blockLength, 0, 0);

                // 1. 고랑 블록 생성
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.transform.position = blockPos;
                block.transform.localScale = new Vector3(blockLength, blockHeight, blockWidth);
                block.transform.parent = this.transform;

                // 재질 설정 (있을 경우)
                if (soilMaterial != null)
                    block.GetComponent<Renderer>().material = soilMaterial;

                // 2. 작물 배치
                if (cropPrefabs.Length > 0)
                {
                    // 작물 프리팹 선택
                    GameObject cropPrefab = cropPrefabs[currentCropIndex];
                    Vector3 cropPos = blockPos + Vector3.up * (blockHeight / 2 + cropYOffset);
                    GameObject crop = Instantiate(cropPrefab, cropPos, Quaternion.identity, block.transform);

                    // 순차 분배
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
