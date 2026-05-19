public class FirstAidKit : Collectibles
{
    protected override void Interact(PlayerInventory other)
    {
        base.Interact(other);

        if (other.TryAddFirstAidKit(amount))
        {
            if (!infinity)
                Disable();
        }
    }
}
