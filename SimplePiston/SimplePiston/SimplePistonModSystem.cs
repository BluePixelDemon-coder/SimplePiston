using SimplePiston.Behaviours;
using SimplePiston.BlockEntitys;
using SimplePiston.Blocks;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using SimplePiston.Network;
using Vintagestory.API.MathTools;

namespace SimplePiston;

public class SimplePistonModSystem : ModSystem
{
    private ICoreServerAPI ServerApi;
    private IServerNetworkChannel ServerNetworkChannel;
    private ICoreClientAPI ClientApi;
    private IClientNetworkChannel ClientNetworkChannel;
    
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {
        //base.Start(api);
        api.Network.RegisterChannel("piston")
            .RegisterMessageType(typeof(DimensionIdPacket));
        
        api.Logger.Notification("Hello from template mod: " + api.Side);
        api.RegisterBlockBehaviorClass("MovableByPiston", typeof(MovableByPiston));
        api.RegisterBlockBehaviorClass("Piston", typeof(Piston));
        api.RegisterBlockEntityClass("pistonentity", typeof(PistonEntity));
        api.RegisterBlockClass(Mod.Info.ModID + ".piston", typeof(BlockPistonBase) );
        api.RegisterBlockClass(Mod.Info.ModID + ".movable", typeof(BlockMovable) );
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Logger.Notification("Hello from template mod server side: " + Lang.Get("simplepiston:hello"));
        ServerNetworkChannel = api.Network.GetChannel("piston");
        ServerApi = api;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Logger.Notification("Hello from template mod client side: " + Lang.Get("simplepiston:hello"));
        ClientApi = api;
        ClientNetworkChannel = api.Network.GetChannel("piston");
        ClientNetworkChannel.SetMessageHandler<DimensionIdPacket>(OnReceivedDimensionId);
    }

    private void OnReceivedDimensionId(DimensionIdPacket packet)
    {
        ClientApi.Logger.Notification(
            "Received currentPos packet with dimensionId " + packet.DimensionId + " and " + packet.CurrentPos);
        IMiniDimension dimension = ClientApi.World.GetOrCreateDimension(packet.DimensionId, new Vec3d(packet.CurrentPos.X, packet.CurrentPos.Y, packet.CurrentPos.Z));
        dimension.CurrentPos.SetPos(packet.CurrentPos);
        //This does not seem to work, is the positions right?
        ClientApi.World.SetBlocksPreviewDimension(packet.DimensionId);

    }
}