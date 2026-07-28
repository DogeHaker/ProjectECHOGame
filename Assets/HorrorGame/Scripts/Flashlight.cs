using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlightObject;
    public bool toggle;
    public AudioSource toggleSound;

    void Start()
    {
        toggle = false;
        flashlightObject.SetActive(toggle);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            toggle = !toggle;
            flashlightObject.SetActive(toggle);
            //toggleSound.Play();
        }
    }
}
