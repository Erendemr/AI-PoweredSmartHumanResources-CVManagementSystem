using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace InsanK.Helpers
{
    public class FileUploadOperationFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(IFormFile))
            {
                schema.Type = "string";
                schema.Format = "binary";
            }
        }
    }
}
