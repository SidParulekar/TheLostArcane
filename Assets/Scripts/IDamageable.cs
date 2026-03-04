using System.Collections;

public interface IDamageable
{
    public void ReduceHealth(float health);

    public IEnumerator DamageFlash(); 
}
