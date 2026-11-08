namespace PassengerTicketService.Exceptions;

// The backend answered 409 on register face: this account already has a face,
// or this face is already registered on another account.
public class FaceAlreadyRegisteredException : Exception
{
    public FaceAlreadyRegisteredException() : base("Dit gezicht is al geregistreerd.")
    {
    }
}
