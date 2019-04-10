using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class InputHandler : BaseInputHandler, IMixedRealityPointerHandler
{
    public float updateInterval = 0.2F;
    private double lastInterval;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval && eventData.Pointer.Result != null)
        {
            Debug.Log(eventData.Pointer.Position.ToString() + "  (1)");
            int step = eventData.Pointer.Result.RayStepIndex;
            Debug.Log(eventData.Pointer.Rays[step].Origin.ToString() + "  (2)");
            Debug.Log(eventData.Pointer.Rays[step].Terminus.ToString() + "  (3)");
            lastInterval = timeNow;
        }
        //throw new System.NotImplementedException();
    }
}
