using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

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
            string data = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition.ToString("F6");
            string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, measurementsFilename);

            //foreach (var ray in eventData.Pointer.Rays)
            //{
            //    Debug.Log(ray.Origin.ToString() + "  " + ray.Terminus.ToString());
            //}
            //Debug.Log(eventData.Pointer.Position.ToString() + "  (1)");

            var action = new Action<string, string>(SaveDataToDisc);
            action.BeginInvoke(path, data, null, null);

            lastInterval = timeNow;
        }
    }

    private static void SaveDataToDisc(string path, string data)
    {
        // Append the point to the specified file
        StreamWriter writer = new StreamWriter(path, true);
        Debug.Log("Data: " + data);
        Debug.Log("Writing data to: " + path);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
        Debug.Log("Data written");
    }
}
