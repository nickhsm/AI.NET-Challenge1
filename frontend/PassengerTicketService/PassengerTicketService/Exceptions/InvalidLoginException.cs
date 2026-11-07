namespace PassengerTicketService.Exceptions;

// The backend answered 401 on login: wrong email or password.
public class InvalidLoginException : Exception
{
    public InvalidLoginException() : base("E-mailadres of wachtwoord is onjuist.")
    {
    }
}
