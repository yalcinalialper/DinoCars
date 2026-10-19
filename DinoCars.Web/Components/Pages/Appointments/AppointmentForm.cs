using System.ComponentModel.DataAnnotations;

namespace DinoCars.Web.Components.Pages.Appointments
{
    public sealed class AppointmentForm
    {
        private string? _email;
        private string? _vin;

        [Required(ErrorMessage = "Merk is verplicht.")]
        [MaxLength(50)]
        public string Make { get; set; } = "";

        [Required(ErrorMessage = "Model is verplicht.")]
        [MaxLength(50)]
        public string Model { get; set; } = "";

        [Required(ErrorMessage = "Nummerplaat is verplicht.")]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = "";

        [RegularExpression("^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$",
            ErrorMessage = "Een chassisnummer (VIN) bestaat uit 17 letters/cijfers, zonder I, O of Q.")]
        public string? Vin
        {
            get => _vin;
            set => _vin = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        [Required(ErrorMessage = "Kilometerstand is verplicht.")]
        [Range(0, 2_000_000, ErrorMessage = "Geef een geldige kilometerstand in.")]
        public int? Mileage { get; set; }

        [Required(ErrorMessage = "Voornaam is verplicht.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Achternaam is verplicht.")]
        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Telefoonnummer is verplicht.")]
        [Phone(ErrorMessage = "Geef een geldig telefoonnummer in.")]
        [MaxLength(30)]
        public string Phone { get; set; } = "";

        [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres in.")]
        [MaxLength(256)]
        public string? Email
        {
            get => _email;
            set => _email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
