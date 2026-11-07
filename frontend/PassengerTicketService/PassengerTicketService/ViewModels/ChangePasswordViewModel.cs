using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject, IChangePasswordViewModel
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmNewPassword = string.Empty;

        [ObservableProperty]
        private bool isCurrentPasswordVisible = false;

        [ObservableProperty]
        private bool isNewPasswordVisible = false;

        [ObservableProperty]
        private bool isConfirmPasswordVisible = false;

        public bool IsCurrentPasswordHidden => !IsCurrentPasswordVisible;
        public bool IsNewPasswordHidden => !IsNewPasswordVisible;
        public bool IsConfirmPasswordHidden => !IsConfirmPasswordVisible;

        // Zorg dat de UI updatet als de zichtbaarheid verandert
        partial void OnIsCurrentPasswordVisibleChanged(bool value) => OnPropertyChanged(nameof(IsCurrentPasswordHidden));
        partial void OnIsNewPasswordVisibleChanged(bool value) => OnPropertyChanged(nameof(IsNewPasswordHidden));
        partial void OnIsConfirmPasswordVisibleChanged(bool value) => OnPropertyChanged(nameof(IsConfirmPasswordHidden));

        public ICommand SaveCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand ToggleCurrentPasswordVisibilityCommand { get; private set; } = null!;
        public ICommand ToggleNewPasswordVisibilityCommand { get; private set; } = null!;
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; private set; } = null!;

        public ChangePasswordViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);

            ToggleCurrentPasswordVisibilityCommand = new RelayCommand(() => IsCurrentPasswordVisible = !IsCurrentPasswordVisible);
            ToggleNewPasswordVisibilityCommand = new RelayCommand(() => IsNewPasswordVisible = !IsNewPasswordVisible);
            ToggleConfirmPasswordVisibilityCommand = new RelayCommand(() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible);
        }

        private async Task SaveAsync()
        {
            // Basis validatie
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alstublieft alle velden in.", "OK");
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De wachtwoorden komen niet overeen.", "OK");
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Je nieuwe wachtwoord mag niet hetzelfde zijn als je huidige wachtwoord.", "OK");
                return;
            }

            // TODO: Roep hier later de API aan om het wachtwoord te wijzigen
            await Application.Current.MainPage.DisplayAlert("Succes", "Je wachtwoord is succesvol gewijzigd.", "OK");
            await _navigationService.NavigateBackAsync();
        }
    }
}