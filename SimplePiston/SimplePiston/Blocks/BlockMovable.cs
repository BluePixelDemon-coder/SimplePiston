using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace SimplePiston.Blocks;

internal class BlockMovable : Block
{
    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPosition, ItemStack byItemStack = null)
    {
        api.Logger.Event("Movable block placed.");
        base.OnBlockPlaced(world, blockPosition, byItemStack);
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos blockPosition, IPlayer byPlayer,
        float dropQuantityMultiplier = 1)
    {
        api.Logger.Event("Movable block broken.");
        base.OnBlockBroken(world, blockPosition, byPlayer, dropQuantityMultiplier);
    }
}