using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPreorderanimator : MonoBehaviour
{
    public RawImage playerPreview;

    private void OnEnable()
    {
        animate(true);
    }

    public void animate(bool state)
    {
        Material mat = playerPreview.material;

        // Convert property name to ID within the function
        int glitchPropertyID = Shader.PropertyToID("_DirectionalAlphaFadeFade");

        DOVirtual.Float(0f, 20f, 1.5f, (float i) =>
        {
            mat.SetFloat(glitchPropertyID, i);
        });
        
    }
}
