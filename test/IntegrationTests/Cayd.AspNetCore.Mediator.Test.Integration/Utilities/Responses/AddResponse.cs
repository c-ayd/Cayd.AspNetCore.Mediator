namespace Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses
{
    public class AddResponse
    {
        public int Result { get; set; }
        public int TransientCounter { get; set; }
        public int ScopedCounter { get; set; }
        public int SingletonCounter { get; set; }
    }
}
