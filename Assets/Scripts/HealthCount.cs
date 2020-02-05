using UnityEngine;
using TMPro;

public class HealthCount : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    public void SetHealth(int health)
    {
        _text.text = health.ToString();
    }
}
