namespace bmiWebAPI_3.Services;

public interface IPasswordService
{
    string HashPassword(string password);

    bool VerifyPassword(string hashedPassword, string password);
}