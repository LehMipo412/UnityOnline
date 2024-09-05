using UnityEngine;
using UnityEngine.UI;
public class EnemyHPbar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private int _maxHP;
    public EnemyHPbar(int maxhp)
    {
        _maxHP = maxhp;
    }
    void Start()
    {
        _hpBar.minValue = 0;
        _hpBar.maxValue = _maxHP;
        _hpBar.value = _maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeHPbarPercent(5);
        }
    }

    public void ChangeHPbarPercent(int damage)
    {
        _hpBar.value -= damage;
    }
}
