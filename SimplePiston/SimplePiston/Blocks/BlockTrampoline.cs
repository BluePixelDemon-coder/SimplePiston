using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace SimplePiston.Blocks;

internal class BlockTrampoline : Block
{
    public override void OnEntityCollide(IWorldAccessor world, Entity entity, BlockPos pos, BlockFacing facing, Vec3d collideSpeed,
        bool isImpact)
    {
        if (isImpact && facing.IsVertical)
        {
            entity.Pos.Motion.Y *= -.8f;
        }
    }

    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPosition, ItemStack byItemStack = null)
    {
        api.Logger.Event("Trampoline block placed.");
        base.OnBlockPlaced(world, blockPosition, byItemStack);
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos blockPosition, IPlayer byPlayer,
        float dropQuantityMultiplier = 1)
    {
        api.Logger.Event("Trampoline block broken.");
        base.OnBlockBroken(world, blockPosition, byPlayer, dropQuantityMultiplier);
    }
}