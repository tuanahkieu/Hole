using UnityEngine;
using System.Collections.Generic;

public class TargetPointer : MonoBehaviour
{
    [Tooltip("Kéo Transform của người chơi (Hố đen) vào đây. Nếu script nằm trên Hố đen, bạn có thể để trống.")]
    public Transform player;

    [Tooltip("Kéo object Mũi Tên vào đây. Trống nếu mũi tên chính là object gắn script này.")]
    public Transform arrow;

    [Tooltip("Tốc độ xoay mũi tên (càng lớn càng nhanh).")]
    public float rotationSpeed = 10f;

    [Tooltip("Đánh dấu true nếu mũi tên nằm trong Canvas UI (như hình Compass bạn chụp). False nếu nó nằm ngoài không gian 3D.")]
    public bool isUI = true;

    [Tooltip("Khoảng cách cách ra so với viền của hố đen (dành cho UI Canvas, cỡ 100-200 pixel).")]
    public float uiOrbitRadius = 150f;

    [Tooltip("Khoảng đệm (padding) cách ra so với viền hố đen (dành cho Không gian 3D World).")]
    public float worldPaddingOffset = 0.5f;

    [Tooltip("Hệ số bán kính của Hố (thường là 0.5 vì bán kính = 1 nửa scale).")]
    public float worldRadiusMultiplier = 0.5f;

    [Tooltip("Độ cao của mũi tên so với hố đen trong Không gian 3D.")]
    public float worldHeightOffset = 0.5f;

    [Tooltip("Dịch tâm xoay dọc theo trục Z (điền số âm để kéo tâm xoay xuống dưới cho khớp với miệng hố).")]
    public float worldZOffset = -0.2f;

    private FoodManager[] foodManagers;
    private RectTransform rectArrow;
    
    // Biến kiểm soát xem la bàn có đang được bật hay không
    private bool isCompassActive = false;

    // TỐI ƯU HÓA: Không tìm mục tiêu mỗi frame để giảm giật lag
    private float targetSearchInterval = 0.2f;
    private float nextTargetSearchTime;
    private Transform currentNearestTarget;
    private bool hasNeededItems = false;

    // Biến lưu kích thước gốc để phóng to theo hố
    private Vector3 initialArrowScale;
    private float initialPlayerScaleX;

    void Start()
    {
        if (player == null) 
        {
            // Tránh dùng FindObjectOfType vì có thể tìm nhầm cục quản lý
            GameObject h = GameObject.Find("Hole");
            if (h != null) player = h.transform;
            else player = transform;
        }
        else
        {
            // Tự động sửa lỗi nếu user kéo nhầm HoleParent
            if (player.name == "HoleParent")
            {
                Transform actualHole = player.Find("Hole");
                if (actualHole != null) player = actualHole;
            }
        }

        if (arrow == null) arrow = transform;
        
        rectArrow = arrow.GetComponent<RectTransform>();
        
        foodManagers = FindObjectsOfType<FoodManager>();
        
        // Lưu lại kích thước ban đầu
        initialArrowScale = arrow.localScale;
        initialPlayerScaleX = player.localScale.x != 0 ? player.localScale.x : 1f;

        // Mặc định ẩn mũi tên khi mới vào game
        arrow.gameObject.SetActive(false);
    }

    // Hàm này sẽ được gọi từ GamePlayManager hoặc Button UI
    public void ActivateCompass(float duration = 10f)
    {
        StopAllCoroutines(); // Nếu bấm liên tiếp thì reset thời gian lại từ đầu
        StartCoroutine(CompassRoutine(duration));
    }

    private System.Collections.IEnumerator CompassRoutine(float duration)
    {
        isCompassActive = true;
        // Hiện mũi tên ngay khi bật (sẽ được Update xử lý vị trí)
        yield return new WaitForSeconds(duration);
        isCompassActive = false;
        
        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu la bàn chưa được kích hoạt thì không làm gì cả
        if (!isCompassActive) 
        {
            if (arrow != null && arrow.gameObject.activeSelf) 
                arrow.gameObject.SetActive(false);
            return;
        }

        // Khắc phục lỗi Race Condition: Nếu lúc Start chưa có FoodManager nào (do TargetManager spawn chậm hơn)
        if (foodManagers == null || foodManagers.Length == 0)
        {
            foodManagers = FindObjectsOfType<FoodManager>();
            if (foodManagers == null || foodManagers.Length == 0) return;
        }

        // Tạo tâm ảo (nếu hình ảnh hố không nằm chính giữa Transform)
        Vector3 virtualCenter = player.position + new Vector3(0, 0, worldZOffset);

        // TỐI ƯU HÓA: Chỉ quét tìm mục tiêu mới sau mỗi khoảng thời gian (0.2s)
        if (Time.time >= nextTargetSearchTime)
        {
            nextTargetSearchTime = Time.time + targetSearchInterval;

            // B1: Lấy danh sách tên vật phẩm cần ăn
            HashSet<string> neededItems = new HashSet<string>();
            foreach (var fm in foodManagers)
            {
                if (fm.currentAmount > 0 && !string.IsNullOrEmpty(fm.targetItemName))
                {
                    neededItems.Add(fm.targetItemName.Trim().ToLower());
                }
            }

            hasNeededItems = neededItems.Count > 0;

            if (hasNeededItems)
            {
                // B3: Tìm vật phẩm gần nhất
                float minDistance = float.MaxValue;
                currentNearestTarget = null;

                foreach (var item in EatableItem.AllItems)
                {
                    if (item != null && !string.IsNullOrEmpty(item.normalizedItemName))
                    {
                        if (neededItems.Contains(item.normalizedItemName))
                        {
                            float dist = Vector3.Distance(virtualCenter, item.transform.position);
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                currentNearestTarget = item.transform;
                            }
                        }
                    }
                }
            }
        }

        // B2: Ẩn mũi tên nếu không cần ăn gì
        if (!hasNeededItems)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        // B4: Di chuyển và xoay mũi tên
        if (currentNearestTarget != null)
        {
            arrow.gameObject.SetActive(true);
            Vector3 direction = currentNearestTarget.position - virtualCenter;
            direction.y = 0; // Bỏ qua trục Y (chiều cao)
            
            if (direction != Vector3.zero)
            {
                direction.Normalize();

                if (isUI && rectArrow != null)
                {
                    // === KHÔNG GIAN UI (CANVAS) ===
                    Vector2 screenDir = new Vector2(direction.x, direction.z); 
                    
                    // 1. Di chuyển xoay quanh tâm (Tức thời, không bị trễ)
                    rectArrow.anchoredPosition = screenDir * uiOrbitRadius;

                    // 2. Xoay mũi tên
                    float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f); 
                    rectArrow.rotation = targetRotation;
                }
                else
                {
                    // === KHÔNG GIAN 3D (VẬT THỂ WORLD SPACE) ===
                    // Tính bán kính: dựa vào Scale của Hố + Khoảng đệm
                    float currentRadius = (player.localScale.x * worldRadiusMultiplier) + worldPaddingOffset;
                    Vector3 targetPosition = virtualCenter + direction * currentRadius;
                    targetPosition.y = player.position.y + worldHeightOffset; 
                    
                    // Cập nhật vị trí TỨC THỜI (để không bị lệch khi nhân vật xoay người)
                    arrow.position = targetPosition;

                    // Để xoay mũi tên 3D nằm dẹp dưới đất và trỏ về phía mục tiêu
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(90f, angle, 0f); 
                    
                    // Cập nhật góc xoay TỨC THỜI
                    arrow.rotation = targetRotation;

                    // Phóng to mũi tên tỷ lệ thuận với hố
                    float scaleRatio = player.localScale.x / initialPlayerScaleX;
                    arrow.localScale = initialArrowScale * scaleRatio;
                }
            }
        }
        else
        {
            // Nếu không tìm thấy vật phẩm nào trên map (dù vẫn còn target), tạm ẩn mũi tên
            arrow.gameObject.SetActive(false);
        }
    }
}
