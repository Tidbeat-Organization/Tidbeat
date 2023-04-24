using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

public class HtmlViewEngine : IViewEngine {
    public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage) {
        // Set the path to the Views/Html folder
        var htmlViewPath = Path.Combine("Views", "VSdoc");

        // Set the path to the HTML file
        var htmlFilePath = Path.Combine(htmlViewPath, $"{viewName}.html");

        // Check if the file exists
        if (File.Exists(htmlFilePath)) {
            return ViewEngineResult.Found(viewName, new HtmlView(htmlFilePath));
        }
        else {
            return ViewEngineResult.NotFound(viewName, new List<string>() { htmlFilePath });
        }
    }

    public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage) {
        return ViewEngineResult.NotFound(viewPath, new List<string>());
    }

    public void ReleaseView(ActionContext context, IView view) {
        // Do nothing
    }
}

public class HtmlView : IView {
    private string _htmlFilePath;

    public HtmlView(string htmlFilePath) {
        _htmlFilePath = htmlFilePath;
    }

    public string Path => _htmlFilePath;

    public async Task RenderAsync(ViewContext context) {
        // Read the HTML file and write it to the output stream
        var html = await File.ReadAllTextAsync(_htmlFilePath);
        await context.Writer.WriteAsync(html);
    }
}
