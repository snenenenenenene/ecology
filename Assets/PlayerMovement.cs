// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {

//     [SerializeField]
//     private float maximumSpeed;

//     [SerializeField]
//     private float rotationSpeed;

//     [SerializeField]
//     private float jumpSpeed;

//     [SerializeField]
//     private float jumpButtonGracePeriod;

//     [SerializeField]
//     private Transform cameraTransform;

//     private Animator animator;
//     private CharacterController characterController;
//     private float ySpeed;
//     private float originalStepOffset;
//     private float? lastGroundedTime;
//     private float? jumpButtonPressedTime;



//     void Start()
//     {
//         animator = GetComponent<Animator>();
//         characterController = GetComponent<CharacterController>();
//         originalStepOffset = characterController.stepOffset;

//     }

//     public void StartPlacement(int ID)
//     {
//         // set the placable object to the object at the index in the list
//         selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
//         placableObject = database.objectsData[selectedObjectIndex].Prefab;
//     }

//     void Update()
//     {
        // // if pressing left mouse button place object at mouse position on the ground and on the grid
        // if (Input.GetMouseButtonDown(0))
        // {
        //     RaycastHit hit;
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //     // if raycast hits something
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         // if raycast hits the ground
        //         if (hit.transform.tag == "Ground" || hit.transform.tag == "Buildable")
        //         {

        //             Vector3 position = hit.point;
        //             position.x = Mathf.Round(position.x / 10) * 10;
        //             position.z = Mathf.Round(position.z / 10) * 10;

        //             position.y = Mathf.Round(position.y / 10) * 10 + 5;

        //             if (hit.transform.tag == "Buildable")
        //             {
        //                 // if raycast hits a buildable object, place the object on the face of the object being clicked
        //                 position = hit.transform.position + hit.normal * 10;

        //             }

        //             Instantiate(placableObject, position, Quaternion.identity);
        //         }
        //     }
        // }

//         float horizontalInput = Input.GetAxis("Horizontal");
//         float verticalInput = Input.GetAxis("Vertical");

//         Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
//         float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

//         if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
//         {
//             inputMagnitude /= 2;
//         }

//         if (animator) animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

//         float speed = inputMagnitude * maximumSpeed;
//         movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
//         movementDirection.Normalize();

//         ySpeed += Physics.gravity.y * Time.deltaTime;

//         if (characterController.isGrounded)
//         {
//             lastGroundedTime = Time.time;
//         }

//         if (Input.GetButtonDown("Jump"))
//         {
//             jumpButtonPressedTime = Time.time;
//         }

//         if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
//         {
//             characterController.stepOffset = originalStepOffset;
//             ySpeed = -0.5f;

//             if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
//             {
//                 ySpeed = jumpSpeed;
//                 jumpButtonPressedTime = null;
//                 lastGroundedTime = null;
//             }
//         }
//         else
//         {
//             characterController.stepOffset = 0;
//         }

//         Vector3 velocity = movementDirection * speed;
//         velocity.y = ySpeed;

//         characterController.Move(velocity * Time.deltaTime);

//         if (movementDirection != Vector3.zero)
//         {
//             Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

//             transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
//         }

//     }


// }



//   float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
//             float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothTime, turnSmoothTime);
//             transform.rotation = Quaternion.Euler(0f, angle, 0f);

//             Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

//             controller.Move(moveDir.normalized * playerSpeed * Time.deltaTime);


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    public Vector2 _move;
    public Vector2 _look;
    public float aimValue;
    public float fireValue;
      public GameObject placableObject;

    [SerializeField]
    private ObjectsDatabaseSO database;
    public int selectedObjectIndex = 0;

    public Vector3 nextPosition;
    public Quaternion nextRotation;

    public float rotationPower = 3f;
    public float rotationLerp = 0.5f;

    public float speed = 1f;
    public Camera camera;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }

    public void OnAim(InputValue value)
    {
        aimValue = value.Get<float>();
    }

    public void OnFire(InputValue value)
    {
        fireValue = value.Get<float>();
    }

    public GameObject followTransform;

    private void Update()
    {
        #region Player Based Rotation

        //Move the player based on the X input on the controller
        //transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);

        #endregion

        #region Follow Transform Rotation

        //Rotate the Follow Target transform based on the input
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);

        #endregion

        #region Building

        // if pressing left mouse button place object at mouse position on the ground and on the grid
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // if raycast hits something
            if (Physics.Raycast(ray, out hit))
            {
                // if raycast hits the ground
                if (hit.transform.tag == "Ground" || hit.transform.tag == "Buildable")
                {

                    Vector3 buildPosition = hit.point;
                    buildPosition.x = Mathf.Round(buildPosition.x / 10) * 10;
                    buildPosition.z = Mathf.Round(buildPosition.z / 10) * 10;

                    buildPosition.y = Mathf.Round(buildPosition.y / 10) * 10 + 5;

                    if (hit.transform.tag == "Buildable")
                    {
                        // if raycast hits a buildable object, place the object on the face of the object being clicked
                        buildPosition = hit.transform.position + hit.normal * 10;

                    }

                    Instantiate(placableObject, buildPosition, Quaternion.identity);
                }
            }
        }

        #endregion

        #region Vertical Rotation
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if(angle < 180 && angle > 40)
        {
            angles.x = 40;
        }


        followTransform.transform.localEulerAngles = angles;
        #endregion


        nextRotation = Quaternion.Lerp(followTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

        if (_move.x == 0 && _move.y == 0)
        {
            nextPosition = transform.position;

            if (aimValue == 1)
            {
                //Set the player rotation based on the look transform
                transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
                followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
            }

            return;
        }
        float moveSpeed = speed;
        // float moveSpeed = speed / 100f;
        Vector3 position = (transform.forward * _move.y * moveSpeed) + (transform.right * _move.x * moveSpeed);
        nextPosition = transform.position + position;


        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        Debug.Log(nextPosition);
    }


}
