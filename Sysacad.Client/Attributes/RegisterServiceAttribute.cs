using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sysacad.Client.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public Type? InterfaceType { get; }

        public RegisterServiceAttribute(ServiceLifetime lifetime, Type? interfaceType = null)
        {
            Lifetime = lifetime;
            InterfaceType = interfaceType;
        }
    }
}