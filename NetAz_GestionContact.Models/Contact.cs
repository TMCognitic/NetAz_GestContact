namespace NetAz_GestionContact.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Email { get; set; }
        public DateTime Naissance { get; set; }
        public string? Tel { get; set; }
    }
}