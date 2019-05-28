//namespace Capstan.Events
//{
//    /// <summary>
//    /// Events should always strive to create a succesful outcome,
//    /// if this is not possible, set state to rollback, which will inform the client and
//    /// the event processor to attempt to revert any changes made.
//    /// Error is a status that is used to explain to a client that the event was
//    /// not successful, but at the same time needs to be rolled back. The actions
//    /// taken by the client might differ between these two negative statuses, that 
//    /// is why they exist.
//    /// </summary>
//    public enum EventResolutionType
//    {
//        Rollback = 0,
//        Commit = 1,
//        Error = 99
//    }
//}
