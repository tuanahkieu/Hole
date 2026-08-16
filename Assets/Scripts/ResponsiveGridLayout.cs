using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteAlways]
public class ResponsiveGridLayout : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    [Header("Tỉ lệ 1 ô (Rộng / Cao)")]
    [Tooltip("Dựa theo ảnh cũ của bạn là 350 / 540 = 0.648")]
    public float cellAspectRatio = 0.648f;

    private void OnEnable()
    {
        UpdateCellSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateCellSize();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateCellSize();
    }
#endif

    public void UpdateCellSize()
    {
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        if (grid == null || rectTransform == null) return;

        // Ưu tiên lấy số cột từ cài đặt Constraint của Grid Layout Group
        int columns = grid.constraintCount;
        if (grid.constraint != GridLayoutGroup.Constraint.FixedColumnCount || columns <= 0)
        {
            columns = 3; // Mặc định là 3 cột nếu bạn chưa set Fixed Column Count
        }

        // Tính toán chiều rộng khả dụng sau khi trừ đi Padding và Spacing
        float availableWidth = rectTransform.rect.width 
                               - grid.padding.left 
                               - grid.padding.right 
                               - (grid.spacing.x * (columns - 1));

        // Tính toán kích thước mỗi ô
        float cellWidth = availableWidth / columns;
        float cellHeight = cellWidth / cellAspectRatio;

        // Cập nhật lại Cell Size cho Grid Layout Group
        if (cellWidth > 0 && cellHeight > 0)
        {
            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }
}
