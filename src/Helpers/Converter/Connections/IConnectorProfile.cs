using System.Collections.Generic;

using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter.Connections
{
    public interface IConnectorProfile
    {
        Dictionary<string, List<ConnectResult>> MakeConnect();
    }
}