using System.Collections;
using UnityEngine;
using TMPro;

public class HealthCount : MonoBehaviour
{
    [SerializeField] public TMP_Text _text;
    public void SetHealth(int health)
    {
        _text.CrossFadeAlpha(0f, 0f, false);
        if (health > 3)
        {
            _text.color = new Color(0f, 0.6f, 0f);
        }
        else if (health < 2)
        {
            _text.color = Color.red;
        }
        else
        {
            _text.color = new Color(1f, 0.6f, 0f);
        }
        _text.text = health.ToString();
        _text.CrossFadeAlpha(1f, 0.8f, false);
    }
}
