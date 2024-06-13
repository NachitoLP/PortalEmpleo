using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Generar un salt aleatorio
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Generar el hash de la contraseña con el salt
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Concatenar el salt al hash para su almacenamiento
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Obtener el salt y el hash almacenados
        string[] parts = hashedPassword.Split(':');
        byte[] salt = Convert.FromBase64String(parts[0]);
        string storedHash = parts[1];

        // Generar el hash de la contraseña proporcionada y comparar con el hash almacenado
        string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return storedHash == computedHash;
    }
}