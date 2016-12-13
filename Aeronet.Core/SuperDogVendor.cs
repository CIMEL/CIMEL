using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Aeronet.Core
{
    public class SuperDogVendor
    {
        private static SuperDogVendor _default=new SuperDogVendor();

        private StringCollection _stringCollection;

        protected SuperDogVendor()
        {
            // next could be considered ugly.
            // build up a string collection holding
            // the status codes in a human readable manner.
            string[] stringRange = new string[] 
            {
                "Request successfully completed.", 
                "Request exceeds data file range.",
                "",
                "System is out of memory.", 
                "Too many open login sessions.", 
                "Access denied.",
                "",
                "Required SuperDog not found.", 
                "Encryption/decryption data length is too short.", 
                "Invalid input handle.", 
                "Specified File ID not recognized by API.", 
                "",
                "",
                "",
                "",
                "Invalid XML format.",
                "",
                "",
                "SuperDog to be updated not found.", 
                "Required XML tags not found; Contents in binary data are missing or invalid.", 
                "Update not supported by SuperDog.", 
                "Update counter is set incorrectly.", 
                "Invalid Vendor Code passed.",
                "",
                "Passed time value is outside supported value range.", 
                "",
                "Acknowledge data requested by the update, however the ack_data input parameter is NULL.", 
                "Program running on a terminal server.", 
                "",
                "Unknown algorithm used in V2C file.", 
                "Signature verification failed.", 
                "Requested Feature not available.", 
                "",
                "Communication error between API and local SuperDog License Manager.",
                "Vendor Code not recognized by API.", 
                "Invalid XML specification.", 
                "Invalid XML scope.", 
                "Too many SuperDog currently connected.", 
                "",
                "Session was interrupted.", 
                "",
                "Feature has expired.", 
                "SuperDog License Manager version too old.",
                "USB error occurred when communicating with a SuperDog.", 
                "",
                "System time has been tampered.", 
                "Communication error occurred in secure channel.", 
                "",
                "",
                "",
                "Unable to locate a Feature matching the scope.", 
                "",
                "",
                "",
                "Trying to install a V2C file with an update counter that is out" + 
                "of sequence with the update counter in the SuperDog." + 
                "The values of the update counter in the file are lower than" + 
                "those in the SuperDog.", 
                "Trying to install a V2C file with an update counter that is out" + 
                "of sequence with the update counter in the SuperDog." + 
                "The first value of the update counter in the file is greater than" + 
                "the value in the SuperDog."
            };

             _stringCollection = new StringCollection();
            _stringCollection.AddRange(stringRange);

            for (int n = _stringCollection.Count; n < 400; n++)
            {
                _stringCollection.Insert(n, "");
            }

            stringRange = new string[]  
            {
                "A required API dynamic library was not found.",
                "The found and assigned API dynamic library could not be verified.",
            };

            _stringCollection.AddRange(stringRange);

            for (int n = _stringCollection.Count; n < 500; n++)
            {
                _stringCollection.Insert(n, "");
            }

            stringRange = new string[]  
            {
                "Object incorrectly initialized.",
                "A parameter is invalid.",
                "Already logged in.",
                "Already logged out."
            };

            _stringCollection.AddRange(stringRange);

            for (int n = _stringCollection.Count; n < 525; n++)
            {
                _stringCollection.Insert(n, "");
            }

            _stringCollection.Insert(525, "Incorrect use of system or platform.");

            for (int n = _stringCollection.Count; n < 698; n++)
            {
                _stringCollection.Insert(n, "");
            }

            _stringCollection.Insert(698, "Capability is not available.");
            _stringCollection.Insert(699, "Internal API error.");
        }

        public static SuperDogVendor Default { get { return _default;} }

        public string GetStatus(int status)
        {
            try
            {
                return this._stringCollection[status];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private const string VENDOR_CODE =
            "gecPzp8tW+/uGVzMaqUSR4On6+ubUxKDxOobq5vaySU8hyiUTKjxry" +
            "BrppGzrtXWeZX1L8H7U42sIXYOFfcyjNsNq2Ay5wzotO5H60Q6rqxH" +
            "Hsj5tHuDdQq9ISfKDUjNqzS2IpE043a9ItUVCioUDggvgzgrrwfSiJ" +
            "3JPX2wUSZSctb5vKU2MDOrmFiT9wB0uOIMh8n+bZaTM2h+HMYBCV+t" +
            "XxfN3uKBtWCl274C5GcAnck+ES5Qanp+m+24zf5DWs18oSCCMfAYLG" +
            "gWnAQ3G+1DIaUHIT1Ba0LT85nigdmMZP3ro42Znx8IASQ+aS06bVjP" +
            "BSYxfAOzHYRXm3vJ5YgtTFZlmoyQp/Z6Bo71XcLCPQA9r7QX7gzrD9" +
            "WKeugDAEO7j+t0ACb41HJAiFpFIPa04H0LITcMW4ihsgwKQWG/kyzz" +
            "EKJTDp0E6OXhMVouzavdffrcLBQSD01uoldVP6l5f9vLDQti0OSj7y" +
            "/0i/YM7Dyq2rcAxGl8ICadb3Y0Hg/htgE4Yfb0F5bscscNQK1JBuU0" +
            "7LjqbtkrdgtGQw/uCq0mU2HNwPlCvPDybEcSgWjZpbLik9MG+4oIMf" +
            "Ok4bdJR4a+rPce9jSb8wR5Gn9KJJlfUeIutZGMK6nyK1EIWZ4opdRy" +
            "EFU/ykfDo4gQspf7u+SVwJna6vcvTMCUFzsVvSJPDb4pVsDInGwHXj" +
            "2MVf+5Db024LiBi3xn6jYtmBrIgsWnODeGZ9r90J13IUyyqt0Ls3cW" +
            "V8oCLzpy7PkpaCZq/KCdLOVl2iQujze8dXWnI4VBKEJOrnhLSkB5cJ" +
            "cFwlWbhHPok5yCZgL+OKiwbUF9iamchxQciGXRqxIlF/mKavtqmQrz" +
            "Wspxpa9tI+bWJY9NfjhlCJRrKX+TFkJxLQwNsUrVCr+4gm/wHF0qlk" +
            "q4R27XOevHuLbalRISFR4SMGf91XPnWKESN8pHPYs4krs5cfvNl04M" +
            "gdCBCPzs1w==";

        private const string DEFAULT_SCOPE = "<dogscope />";

        public static string VendorCode
        {
            get
            {
                return VENDOR_CODE;
            }
        }

        public static string DefaultScope { get { return DEFAULT_SCOPE; } }
    }
}
