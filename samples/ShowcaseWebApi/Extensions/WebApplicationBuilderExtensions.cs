using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using ModEndpoints;
using ShowcaseWebApi.ApiVersioning;
using ShowcaseWebApi.Features.Books;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShowcaseWebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
  public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(options =>
    {
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
      {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
      });
      options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecuritySchemeReference("Bearer", document),
          new List<string>()
        }
      });

      options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
      options.CustomSchemaIds(type => type.ToString());
    });
    return builder;
  }

  public static WebApplicationBuilder AddWebServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddProblemDetails(options =>
    {
      options.CustomizeProblemDetails = context =>
      {
        context.ProblemDetails.Instance =
          $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId",
          context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId",
          activity?.Id);
      };
    });
    builder.Services
      .AddApiVersioning(opt =>
      {
        opt.ApiVersionReader = new UrlSegmentApiVersionReader();
      })
      .AddApiExplorer(options =>
      {
        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
      });
    return builder;
  }

  public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
  {
    builder.Services.AddModEndpointsFromAssemblyContaining<GetBookById>();
    builder.Services.AddValidatorsFromAssemblyContaining<GetBookByIdRequestValidator>(includeInternalTypes: true);

    return builder;
  }
}
