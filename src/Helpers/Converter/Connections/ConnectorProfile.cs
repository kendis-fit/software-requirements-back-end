using System.Collections.Generic;

using SoftwareRequirements.Helpers.Converter.Structs.Result;

namespace SoftwareRequirements.Helpers.Converter.Connections
{
    public class ConnectorProfile : IConnectorProfile
    {
        public Dictionary<string, List<ConnectResult>> MakeConnect() => new Dictionary<string, List<ConnectResult>>
            {
                { "I9", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I10" },
                        new ConnectResult { Coefficient = "K2", Index = "I15" }   
                    }
                },
                { "I10", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I11" },
                        new ConnectResult { Coefficient = "K2", Index = "I12" },
                        new ConnectResult { Coefficient = "K3", Index = "I13" },
                        new ConnectResult { Coefficient = "K4", Index = "I14" }
                    }
                },
                { "I15", new List<ConnectResult>
                    {
                        new ConnectResult { Coefficient = "K1", Index = "I16" },
                        new ConnectResult { Coefficient = "K2", Index = "I17" }
                    }
                }
            };
    }
}