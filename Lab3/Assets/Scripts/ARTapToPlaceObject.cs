using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.UI;
using LightScrollSnap;
using TMPro;

public class ARTapToPlaceObject : MonoBehaviour
{
    enum State
    {
        PlacingObject,
        SelectingObject,
        MovingObject
    }

    [SerializeField] private ARRaycastManager aRRaycastManager;
    [SerializeField] private GameObject placementPrompt;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private TextMeshProUGUI selectedObjectText;
    [SerializeField] private Button placeButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private CanvasGroup scrollSnapGroup;
    [SerializeField] private ScrollSnap scrollSnap;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Material errorMaterial;
    [SerializeField] private GameObject[] objectList;

    private GameObject objectToPlace;
    private Pose PlacementPose;
    private bool placementPoseIsValid = false;
    private GameObject ghostObject;
    private GhostObject ghostObjectScript;
    private GameObject selectedObject;
    private State state;

    private void Start()
    {
        scrollSnap.OnItemSelected.AddListener(OnSelectedItemChanged);
        OnSelectedItemChanged(null, 0); // Initialize to select the first item
        
        SelectObject(null);

        SetState(State.PlacingObject);
    }

    private void OnSelectedItemChanged(RectTransform go, int index)
    {
        objectToPlace = objectList[index];
        UpdateGhostObject();
    }

    private void UpdateGhostObject()
    {
        Destroy(ghostObject);
        ghostObject = Instantiate(objectToPlace, placementIndicator.transform);
        ghostObject.name = objectToPlace.name;
        ghostObjectScript = ghostObject.AddComponent<GhostObject>();
        ghostObjectScript.ghostMaterial = this.ghostMaterial;
        ghostObjectScript.errorMaterial = this.errorMaterial;
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

        if (PointerOverUI.IsPointerOverUIObject(touch.position)) return; // Blocked by UI

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, ~LayerMask.GetMask("ARPlanes")))
        {
            SelectObject(hitInfo.collider.gameObject);
        }
    }

    private void SelectObject(GameObject obj)
    {
        if (selectedObject != null)
        {
            selectedObject.GetComponentInChildren<PlaceableObject>().Deselect();
        }

        selectedObject = obj;

        if (selectedObject == null)
        {
            SetState(State.PlacingObject);
        }
        else
        {
            selectedObject.GetComponentInChildren<PlaceableObject>().Select();
            SetState(State.SelectingObject);
        }
    }

    private void SetState(State newState)
    {
        switch (newState)
        {
            case State.PlacingObject:
                selectedObjectText.text = "No Object Selected";
                placeButton.interactable = true;
                moveButton.interactable = false;
                deleteButton.interactable = false;
                scrollSnapGroup.interactable = true;
                scrollSnapGroup.alpha = 1;
                ghostObject.SetActive(true);
                break;
            case State.SelectingObject:
                selectedObjectText.text = "Selected " + selectedObject.name;
                placeButton.interactable = true;
                moveButton.interactable = true;
                deleteButton.interactable = true;
                scrollSnapGroup.interactable = true;
                scrollSnapGroup.alpha = 1;
                ghostObject.SetActive(true);
                break;
            case State.MovingObject:
                selectedObjectText.text = "Moving " + selectedObject.name;
                placeButton.interactable = true;
                moveButton.interactable = false;
                deleteButton.interactable = true;
                scrollSnapGroup.interactable = false;
                scrollSnapGroup.alpha = 0.2f;
                ghostObject.SetActive(false);
                break;
        }
    }

    public void AddFurniture()
    {
        if (!placementPoseIsValid) return;

        if (!ghostObjectScript.IsPlaceable()) return;

        PlaceObject();
    }

    public void PickUpFurniture()
    {
        if (selectedObject == null) return;
        
        for (int i = 0; i <= objectList.Length; i++)
        {
            if (objectList[i].name == selectedObject.name)
            {
                scrollSnap.ScrollToItem(i);
                UpdateGhostObject();
                break;
            }
        }
        
        Destroy(selectedObject);
        SelectObject(null);

        SetState(State.PlacingObject);
    }

    public void DeleteFurniture()
    {
        if (selectedObject == null) return;

        Destroy(selectedObject);
        SelectObject(null);

        SetState(State.PlacingObject);
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

            placeButton.interactable = ghostObjectScript.IsPlaceable();
        }
        else
        {
            placementPrompt.SetActive(true);
            placementIndicator.SetActive(false);
            placeButton.interactable = false;
        }
    }

    private void PlaceObject()
    {
        GameObject placedObject = Instantiate(objectToPlace, PlacementPose.position, PlacementPose.rotation);
        placedObject.name = objectToPlace.name;
        placedObject.transform.Rotate(new Vector3(-90, 0, 0));
    }
}