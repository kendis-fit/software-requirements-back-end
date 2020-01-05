using Microsoft.AspNetCore.Mvc;

namespace SoftwareRequirements.Exceptions
{
    public class NotFoundException : RestException
    {
        public NotFoundException(string message) : base(message) {}

        public override StatusCodeResult SendStatusCode()
        {
            return new NotFoundResult();
        }
    }
}