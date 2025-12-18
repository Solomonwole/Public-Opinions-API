namespace Crud.Api.DTOs
{
    public class AuthDTOs
    {
        public record RegisterDto(string Email, string UserName, string Password);
        public record LoginDto(string Email, string Password);
        public record ForgotPasswordDto(string Email);
        public record ResetPasswordDto(string Token, string NewPassword);
    }
}
