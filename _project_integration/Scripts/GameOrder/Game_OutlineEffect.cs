using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_OutlineEffect : MonoBehaviour
{
    public static Game_OutlineEffect Instance;

    public Material outlineMat;
    private Renderer targetRenderer;
    private Material[] originalMats;

    void Awake()
    {
        Instance = this;
    }

    public void HighlightObject(Renderer rend)
    {
        if (rend == null) return;

        targetRenderer = rend;
        originalMats = rend.materials;

        Material[] mats = new Material[originalMats.Length + 1];
        originalMats.CopyTo(mats, 0);
        mats[mats.Length - 1] = outlineMat;
        rend.materials = mats;
    }

    public void RemoveHighlight()
    {
        if (targetRenderer == null) return;

        targetRenderer.materials = originalMats;
        targetRenderer = null;
    }
}
