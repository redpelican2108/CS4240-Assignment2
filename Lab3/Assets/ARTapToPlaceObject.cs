using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;


public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject placementIndicator;
    //change the objectToPlace through UI click
    [SerializeField] private  GameObject objectToPlace;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    private GameObject selectedObject;
    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
        DetectSelect();
    }

    private void DetectSelect()
    {
        var ray = Camera.current.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo))
        {
            selectedObject = hitInfo.collider.gameObject;
        }

    }

    public void DeleteFurniture()
    {
        Destroy(selectedObject);

    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
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
