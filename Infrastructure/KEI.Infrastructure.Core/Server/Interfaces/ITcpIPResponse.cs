using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public interface ITcpIPResponse
    {
        public uint ResponseID { get; }
        public string ResponseName { get; }
        public void ExecuteResponse(Socket client, bool addCRLF = false);
    }
}
