using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction touchPositionAction;
    InputAction touchPressAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPressAction = playerInput.actions["TouchPress"];
    }

    private void Start()
    {
        touchPressAction.performed += TouchPress;
    }

    private void TouchPress(InputAction.CallbackContext ctx)
    {
        Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();

        //Debug.Log("touch tap pos : " + touchPosition);

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                //Debug.Log("hit : " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.TryGetComponent<ITouch>(out ITouch touchObj))
                {
                    touchObj.OnTouch();
                }
            }
        }
    }
}