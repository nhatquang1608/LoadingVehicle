using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Move")]
    public static Vector2 moveDirection;
    private enum DirectionType
    {
        Left,
        Right
    }
    private DirectionType directionType;
    [SerializeField] private bool isReleasing;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool onSurface;
    private float surfaceDistance = 0.4f;
    [SerializeField] private LayerMask surfaceMask;
    public bool holdingBox;
    public bool holdingNail;
    [SerializeField] private HingeJoint2D hingeJoint2D;
    [SerializeField] private HingeJoint2D hingeJoint42D;
    [SerializeField] private Rigidbody2D robotRB;
    [SerializeField] private Rigidbody2D frontTireRB;
    [SerializeField] private Rigidbody2D backTireRB;

    [Header("IK")]
    public Vector3[] listTargetJointsPosition;
    public Quaternion[] listTargetJointsRotation;
    public Transform box;
    public Transform nail;
    [SerializeField] private EasyIK easyIK;
    [SerializeField] private GameObject[] grippers;
    [SerializeField] private GameObject[] armJoints;
    [SerializeField] private float moveSpeed;
    private float maxSpeed = 500;
    private float maxSpeedOnJump = 200;
    private float jumpForce = 1800;
    private Vector3 armAngleHoldingNail = new Vector3(-2.2f, -107.23f, -4.9f);
    private Vector3 armAngleLeft = new Vector3(-20, -20, 20);
    private Vector3 armAngleRight = new Vector3(20, -190, -35);
    private Vector2 grippersAngleClose = new Vector2(150, 80);
    private Vector2 grippersAngleOpen = new Vector2(180, 50);

    private void OnEnable()
    {
        hingeJoint2D.enabled = false;
        GameManager.OnJump += Jump;
        GameManager.OnGrab += Grab;
    }

    private async void Update()
    {
        // if(Physics2D.OverlapCircle(frontTireRB.transform.position, surfaceDistance, surfaceMask) ||
        // Physics2D.OverlapCircle(backTireRB.transform.position, surfaceDistance, surfaceMask))
        // {
        //     onSurface = true;
        // }
        // else
        // {
        //     onSurface = false;
        // }

        RaycastHit2D frontTireHit = Physics2D.Raycast(frontTireRB.transform.position, Vector2.down, surfaceDistance, surfaceMask);
        RaycastHit2D backTireHit = Physics2D.Raycast(backTireRB.transform.position, Vector2.down, surfaceDistance, surfaceMask);

        if (frontTireHit.collider || backTireHit.collider)
        {
            onSurface = true;
        }
        else
        {
            onSurface = false;
        }

        if(Input.GetKey(KeyCode.D) || moveDirection.x == 1)
        {
            moveSpeed = maxSpeed;
            if(directionType == DirectionType.Left) 
            {
                directionType = DirectionType.Right;
                if(!holdingNail) await SetArmAngle(armAngleRight);
            }
        }
        else if(Input.GetKey(KeyCode.A) || moveDirection.x == -1)
        {
            moveSpeed = maxSpeed;
            if(directionType == DirectionType.Right) 
            {
                directionType = DirectionType.Left;
                if(!holdingNail) await SetArmAngle(armAngleLeft);
            }
        }
        else
        {
            moveSpeed = 0;
            frontTireRB.angularVelocity = 0;
            backTireRB.angularVelocity = 0;
        }

        if(Input.GetKeyDown(KeyCode.W) && (onSurface || holdingNail))
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Grab();
        }
    }

    private void FixedUpdate()
    {
        if(onSurface)
        {
            isJumping = false;
            frontTireRB.AddTorque(-moveDirection.x * moveSpeed * Time.fixedDeltaTime);
            backTireRB.AddTorque(-moveDirection.x * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if(isJumping) 
            {
                robotRB.velocity = new Vector3(moveDirection.x * maxSpeedOnJump * Time.fixedDeltaTime, robotRB.velocity.y, 0f);
                if(holdingNail) isJumping = false;
            }
            else if(holdingNail)
            {
                robotRB.AddTorque(moveDirection.x * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private async void Grab()
    {
        if(isReleasing) return;
        if(!holdingBox && !nail && box)
        {
            while(box)
            {
                if(!box.GetComponent<Box>().canBeGrabbed || Vector2.Distance(armJoints[3].transform.position, box.transform.position) <= 0.5) 
                {
                    break;
                }
                easyIK.SolveIK(box);
                await ArmControl();
            }
            if(box)
            {
                box.transform.position = armJoints[3].transform.position;
                box.GetComponent<BoxCollider2D>().isTrigger = true;
                box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                box.SetParent(armJoints[3].transform);
                holdingBox = true;
                box.GetComponent<Box>().canBeThrown = true;
            }
            else
            {
                await SetGripperAngle(grippersAngleClose);
            }

            if(directionType == DirectionType.Right) await SetArmAngle(armAngleRight);
            else await SetArmAngle(armAngleLeft);
        }
        else if(holdingBox)
        {
            if(!box) return;
            isReleasing = true;
            box.SetParent(null);
            box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            box.GetComponent<BoxCollider2D>().isTrigger = false;
            holdingBox = false;
            if(box.GetComponent<Box>().canBeThrown)
            {
                box.GetComponent<Box>().canBeThrown = false;
                Vector3 throwDirection = (armJoints[2].transform.position - armJoints[1].transform.position).normalized;
                box.GetComponent<Rigidbody2D>().AddForce(throwDirection * 500);
            }
            await SetGripperAngle(grippersAngleClose);
            isReleasing = false;
        }
        else if(!holdingNail && !box && nail)
        {
            while(nail)
            {
                if(!nail.GetComponent<Nail>().canBeGrabbed || Vector2.Distance(armJoints[3].transform.position, nail.transform.position) <= 0.5) 
                {
                    break;
                }
                easyIK.SolveIK(nail);
                transform.SetParent(null);
                await ArmControl();
            }
            if(nail)
            {
                transform.SetParent(null);
                hingeJoint42D.connectedBody = nail.GetComponent<Rigidbody2D>();
                hingeJoint42D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                Vector2 localAnchor = hingeJoint2D.transform.InverseTransformPoint(hingeJoint42D.transform.position);
                hingeJoint2D.anchor = localAnchor;
                hingeJoint2D.enabled = true;
                robotRB.constraints = RigidbodyConstraints2D.None;
                holdingNail = true;
                await SetGripperAngle(grippersAngleClose);
            }
            else
            {
                isReleasing = true;
                hingeJoint2D.enabled = false;
                hingeJoint42D.connectedBody = null;
                hingeJoint42D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                robotRB.constraints = RigidbodyConstraints2D.FreezeRotation;
                holdingNail = false;
                isReleasing = false;

                await SetGripperAngle(grippersAngleClose);
                if(directionType == DirectionType.Right) await SetArmAngle(armAngleRight);
                else await SetArmAngle(armAngleLeft);
            }
        }
        else if(holdingNail)
        {
            isReleasing = true;
            holdingNail = false;
            hingeJoint2D.enabled = false;
            hingeJoint42D.connectedBody = null;
            hingeJoint42D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            robotRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            isReleasing = false;

            await SetGripperAngle(grippersAngleClose);
            if(directionType == DirectionType.Right) await SetArmAngle(armAngleRight);
            else await SetArmAngle(armAngleLeft);
        }
        else
        {
            await SetGripperAngle(grippersAngleOpen);
            await SetGripperAngle(grippersAngleClose);
        }
    }

    private async void Jump()
    {
        if(onSurface || holdingNail) await OnJump();
    }

    private async UniTask OnJump()
    {
        if(holdingNail)
        {
            holdingNail = false;
            hingeJoint2D.enabled = false;
            hingeJoint42D.connectedBody = null;
            hingeJoint42D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        robotRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        robotRB.AddForce(Vector2.up * jumpForce);

        await UniTask.Delay(TimeSpan.FromSeconds(0.1));
        isJumping = true;
        await SetGripperAngle(grippersAngleClose);
        if(directionType == DirectionType.Right) await SetArmAngle(armAngleRight);
        else await SetArmAngle(armAngleLeft);
    }

    private async UniTask ArmControl()
    {
        grippers[0].transform.DOLocalRotate(new Vector3(0, 0, grippersAngleOpen.x), 0.25f);
        grippers[1].transform.DOLocalRotate(new Vector3(0, 0, grippersAngleOpen.y), 0.25f);
        for(int i=0; i<3; i++)
        {
            // armJoints[i].transform.DOLocalMove(listTargetJointsPosition[i], 0);
            armJoints[i].transform.DORotate(listTargetJointsRotation[i].eulerAngles, 0.25f);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
    }

    private async UniTask SetArmAngle(Vector3 angle)
    {
        armJoints[0].transform.DORotate(new Vector3(0, 0, angle.x), 0.25f);
        armJoints[1].transform.DOLocalRotate(new Vector3(0, 0, angle.y), 0.25f);
        armJoints[2].transform.DOLocalRotate(new Vector3(0, 0, angle.z), 0.25f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
    }

    private async UniTask SetGripperAngle(Vector2 angle)
    {
        grippers[0].transform.DOLocalRotate(new Vector3(0, 0, angle.x), 0.25f);
        grippers[1].transform.DOLocalRotate(new Vector3(0, 0, angle.y), 0.25f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
    }

    private void OnDisable()
    {
        GameManager.OnJump -= Jump;
        GameManager.OnGrab -= Grab;
    }
}
