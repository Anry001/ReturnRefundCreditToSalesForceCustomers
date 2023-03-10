using System.Data;
using System.Data.SqlClient;
using WallaShops.Data;
using WallaShops.Objects;

namespace ReturnRefundCreditToSalesForceCustomers.DAL
{
  public class SFCustomerClubRefundDAL : WSSqlHelper
  {
    #region Singleton initialization

    private static SFCustomerClubRefundDAL s_Instance = null;

    private SFCustomerClubRefundDAL() : base(WSPlatforms.WallaShops) { }

    public static SFCustomerClubRefundDAL Instance
    {
      get
      {
        if (s_Instance == null)
        {
          s_Instance = new SFCustomerClubRefundDAL();
        }

        return s_Instance;
      }
    }

    #endregion

    #region DAL operations

    public DataTable GetSalesForceClientsThatNeedRefund()
    {
      DataTable customersToRefundCredits = GetDataTable("get_SF_cutomer_club_refund_details");
      customersToRefundCredits.TableName = "customer";
      return customersToRefundCredits;
    }

    public void UpdateRefundStatusForCustomers(string @shopper_id)
    {
      SqlParameter[] spParams = new SqlParameter[1];
      spParams[0] = new SqlParameter("@shopper_id", @shopper_id);
      ExecuteNonQuery("SF_update_refund_status_for_customer", ref spParams);
    }

    #endregion
  }
}