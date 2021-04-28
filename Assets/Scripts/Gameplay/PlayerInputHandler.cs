using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float LookSensitivity = 1f;

    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float WebglLookSensitivityMultiplier = 0.25f;

    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float TriggerAxisThreshold = 0.4f;

    [Tooltip("Used to flip the vertical input axis")]
    public bool InvertYAxis = false;

    [Tooltip("Used to flip the horizontal input axis")]
    public bool InvertXAxis = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
       if (CanProcessInput())
        {    
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f,
                Input.GetAxisRaw("Vertical"));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }
    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown("Jump");
        }

        return false;
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseLookAxis("Mouse X");
    }

    public float GetLookInputsVertical()
    {
        return GetMouseLookAxis("Mouse Y");
    }

    float GetMouseLookAxis(string axe) //Mouse X , Mouse Y
    {
        if (CanProcessInput())
        {
            // Check if this look input is coming from the mouse
            //bool isGamepad = Input.GetAxis(stickInputName) != 0f;
            float i = Input.GetAxisRaw(axe);

            i *= LookSensitivity;

           
            // reduce mouse input amount to be equivalent to stick movement
            i *= 0.01f;
#if UNITY_WEBGL
                // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                i *= WebglLookSensitivityMultiplier;
#endif

            return i;
        }

        return 0f;
    }
}
