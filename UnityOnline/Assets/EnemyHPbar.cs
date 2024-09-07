using UnityEngine;
using UnityEngine.UI;
public class EnemyHPbar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int _maxHP;
    public Canvas hpCanvas;
    public float currentHP;
    public EnemyHPbar(int maxhp)
    {
        _maxHP = maxhp;

    }
    void Start()
    {
        _hpBar.minValue = 0;
        _hpBar.maxValue = _maxHP;
        _hpBar.value = _maxHP;
        currentHP = _hpBar.value;
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeHPbarPercent(0.8f);
        }
    }

    public void ChangeHPbarPercent(float change)
    {
        Debug.LogWarning("Value went down to" + change);
        _hpBar.value = change;
        
    }
}
