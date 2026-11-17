namespace POS_Backend.Logica.Domain;

// Resultado común para las operaciones de los Domain (Exito + mensaje de error)
public record Resultado(bool Exito, string? Error);
