public class SlowEffect : Debuff
{
    public float intensity = 0.7f;
    public BaseMovementSystem movementSystem = null;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!movementSystem)
            movementSystem.GetComponent<BaseMovementSystem>();

        ApplyEffect();
    }

    protected override void ApplyEffect()
    {
        base.ApplyEffect();
        movementSystem.ApllySlow(intensity);
    }

    protected override void RemoveEffect()
    {
        movementSystem.RemoveSlow();
        base.RemoveEffect();
    }
}
