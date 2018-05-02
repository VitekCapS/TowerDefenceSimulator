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

    // Use this for initialization
    void Start () {
        slider.value = 1;
	}
	
    void UpdateSlider(params int[] args)
    {
        float health = args[0];
        float maxHealth = args[1];
        slider.value = health / maxHealth;
    }
}
