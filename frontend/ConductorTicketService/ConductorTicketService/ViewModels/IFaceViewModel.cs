using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IFaceViewModel
{
    ICommand ReturnCommand { get; set; }
}