using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.PassFort.Client.Models;

namespace WX.B2C.User.Verification.PassFort
{
    internal interface IPatchDataComparer
    {
        bool Equals(FullName x, FullName y);

        bool Equals(IList<DatedAddressHistoryItem> x, IList<DatedAddressHistoryItem> y);
    }

    internal class PatchDataComparer : IPatchDataComparer
    {
        public bool Equals(FullName x, FullName y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            if (x.FamilyName != y.FamilyName)
                return false;

            return x.GivenNames.IsEquivalent(y.GivenNames);
        }

        public bool Equals(IList<DatedAddressHistoryItem> x, IList<DatedAddressHistoryItem> y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            if (x.Count != y.Count)
                return false;

            return !x.Where((t, i) => !Equals(t.Address, y[i].Address)).Any();
        }

        private bool Equals(Address x, Address y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;

            var xFreeFrom = x as FreeformAddress;
            var yFreeForm = y as FreeformAddress;
            var xStructured = x as StructuredAddress;
            var yStructured = y as StructuredAddress;
            if (xFreeFrom != null && yFreeForm != null)
                return Equals(xFreeFrom, yFreeForm);
            if (xStructured != null && yStructured != null)
                return Equals(xStructured, yStructured);
            if (xStructured != null && yFreeForm != null)
                return Equals(xStructured, yFreeForm);
            if (yStructured != null && xFreeFrom != null)
                return Equals(yStructured, xFreeFrom);

            return false;
        }

        private bool Equals(FreeformAddress x, FreeformAddress y) =>
            x.Text == y.Text &&
            x.Country == y.Country;

        /// <summary>
        ///  Logic based on fields which were updated on old verification
        /// </summary>
        private bool Equals(StructuredAddress x, StructuredAddress y) =>
            x.Country == y.Country &&
            x.Locality == y.Locality &&
            x.PostalCode == y.PostalCode &&
            x.Route == y.Route &&
            x.StreetNumber == y.StreetNumber &&
            x.Subpremise == y.Subpremise &&
            x.StateProvince == y.StateProvince &&
            x.County == y.County;

        /// <summary>
        /// If we send freeform address we will get structured address anyway. Therefore we need to compare them.
        /// </summary>
        private bool Equals(StructuredAddress x, FreeformAddress y) =>
            x.OriginalFreeformAddress.StartsWith(y.Text) &&
            x.Country == y.Country;
    }
}