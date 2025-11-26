using UnityEngine;

public class PlanetInfo : MonoBehaviour
{
    public string planetName = "Unknown Planet";
    [TextArea] public string description = "No data available.";
    private Material mat;
    private Color originalEmission;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        if (mat.HasProperty("_EmissionColor"))
            originalEmission = mat.GetColor("_EmissionColor");
    }

    public void OnScanned()
    {
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", Color.cyan * 2f);
            DynamicGI.SetEmissive(GetComponent<Renderer>(), Color.cyan * 2f);
        }
    }
}
