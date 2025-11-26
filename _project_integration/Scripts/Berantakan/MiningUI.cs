using UnityEngine;
using UnityEngine.UI;

public class MiningUI : MonoBehaviour
{
    public Slider progressBar;

    public void Show() => progressBar.gameObject.SetActive(true);
    public void Hide() => progressBar.gameObject.SetActive(false);

    public void SetProgress(float value)
    {
        progressBar.value = value;
    }
	
	void Awake()
	{
		Game_ResourceNode.MiningUIInstance = this;
		Hide();
	}

}
