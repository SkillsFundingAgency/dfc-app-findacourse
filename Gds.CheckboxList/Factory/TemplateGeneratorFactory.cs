using GdsCheckboxList.Models;
using GdsCheckboxList.Templates;
using System.Diagnostics.CodeAnalysis;

namespace GdsCheckboxList.Factory
{
    /// <summary>
    /// Factory class used to generate Template generator
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class TemplateGeneratorFactory
    {
        /// <summary>
        /// Returns template generator for template type
        /// </summary>
        /// <param name="templateType">Template type</param>
        /// <returns>Template Generator for template type</returns>
        internal static ITemplateGenerator GetTemplateGenerator(TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.GDS:
                    return new GDSTemplateGenerator();
                default:
                    return new BasicTemplateGenerator();
            }
        }
    }
}
