using UnityEngine;
using UnityEngine.Events;

public class InputControl : Singleton<InputControl>
{
    public UnityAction<int> OnNumPressed = null;
    public UnityAction<int> OnScrollRoll = null;
    [Space]

    public UnityAction OnPressEscape = null;
    public UnityAction OnPressTab = null;

    public UnityAction OnPressMouse0 = null;
    public UnityAction OnReleasingMouse0 = null;
    public UnityAction WhilePressMouse0 = null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetButtonPressed(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetButtonPressed(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetButtonPressed(2);

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
            SetScrollRotation((int)Mathf.Sign(scroll));


        if (Input.GetKeyDown(KeyCode.Escape)) OnPressEscape?.Invoke();
        
        if (Input.GetKey(KeyCode.Mouse0)) WhilePressMouse0?.Invoke();
    }

    private void SetButtonPressed(int num)
    {
        OnNumPressed?.Invoke(num);
    }

    private void SetScrollRotation(int num)
    {
        OnScrollRoll?.Invoke(num);
    }
}
