using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace SchoolManagementSystem.Helpers
{ 
    [HtmlTargetElement("label", Attributes = ForAttributeName)]
    public class RequiredLabelTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For.Metadata.IsRequired)
            {
                var star = "<span class='text-danger'> *</span>";
                output.Content.AppendHtml(star);
            }
        }
    }
}
