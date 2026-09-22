using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class LoginParser : BaseParser
    {
        public LoginParser()
        {

        }
        public LoginResult Parse(string response)
        {
            LoginResult result = new();

            if (string.IsNullOrWhiteSpace(response))
            {
                result.ErrorMessage = "Empty server response.";
                return result;
            }

            var info = response.Split('~');

            if (info.Length < 2)
            {
                result.ErrorMessage = "Invalid server response.";
                return result;
            }

            var user = info[0].Split('|');
            var cust = info[1].Split('|');

            if (user.Length == 0 || string.IsNullOrWhiteSpace(user[0]))
            {
                result.ErrorMessage = "Invalid login response.";
                return result;
            }

            result.Status = user[0];

            switch (user[0])
            {
                case "V":

                    result.Success = true;

                    result.Settings = BuildSettings(user);

                    result.Customer = BuildCustomer(user, cust);

                    result.Location = BuildLocation(cust);

                    result.RefreshData = true;

                    break;

                case "P":
                    result.ErrorMessage = "Invalid password.";
                    break;

                case "I":
                    result.ErrorMessage = "Inactive account.";
                    break;

                case "U":
                    result.ErrorMessage = "Account does not exist.";
                    break;

                default:
                    result.ErrorMessage = "Login failed.";
                    break;
            }
            return result;
        }

        public LoginResult ParseActiveUser(string response)
        {
            LoginResult result = new() { Status = response };
            return result;
        }

        private Location BuildLocation(string[] aCust)
        {
            if (aCust.Count() < 20)
            {
                return new Location();
            }
            else
            {
                return new Location()
                {
                    LocationId = 1,
                    Name = aCust[14],
                    Address = aCust[15],
                    City = aCust[16],
                    State = aCust[17],
                    Zip = aCust[18],
                    CityStateZip = aCust[16] + ", " + aCust[17] + " " + aCust[18],
                    Phone = aCust[19],
                };
            }
        }

        private Customer BuildCustomer(string[] aUser, string[] aCust)
        {
            if (aCust.Count() > 0)
            {
                return new Customer();
            }
            else
            {
                return new Customer()
                {
                    Status = "9",
                    CompanyName = aCust[1],
                    Warehouse = GetIntegerValue("Warehouse", aCust[3], 0),
                    Address1 = aCust[4],
                    City = aCust[5],
                    State = aCust[6],
                    Zip = aCust[7],
                    CityStateZip = aCust[5] + ", " + aCust[6] + "  " + aCust[7],
                    Phone = aCust[8],
                    Contact = aCust[9],
                    Delivery = GetIntegerValue("Delivery", aCust[10], 0),
                    Pickup = GetIntegerValue("Pickup", aCust[11], 0),
                    CreditLimit = GetDecimalValue("Credit Limit", aCust[12], 0),
                    ARBalance = GetDecimalValue("AR Balance", aCust[13], 0),
                    MinOrderAmount = GetDecimalValue("Min Order Amount", aCust[20], 0),
                    ShippingFee = GetDecimalValue("Shipping Fee", aCust[21], 0),
                    CustNo = aUser[1]
                };
            }
        }

        private AppSettings BuildSettings(string[] aUser)
        {
            AppSettings appSettings = new AppSettings();
            appSettings.IsLoggedIn = true;
            appSettings.CustomerNo = aUser.ElementAtOrDefault(1) ?? "";
            appSettings.IsCredits = aUser.ElementAtOrDefault(2) == "1";
            appSettings.HoldForReview = aUser.ElementAtOrDefault(3) == "1";
            appSettings.ForceSubmit = aUser.ElementAtOrDefault(4) == "1";
            appSettings.QOHDisplay = aUser.ElementAtOrDefault(5) ?? "X";
            appSettings.BlockItemsNoQOH = aUser.ElementAtOrDefault(6) == "1";
            appSettings.IsSalesUser = aUser.ElementAtOrDefault(8) == "1";
            return appSettings;
        }
    }
}