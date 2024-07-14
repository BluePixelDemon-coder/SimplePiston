using SimplePiston.Behaviours;
using SimplePiston.BlockEntitys;
using SimplePiston.Blocks;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

namespace SimplePiston;

public class SimplePistonModSystem : ModSystem
{
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {
        //base.Start(api);
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
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Logger.Notification("Hello from template mod client side: " + Lang.Get("simplepiston:hello"));
    }
}