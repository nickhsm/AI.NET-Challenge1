namespace PassengerTicketService.Components;

public partial class BottomNavBar : ContentView
{
    // Hiermee kunnen we instellen welke tab blauw moet kleuren
    public static readonly BindableProperty SelectedTabProperty =
        BindableProperty.Create(nameof(SelectedTab), typeof(string), typeof(BottomNavBar), string.Empty);

    public string SelectedTab
    {
        get => (string)GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }

    public BottomNavBar()
    {
        InitializeComponent();
    }
}