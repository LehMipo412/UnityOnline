using UnityEngine;
using UnityEngine.UI;
public class EnemyHPbar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int _maxHP;

    public GameObject HpCanvas;

    void Start()
    {
        _hpBar.minValue = 0;
        _hpBar.maxValue = _maxHP;
        _hpBar.value = _maxHP;
    }

    public void ChangeHPbarPercent(float change)
    {
        Debug.LogWarning("Value went down to" + change);
        _hpBar.value = change;
    }
}
