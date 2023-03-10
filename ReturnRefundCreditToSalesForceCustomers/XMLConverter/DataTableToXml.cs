using ReturnRefundCreditToSalesForceCustomers.DAL;
using ReturnRefundCreditToSalesForceCustomers.LogToConsole;
using System;
using System.Data;
using System.Xml.Linq;
using WallaShops.Objects;
using WallaShops.Utils;

namespace ReturnRefundCreditToSalesForceCustomers.XMLConverter
{
  public class DataTableToXml
  {
    #region Methods to create customers XML

    public static XDocument CustomersToRefundDataTableToXmlDocument(DataTable customersToRefundDataTable)
    {
      int numberOfCustomersSuccessfullyUpdated = 0;
      SFCustomerClubRefundDAL CustomerRefundDAL = SFCustomerClubRefundDAL.Instance;
      XNamespace NameSpaceXml = WSGeneralUtils.GetAppSettings("NameSpace");
      XDocument SFRefundCreditsXML = new XDocument();
      XElement customersElement = new XElement(NameSpaceXml + "customers");

      SFRefundCreditsXML.Add(customersElement);
      foreach (DataRow customer in customersToRefundDataTable.Rows)
      {
        XElement customerElement = new XElement(NameSpaceXml + "customer");
        customerElement.SetAttributeValue("customer-no", customer["shopper_id"]);
        XElement profileElement = new XElement(NameSpaceXml + "profile");
        XElement customAttributes = new XElement(NameSpaceXml + "custom-attributes");
        XElement customAttribute = new XElement(NameSpaceXml + "custom-attribute", customer["amount"]);

        customAttribute.SetAttributeValue("attribute-id", "membersClubMonthlyPoints");
        customersElement.Add(customerElement);
        customerElement.Add(profileElement);
        profileElement.Add(customAttributes);
        customAttributes.Add(customAttribute);
        CustomerRefundDAL.UpdateRefundStatusForCustomers(WSStringUtils.ToString(customer["shopper_id"]));
        numberOfCustomersSuccessfullyUpdated++;
      }

      Logger.LogToConsole($"Number of customers updated: {WSStringUtils.ToString(numberOfCustomersSuccessfullyUpdated)}{Environment.NewLine}");
      using (WSLog log = new WSLog(WSEventType.Info))
      {
        log["ReturnRefundCreditToSalesForceCustomers", "number_of_customers_updated"] = numberOfCustomersSuccessfullyUpdated;
        log.Dispose();
      }

      return SFRefundCreditsXML;
    }

    #endregion
  }
}