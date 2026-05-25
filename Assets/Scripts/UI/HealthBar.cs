using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public GameObject fillArea;
    private Slider healthSlider;

    public bool uiWorld = true;

    private void Awake()
    {
        healthSlider = transform.GetChild(0).GetComponent<Slider>();
    }
    private void Start()
    {
        fillArea.SetActive(false);

        if(uiWorld)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(uiWorld)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
    }

    public void UpdateBar(float ratio)
    {
        if (ratio >= 1f || ratio <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }
        fillArea.SetActive(true);
        gameObject.SetActive(true);
        healthSlider.value = 1f - ratio;
    }
}
