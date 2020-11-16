using KEI.Infrastructure.Service;

namespace KEI.Infrastructure.Server
{
    [OptionalService("Commander")]
    public interface ICommander : ITcpCommander, ITcpResponder
    {

    }
}
