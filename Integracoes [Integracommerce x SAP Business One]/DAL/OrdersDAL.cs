using Integracommerce.Business;
using Integracommerce.Entity;
using Integracommerce.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.DAL
{
    public class OrdersDAL
    {
        private SAPbobsCOM.Company oCompany;

        private Log log;
        internal OrdersDAL(SAPbobsCOM.Company company) {
            this.oCompany = company;
        }

        public int InsertOrder(OrderIntegraCommerce pedido, out string messageError) {
            this.log = new Log();
            try
            {
                int oOrderNum = 0;

                log.WriteLogPedido("Inserindo Pedido de Venda "+pedido.IdOrder);

                SAPbobsCOM.Documents oOrder = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

                int filial = Convert.ToInt32(ConfigurationManager.AppSettings["Empresa"]);
                string usage = ConfigurationManager.AppSettings["Usage"];
                string WhsCode = ConfigurationManager.AppSettings["WhsCode"];
                int SlpCode = Convert.ToInt32(ConfigurationManager.AppSettings["SlpCode"]);
                string comments = ConfigurationManager.AppSettings["Comments"];
                string plataforma = ConfigurationManager.AppSettings["Plataforma"];
                string carrier = ConfigurationManager.AppSettings["Carrier"];
                string packDesc = ConfigurationManager.AppSettings["PackDesc"];
                int qoP = Convert.ToInt32(ConfigurationManager.AppSettings["QoP"]);
                int expnsCode = Convert.ToInt32(ConfigurationManager.AppSettings["ExpnsCode"]);
                string expnsTax = ConfigurationManager.AppSettings["ExpnsTax"];
                string cardCodePrefix = ConfigurationManager.AppSettings["CardCodePrefix"];
                string pickRemark = ConfigurationManager.AppSettings["PickRemark"];
                string document = String.Empty;

                oOrder.BPL_IDAssignedToInvoice = filial;
                oOrder.NumAtCard = pedido.IdOrder;
                oOrder.SalesPersonCode = SlpCode;
                oOrder.Comments = comments;
                oOrder.UserFields.Fields.Item("U_PLATF").Value = plataforma;
                oOrder.UserFields.Fields.Item("U_NumPedEXT").Value = pedido.IdOrder;
                oOrder.TaxExtension.Carrier = carrier;
                oOrder.TaxExtension.PackDescription = packDesc;
                oOrder.TaxExtension.PackQuantity = qoP;
                oOrder.Expenses.ExpenseCode = expnsCode;
                oOrder.Expenses.TaxCode = expnsTax;

                if (!String.IsNullOrEmpty(pedido.CustomerPjCnpj))
                {
                    document = pedido.CustomerPjCnpj;
                }
                else if (!String.IsNullOrEmpty(pedido.CustomerPfCpf))
                {
                    document = pedido.CustomerPfCpf;
                }

                oOrder.CardCode = cardCodePrefix + document;

                if (!string.IsNullOrEmpty(pedido.ShippedEstimatedDelivery))
                {
                    oOrder.DocDueDate = DateTime.Parse(pedido.ShippedEstimatedDelivery);
                }
                else
                {
                    oOrder.DocDueDate = DateTime.Today.AddDays(5);
                }

                if (!String.IsNullOrEmpty(pedido.ShippedCarrierName))
                {
                    oOrder.PickRemark = pedido.ShippedCarrierName;
                }

                /*
                double _valorFrete = 0.00;
                double _valorDescont = 0.00;
                double _valorTaxa = 0.00;

                despesas adicionais
                if (pedido.totals.Length > 0)
                {
                    foreach (Total total in pedido.totals)
                    {
                        if (total.id.Equals("Discounts"))
                        {
                            if (total.value != 0)
                            {
                                _valorDescont = Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                            }
                        }
                        if (total.id.Equals("Shipping"))
                        {
                            if (total.value != 0)
                            {
                                _valorFrete = Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                            }
                        }
                        if (total.id.Equals("Tax"))
                        {
                            if (total.value != 0)
                            {
                                _valorTaxa = Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                            }
                        }
                    }
                }
                oOrder.Expenses.LineGross = _valorFrete;
                 */

                //DocumentLines
                if (pedido.Products.Length > 0)
                {
                    
                    //_valorFrete.ToString().Insert(1,".");
                    int _lineNum = 0;

                    foreach (Product item in pedido.Products)
                    {
                        if (!String.IsNullOrEmpty(item.IdSku))
                        {
                            //Recuperar Item

                            Repositorio repositorio = new Repositorio();

                            Task<HttpResponseMessage> responseSku = repositorio.BuscarItemPorSKU(item.IdSku, this.oCompany);

                            if (responseSku.Result.IsSuccessStatusCode)
                            {
                                string jsonResponseSku = responseSku.Result.Content.ReadAsStringAsync().Result;

                                var itemResponseSku = JsonConvert.DeserializeObject<Item>(jsonResponseSku);

                                if (!String.IsNullOrEmpty(itemResponseSku.IdSkuErp))
                                {
                                    oOrder.Lines.ItemCode = itemResponseSku.IdSkuErp;
                                }
                            }

                            oOrder.Lines.Quantity = item.Quantity;
                            oOrder.Lines.WarehouseCode = WhsCode;
                            oOrder.Lines.Usage = usage;
                            oOrder.Lines.SetCurrentLine(_lineNum);
                            oOrder.Lines.Add();
                        }

                        _lineNum++;
                    }
                }

                oOrderNum = oOrder.Add();

                if (oOrderNum != 0)
                {
                    messageError = oCompany.GetLastErrorDescription();
                    log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.IdOrder, "", EnumStatusIntegracao.Erro, messageError);
                    log.WriteLogPedido("InsertOrder error SAP: " + messageError);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oOrder);
                    return oOrderNum;
                }
                else
                {
                    messageError = "";
                    string docNum = oCompany.GetNewObjectKey();
                    log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.IdOrder, docNum, EnumStatusIntegracao.Sucesso, "Pedido de venda inserido com sucesso.");
                    log.WriteLogPedido("Pedido de venda inserido com sucesso.");
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oOrder);
                    return oOrderNum;
                }


            }
            catch (Exception e)
            {
                log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.IdOrder, "", EnumStatusIntegracao.Erro, e.Message);
                log.WriteLogPedido("Excpetion InsertOrder. "+e.Message);

                throw;
            }
        }

        public SAPbobsCOM.Recordset RecuperarNumeroNF()
        {
            string _query = string.Empty;

            //string whsCode = ConfigurationManager.AppSettings["WhsCode"];
            SAPbobsCOM.Recordset recordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                //this.oCompany = CommonConn.InitializeCompany();

                if (this.oCompany.Connected)
                {
                    recordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                    _query = string.Format("SELECT " +
                            "T0.DocNum AS docNPIntegraCommerce " +
                            ",T0.NumAtCard AS idOrderIntegraCommerce " +
                            ", T0.U_NumPedEXT AS idOrderIntegraCommerce2 " +
                            ",T2.DocEntry AS externalId " +
                            ",T2.DocNum	AS docSAP " +
                            ",T2.Serial AS invoiceNumber " +
                            ",T2.DocDate AS invoiceDate " +
                            ",T3.KeyNfe AS nfeKey " +
                            ",T0.PickRmrk AS shippingMethod " +
                            ",T2.SeriesStr AS invoiceOrderSeries " +
                            ",T1.ItemCode AS codItem " +
                            ",T1.Price AS precoItem " +
                            ",T1.Quantity AS qtdItem " +
                            ",T0.DocTotal AS totalNF " +
                            "FROM    ORDR T0 " +
                            "INNER JOIN INV1 T1 ON T0.DocEntry = T1.BaseEntry  " +
                            "INNER JOIN OINV T2 ON T1.DocEntry = T2.DocEntry and T0.BPLId = T2.BPLId  " +
                            "INNER JOIN [DBInvOne].[dbo].[Process] T3 on T3.DocEntry = T2.DocEntry " +
                            "WHERE	T0.U_PLATF = '{0}' " +
                            "AND    T2.U_EnvioNFIC IS NULL", ConfigurationManager.AppSettings["Plataforma"]);

                    recordSet.DoQuery(_query);

                    //Log.WriteLog("Query: "+_query);

                    if (recordSet.RecordCount > 0)
                    {

                        return recordSet;
                    }
                }

                //CommonConn.FinalizeCompany();

            }
            catch (Exception e)
            {
                this.log = new Log();
                this.log.WriteLogEstoque("Exception recuperarSaldoEstoqueSAP " + e.Message);
                throw;
            }

            return recordSet;
        }

        public int AtualizarPedidoVenda(SAPbobsCOM.Company company, int docEntry) {
            this.log = new Log();
            try
            {
                this.oCompany = company;

                log.WriteLogRetornoNF("Atualizando Pedido de Venda - NF enviada p/ IntegraCommerce");

                SAPbobsCOM.Documents oInvoice = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                if (oInvoice.GetByKey(docEntry))
                {
                    //SAPbobsCOM.Documents oOrderUpdate = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    //oOrderUpdate = oOrder;

                    oInvoice.UserFields.Fields.Item("U_EnvioNFIC").Value = "S";

                    int updateOrderNum = oInvoice.Update();

                    if (updateOrderNum != 0)
                    {
                        string messageError = oCompany.GetLastErrorDescription();
                        log.WriteLogRetornoNF("AtualizarPedidoVenda error SAP: " + messageError);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);
                        return 1;
                    }
                    else
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);
                        return 0;
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                return 1;
                throw;
            }
        }
    }
}
