using Microsoft.AspNetCore.Mvc;

namespace SoftwareRequirements.Exceptions
{
    public class BadRequestException : RestException
    {
        public BadRequestException(string message) : base(message) {}

        public override StatusCodeResult SendStatusCode()
        {
            return new BadRequestResult();
        }
    }
}