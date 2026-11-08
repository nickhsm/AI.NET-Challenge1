using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject, IChangePasswordViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAccountService _accountService;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

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

        public ChangePasswordViewModel(INavigationService navigationService, IAccountService accountService)
        {
            _navigationService = navigationService;
            _accountService = accountService;
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
            // Don't leave the error of a previous attempt on screen.
            ErrorMessage = null;

            // Basis validatie: check of ALLE drie de velden zijn ingevuld
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alstublieft alle velden in.", "OK");
                return;
            }

            // Check of de nieuwe wachtwoorden overeenkomen
            if (NewPassword != ConfirmNewPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De nieuwe wachtwoorden komen niet overeen.", "OK");
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Je nieuwe wachtwoord mag niet hetzelfde zijn als je huidige wachtwoord.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                await _accountService.ChangePasswordAsync(CurrentPassword, NewPassword);
            }
            catch (WrongPasswordException ex)
            {
                ErrorMessage = ex.Message;
                return;
            }
            catch (ApiException ex)
            {
                ErrorMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
                Debug.WriteLine(ex);
                return;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                ErrorMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
                Debug.WriteLine(ex);
                return;
            }
            finally
            {
                IsBusy = false;
            }

            // Only reached when the change worked: every catch above returns.
            // Don't keep the passwords in memory longer than needed.
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;

            await Application.Current.MainPage.DisplayAlert("Succes", "Je wachtwoord is succesvol gewijzigd.", "OK");
            await _navigationService.NavigateBackAsync();
        }
    }
}