namespace Tossit.Core
{
    /// <summary>
    /// General options to send.
    /// </summary>
    public class SendOptions
    {
        /// <summary>
        /// Default confirm receipt timeout as second. Value: 10 sec.
        /// </summary>
        private const int DEAFULT_CONFIRM_RECEIPT_TIMEOUT = 10;

        /// <summary>
        /// ConfirmReceiptTimeoutSeconds field.
        /// </summary>
        private int _confirmReceiptTimeoutSeconds;

        /// <summary>
        /// Default 10 seconds. Wait until a dispatched data have been confirmed.
        /// Returns default if not specified.
        /// </summary>
        public virtual int ConfirmReceiptTimeoutSeconds
        {
            get
            {
                return _confirmReceiptTimeoutSeconds > 0 ? _confirmReceiptTimeoutSeconds : DEAFULT_CONFIRM_RECEIPT_TIMEOUT;
            }
            set
            {
                _confirmReceiptTimeoutSeconds = value;
            }
        }

        /// <summary>
        /// True if u want to wait to successfully receive message from broker until timeout, otherwise should be false.
        /// It is highly recommended to be true.
        /// Default: true.
        /// </summary>
        public virtual bool ConfirmReceiptIsActive { get; set; } = true;
    }
}