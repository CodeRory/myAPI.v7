using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.ComponentModel.DataAnnotations;
using TodoApi.Middleware;
using TodoApi.Models;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection
builder.Services.AddScoped<IEmploymentRepository, EmploymentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TodoApi", Version = "v1" });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();   

}
//THIS ELSE IS FROM EXERCISE, SHOULD I USE ELSE OR JUST INTO THE FIREST IF?
/*else
{
    // app.UseMiddleware
    
}
*/


//THESE ARE MIDDLEWARE
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

//According to the documentation, custom middleware should be here, after Authorization

app.UseMiddleware<MyMiddleware>();


app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();

