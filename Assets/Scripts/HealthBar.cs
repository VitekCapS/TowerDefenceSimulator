using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Unit unit;
    public Slider slider;

    private void OnEnable()
    {
        unit.OnHealthValueChanged += UpdateSlider;
    }

    private void OnDisable()
    {
        unit.OnHealthValueChanged -= UpdateSlider;
    }

    private void Start () {
        slider.value = 1;
	}
	
    private void UpdateSlider(params int[] args)
    {
        float health = args[0];
        float maxHealth = args[1];
        slider.value = health / maxHealth;
    }
}
