using UnityEngine;

/// Этот скрипт - шаблон
/// Чтобы создать экземпляр, выбираем меню -> Assets -> Create -> "menuName"
/// Создаётся файл .asset по данному шаблону
[CreateAssetMenu(fileName = "DataScript", menuName = "Scriptable Object", order = 51)] //Отображение в Unity - меню Assets
public class DataScript : ScriptableObject
{
    [SerializeField] private string stringProperty;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int intProperty;
}
