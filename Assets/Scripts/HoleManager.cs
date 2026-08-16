using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class HoleManager : MonoBehaviour
{
    public static event System.Action<string> OnItemEatenEvent;

    [Header("Skin Settings")]
    [SerializeField] private HoleSkinData[] availableSkins;
    [SerializeField] private MeshRenderer[] holeRenderers; // Mảng chứa các MeshRenderer cần đổi Material (vd: mặt hố)
    [SerializeField] private int materialIndexToChange = 1; // Vị trí của Material cần đổi (0 là cái đầu, 1 là cái thứ hai)


    // TỐI ƯU HÓA: Lưu lại bộ nhớ đệm (cache) cho các chuỗi điểm số để tránh tạo rác (Garbage Collection)
    private static Dictionary<int, string> scoreStringCache = new Dictionary<int, string>();

    [SerializeField] private Image EnergyImg;
    [SerializeField] private Transform holeGameObj;
    [SerializeField] private TextMeshProUGUI levelText;
    // [SerializeField] private TextMeshProUGUI timer_txt;

    [SerializeField] private AudioManager audioManager;
    

    private int currentEnergy = 0;
    private int maxEnergy = 10;
    private int totalFoodCount = 0;
    private int currentHoleLevel = 1;

#if UNITY_6000_0_OR_NEWER
    private Unity.Cinemachine.CinemachineCamera virtualCamera;
    private Unity.Cinemachine.CinemachineFollow cameraFollow;
#endif

    void Start()
    {
#if UNITY_6000_0_OR_NEWER
        virtualCamera = FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
        if (virtualCamera != null)
        {
            cameraFollow = virtualCamera.GetComponent<Unity.Cinemachine.CinemachineFollow>();
        }
#endif
        if (EnergyImg != null)
        {
            EnergyImg.fillAmount = 0f;
        }

        // Đếm tổng số lượng vật thể có trong màn chơi lúc bắt đầu
        totalFoodCount = GameObject.FindGameObjectsWithTag("food").Length;

        string selectedFocus = PlayerPrefs.GetString("SelectedFocus", "");
        if (selectedFocus == "Energy")
        {
            maxEnergy += 20; 
            holeGameObj.localScale = new Vector3(holeGameObj.localScale.x * 1.15f, holeGameObj.localScale.y, holeGameObj.localScale.z * 1.15f);
            currentHoleLevel++;
            
            PlayerPrefs.SetString("SelectedFocus", "");
            PlayerPrefs.Save();
        }

        if (levelText != null)
        {
            levelText.text = "Cấp " + currentHoleLevel.ToString();
        }

        // TẢI SKIN KHI VÀO GAME
        string equippedID = PlayerPrefs.GetString("EquippedHole", "capybara");
        if (availableSkins != null && availableSkins.Length > 0)
        {
            HoleSkinData equippedData = null;
            foreach (var skin in availableSkins)
            {
                if (skin.skinID == equippedID)
                {
                    equippedData = skin;
                    break;
                }
            }
            
            // Nếu không tìm thấy, thử tìm skin capybara trước khi lấy skin đầu tiên
            if (equippedData == null)
            {
                foreach (var skin in availableSkins)
                {
                    if (skin.skinID == "capybara")
                    {
                        equippedData = skin;
                        break;
                    }
                }
                if (equippedData == null) equippedData = availableSkins[0];
            }

            if (holeRenderers != null && equippedData.inGameMaterial != null)
            {
                foreach (var renderer in holeRenderers)
                {
                    if (renderer != null)
                    {
                        Material[] mats = renderer.materials;
                        if (materialIndexToChange >= 0 && materialIndexToChange < mats.Length)
                        {
                            mats[materialIndexToChange] = equippedData.inGameMaterial;
                            renderer.materials = mats; // Gán ngược lại mảng material mới
                        }
                        else
                        {
                            renderer.material = equippedData.inGameMaterial; // Rơi vào mặc định nếu index không hợp lệ
                        }
                    }
                }
            }
        }
    }
    void Update()
    {
        
    }

#if UNITY_6000_0_OR_NEWER
    private void ZoomCamera(float multiplier)
    {
        if (cameraFollow != null)
        {
            cameraFollow.FollowOffset *= multiplier;
        }
        else if (virtualCamera != null)
        {
            virtualCamera.Lens.OrthographicSize *= multiplier;
            virtualCamera.Lens.FieldOfView *= multiplier;
        }
    }
#endif

    private void UpdateEnergyBar(int amount)
    {
        currentEnergy += amount;

        // Nếu năng lượng hiện tại đạt hoặc vượt mức tối đa của cấp độ này
        if (currentEnergy >= maxEnergy)
        {
            currentHoleLevel++;
            currentEnergy -= maxEnergy; 
            maxEnergy += Mathf.CeilToInt((5 * currentHoleLevel)); 
            audioManager.playHoleUpSound();

            // Tăng bán kính (x và z) lên 1.1 lần, giữ nguyên độ sâu (y)
            holeGameObj.localScale = new Vector3(holeGameObj.localScale.x * 1.15f, holeGameObj.localScale.y, holeGameObj.localScale.z * 1.15f);
            
#if UNITY_6000_0_OR_NEWER
            // Dịch camera xa ra 1 chút (Zoom out)
            ZoomCamera(1.05f);
#endif
            
            if (levelText != null)
            {
                levelText.text = "Cấp " + currentHoleLevel.ToString();
            }
        }

        // Cập nhật thanh UI (hiển thị phần trăm)
        if (EnergyImg != null)
        {
            EnergyImg.fillAmount = (float)currentEnergy / maxEnergy;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        EatableItem item = other.GetComponent<EatableItem>();
        if(item != null && other.CompareTag("food"))
        {
            // Âm thanh ăn
            audioManager.playEatSound();

            // Cộng điểm / năng lượng cho hố
            UpdateEnergyBar(item.scoreValue);

            // Cộng vàng vào GamePlayManager
            if (GamePlayManager.Instance != null) {
                GamePlayManager.Instance.AddGold(item.goldValue);
            }

            // Báo cho toàn bộ FoodManager biết vật phẩm này vừa bị ăn
            OnItemEatenEvent?.Invoke(item.itemName);

            // Sinh ra chữ nổi (Floating Text)
            GameObject textObj = PoolManager.Instance.Spawn(PoolManager.PoolType.FloatingText, other.transform.position);
            if(textObj != null){
                TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();

                if (!scoreStringCache.TryGetValue(item.scoreValue, out string scoreStr))
                {
                    scoreStr = "+" + item.scoreValue.ToString();
                    scoreStringCache[item.scoreValue] = scoreStr;
                }
                tmp.text = scoreStr;

                tmp.alpha = 1f;
                textObj.transform.DOMoveY(other.transform.position.y + 2f, 1f);
                tmp.DOFade(0, 1f).OnComplete(()=>{
                    PoolManager.Instance.Despawn(PoolManager.PoolType.FloatingText, textObj);
                });
            }

            // Tắt vật phẩm đi
            other.gameObject.SetActive(false);
            totalFoodCount--;
        }
    }



    public void SizeUp()
    {
        StartCoroutine(SizeUpCoroutine());
    }

    private IEnumerator SizeUpCoroutine()
    {
        // Phóng to kích thước tương đương 3 cấp độ
        for (int i = 0; i < 3; i++)
        {
            holeGameObj.localScale = new Vector3(holeGameObj.localScale.x * 1.1f, holeGameObj.localScale.y, holeGameObj.localScale.z * 1.1f);
#if UNITY_6000_0_OR_NEWER
            ZoomCamera(1.1f);
#endif
        }

        // Đợi 10 giây
        yield return new WaitForSeconds(10f);

        // Thu nhỏ lại như cũ
        for (int i = 0; i < 3; i++)
        {
            holeGameObj.localScale = new Vector3(holeGameObj.localScale.x / 1.1f, holeGameObj.localScale.y, holeGameObj.localScale.z / 1.1f);
#if UNITY_6000_0_OR_NEWER
            ZoomCamera(1f / 1.1f);
#endif
        }
    }

    public void Magnet()
    {
        StartCoroutine(MagnetCoroutine());
    }

    private IEnumerator MagnetCoroutine()
    {
        audioManager.playMagnetSound();
        float duration = 10f; // Thời gian hiệu lực của Nam châm (10s)
        float elapsed = 0f;
        
        Transform holeRim = holeGameObj.Find("HoleRim");

        while (elapsed < duration)
        {
            float radius = holeGameObj.localScale.x * 2f;
            if (holeRim != null)
            {
                radius = holeRim.lossyScale.x * 2f;
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            
            foreach (var col in colliders)
            {
                if (col.CompareTag("food"))
                {
                    // Di chuyển vật phẩm từ từ về tâm hố (chỉ theo trục ngang)
                    Vector3 targetPos = new Vector3(transform.position.x, col.transform.position.y, transform.position.z);
                    
                    Rigidbody rb = col.attachedRigidbody;
                    if (rb != null) {
                        rb.WakeUp(); // Đánh thức vật lý để tránh bị kẹt
                    }

                    // Tăng tốc độ hút (từ 1f lên 15f)
                    col.transform.position = Vector3.MoveTowards(col.transform.position, targetPos, 1f * Time.deltaTime);
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioManager.stopMagnetSound();
    }
}
