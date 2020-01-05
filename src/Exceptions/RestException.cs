using System;
using Microsoft.AspNetCore.Mvc;

namespace SoftwareRequirements.Exceptions
{
    public abstract class RestException : Exception
    {
        public RestException(string message) : base(message) {}

        public abstract StatusCodeResult SendStatusCode();
    }
}