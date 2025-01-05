using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction touchPositionAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void Start()
    {
        touchPositionAction.performed += OnTouchPositionAction;
    }

    private void OnTouchPositionAction(InputAction.CallbackContext ctx)
    {
        Vector2 touchPosition = ctx.ReadValue<Vector2>();

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