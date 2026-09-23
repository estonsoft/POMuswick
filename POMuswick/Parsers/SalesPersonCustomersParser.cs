using System.Collections.Concurrent;
using POMuswick.Models;

namespace POMuswick.Parsers
{
    public class SalesPersonCustomersParser : BaseParser
    {
        public SalesPersonCustomersParser()
        {

        }
        public async Task<SalesPersonCustomersResult> Parse(string response)
        {
            Console.WriteLine("Get SalespersonCustomerss returned");
            SalesPersonCustomersResult salesPersonCustomersResult = new SalesPersonCustomersResult();
            List<SalesCustomer> salesCustomers = new List<SalesCustomer>();
            try
            {
                String sCustomers = response;
                String[] aCustomers = sCustomers.Split('~');
                if (aCustomers.Length > 1)
                {
                    foreach (String s in aCustomers)
                    {
                        String[] aCust = s.Split("|");

                        if (aCust.Length < 2)
                        {
                            continue;
                        }

                        SalesCustomer c = new SalesCustomer();
                        c.CustNo = aCust[0];
                        c.CompanyName = aCust[1];
                        c.Address1 = aCust[2];
                        c.City = aCust[3];
                        c.State = aCust[4];
                        c.Zip = aCust[5];
                        c.CityStateZip = c.City.Trim() + ", " + c.State.Trim() + " " + c.Zip.Trim();
                        c.ARBalance = 0;
                        c.ARBalance = GetDecimalValue("AR Balance", aCust[6], 0);

                        c.ARBalanceDisplay = $"${c.ARBalance:0.00}";
                        c.CreditLimit = GetDecimalValue("Credit Limit", aCust[7], 0);
                        if (c.CreditLimit > 0)
                        {
                            c.CreditLimitDisplay = $"${c.CreditLimit:0.00}";
                        }
                        else
                        {
                            c.CreditLimitDisplay = "N/A";
                        }
                        c.Contact = aCust[8];
                        c.Phone = aCust[9];
                        c.Email = aCust[10];
                        // invoice multiplier aCust[11]
                        c.TermsDesc = aCust[12];
                        try
                        {
                            if (aCust[13] == "0")
                            {
                                c.LastPaymentDate = "N/A";
                            }
                            else
                            {
                                c.LastPaymentDate = aCust[13].Substring(3, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(5, 2) + "/";
                                c.LastPaymentDate += aCust[13].Substring(1, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }
                        try
                        {
                            if ((aCust[14] == "0") || (aCust[14] == ""))
                            {
                                c.LastOrderDate = "N/A";
                            }
                            else
                            {
                                c.LastOrderDate = aCust[14].Substring(3, 2) + "/";
                                c.LastOrderDate += aCust[14].Substring(5, 2) + "/";
                                c.LastOrderDate += aCust[14].Substring(1, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
                        }

                        c.MinOrderAmount = GetDecimalValue("Min Order Amount", aCust[15], 0);
                        c.ShippingFee = GetDecimalValue("Shipping Fee", aCust[16], 0);
                        salesCustomers.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get Validate Login exception: " + ex.Message + ex.StackTrace);
            }
            salesPersonCustomersResult.salesCustomers = salesCustomers;
            return salesPersonCustomersResult;
        }
    }
}