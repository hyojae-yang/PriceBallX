using UnityEngine;

/// <summary>
/// 픽업 가능한 오브젝트에 부착되는 정보를 담습니다.
/// 크기 값과, 플레이어에 부착/분리 시 필요한 성장량만 처리합니다.
/// </summary>
public class PickupObject : MonoBehaviour
{
    [Tooltip("오브젝트의 크기 값 (예: 부피 단위). 플레이어가 이 값을 참고해 성장량을 계산합니다.")]
    [SerializeField] private float sizeValue;
   
    // 플레이어에 붙을 때 적용된 성장량 (Detach 시 플레이어 크기 감소에 사용)
    private float growthAmountWhenAttached;

    /// <summary>
    /// 플레이어에 붙을 때 호출되는 이벤트
    /// </summary>
    public event System.Action onAttached;
    /// <summary>
    /// 오브젝트 크기 반환 (부착 조건 등에 사용)
    /// </summary>
    public float GetSizeValue() => sizeValue;

    /// <summary>
    /// 플레이어에 붙을 때 계산된 성장량 저장
    /// </summary>
    public void SetGrowthAmount(float amount) 
    { 
        growthAmountWhenAttached = amount;
        onAttached?.Invoke(); // 이벤트 발동
    }


    /// <summary>
    /// 플레이어에서 떨어질 때 줄어들 크기 반환
    /// </summary>
    public float GetGrowthAmount() => growthAmountWhenAttached;
}
