using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace SimplePiston.Entity;

public class EntityMovedBlock : EntityChunky
{
    public override void OnEntitySpawn()
    {
        base.OnEntitySpawn();
    }

    public static EntityChunky CreateMovableBlock(ICoreServerAPI serverApi, IMiniDimension dimension)
    {
        EntityChunky entity = (EntityChunky)serverApi.World.ClassRegistry.CreateEntity("EntityMovedBlock");
        entity.Code = new AssetLocation("simplepiston:movedblock");
        entity.AssociateWithDimension(dimension);
        return entity;
    }

    public override void OnGameTick(float dt)
    {
        if (this.blocks == null || this.SidedPos == null) return;
        base.OnGameTick(dt);

        this.SidedPos.Motion.X = 0.01;
    }
}