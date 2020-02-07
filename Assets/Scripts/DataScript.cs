using UnityEngine;

[CreateAssetMenu(fileName = "DataScript", menuName = "Scriptable Object", order = 51)] //Отображение в Unity - меню Assets
public class DataScript : ScriptableObject
{
    [SerializeField] private string stringProperty;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int intProperty;
}
