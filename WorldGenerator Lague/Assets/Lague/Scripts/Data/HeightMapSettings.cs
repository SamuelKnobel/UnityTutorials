using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{

    public NoiseSettings noiseSettings;
    

    public bool useFalloff;

    public float heightMultiplayer; // Scales on the y axis

    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return  heightMultiplayer * heightCurve.Evaluate(0);
        }
    }
    public float maxHeight
    {
        get
        {
            return  heightMultiplayer * heightCurve.Evaluate(1);
        }
    }


#if UNITY_EDITOR
    protected override void OnValidate()     // Alternative to clamping the values from the Editor Script. is automaticalle calles if one of the variables in the inspector is changed
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}
