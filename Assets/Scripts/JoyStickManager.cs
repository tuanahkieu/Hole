using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
public class JoystickManager : MonoBehaviour
{
    public static JoystickManager JoystickManagerInstance;
    [SerializeField] private Vector2 JoystickSize = new Vector2(200, 200);
    public JoyStick Joystick;
    public NavMeshAgent playerNavMeshAgent;
    private Finger MovementFinger;
    public Vector2 MovementAmount;
    private Canvas canvas;
    private float initialY;

    void Start()
    {
        playerNavMeshAgent = GetComponent<NavMeshAgent>();
        playerNavMeshAgent.updatePosition = false;
        initialY = transform.position.y;

        JoystickManagerInstance = this;
        if (Joystick != null)
        {
            canvas = Joystick.GetComponentInParent<Canvas>();
        }
    }
    
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    
    }
    private void HandleFingerMove(Finger movedFinger)
    {
        if (Time.timeScale == 0f) return; // Không xử lý khi đang tạm dừng

        if (movedFinger == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = movedFinger.currentTouch;

            Vector2 scaledTouchPos = currentTouch.screenPosition;
            if (canvas != null)
            {
                scaledTouchPos /= canvas.scaleFactor;
            }

            if (Vector2.Distance(scaledTouchPos, Joystick.joyStickObj.anchoredPosition) > maxMovement)
            {
                knobPosition = (scaledTouchPos - Joystick.joyStickObj.anchoredPosition).normalized * maxMovement;
            }
            else
            {
                knobPosition = scaledTouchPos - Joystick.joyStickObj.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }
        
    }
   
    private void HandleFingerDown(Finger touchedFinger)
    {
        if (Time.timeScale == 0f) return; // Không xử lý khi đang tạm dừng

        if (MovementFinger == null && touchedFinger.screenPosition.x <= Screen.width)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(true);
            Joystick.joyStickObj.sizeDelta = JoystickSize;

            Vector2 scaledTouchPos = touchedFinger.screenPosition;
            if (canvas != null)
            {
                scaledTouchPos /= canvas.scaleFactor;
            }

            Joystick.joyStickObj.anchoredPosition = ClampStartPosition(scaledTouchPos);
        }
    }
    
    
    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
        }
    }
    
    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < JoystickSize.x / 2)
        {
            startPosition.x = JoystickSize.x / 2;
        }

        if (startPosition.y < JoystickSize.y / 2)
        {
            startPosition.y = JoystickSize.y / 2;
        }
        else 
        {
            float scaledScreenHeight = Screen.height;
            if (canvas != null)
            {
                scaledScreenHeight /= canvas.scaleFactor;
            }

            if (startPosition.y > scaledScreenHeight - JoystickSize.y / 2)
            {
                startPosition.y = scaledScreenHeight - JoystickSize.y / 2;
            }
        }

        return startPosition;
    }
    void Update()
    {
        if (Time.timeScale == 0f) 
        {
            // Bắt buộc nhả cần điều khiển nếu game đang bị tạm dừng (có popup)
            if (MovementFinger != null)
            {
                MovementFinger = null;
                Joystick.Knob.anchoredPosition = Vector2.zero;
                Joystick.gameObject.SetActive(false);
                MovementAmount = Vector2.zero;
            }
            return;
        }

        if (MovementAmount != Vector2.zero && TimeManager.Instance != null)
        {
            TimeManager.Instance.StartTimer();
        }

        Vector3 scaledMovement = playerNavMeshAgent.speed * Time.deltaTime * new Vector3(MovementAmount.x, 0, MovementAmount.y);
        playerNavMeshAgent.Move(scaledMovement);

        Vector3 nextPos = playerNavMeshAgent.nextPosition;
        Vector3 currentPos = transform.position;
        
        // Sync position without changing Y to avoid breaking the depth mask
        // Also avoid unnecessary assignments to prevent transform.hasChanged from triggering when idle
        if (currentPos.x != nextPos.x || currentPos.z != nextPos.z)
        {
            transform.position = new Vector3(nextPos.x, 0f, nextPos.z);
        }
    }
}
