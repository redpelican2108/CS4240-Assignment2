using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using LightScrollSnap;
using TMPro;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject placementIndicator;

    //change the objectToPlace through UI click

    // Scroll bar
    [SerializeField] private GameObject scrollSnap;

    [SerializeField] private GameObject[] objectList;

    [SerializeField] private GameObject DeleteButton;

    private GameObject objectToPlace;

    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    private GameObject selectedObject;

    public GameObject DebugText;
    public GameObject DebugText2;

    // Start is called before the first frame update
    private void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        scrollSnap.GetComponent<ScrollSnap>().OnItemSelected.AddListener(OnSelectedItemChanged);
        OnSelectedItemChanged(null, 0); // Initialize to select the first item
    }

    private void OnSelectedItemChanged(RectTransform go, int index)
    {
        objectToPlace = objectList[index];
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.current.ScreenPointToRay(touch.position);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    selectedObject = hitInfo.collider.gameObject;
                    DebugText.GetComponent<TextMeshProUGUI>().text = selectedObject.name;
                }
            }
            DebugText2.GetComponent<TextMeshProUGUI>().text = Input.GetTouch(0).position.y.ToString();
        }
    }

    public void AddFurniture()
    {
        if (placementPoseIsValid)
        {
            PlaceObject();
        }
    }

    public void DeleteFurniture()
    {
        if (selectedObject)
        {
            Destroy(selectedObject);
            DebugText.GetComponent<TextMeshProUGUI>().text = "No Object Selected";
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
    }

    public void SetActiveObject(GameObject gameObject)
    {
        objectToPlace = gameObject;
    }
}