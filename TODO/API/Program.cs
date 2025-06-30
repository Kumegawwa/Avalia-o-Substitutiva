using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();

app.MapGet("/", (AppDataContext context) =>
{
    var status = context.Status.ToList();
    return Results.Ok(status);
});

app.MapPost("/api/tarefas", (AppDataContext context, Tarefa tarefa) =>
{
    if (string.IsNullOrEmpty(tarefa.Titulo))
    {
        return Results.BadRequest("Título é obrigatório.");
    }
    if (tarefa.Titulo.Length < 3)
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    if (tarefa.StatusId == null)
    {
        return Results.BadRequest("Status é obrigatório.");
    }

    var status = context.Status.Find(tarefa.StatusId);
    if (status == null)
    {
        return Results.NotFound($"Status com ID {tarefa.StatusId} não encontrado");
    }

    tarefa.Status = status;
    context.Tarefas.Add(tarefa);
    context.SaveChanges();

    return Results.Created($"/api/tarefas/{tarefa.Id}", tarefa);
});

app.MapGet("/api/tarefas", (AppDataContext context) =>
{
    var tarefas = context.Tarefas.Include(t => t.Status).ToList();
    return Results.Ok(tarefas);
});

app.MapGet("/api/tarefas/{id}", (AppDataContext context, int id) =>
{
    var tarefa = context.Tarefas.Include(t => t.Status).FirstOrDefault(t => t.Id == id);

    if (tarefa == null)
    {
        return Results.NotFound($"Tarefa com ID {id} não encontrada.");
    }

    return Results.Ok(tarefa);
});

app.MapPut("/api/tarefas/{id}", (AppDataContext context, int id, Tarefa tarefaAtualizada) =>
{
    var tarefaExistente = context.Tarefas.Find(id);

    if (tarefaExistente == null)
    {
        return Results.NotFound($"Tarefa com ID {id} não encontrada para atualização.");
    }

    if (string.IsNullOrEmpty(tarefaAtualizada.Titulo))
    {
        return Results.BadRequest("Título é obrigatório.");
    }
    if (tarefaAtualizada.Titulo.Length < 3)
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    if (tarefaAtualizada.StatusId == null)
    {
        return Results.BadRequest("Status é obrigatório.");
    }

    var status = context.Status.Find(tarefaAtualizada.StatusId);
    if (status == null)
    {
        return Results.NotFound($"Status com ID {tarefaAtualizada.StatusId} não encontrado");
    }

    tarefaExistente.Titulo = tarefaAtualizada.Titulo;
    tarefaExistente.StatusId = tarefaAtualizada.StatusId;
    tarefaExistente.Status = status;

    context.SaveChanges();

    return Results.Ok(tarefaExistente);
});

app.MapDelete("/api/tarefas/{id}", (AppDataContext context, int id) =>
{
    var tarefa = context.Tarefas.Find(id);

    if (tarefa == null)
    {
        return Results.NotFound($"Tarefa com ID {id} não encontrada para remoção.");
    }

    context.Tarefas.Remove(tarefa);
    context.SaveChanges();

    return Results.NoContent();
});

app.Run();
