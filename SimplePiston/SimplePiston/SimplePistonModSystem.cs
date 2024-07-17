using SimplePiston.Behaviours;
using SimplePiston.BlockEntitys;
using SimplePiston.Blocks;
using SimplePiston.Entity;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using SimplePiston.Network;
using Vintagestory.API.MathTools;
using Vintagestory.ServerMods.WorldEdit;

namespace SimplePiston;

public class SimplePistonModSystem : ModSystem
{
    private ICoreServerAPI ServerApi;
    private IServerNetworkChannel ServerNetworkChannel;
    private ICoreClientAPI ClientApi;
    private IClientNetworkChannel ClientNetworkChannel;
    private IMiniDimension PreviewBlocks;
    
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {
        //base.Start(api);
        api.Network.RegisterChannel("piston")
            .RegisterMessageType(typeof(DimensionIdPacket))
            .RegisterMessageType(typeof(CreatedDimensionPacket))
            .RegisterMessageType(typeof(PreviewBlocksPacket));
        
        api.Logger.Notification("Hello from template mod: " + api.Side);
        api.RegisterBlockBehaviorClass("MovableByPiston", typeof(MovableByPiston));
        api.RegisterBlockBehaviorClass("Piston", typeof(Piston));
        api.RegisterBlockEntityClass("pistonentity", typeof(PistonEntity));
        api.RegisterBlockClass(Mod.Info.ModID + ".piston", typeof(BlockPistonBase) );
        api.RegisterBlockClass(Mod.Info.ModID + ".movable", typeof(BlockMovable) );
        //api.RegisterBlockClass(Mod.Info.ModID + ".movedblock", typeof(BlockMovable) );
        api.RegisterEntity("EntityMovedBlock", typeof(EntityMovedBlock));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Logger.Notification("Hello from template mod server side: " + Lang.Get("simplepiston:hello"));
        ServerNetworkChannel = api.Network.GetChannel("piston");
        ServerNetworkChannel.SetMessageHandler<CreatedDimensionPacket>(OnCreatedDimension);
        ServerApi = api;
        PreviewBlocks = ServerApi.World.BlockAccessor.CreateMiniDimension(new Vec3d());
        int dimensionId = ServerApi.Server.SetMiniDimension(PreviewBlocks, 0);
        PreviewBlocks.subDimensionId = dimensionId;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Logger.Notification("Hello from template mod client side: " + Lang.Get("simplepiston:hello"));
        ClientApi = api;
        ClientNetworkChannel = api.Network.GetChannel("piston");
        ClientNetworkChannel
            .SetMessageHandler<DimensionIdPacket>(OnReceivedDimensionId)
            .SetMessageHandler<PreviewBlocksPacket>(OnReceivedPreviewBlocks);
    }

    /*public void SendPreviewOriginToClient(BlockPos origin, int dimensionId)
    {
        ServerNetworkChannel.SendPacket(new PreviewBlocksPacket()
        {
            pos = origin,
            dimId = dimensionId
        }, fromPlayer);
    }*/
    
    private void OnCreatedDimension(IServerPlayer fromplayer, CreatedDimensionPacket packet)
    {
        ServerApi.World.BlockAccessor.SetBlock(0, packet.BlockPos);
    }

    private void OnReceivedDimensionId(DimensionIdPacket packet)
    {
        ClientApi.Logger.Notification(
            "Received DimensionIdPacket packet with dimensionId " + packet.DimensionId + " and currentPos " + packet.CurrentPos);
        IMiniDimension dimension = ClientApi.World.GetOrCreateDimension(packet.DimensionId, packet.CurrentPos.ToVec3d());
        ClientApi.World.SetBlocksPreviewDimension(packet.DimensionId);
        Block block = ClientApi.World.BlockAccessor.GetBlock(packet.CurrentPos);
        
        BlockPos pos = new BlockPos(0, 3, 0, packet.DimensionId);
        ClientApi.Logger.Notification("currentpos before adjustment: " + pos);
        dimension.AdjustPosForSubDimension(pos);
        ClientApi.Logger.Notification("currentpos after adjustment: " + pos);
        ClientApi.World.SetBlocksPreviewDimension(packet.DimensionId);
        
        ClientApi.World.BlockAccessor.SetBlock(0, pos);
        
        ClientNetworkChannel.SendPacket(new CreatedDimensionPacket()
        {
            BlockId = block.BlockId,
            BlockPos = pos
        });
    }

    private void OnReceivedPreviewBlocks(PreviewBlocksPacket packet)
    {
        ClientApi.World.SetBlocksPreviewDimension(packet.dimId);
        if (packet.dimId >= 0)
        {
            IMiniDimension dimension = ClientApi.World.GetOrCreateDimension(packet.dimId, packet.pos.ToVec3d());
            dimension.ClearChunks();
            dimension.selectionTrackingOriginalPos = packet.pos;
        }
    }
}