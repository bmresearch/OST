using Ost.Services.Store.State;

namespace Ost.Services.Store.Events
{
    public class NonceAccountMappingStateChangedEventArgs
    {
        public NonceAccountMappingState State;

        public NonceAccountMappingStateChangedEventArgs(NonceAccountMappingState state)
        {
            State = state;
        }
    }
}
