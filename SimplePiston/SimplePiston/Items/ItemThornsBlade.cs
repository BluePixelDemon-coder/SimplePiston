using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace SimplePiston.Items;

internal class ItemThornsBlade : Item
{
    public override void OnAttackingWith(IWorldAccessor world, Entity byEntity, Entity attackedEntity, ItemSlot itemslot)
    {
        base.OnAttackingWith(world, byEntity, attackedEntity, itemslot);
        DamageSource selfInflictedDamage = new DamageSource()
        {
            Type = EnumDamageType.PiercingAttack,
            CauseEntity = byEntity
        };
        if (attackedEntity.Alive)
        {
            byEntity.ReceiveDamage(selfInflictedDamage, 0.25f);
        }

        if (!attackedEntity.Alive)
        {
            attackedEntity.Revive();
        }
    }
}