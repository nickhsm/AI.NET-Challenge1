using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IIdViewModel
{
    ICommand ReturnCommand { get; set; }
}