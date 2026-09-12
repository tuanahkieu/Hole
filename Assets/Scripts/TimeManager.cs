using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private Image timeImg;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float timeMax;
    
    private float currentTime;
    private bool isStarted = false;
    public bool isTimeFreezed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        string selectedFocus = PlayerPrefs.GetString("SelectedFocus", "None");
        if (selectedFocus == "Time")
        {
            timeMax += 30f; 
        }

        currentTime = timeMax;
        UpdateTimerDisplay(currentTime);
    }

    public void StartTimer()
    {
        isStarted = true;
    }

    public void AddTime(float amount)
    {
        timeMax += amount;
        currentTime += amount;
        UpdateTimerDisplay(currentTime);
        isStarted = true;
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timeText != null)
        {

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void Update()
    {
        if (!isStarted) return;

        // Giảm thời gian nếu vẫn còn thời gian
        if (currentTime > 0 && !isTimeFreezed)
        {
            currentTime -= Time.deltaTime;
            
            // Cập nhật thanh Image fillAmount (chạy từ 1 về 0)
            if (timeImg != null)
            {
                timeImg.fillAmount = currentTime / timeMax;
            }

            UpdateTimerDisplay(currentTime);

            // Xử lý khi hết giờ
            if (currentTime <= 0)
            {
                currentTime = 0;
                UpdateTimerDisplay(currentTime);
                GamePlayManager.Instance.GameLose();
            }
        }
    }
}