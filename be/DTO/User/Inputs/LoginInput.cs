namespace TaskSystem.DTO.User.Inputs;

public class LoginInput
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}