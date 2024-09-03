using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositioner : MonoBehaviour
{
    public float smoothFactor = 2;
    public Transform target;
    public Vector3 offset = new Vector3(0, .15f, .4f);
    public Vector3 euler = new Vector3(15, 0, 0);
    private bool isFollowing = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private CanvasGroup canvasGroup;
    private Transform anchorCenter;
    private bool isInArmMode = true;
    private GameObject screen;
    private bool isScreenVisible = true;
    
    // References to the poke buttons
    public GameObject placePoke;
    public GameObject followPoke;
    public GameObject armPoke;

    private void Start()
    {
        // Store the original parent, local position, and rotation
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        
        // Initially, the menu is attached to the left hand
        isFollowing = false;
        
        // Find the screen GameObject
        screen = transform.Find("Main Canvas/Screen").gameObject;
        if (screen == null)
        {
            Debug.LogError("Screen GameObject not found!");
        }
        
        

        // Find the CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on Menu!");
        }

        // Find the AnchorCenter GameObject
        anchorCenter = transform.Find("Main Canvas/TabBar/AnchorCenter");
        if (anchorCenter == null)
        {
            Debug.LogError("AnchorCenter GameObject not found!");
        }

        // Check the initial angle
        CheckAnchorAngle();
    }

    void Update()
    {
        if (isFollowing)
        {
            Vector3 targetPos = GetTargetPos();
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);

            Quaternion targetRot = GetTargetRot();
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothFactor * Time.deltaTime);
        }

        if (isInArmMode)
        {
            CheckAnchorAngle();
        }
    }

    Vector3 GetTargetPos()
    {
        Vector3 targetPos = target.TransformPoint(offset);
        Vector3 forward = Vector3.ProjectOnPlane(target.forward, Vector3.up);
        targetPos = target.position + (forward * offset.z);
        targetPos.y = target.position.y - offset.y;
        return targetPos;
    }

    Quaternion GetTargetRot()
    {
        return Quaternion.Euler(euler.x, target.eulerAngles.y, euler.z);
    }

    public void Place()
    {
        isFollowing = false;
        isInArmMode = false;
        transform.SetParent(null); // Ensure the menu is not a child of the hand when placed
    }

    public void FollowMe()
    {
        isFollowing = true;
        isInArmMode = false;
        transform.SetParent(null); // Ensure the menu is not a child of the hand when following
    }

    public void Arm()
    {
        // Re-parent the menu back to the original parent (left hand)
        transform.SetParent(originalParent);
        // Reset the local position and rotation to the original values
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        isFollowing = false;
        isInArmMode = true;
    }

    public void ShowHideScreen()
    {
        if (screen != null)
        {
            isScreenVisible = !isScreenVisible;
            screen.SetActive(isScreenVisible);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckAnchorAngle()
    {
        if (anchorCenter != null)
        {
            float angle = Vector3.Angle(anchorCenter.up, Vector3.up);
            Debug.Log($"AnchorCenter angle: {angle}");
            if (angle > 60.0f)
            {
                // Hide the menu
                Debug.Log("Hiding menu due to angle > 60 degrees");
                SetMenuVisibility(false);
            }
            else
            {
                // Show the menu
                Debug.Log("Showing menu due to angle <= 60 degrees");
                SetMenuVisibility(true);
            }
        }
        else
        {
            Debug.LogError("AnchorCenter is null");
        }
    }

    private void SetMenuVisibility(bool isVisible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }

        // Set the active state of the poke buttons
        if (placePoke != null) placePoke.SetActive(isVisible);
        if (followPoke != null) followPoke.SetActive(isVisible);
        if (armPoke != null) armPoke.SetActive(isVisible);
    }
}