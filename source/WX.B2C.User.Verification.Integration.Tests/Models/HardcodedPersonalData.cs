namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public static class HardcodedPersonalData
    {
        // Success flow resident CVI = 50 and no HRI.
        public const string ResidentSuccessFirstName = "Flostri";
        public const string ResidentSuccessLastName = "Cocomeow";
        public const string ResidentSuccessState = "GA";
        public const string ResidentSuccessCity = "Atlanta";
        public const string ResidentSuccessDob = "1989-08-24";
        public const string ResidentSuccessZipCode = "30315";
        public const string ResidentSuccessAddressLine = "96 Vanira Ave SE";
        public const string ResidentSuccessSsn = "650139522";
        public const int ResidentSuccessCvi = 50;

        // Success flow resident CVI = 50 and HRI = 80.
        public const string ResidentSuccessWithHriFirstName = "Ginn";
        public const string ResidentSuccessWithHriLastName = "Bellow";
        public const string ResidentSuccessWithHriState = "MT";
        public const string ResidentSuccessWithHriCity = "Great Falls";
        public const string ResidentSuccessWithHriDob = "1988-12-19";
        public const string ResidentSuccessWithHriZipCode = "59405";
        public const string ResidentSuccessWithHriAddressLine = "2107 4th Ave";
        public const string ResidentSuccessWithHriSsn = "473241830";
        public const int ResidentSuccessWithHriCvi = 50;
        public const string ResidentSuccessWithHri = "80";

        // Success flow resident CVI = 50 and HRI (11,28).
        public const string ResidentPartialSuccessWithHriFirstName = "John";
        public const string ResidentPartialSuccessWithHriLastName = "Brzenk";
        public const string ResidentPartialSuccessWithHriState = "AK";
        public const string ResidentPartialSuccessWithHriCity = "Alakanuk";
        public const string ResidentPartialSuccessWithHriDob = "1982-12-11";
        public const string ResidentPartialSuccessWithHriZipCode = "99554";
        public const string ResidentPartialSuccessWithHriAddressLine = "9012 Pine Ave";
        public const string ResidentPartialSuccessWithHriSsn = "540139012";
        public const int ResidentPartialSuccessWithHriCvi = 50;
        public const string ResidentPartialSuccessWithHri = "11";

        // Success flow resident CVI = 30 and no HRI.
        public const string ResidentPartialSuccessFirstName = "Ronald";
        public const string ResidentPartialSuccessLastName = "Gossling";
        public const string ResidentPartialSuccessState = "LA";
        public const string ResidentPartialSuccessCity = "Alexandria";
        public const string ResidentPartialSuccessDob = "1956-04-17";
        public const string ResidentPartialSuccessZipCode = "71303";
        public const string ResidentPartialSuccessAddressLine = "110 Kastanek Rd";
        public const string ResidentPartialSuccessSsn = "433214838";
        public const int ResidentPartialSuccessCvi = 30;

        // Fail flow resident CVI = 10.
        public const string ResidentFailFirstName = "Dolly";
        public const string ResidentFailLastName = "O'Hara";
        public const string ResidentFailState = "NV";
        public const string ResidentFailCity = "Spring Creek";
        public const string ResidentFailDob = "1947-06-14";
        public const string ResidentFailZipCode = "89815";
        public const string ResidentFailAddressLine = "639 Rock Island Dr";
        public const string ResidentFailSsn = "546908734";
        public const int ResidentFailCvi = 10;

        // Success flow nonresident CVI = 50.
        public const string NonResidentSuccessFirstName = "Katrin";
        public const string NonResidentSuccessLastName = "Donnovan";
        public const string NonResidentSuccessState = "MT";
        public const string NonResidentSuccessCity = "Miles City";
        public const string NonResidentSuccessDob = "1975-04-24";
        public const string NonResidentSuccessZipCode = "59301";
        public const string NonResidentSuccessAddressLine = "301 Marshall St";
        public const int NonResidentSuccessCvi = 50;

        // Success flow nonresident CVI = 30.
        public const string NonResidentPartialSuccessFirstName = "Adam";
        public const string NonResidentPartialSuccessLastName = "Smith";
        public const string NonResidentPartialSuccessState = "TN";
        public const string NonResidentPartialSuccessCity = "Franklin";
        public const string NonResidentPartialSuccessDob = "1978-07-19";
        public const string NonResidentPartialSuccessZipCode = "37064";
        public const string NonResidentPartialSuccessAddressLine = "112 Linwood St";
        public const int NonResidentPartialSuccessCvi = 30;

        // Fail flow nonresident CVI = 10.
        public const string NonResidentFailFirstName = "Allan";
        public const string NonResidentFailLastName = "Beer";
        public const string NonResidentFailState = "VA";
        public const string NonResidentFailCity = "Raleigh";
        public const string NonResidentFailDob = "1937-04-15";
        public const string NonResidentFailZipCode = "27603";
        public const string NonResidentFailAddressLine = "7965 Goldfish Ct";
        public const int NonResidentFailCvi = 10;

        // Pep result bridger search.
        public const string BridgerPepResultFirstName = "Sebastian";
        public const string BridgerPepResultLastName = "Pereira";
        public const string BridgerPepResultState = "AK";
        public const string BridgerPepResultCity = "Emmonak";
        public const string BridgerPepResultDob = "1967-03-27";
        public const string BridgerPepResultZipCode = "99581";
        public const string BridgerPepResultAddressLine = "9234 Hannover Dr";
        public const string BridgerPepResultSsn = "530201236";
    }
}
