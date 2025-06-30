using System.ComponentModel.DataAnnotations;

namespace API.Modelos;

public class Tarefa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Título é obrigatório.")]
    [MinLength(3, ErrorMessage = "Título deve ter no mínimo 3 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "Status é obrigatório.")]
    public int? StatusId { get; set; }

    public Status? Status { get; set; }
}