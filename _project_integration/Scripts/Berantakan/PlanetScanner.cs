using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;


public class PlanetScanner : MonoBehaviour
{
    [Header("Scan Settings")]
    public float scanRange = 500f;
    public Camera playerCamera;
    public LayerMask planetLayer;

    [Header("Pulse Effect")]
    public Transform pulseOrigin;
    public GameObject pulseSpherePrefab; // prefab sphere transparan untuk efek sonar

    [Header("UI Elements")]
    public CanvasGroup scanPanel;
    public TextMeshProUGUI scanTitle;
    public TextMeshProUGUI scanDesc;
    public Slider scanProgress;

    private bool isScanning;
    private float holdTime;
    private RaycastHit hit;
	private bool isShowInfo = false;

    void Update()
    {
		//if (Input.GetMouseButton(1))
        if (Input.GetMouseButton(1) && isShowInfo == false)
        {
            HandleScan();
        }
        else
        {
            if (isScanning)
            {
                CancelScan();
            }
        }
    }

    async void HandleScan()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, scanRange, planetLayer))
        {
            holdTime += Time.deltaTime;
            scanProgress.value = holdTime / 2f; // butuh 2 detik penuh untuk selesai

            if (!isScanning)
            {
                isScanning = true;
                StartCoroutine(SpawnPulse());
                ShowScanUI("Scanning planet...", "");
            }

            if (holdTime >= 2f)
            {
				isShowInfo = true;
                PlanetInfo info = hit.collider.GetComponent<PlanetInfo>();
                if (info != null)
                {
                    info.OnScanned(); // aktifkan glow
                    ShowScanUI(info.planetName, info.description);
                }
                CancelScan(true);
				await Task.Delay(1000);
				// Tunggu beberapa detik
				//yield return new WaitForSeconds(5f);
				isShowInfo = false;
            }
        }
        else
        {
            CancelScan();
        }
    }

    void CancelScan(bool completed = false)
    {
        isScanning = false;
        holdTime = 0;
        scanProgress.value = 0;

        if (!completed)
			
            HideScanUI();
    }

    IEnumerator SpawnPulse()
    {
        GameObject pulse = Instantiate(pulseSpherePrefab, pulseOrigin.position, Quaternion.identity);
        float duration = 1.5f;
        float elapsed = 0;
        float startScale = 0.1f;
        float endScale = scanRange / 100f;

        var mat = pulse.GetComponent<MeshRenderer>().material;
        Color startColor = mat.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            pulse.transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(0.8f, 0f, t));

            yield return null;
        }

        Destroy(pulse);
    }

    void ShowScanUI(string title, string desc)
    {
        scanTitle.text = title;
        scanDesc.text = desc;
        scanPanel.alpha = 1;
        scanPanel.blocksRaycasts = true;
    }

    void HideScanUI()
    {
        scanPanel.alpha = 0;
        scanPanel.blocksRaycasts = false;
    }
}
