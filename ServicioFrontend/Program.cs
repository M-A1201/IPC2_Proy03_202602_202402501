using ServicioFrontend.Services; // <-- 1. Agregamos el using para reconocer la carpeta Services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 2. Registramos nuestro servicio ApiServicio indicando la URL base de tu ServicioAPI
builder.Services.AddHttpClient<ApiServicio>(client =>
{
    // Asegúrate de colocar el puerto exacto en el que se ejecuta tu ServicioAPI (por ejemplo, 5220)
    client.BaseAddress = new Uri("http://localhost:5220/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();