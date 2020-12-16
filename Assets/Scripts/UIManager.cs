using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class UIManager : MonoBehaviour
{
    public List<XRController> controllers = null;
    bool buttonIncreaseValue; // value for secondary button
    bool buttonDecreaseValue; // value for primary button

    // references to UI Booleans
    public bool helpIsOn = false;
    public bool menuIsOn = false;
    public bool cluesIsOn = false;
    public bool heatIsOn = false;
    public bool nightIsOn = false;
    public bool flipIsOn = false;
    public bool fovIsOn = false;

    //references to button planes
    public GameObject helpButton;
    public GameObject menuButton;
    public GameObject cluesButton;
    public GameObject heatButton;
    public GameObject nightButton;
    public GameObject flipButton;
    public GameObject fovButton;

    public GameObject helpCanvas; // UI help text
    public GameObject otherIcons; // UI icons

    public CameraEffect heatMode; // the thermal vision component
    public Camera glassCamera; // The Lens' camera

    // Start is called before the first frame update
    void Start()
    {
        otherIcons.SetActive(false); // turns off the other icons
        helpCanvas.SetActive(false); // turns off the help text
        glassCamera.cullingMask = ~(1 << 8); // stops the lens seeing the hidden objects
        glassCamera.fieldOfView = 50; // sets the default lens camera FOV
    }
    private void Update()
    {
        CheckForInput();
    }

    private void CheckForInput() // checks for input from the controllers
    {
        foreach (XRController controller in controllers)
        {
            if (controller.enableInputActions)
                CheckForTrigger(controller.inputDevice);
        }
    }

    private void CheckForTrigger(InputDevice device)
    {
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out buttonDecreaseValue) && buttonDecreaseValue)
            DecreaseFOV(buttonDecreaseValue); // input for the primary button A

        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out buttonIncreaseValue) && buttonIncreaseValue)
            IncreaseFOV(buttonIncreaseValue); // input for the secondary button B
    }

    private void DecreaseFOV(bool buttonDecreaseValue)
    {
        if (buttonDecreaseValue == true)
        {
            if (fovIsOn == true) // checks if the FOV mode is on
            {
                glassCamera.fieldOfView += 1; // increases the FOV by 1 when the buttons is pressed or held
                if(glassCamera.fieldOfView >= 140) // FOV upper limit
                {
                    glassCamera.fieldOfView = 140;
                }
            }
        }
    }

    private void IncreaseFOV(bool buttonIncreaseValue)
    {
        if (buttonIncreaseValue == true)
        {
            if (fovIsOn == true) // checks if the FOV mode is on
            {
                glassCamera.fieldOfView -= 1; // decreases the FOV by 1 when the buttons is pressed or held
                if (glassCamera.fieldOfView <= 5) // FOV lower limit
                {
                    glassCamera.fieldOfView = 5;
                }
            }
        }
    }

    public void FOVCamera() // this function enables the field of view change
    {
        if (fovIsOn == false)
        {
            fovButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green); // sets the albedo colour to green
            fovIsOn = true; // switches the bool
        }
        else if (fovIsOn == true)
        {
            fovButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white); // sets the albedo colour to green
            fovIsOn = false; // switches the bool
        }
    }

    public void FlipCamera() // this function flips the camera around
    {
        if (flipIsOn == false)
        {
            flipButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green); // sets the albedo colour to green
            glassCamera.transform.rotation = new Quaternion(0, 180, 0, 0); // rotates the camera to face backward
            flipIsOn = true; // switches the bool
        }
        else if (flipIsOn == true)
        {
            flipButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white); // sets the albedo colour to green
            glassCamera.transform.rotation = new Quaternion(0, 0, 0, 0); // rotates the camera to face forward
            flipIsOn = false; // switches the bool
        }
    }

    public void NightCamera() // this function represents a typical green night vision effect
    {
        if (heatIsOn == true) // turns off the Heat Vision if it's on
        {
            heatMode.m_Enable = false;
            heatButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            heatIsOn = false;
        }
        if (nightIsOn == false)
        {
            glassCamera.GetComponent<DeferredNightVisionEffect>().enabled = true; // turns on the night vision effect
            glassCamera.GetComponent<PostProcessVolume>().enabled = true; // turns on the grain effect
            nightButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green); // sets the albedo colour to green
            nightIsOn = true; // switches the bool
        }
        else if (nightIsOn == true)
        {
            glassCamera.GetComponent<DeferredNightVisionEffect>().enabled = false; // turns off the night vision effect
            glassCamera.GetComponent<PostProcessVolume>().enabled = false; // turns off the grain effect
            nightButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white); // sets the albedo colour back to white
            nightIsOn = false; // switches the bool
        }
    }

    public void HeatCamera() // this function shows the heat of objects according to their luminance
    {
        if (nightIsOn == true)
        {
            glassCamera.GetComponent<DeferredNightVisionEffect>().enabled = false; // turns off the night vision effect
            glassCamera.GetComponent<PostProcessVolume>().enabled = false; // turns off the grain effect
            nightButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            nightIsOn = false; // switches the bool
        }

        if (heatIsOn == false)
        {
            heatMode.m_Enable = true; // turns on the heat/thermal vision
            heatButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            heatIsOn = true; // switches the bool
        }
        else if (heatIsOn == true)
        {
            heatMode.m_Enable = false; // turns off the heat/thermal vision
            heatButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            heatIsOn = false; // switches the bool
        }
    }

    public void ClueCamera() // this function allows the lens' camera to see hidden objects in the scene
    {
        if (cluesIsOn == false)
        {
            glassCamera.cullingMask |= (1 << 8); // shows the hidden objects
            cluesButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            cluesIsOn = true; // switches the bool
        }
        else if (cluesIsOn == true)
        {
            glassCamera.cullingMask = ~(1 << 8); // hides the hidden objects
            cluesButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            cluesIsOn = false; // switches the bool
        }
    }

    public void OtherIcons() // this function shows or hides the lens' icons
    {
        if (menuIsOn == false)
        {
            otherIcons.SetActive(true); // shows the other icons
            menuButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            menuIsOn = true;
        }
        else if (menuIsOn == true)
        {
            otherIcons.SetActive(false); // hides the other icons
            helpCanvas.SetActive(false); // hides the help text if on
            helpButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            menuButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            menuIsOn = false;
        }
    }

    public void HelpCanvas() // this function shows or hides the lens' help text
    {
        if (helpIsOn == false)
        {
            helpCanvas.SetActive(true);
            helpButton.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            helpIsOn = true; 
        }
        else if (helpIsOn == true)
        {
            helpCanvas.SetActive(false);
            helpButton.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            helpIsOn = false;
        }
    }
}
