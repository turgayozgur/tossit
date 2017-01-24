namespace Tossit.Core
{
    /// <summary>
    /// General options to send, receive etc.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// If it is true, sender waits to confirm from receiver.
        /// </summary>
        public bool ConfirmIsActive { get; set; }
        /// <summary>
        /// Timeout to confirmation.
        /// </summary>
        public int ConfirmTimeout { get; set; }
    }
}