using ExamenCSharp.DTO;
using ExamenCSharp.Models;


namespace ExamenCSharp.Services{
    public interface IReservation
    {
        Task<List<InterventionDTO>> GetReservationsAsync(User user);
        Task<Intervention?> ReserverAsync(Intervention intervention, string emailReserviste, List<string> emailsTechnicien);
        Task ModifierReservationAsync(int id, Intervention intervention);
        Task AnnulerReservationAsync(int id);
        Task<List<ServiceDTO>> GetAllServicesAsync();
        Task<Service?> GetServiceAsync(string name);
        Task AjouterNouveauService(string name);
    }
}