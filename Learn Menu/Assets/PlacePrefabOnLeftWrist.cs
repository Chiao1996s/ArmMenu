using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class PlacePrefabOnLeftWrist : MonoBehaviour
{
    [SerializeField, Interface(typeof(IHand))]
    private Object _leftHand;

    [SerializeField]
    private GameObject prefab;

    private IHand LeftHand { get; set; }
    private GameObject instantiatedPrefab;
    private Transform tabBarImage;

    protected virtual void Awake()
    {
        LeftHand = _leftHand as IHand;
    }

    private void Start()
    {
        if (prefab != null)
        {
            instantiatedPrefab = Instantiate(prefab);
            // 獲取 Tab Bar > Image 的 Transform
            tabBarImage = instantiatedPrefab.transform.Find("Tab Bar/Image");
            if (tabBarImage == null)
            {
                Debug.LogError("Tab Bar/Image not found in the prefab.");
            }
        }
        else
        {
            Debug.LogError("Prefab is not assigned.");
        }
    }

    private void Update()
    {
        if (instantiatedPrefab != null && LeftHand != null)
        {
            Pose wristPose, phalangePose;
            if (LeftHand.GetJointPose(HandJointId.HandWristRoot, out wristPose) &&
                LeftHand.GetJointPose(HandJointId.HandMiddle1, out phalangePose))
            {
                // 計算位置和方向
                Vector3 position = (wristPose.position + phalangePose.position) / 2.0f;
                Vector3 direction = (phalangePose.position - wristPose.position).normalized;

                if (tabBarImage != null)
                {
                    // 計算旋轉，使 x 軸與方向平行
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    // 設置 prefab 的位置和旋轉
                    instantiatedPrefab.transform.position = position;
                    instantiatedPrefab.transform.rotation = rotation;

                    // 調整 Tab Bar > Image 的位置，使其位於 wristPose 和 phalangePose 之間
                    tabBarImage.position = position;

                    // 設置 Tab Bar > Image 的旋轉，使其 x 軸與方向平行
                    tabBarImage.rotation = rotation;
                }
            }
        }
        else
        {
            Debug.LogError("Instantiated Prefab or LeftHand is null.");
        }
    }
}