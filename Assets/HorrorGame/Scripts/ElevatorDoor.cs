using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    public GameObject doorLeft;
    public GameObject doorRight;
    public float openSpeed = 2f;
    public float openDistance = 1.5f;

    private bool isOpening = false;
    private Vector3 targetPosLeft;
    private Vector3 targetPosRight;

    void Start()
    {
        // Calculate where the doors need to stop when fully slid open
        targetPosLeft = doorLeft.transform.position - (doorLeft.transform.right * openDistance);
        targetPosRight = doorRight.transform.position - (doorRight.transform.right * openDistance);
    }

    // THIS IS THE SYSTEM FUNCTION CALLED BY THE RAYCAST!
    public void OnInteract()
    {
        if (!isOpening)
        {
            isOpening = true;
            Debug.Log("The system authorized the door release protocol.");
        }
    }

    void Update()
    {
        // Smoothly move the door panels if the interaction flipped the switch
        if (isOpening)
        {
            doorLeft.transform.position = Vector3.MoveTowards(doorLeft.transform.position, targetPosLeft, openSpeed * Time.deltaTime);
            doorRight.transform.position = Vector3.MoveTowards(doorRight.transform.position, targetPosRight, openSpeed * Time.deltaTime);
        }
    }
}