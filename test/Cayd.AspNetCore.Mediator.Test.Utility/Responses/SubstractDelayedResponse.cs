namespace Cayd.AspNetCore.Mediator.Test.Utility.Responses
{
    public class SubstractDelayedResponse
    {
        public int Result { get; set; }
        public int TransientCounter { get; set; }
        public int ScopedCounter { get; set; }
        public int SingletonCounter { get; set; }
    }
}
