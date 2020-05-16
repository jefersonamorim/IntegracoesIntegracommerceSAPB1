using Integracommerce.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.DAL
{
    public class CommonConn
    {
        static SAPbobsCOM.Company oCompany;

        public static SAPbobsCOM.Company InitializeCompany()
        {
            try
            {
                Log log = new Log();

                oCompany = new SAPbobsCOM.Company();

                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
                oCompany.Server = ConfigurationManager.AppSettings["Server"];
                oCompany.CompanyDB = ConfigurationManager.AppSettings["DataBase"];
                oCompany.DbUserName = ConfigurationManager.AppSettings["DbUser"];
                oCompany.DbPassword = ConfigurationManager.AppSettings["DbPassword"];
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Portuguese_Br;

                oCompany.UserName = ConfigurationManager.AppSettings["SapUser"];
                oCompany.Password = ConfigurationManager.AppSettings["SapPassword"];

                long con = oCompany.Connect();

                if (con != 0)
                {
                    //logar erro
                    string erro = oCompany.GetLastErrorDescription();
                    log.WriteLogPedido("Erro ao conectar ao SAP. Msg erro: "+erro);
                    return oCompany;
                }

                log.WriteLogPedido("Conectado ao SAP.");
                return oCompany;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}