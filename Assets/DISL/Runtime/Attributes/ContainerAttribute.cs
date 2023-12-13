using System;

namespace DISL.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ContainerAttribute : System.Attribute
    {
        public string Name { get; }
        
        public ContainerAttribute(string name)
        {
            Name = name;
        }
    }
}