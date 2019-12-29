using System.Collections.Generic;

using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter.Connections
{
    public class BaseConnectorProfile : IConnectorProfile
    {
        public Dictionary<string, List<ConnectResult>> MakeConnect() => new Dictionary<string, List<ConnectResult>>
            {
                { "I1", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I2" },
                        new ConnectResult { Coefficient = "K2", Index = "I8" }
                    }
                },
                { "I2", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I3" },
                        new ConnectResult { Coefficient = "K2", Index = "I4" },
                        new ConnectResult { Coefficient = "K3", Index = "I5" },
                        new ConnectResult { Coefficient = "K4", Index = "I6" },
                        new ConnectResult { Coefficient = "K5", Index = "I7" }
                    }
                }
            };
    }
}