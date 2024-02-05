using System;
using System.Collections.Generic;
using System.Text;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public partial class OnfidoApiClient
    {
        /// <summary>
        /// Gets the IExtractions.
        /// </summary>
        public virtual IExtractions Extractions { get; private set; }

        /// <summary>
        /// An optional partial-method to perform custom initialization.
        ///</summary>
        partial void CustomInitialize()
        {
            Extractions = new Extractions(this);
        }
    }
}
