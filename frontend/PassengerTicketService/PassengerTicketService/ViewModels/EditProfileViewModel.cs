using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class EditProfileViewModel : ObservableObject, IEditProfileViewModel
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private DateTime birthDate;

        public ICommand SaveCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand ChangePasswordCommand { get; private set; } = null!;
        public ICommand DeleteAccountCommand { get; private set; } = null!;

        public EditProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            // TODO: Haal deze data later op uit de database/API voor de ingelogde gebruiker
            FirstName = "JONATHAN";
            LastName = "YANG";
            Email = "jonathan.zirui.yang@gmail.com";
            BirthDate = new DateTime(2005, 12, 4);

            BindCommands(); 
        }

        private void BindCommands()
        {
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
            DeleteAccountCommand = new AsyncRelayCommand(DeleteAccountAsync);
        }

        private async Task SaveAsync()
        {
            // TODO: Roep hier later de API aan om de gegevens op te slaan
            await Application.Current.MainPage.DisplayAlert("Opgeslagen", "Je profielgegevens zijn bijgewerkt.", "OK");
            await _navigationService.NavigateBackAsync();
        }

        private async Task ChangePasswordAsync()
        {
            // TODO: Navigeer later naar een 'Wachtwoord wijzigen' scherm
            await Application.Current.MainPage.DisplayAlert("Wachtwoord", "Navigeer naar wachtwoord wijzigen...", "OK");
        }

        private async Task DeleteAccountAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Account verwijderen",
                "Weet je zeker dat je je account permanent wilt verwijderen? Dit kan niet ongedaan worden gemaakt.",
                "Ja, verwijder",
                "Annuleren");

            if (confirm)
            {
                // TODO: Roep hier later de API aan om het account te verwijderen
                await Application.Current.MainPage.DisplayAlert("Verwijderd", "Je account is verwijderd.", "OK");
                // Stuur de gebruiker terug naar het startscherm / login
                await _navigationService.NavigateToHomePageAsync();
            }
        }
    }
}