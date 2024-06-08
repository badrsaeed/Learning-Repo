
using ApiErrorHandlingTemplete.Errors;
using ApiErrorHandlingTemplete.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace ApiErrorHandlingTemplete
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors =  context.ModelState.Where(P => P.Value.Errors.Any())
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToList();

                    var validationResponse = new ApiValidationResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationResponse);
                };
            });

            var app = builder.Build();


            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
