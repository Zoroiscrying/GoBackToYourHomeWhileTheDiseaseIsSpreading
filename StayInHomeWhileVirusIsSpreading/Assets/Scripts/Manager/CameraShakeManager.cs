using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeManager : Singleton<CameraShakeManager>
{
    public CinemachineImpulseSource SourceSmall;
    public CinemachineImpulseSource Source;
    public CinemachineImpulseSource SourceHigh;
    

    public void SmallImpulse()
    {
        SourceSmall.GenerateImpulse();
    }

    public void MediumImpulse()
    {
        Source.GenerateImpulse();
    }

    public void HighImpulse()
    {
        SourceHigh.GenerateImpulse();
    }
    
}
