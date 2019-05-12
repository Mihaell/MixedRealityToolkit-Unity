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
    private StreamWriter writer;

    public void Awake()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string path = string.Format("{0}/{1}.txt", Application.persistentDataPath, measurementsFilename + "_" + timestamp);
        writer = new StreamWriter(path, true);
        Debug.Log("Writing all data to: " + path);
    }

    public void OnDestroy()
    {
        writer.Close();
    }

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
            string selectionPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition.ToString("F6");
            string hololensPosition = MixedRealityToolkit.InputSystem.GazeProvider.GazeOrigin.ToString("F6");
            string data = hololensPosition + " " + selectionPosition;
            print(data);

            var action = new Action<string>(this.SaveDataToDisc);
            action.BeginInvoke(data, null, null);

            lastInterval = timeNow;
        }
    }

    private void SaveDataToDisc(string data)
    {
        // Append the point to the specified file
        Debug.Log("Data: " + data);
        writer.WriteLine(data);
        writer.Flush();
        Debug.Log("Data written");
    }
}
