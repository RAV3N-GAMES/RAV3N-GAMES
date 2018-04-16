using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    Slider HpSlider;
    RectTransform sliderRect;
    Vector3 targetPos;

    bool isInitHPSlider = false;

    // Use this for initialization
    private void Start()
    {
        InitHPSlider();
    }
    void InitHPSlider()
    {
        if (isInitHPSlider)
            return;

        HpSlider = Instantiate(UIManager.current.UIBarPrefab).GetComponent<Slider>();
        HpSlider.transform.SetParent(UIManager.current.UIBarParent);
        sliderRect = HpSlider.GetComponent<RectTransform>();

        HpSlider.gameObject.SetActive(false);

        isInitHPSlider = true;
    }

	private void Update()
	{
		if (!gameObject.activeSelf)
			return;

		targetPos = Camera.main.WorldToScreenPoint(transform.position);
		sliderRect.position = targetPos + Vector3.up * 30;
	}
	public void ValueInit(int value)
	{
		HpSlider.minValue = 0;
		HpSlider.maxValue = value;
		HpSlider.value = value;
	}
	public void ValueDecrease(int damage)
	{
		HpSlider.value -= damage;
	}
	public void ValueIncrease(int heal)
	{
		HpSlider.value += heal;
	}
	public void HealthActvie(bool isActive)
	{
        if(HpSlider == null)
            InitHPSlider();
        HpSlider.gameObject.SetActive(isActive);        
	}
}
    