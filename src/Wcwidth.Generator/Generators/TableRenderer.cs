using ScribanTemplate = Scriban.Template;

namespace Wcwidth.Generator;

public static class TableRenderer
{
    private const string Template = "Templates/Table.template";

    public static async Task<string> Render(UnicodeRenderingContext ctx)
    {
        // Compile the template
        var template = ScribanTemplate.Parse(await File.ReadAllTextAsync(Template));

        // Prepare template context
        var context = new TemplateContext { LoopLimit = int.MaxValue };
        context.PushGlobal(new ScribanHelpers());
        context.PushGlobal(new ScriptObject
        {
            ["data"] = ctx.Data,
            ["name"] = ctx.ClassName,
        });

        // Render template
        return await template.RenderAsync(context);
    }
}