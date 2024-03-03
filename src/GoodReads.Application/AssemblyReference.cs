using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GoodReads.Application
{
    [ExcludeFromCodeCoverage]
    public class AssemblyReference
    {
        public Assembly GetAssembly() => GetType().Assembly;
    }
}