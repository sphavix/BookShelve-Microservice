using Blazored.LocalStorage;
using BookShelve.ServerUI.Providers;
using BookShelve.ServerUI.Services.Auth;
using BookShelve.ServerUI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage(); // used to store session tokens

builder.Services.AddHttpClient<IClient, Client>(cl => cl.BaseAddress = new Uri("https://localhost:7100"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtAuthenticationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(options => options.GetRequiredService<JwtAuthenticationProvider>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
