using SimplePiston.Entity;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using SimplePiston.Network;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.ServerMods.WorldEdit;

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

                IMiniDimension dimension = serverApi.World.BlockAccessor.CreateMiniDimension(blockToPushPosition.ToVec3d());
                int dimensionId = serverApi.Server.LoadMiniDimension(dimension);
                dimension.subDimensionId = dimensionId;
                
                IServerNetworkChannel serverNetworkChannel = serverApi.Network.GetChannel("piston");
                serverNetworkChannel.SendPacket(new DimensionIdPacket()
                {
                    DimensionId = dimensionId,
                    CurrentPos = blockToPushPosition
                }, (IServerPlayer)byPlayer);
            }

            if (world.Api.Side == EnumAppSide.Client)
            {
                //IMiniDimension dimension = 
            }
            //world.BlockAccessor.SetBlock(0, blockToPushPosition);
            //world.BlockAccessor.SetBlock(blockToPush.BlockId, newBlockPosition);
        }

        handling = EnumHandling.PreventDefault;
        return true;
    }
}