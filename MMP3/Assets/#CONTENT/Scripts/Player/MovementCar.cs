using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float motorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public float brakeForce = 3000f;

    private float steeringInput;
    private float accelerationInput;
    private float brakeInput;

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate called");
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        steeringInput = input.x;
        accelerationInput = input.y;
        Debug.Log("Steering Input: " + steeringInput + ", Acceleration Input: " + accelerationInput);
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = context.ReadValue<float>();
        Debug.Log("Brake Input: " + brakeInput);
    }

    void HandleMotor()
    {
        float torque = accelerationInput * motorTorque;
        frontLeft.motorTorque = torque;
        frontRight.motorTorque = torque;

        float brake = brakeInput * brakeForce;
        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
        rearLeft.brakeTorque = brake;
        rearRight.brakeTorque = brake;
    }

    void HandleSteering()
    {
        float steer = steeringInput * maxSteerAngle;
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;
    }

    void UpdateWheels()
    {
        UpdateWheelPose(frontLeft, frontLeftMesh);
        UpdateWheelPose(frontRight, frontRightMesh);
        UpdateWheelPose(rearLeft, rearLeftMesh);
        UpdateWheelPose(rearRight, rearRightMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
