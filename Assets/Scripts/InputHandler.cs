using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class InputHandler : BaseInputHandler, IMixedRealityPointerHandler
{
    public float updateInterval = 0.2F;
    public string measurementsFilename;
    private double lastInterval;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        // We limit the rate of events to prevent the same event from activating multiple times in succession
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval && eventData.Pointer.Result != null)
        {
            int step = eventData.Pointer.Result.RayStepIndex;
            string data = eventData.Pointer.Rays[step].Terminus.ToString();
            string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, measurementsFilename);

            Debug.Log(eventData.Pointer.Position.ToString() + "  (1)");
            Debug.Log(eventData.Pointer.Rays[step].Origin.ToString() + "  (2)");
            Debug.Log(eventData.Pointer.Rays[step].Terminus.ToString() + "  (3)");

            // Append the point to the specified file
            StreamWriter writer = new StreamWriter(path, true);
            Debug.Log("Writing data to: " + path);
            writer.WriteLine(data);
            writer.Flush();
            writer.Close();
            Debug.Log("Data written");

            lastInterval = timeNow;
        }
    }
}
