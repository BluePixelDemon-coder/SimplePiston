using ProtoBuf;
using Vintagestory.API.MathTools;

namespace SimplePiston.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class RequestMoveBlockPacket
{
    public BlockPos MoveFrom;
    public BlockPos MoveTo;
}

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class DimensionIdPacket
{
    public int DimensionId;
    public BlockPos CurrentPos;
}
