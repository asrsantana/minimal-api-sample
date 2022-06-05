using Microsoft.EntityFrameworkCore;
using MinimalWebApi.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

BuildRoutes(app);

app.Run();

static void BuildRoutes(WebApplication app)
{
    BuildGetRoutes(app);
    BuildPostRoutes(app);
    BuildPutRoutes(app);
    BuildDeleteRoutes(app);
}

static void BuildGetRoutes(WebApplication app)
{
    app.MapGet("/", () => "Hello World!");

    app.MapGet("/todoItems", async (TodoDb db) =>
        await db.Todos.ToListAsync());

    app.MapGet("/todoItems/complete", async (TodoDb db) =>
        await db.Todos.Where(t => t.IsComplete).ToListAsync());

    app.MapGet("/todoItems/{id}", async (int id, TodoDb db) =>
        await db.Todos.FindAsync(id)
            is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound());
}

static void BuildPostRoutes(WebApplication app)
{
    app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return Results.Created($"/todoitems/{todo.Id}", todo);
    });
}

static void BuildPutRoutes(WebApplication app)
{
    app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return Results.NotFound();

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });
}

static void BuildDeleteRoutes(WebApplication app)
{
    app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.Ok(todo);
        }

        return Results.NotFound();
    });
}