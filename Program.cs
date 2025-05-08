using DCCR_SERVER.Context;
using DCCR_SERVER.Services.Credits;
using DCCR_SERVER.Services.Dashboard;
using DCCR_SERVER.Services.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DCCR_SERVER.Context.BddContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
sqlOptions => sqlOptions.CommandTimeout(1000)));
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("Content-Disposition"));

});
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddScoped<ServiceIntegration>();
builder.Services.AddScoped<ErreurExcelExportService>();
builder.Services.AddScoped<ServiceCreditsCRUD>();
builder.Services.AddScoped<ServiceTBD>();
builder.Services.AddScoped<ServiceExcelCRUD>();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwtSettings["issuer"],
//            ValidateAudience = true,
//            ValidAudience = jwtSettings["audience"],
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["cle_jwt"])),
//            ValidateLifetime = true
//        };
//    });

var app = builder.Build();
app.UseCors("cors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
