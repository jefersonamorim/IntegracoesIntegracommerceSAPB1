using Integracommerce.DAL;
using Integracommerce.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.DAL
{
    public class CountyDAL
    {
        private SAPbobsCOM.Company oCompany;

        public string RecuperarCodigoMunicipio(string municipio, SAPbobsCOM.Company company)
        {

            Log log = new Log();

            string _query = string.Empty;

            try
            {
                this.oCompany = company;

                if (this.oCompany.Connected)
                {
                    SAPbobsCOM.Recordset recordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                    if (municipio.Contains("'"))
                    {
                        municipio = municipio.Replace("'", "''");
                    }

                    _query = string.Format("SELECT OC.AbsId FROM OCNT OC WHERE OC.Name = '{0}'", municipio);

                    recordSet.DoQuery(_query);

                    //Log.WriteLog("Query: "+_query);

                    if (recordSet.RecordCount > 0)
                    {
                        string absid =  recordSet.Fields.Item("AbsId").Value.ToString();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);

                        return absid;
                    }

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);
                }

                //CommonConn.FinalizeCompany();

            }
            catch (Exception e)
            {
               log.WriteLogPedido("Exception RecuperarCodigoMunicipio " + e.Message);
                throw;
            }
            return null;

        }
    }
}