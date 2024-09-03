using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using UnityEngine.UI;

public class MenuPositionerRoom : MonoBehaviour
{
    public float smoothFactor = 2;
    public Transform target;
    public Vector3 offset = new Vector3(0, .15f, .4f);
    public Vector3 euler = new Vector3(15, 0, 0);
    private bool isFollowing = false;
    private bool isPlacing = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    public GameObject _debugSphere;
    public GameObject placeButtonPrefab; // Assign a prefab for the place button
    private GameObject placeButtonInstance;

    private void Start()
    {
        // Store the original parent, local position, and rotation
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        // Initially, the menu is attached to the left hand
        isFollowing = false;

        // Get the current room
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        // Get the room anchors
        List<MRUKAnchor> anchors = room.GetRoomAnchors();
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

        if (isPlacing)
        {
            PlaceOnSurface();
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
        isPlacing = true;
        transform.SetParent(null); // Ensure the menu is not a child of the hand when placed
    }

    public void FollowMe()
    {
        isFollowing = true;
        isPlacing = false;
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
        isPlacing = false;
    }

    private void PlaceOnSurface()
    {
        isFollowing = false;
        Vector3 origin = target.position;
        Vector3 surfacePosition = Vector3.zero;
        MRUKAnchor closestAnchor = null;

        // Try to get the closest surface position
        MRUK.Instance?.GetCurrentRoom()
            ?.TryGetClosestSurfacePosition(origin, out surfacePosition, out closestAnchor);

        if (closestAnchor != null && (closestAnchor.name == "TABLE" || closestAnchor.name == "WALL_FACE"))
        {
            if (placeButtonInstance == null)
            {
                placeButtonInstance = Instantiate(placeButtonPrefab, surfacePosition, Quaternion.identity);
                placeButtonInstance.GetComponent<Button>().onClick.AddListener(() =>
                {
                    transform.position = surfacePosition;
                    transform.rotation = Quaternion.LookRotation(closestAnchor.transform.forward, Vector3.up);
                    isPlacing = false;

                    // Optionally, set a debug sphere to visualize the position
                    if (_debugSphere != null)
                    {
                        _debugSphere.transform.position = surfacePosition;
                    }

                    Destroy(placeButtonInstance);
                });
            }
        }
    }
}