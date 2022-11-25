using Flunt.Notifications;
using Flunt.Validations;
 
namespace MiniPI.ViewModels
{
    public class CreatePIViewModel : Notifiable<Notification>
    {
        public string Disciplina { get; set; }
        public string Comentario { get; set; }
        public int Nota { get; set; }
 

        public PI MapTo()
        {
            AddNotifications(new Contract<Notification>()
                .Requires()
                .IsNotNull(Disciplina, "Informe o nome da disciplina")
                .IsNotNull(Nota,"Insira uma nota"));
                
            return new PI(Guid.NewGuid(), Disciplina, Comentario, Nota);
        }
    }
}
