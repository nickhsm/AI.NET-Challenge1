namespace PassengerTicketService.Exceptions;

// The backend answered 422 on register face: the photo does not contain exactly one face.
public class InvalidPhotoException : Exception
{
    public InvalidPhotoException() : base("Zorg ervoor dat er exact één gezicht op de foto staat.")
    {
    }
}
