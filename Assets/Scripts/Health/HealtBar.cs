using UnityEngine;
using UnityEngine.UI;

public class Healtbar : MonoBehaviour
{
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currentHealthBar;

    private float hp2 = 10f;
    float BossHealth = 10f;

    private void Update()
    {
        Bar2(BossHealth);
    }
    public void Bar(float hp)
    {
        currentHealthBar.fillAmount = hp / 10;
        BossHealth = hp;
    }

    public void Bar2(float hp)
    {
        if (hp < hp2)
            hp2 -= 1 * Time.deltaTime;

        totalhealthBar.fillAmount = hp2 / 10;
    }
}
