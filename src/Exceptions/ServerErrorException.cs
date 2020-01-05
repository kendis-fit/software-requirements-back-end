using Microsoft.AspNetCore.Mvc;

namespace SoftwareRequirements.Exceptions
{
    public class ServerErrorException : RestException
    {
        public ServerErrorException(string message) : base(message) {}

        public override StatusCodeResult SendStatusCode()
        {
            return new StatusCodeResult(500);
        }
    }
}