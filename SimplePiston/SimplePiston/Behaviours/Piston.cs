using SimplePiston.Entity;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using SimplePiston.Network;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace SimplePiston.Behaviours;


public class Piston : BlockBehavior
{
    public Piston(Block block) : base(block)
    {
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        world.Api.Logger.Notification("Piston interacted with from " + world.Api.Side);
        BlockPos pistonPosition = blockSel.Position;
        Block piston = world.BlockAccessor.GetBlock(pistonPosition);
        BlockFacing direction;
        
        switch (piston.Variant["rotation"])
        {
            case "up":
                direction = BlockFacing.UP;
                break;
            case "down":
                direction = BlockFacing.DOWN;
                break;
            case "north":
                direction = BlockFacing.NORTH;
                break;
            case "south":
                direction = BlockFacing.SOUTH;
                break;
            case "east":
                direction = BlockFacing.EAST;
                break;
            case "west":
                direction = BlockFacing.WEST;
                break;
            default:
                return false;
        }

        BlockPos blockToPushPosition = blockSel.Position.AddCopy(direction, 1);
        BlockPos newBlockPosition = blockToPushPosition.AddCopy(direction, 1);


        Block blockToPush = world.BlockAccessor.GetBlock(blockToPushPosition);
        if (blockToPush.BlockId == 0) // is the block air?
        {
            world.Api.Logger.Notification("No block to push" + world.Api.Side);
            return false;
        }
        
        if (world.BlockAccessor.GetBlock(newBlockPosition).IsReplacableBy(block))
        {
            if (world.Api.Side == EnumAppSide.Server)
            {
                ICoreServerAPI serverApi = world.Api as ICoreServerAPI;
                if (serverApi == null)
                {
                    return false;
                }

                BlockPos dimensionPosition =
                    new BlockPos(blockToPushPosition.X, blockToPushPosition.Y, blockToPushPosition.Z, 1);
                IMiniDimension dimension =
                    serverApi.World.BlockAccessor.CreateMiniDimension(new Vec3d(dimensionPosition.X, dimensionPosition.Y, dimensionPosition.Z));
                int dimensionId = serverApi.Server.LoadMiniDimension(dimension);
                world.Logger.Notification("dimensionId from server: " + dimensionId);
                dimension.subDimensionId = dimensionId;
                dimension.CurrentPos.SetPos(dimensionPosition);
                dimension.SetSelectionTrackingSubId_Server(dimensionId);
                dimension.AdjustPosForSubDimension(dimensionPosition);
                IServerNetworkChannel serverNetworkChannel = serverApi.Network.GetChannel("piston");
                dimension.UnloadUnusedServerChunks();
                serverNetworkChannel.SendPacket(new DimensionIdPacket()
                {
                    DimensionId = dimension.subDimensionId,
                    CurrentPos = dimensionPosition
                }, (IServerPlayer)byPlayer);
                
                Vintagestory.API.Common.Entities.Entity launched = EntityMovedBlock.CreateMovableBlock(serverApi, dimension);
                launched.Pos.SetFrom(launched.ServerPos);
                world.SpawnEntity(launched);
            }
            //world.BlockAccessor.SetBlock(0, blockToPushPosition);
            //world.BlockAccessor.SetBlock(blockToPush.BlockId, newBlockPosition);
        }

        handling = EnumHandling.PreventDefault;
        return true;
    }
}