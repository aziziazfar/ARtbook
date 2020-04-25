using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

[RequireComponent(typeof(ARRaycastManager))]
public class ImagePlacement : MonoBehaviour
{

    [SerializeField]
    private GameObject[] placedPrefabs;

    //[SerializeField]
    //private GameObject welcomePanel;
    [SerializeField]
    private Text debugText;

    [SerializeField]
    private int maxNumberOfTVs = 1;

    //[SerializeField]
    //private Button dismissButton;

    private List<GameObject> addedInstances = new List<GameObject>();

    private Vector2 touchPosition = default;

    public GameObject[] PlacedPrefab
    {
        get 
        {
            return placedPrefabs;
        }
        set 
        {
            placedPrefabs = value;
        }
    }

    [SerializeField]
    private VideoClip[] videoClips;

    private ARRaycastManager arRaycastManager;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        //dismissButton.onClick.AddListener(Dismiss);
        debugText.text += "PlacedPrefab name: " + PlacedPrefab[0].name + "\n";
    }
    //private void Dismiss() => welcomePanel.SetActive(false);

    void Update()
    {
        // on double touch swap clips
        if(Input.touchCount >= 2)
        {
            foreach(GameObject go in placedPrefabs)
            {
                go.GetComponent<VideoPlayer>().clip = videoClips[Random.Range(0, videoClips.Length - 1)];
            }
        }

        if(Input.touchCount > 0)
        {
            debugText.text = "Touched!";
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;
                debugText.text += "Touch Phase began!\n";

                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    debugText.text += "Hit Plane! Added Instances Count : " + addedInstances.Count.ToString() +"\n";
                    var hitPose = hits[0].pose;
                    if(addedInstances.Count < maxNumberOfTVs)
                    {
                        //GameObject randomPrefab =  placedPrefabs[Random.Range(0, placedPrefabs.Length - 1)];
                        GameObject randomPrefab =  placedPrefabs[0];
                        debugText.text += "Prefab Placed\n";
                        Quaternion newRotation = new Quaternion();
                        debugText.text += hitPose.rotation.x.ToString() + ", " + hitPose.rotation.y.ToString() + ", " + hitPose.rotation.z.ToString() + "\n";
                        newRotation.Set(90, 90, 90, 1 );
                        //GameObject addedPrefab = Instantiate(randomPrefab, hitPose.position, hitPose.rotation);
                        GameObject addedPrefab = Instantiate(randomPrefab, hitPose.position, newRotation);
                        addedInstances.Add(addedPrefab);
                    }
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    if(addedInstances.Count > 0)
                    {
                       GameObject lastAdded = addedInstances[addedInstances.Count - 1];
                       lastAdded.transform.position = hitPose.position;
                       lastAdded.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }

    }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}