using System.Reflection;
using System.Reflection.Emit;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Extensions for the TypeBuilder class
    /// </summary>
    public static class TypeBuilderExtensions
    {
        /// <summary>
        /// Defines a Default Constructor with common method attributes
        /// </summary>
        /// <param name="builder">The builder to construct the default constructor for</param>
        /// <returns>The builder used to construct the constructor</returns>
        public static TypeBuilder DefineSimpleDefaultConstructor(this TypeBuilder builder)
        {
            builder.DefineDefaultConstructor(MethodAttributes.Public |
                MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            return builder;
        }
        
    }
}