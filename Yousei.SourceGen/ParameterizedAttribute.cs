using System;

namespace Yousei.SourceGen
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class ParameterizedAttribute : Attribute
    {
        public ParameterizedAttribute(Type targetType)
        {
        }
    }
}