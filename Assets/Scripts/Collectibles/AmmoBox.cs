public class AmmoBox : Collectibles
{
    public AmmoType ammoType = AmmoType.Pistol;

    protected override void Interact(PlayerInventory other)
    {
        base.Interact(other);

        if (other.TryAddAmmo(ammoType, amount))
        {
            if (!infinity)
                Disable();
        }
    }
}
