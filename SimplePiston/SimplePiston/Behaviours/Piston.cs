using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace SimplePiston.Behaviours;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class RequestMoveBlockPacket
{
    public BlockPos MoveFrom;
    public BlockPos MoveTo;
}

public class NetworkSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.Network.RegisterChannel("piston")
            .RegisterMessageType(typeof(RequestMoveBlockPacket));
    }
    #region Server

    private IServerNetworkChannel _serverNetworkChannel;
    private ICoreServerAPI _serverApi;

    public override void StartServerSide(ICoreServerAPI api)
    {
        _serverApi = api;
        _serverNetworkChannel = api.Network.GetChannel("piston")
            .SetMessageHandler<RequestMoveBlockPacket>(OnRequestMoveBlock);
    }

    private void OnRequestMoveBlock(IServerPlayer fromplayer, RequestMoveBlockPacket packet)
    {
        Block blockToMove = _serverApi.World.BlockAccessor.GetBlock(packet.MoveFrom);
        _serverApi.World.BlockAccessor.SetBlock(0, packet.MoveFrom);
        _serverApi.World.BlockAccessor.SetBlock(blockToMove.BlockId, packet.MoveTo);
        _serverApi.Logger.Notification("Received move block request from client");
    }
    #endregion
    #region Client

    private IClientNetworkChannel _clientNetworkChannel;
    private ICoreClientAPI _clientApi;

    public override void StartClientSide(ICoreClientAPI api)
    {
        _clientApi = api;
        _clientNetworkChannel = api.Network.GetChannel("piston");
    }

    #endregion
}

public class Piston : BlockBehavior
{
    private NetworkSystem _networkSystem = new NetworkSystem();
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
            world.Api.Logger.Notification("No block on north side of piston" + world.Api.Side);
            return true;
        }
        
        if (world.BlockAccessor.GetBlock(newBlockPosition).IsReplacableBy(block))
        {
            ((IClientNetworkChannel)byPlayer.Entity.Api.Network.GetChannel("piston")).SendPacket(new RequestMoveBlockPacket()
            {
                MoveFrom = blockToPushPosition,
                MoveTo = newBlockPosition
            });
            //world.BlockAccessor.SetBlock(0, blockToPushPosition);
            //world.BlockAccessor.SetBlock(blockToPush.BlockId, newBlockPosition);
        }

        //handling = EnumHandling.PreventDefault;
        return true;
    }
}