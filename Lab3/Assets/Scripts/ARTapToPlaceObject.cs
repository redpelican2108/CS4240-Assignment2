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
    [SerializeField] private GameObject placementPrompt;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject scrollSnap;
    [SerializeField] private GameObject[] objectList;

    private GameObject objectToPlace;

    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    private GameObject selectedObject;

    public TextMeshProUGUI touchedObjectText;
    public TextMeshProUGUI touchedCoordsText;

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

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Input.touchCount <= 0) return; // No touches

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return; // Not first frame of touch
        touchedCoordsText.text = touch.position.ToString();

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, ~LayerMask.GetMask("ARPlanes")))
        {
            selectedObject = hitInfo.collider.gameObject;
            touchedObjectText.text = selectedObject.name;
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
            touchedObjectText.text = "No Object Selected";
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
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
            placementPrompt.SetActive(false);
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementPrompt.SetActive(true);
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        GameObject placedObject = Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
        placedObject.transform.Rotate(new Vector3(-90, 0, 0));
    }

    public void SetActiveObject(GameObject gameObject)
    {
        objectToPlace = gameObject;
    }
}