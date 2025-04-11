using ExamenCSharp.Models;


namespace ExamenCSharp.Services{
    public interface IJwt {
        public Task<string> GenerateToken(User user, string password);
    }
}