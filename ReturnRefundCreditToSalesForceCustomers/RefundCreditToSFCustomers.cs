using ReturnRefundCreditToSalesForceCustomers.DAL;
using ReturnRefundCreditToSalesForceCustomers.LogToConsole;
using ReturnRefundCreditToSalesForceCustomers.XMLConverter;
using System;
using System.Data;
using System.Xml.Linq;
using WallaShops.Common.SalesForce;
using WallaShops.Objects;
using WallaShops.Utils;

namespace ReturnRefundCreditToSalesForceCustomers
{
  public class RefundCreditToSFCustomers
  {
    #region Class fields

    private DataTable CustomersToRefundDataTable { get; set; }

    #endregion

    #region CTOR
    
    public RefundCreditToSFCustomers()
    {
      getCustomersToRefundCredits();
    }

    private void getCustomersToRefundCredits()
    {
      try
      {
        CustomersToRefundDataTable = SFCustomerClubRefundDAL.Instance.GetSalesForceClientsThatNeedRefund();
      }
      catch (Exception ex)
      {
        Logger.LogToConsole(ex.Message);
        using (WSLog log = new WSLog(WSEventType.Info))
        {
          log["ReturnRefundCreditToSalesForceCustomers", "ex_msg"] = ex.Message;
          log.Dispose();
        }
      }
    }

    #endregion

    #region Execute credit refund process to customers

    public void Execute()
    {
      try
      {
        bool isDataTableOfCustomersPendingForRefundNotEmpty = CustomersToRefundDataTable.Rows.Count > 0;

        Logger.LogToConsole($"console msg: Number of orders pending to update: {WSStringUtils.ToString(CustomersToRefundDataTable.Rows.Count)}{Environment.NewLine}");
        using (WSLog log = new WSLog(WSEventType.Info))
        {
          log["ReturnRefundCreditToSalesForceCustomers", "total_orders"] = WSStringUtils.ToString(CustomersToRefundDataTable.Rows.Count);
          log.Dispose();
        }

        if (isDataTableOfCustomersPendingForRefundNotEmpty)
        {
          createCustomersXml();
        }
      }
      catch (Exception ex)
      {
        Logger.LogToConsole(ex.Message);
        using (WSLog log = new WSLog(WSEventType.Info))
        {
          log["ReturnRefundCreditToSalesForceCustomers", "ex_msg"] = ex.Message;
          log.Dispose();
        }
      }
    }

    private void createCustomersXml()
    {
      string fileName = "NewCustomers_" + DateTime.Now.ToString(format: "ddmmyyyy_HHmmss") + ".xml";
      string fullPath = WSGeneralUtils.GetAppSettings("ExportPathCustomersXml") + "\\" + fileName;

      XDocument CustomersRefundXml = DataTableToXml.CustomersToRefundDataTableToXmlDocument(CustomersToRefundDataTable);
      CustomersRefundXml.Save(fullPath);
      Logger.LogToConsole($"Saved to:{fullPath}");
      SFSftpClient sftpClient = new SFSftpClient(WSGeneralUtils.GetAppSettings("SFTPHostAddress"), WSGeneralUtils.GetAppSettings("SFTPUserName"),
        WSGeneralUtils.GetAppSettings("SFTPPassword"), WSGeneralUtils.GetAppSettingsInt("SFTPPort"));

      using (WSLog log = new WSLog(WSEventType.Info))
      {
        log["ReturnRefundCreditToSalesForceCustomers", "err"] = sftpClient.UploadFile(fullPath, WSGeneralUtils.GetAppSettings("SFTPRemotePath"), fileName);
        log["ReturnRefundCreditToSalesForceCustomers", "SFTP_updated"] = "SFTP success";
        log.Dispose();
      }
      Logger.LogToConsole($"UploadFile to sftp path:{WSGeneralUtils.GetAppSettings("SFTPRemotePath")}");
      Logger.LogToConsole("press any key to continue");
      Logger.ReadLine();
      string err = sftpClient.UploadFile(fullPath, WSGeneralUtils.GetAppSettings("SFTPRemotePath"), fileName);

      Logger.LogToConsole(err);
    }

    #endregion
  }
}