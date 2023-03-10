using ReturnRefundCreditToSalesForceCustomers.LogToConsole;
using System;

namespace ReturnRefundCreditToSalesForceCustomers
{
  class Program
  {
    static void Main()
    {
      RefundCreditToSFCustomers refund = new RefundCreditToSFCustomers();
      refund.Execute();
      Logger.LogToConsole("done");
      Logger.ReadLine();
    }
  }
}
