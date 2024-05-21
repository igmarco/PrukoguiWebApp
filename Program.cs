// https://learn.microsoft.com/es-es/visualstudio/get-started/csharp/tutorial-aspnet-core?view=vs-2022
// https://learn.microsoft.com/es-es/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio
// https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0&tabs=visual-studio

using Microsoft.EntityFrameworkCore;
using PrukoguiWebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Context>(opt => opt.UseInMemoryDatabase("ItemList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages(); // Razor

var app = builder.Build();

if (!app.Environment.IsDevelopment()) // Razor
{ // Razor
    app.UseExceptionHandler("/Error"); // Razor
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts. // Razor
    app.UseHsts(); // Razor
} // Razor

app.UseHttpsRedirection(); // Razor
app.UseStaticFiles(); // Razor

app.UseRouting(); // Razor

app.UseAuthorization(); // Razor

app.MapRazorPages(); // Razor

var items = app.MapGroup("/items");

items.MapGet("/", GetAllItems);
items.MapGet("/complete", GetCompleteItems);
items.MapGet("/{id}", GetItem);
items.MapPost("/", CreateItem);
items.MapPut("/{id}", UpdateItem);
items.MapDelete("/{id}", DeleteItem);

app.Run();

static async Task<IResult> GetAllItems(Context db)
{
    return TypedResults.Ok(await db.Items.ToArrayAsync());
}

static async Task<IResult> GetCompleteItems(Context db)
{
    return TypedResults.Ok(await db.Items.Where(t => t.IsComplete).ToListAsync());
}

static async Task<IResult> GetItem(int id, Context db)
{
    return await db.Items.FindAsync(id)
        is Item item
            ? TypedResults.Ok(item)
            : TypedResults.NotFound();
}

static async Task<IResult> CreateItem(Item item, Context db)
{
    db.Items.Add(item);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/items/{item.Id}", item);
}

static async Task<IResult> UpdateItem(int id, Item inputItem, Context db)
{
    var item = await db.Items.FindAsync(id);

    if (item is null) return TypedResults.NotFound();

    item.Name = inputItem.Name;
    item.IsComplete = inputItem.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteItem(int id, Context db)
{
    if (await db.Items.FindAsync(id) is Item item)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}