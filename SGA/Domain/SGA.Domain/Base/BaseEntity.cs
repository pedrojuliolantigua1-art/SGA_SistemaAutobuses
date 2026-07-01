
namespace SGA.Domain.Base
{
    public abstract class BaseEntity : Auditable
    {
        public int Id { get; set; }
    }
}
