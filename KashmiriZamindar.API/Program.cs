using KashmiriZamindar.Core.Helpers;
using KashmiriZamindar.Core.Interfaces;
using KashmiriZamindar.Core.Services;
using KashmiriZamindar.Infrastructure.Data;
using KashmiriZamindar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);

// =======================
// SERVICES
// =======================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));
// ? Auth services
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<AuthService>();


// ? Product services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

// ? Cart services
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<CartService>();

// ? Checkout services
builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();
builder.Services.AddScoped<CheckoutService>();

// After other services
builder.Services.AddHttpClient();  // For Razorpay API calls
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService>();
// ? Admin services
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailProcessingBackgroundService>();

// ? Cart services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserOrderService>();
// ?? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// =======================
// MIDDLEWARE
// =======================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ?? CORS MUST BE HERE (ORDER IS IMPORTANT)
app.UseCors("AllowAngular");

app.UseAuthorization();

// ? Map controllers
app.MapControllers();

app.Run();





