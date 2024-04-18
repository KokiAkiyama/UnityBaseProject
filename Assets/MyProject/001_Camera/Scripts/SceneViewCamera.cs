using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// GameビューにてSceneビューのようなカメラの動きをマウス操作によって実現する
/// </summary>
[RequireComponent(typeof(Camera))]
public class SceneViewCamera : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)]
    private float wheelSpeed = 1f;

    [SerializeField, Range(5f, 15f)]
    private float moveSpeed = 8.5f;

    [SerializeField, Range(0.1f, 10f)]
    private float rotateSpeed = 0.3f;

    private Vector3 preMousePos;

    private void Update()
    {
        MouseUpdate();
        AxisUpdate();
    }

    private void MouseUpdate()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollWheel != 0.0f)
        {
            MouseWheel(scrollWheel);
        }

        if (Input.GetMouseButtonDown(0)
            || Input.GetMouseButtonDown(1)
            || Input.GetMouseButtonDown(2))
        {

            preMousePos = Input.mousePosition;
        }


        

        MouseDrag(Input.mousePosition);
    }

    private void MouseWheel(float delta)
    {
        transform.position += delta * wheelSpeed * transform.forward;
    }

    private void MouseDrag(Vector3 mousePos)
    {
        Vector3 diff = mousePos - preMousePos;

        if (diff.magnitude < Vector3.kEpsilon) return;
        
        
        else if (Input.GetMouseButton(1) && GameManager.Instance.InputManager.Game["CamRotButton"].IsPressed())
        {
            CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);
        }

        preMousePos = mousePos;
    }

    private void AxisUpdate()
    {
        
        // float vertical   = Input.GetAxisRaw("Vertical");
        // float horizontal = Input.GetAxisRaw("Horizontal");
        float horizontal = GameManager.Instance.InputManager.Game["CamMoveX"].ReadValue<float>();
        float vertical   = GameManager.Instance.InputManager.Game["CamMoveY"].ReadValue<float>();
        transform.Translate(new Vector3(0.0f, vertical * Time.unscaledDeltaTime * moveSpeed, 0.0f));
        transform.Translate(new Vector3(horizontal * Time.unscaledDeltaTime * moveSpeed, 0.0f, 0.0f));
    }

    public void CameraRotate(Vector2 angle)
    {
        transform.RotateAround(
            point: transform.position,
            axis: transform.right,
            angle: angle.x
        );
        
        transform.RotateAround(
            point: transform.position,
            axis: Vector3.up,
            angle: angle.y
        );
    }
}


