// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Usuario")]
public class User
{
    [Key]
    [Column("id_usuario")]
    public int Id { get; set; }

    [Column("id_empresa")]
    public int? CompanyId { get; set; }

    [Column("nombre_completo")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Required]
    [EmailAddress]
    [Column("correo")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [Column("contrasena_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [Column("rol")]
    [StringLength(20)]
    public string Role { get; set; } = "U"; // Default role

    [Column("fecha_creacion")]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Column("activo")]
    public bool IsActive { get; set; } = true;
}
