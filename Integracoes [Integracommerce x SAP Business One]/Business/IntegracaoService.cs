using Integracommerce.DAL;
using Integracommerce.Entity;
using Integracommerce.Util;
using Newtonsoft.Json;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integracommerce.Business
{
    public class IntegracaoService
    {
        private Log log;

        public IntegracaoService() {
            this.log = new Log();
        }

        public void IniciarIntegracaoPedido(SAPbobsCOM.Company oCompany)
        {
            try
            {
                Repositorio repositorioPedido = new Repositorio();
                List<OrderQueues> listaOrders = new List<OrderQueues>();
                OrderIntegraCommerce _orderIntegraCommerce = new OrderIntegraCommerce();
                String document = String.Empty;

                Task<HttpResponseMessage> responsePedido = repositorioPedido.ConsultarFilaDeEventos();

                if (responsePedido.Result.IsSuccessStatusCode)
                {
                    var jsonResponseOrderQueue = responsePedido.Result.Content.ReadAsStringAsync().Result;

                    var _orderQueue = JsonConvert.DeserializeObject<OrderQueue>(jsonResponseOrderQueue);

                    listaOrders = _orderQueue.OrderQueues;

                    if (listaOrders.Count > 0)
                    {
                        string pedidosARemover = string.Empty;
                        string pedidosARemoverML = string.Empty;
                        //Validando evento do pedido
                        foreach (OrderQueues order in listaOrders)
                        {
                            //Se o evento do pedido for Pronto para Manuseio (ready-for-handling)
                            if (order.OrderStatus.Equals("APPROVED"))
                            {
                                Task<HttpResponseMessage> responseOrder = repositorioPedido.BuscarPedido(order.IdOrder);

                                if (responseOrder.Result.IsSuccessStatusCode)
                                {
                                    var jsonPedido = responseOrder.Result.Content.ReadAsStringAsync().Result;

                                    _orderIntegraCommerce = JsonConvert.DeserializeObject<OrderIntegraCommerce>(jsonPedido);

                                    if (!_orderIntegraCommerce.OrderStatus.Equals("PROCESSING"))
                                    {
                                        if (!String.IsNullOrEmpty(_orderIntegraCommerce.CustomerPfCpf))
                                        {
                                            document = _orderIntegraCommerce.CustomerPfCpf;
                                        }
                                        else
                                        {
                                            document = _orderIntegraCommerce.CustomerPjCnpj;
                                        }

                                        if (document != null)
                                        {
                                            this.InserirClientes(oCompany, _orderIntegraCommerce);
                                        }
                                        else
                                        {
                                            this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, "", EnumStatusIntegracao.Erro, "Cliente não cadastrado pois o número do Documento é inválido.");

                                        }

                                        //Inserir Pedido de venda
                                        //this.InserirPedidoVenda(oCompany, _orderIntegraCommerce, order);
                                        this.InserirPedidoVenda(oCompany, _orderIntegraCommerce, order.Id);
                                        /*if (_orderIntegraCommerce.origin.Equals("Fulfillment"))
                                        {
                                            Cliente clienteMkt = new Cliente();
                                            Endereco enderecoMkt = new Endereco();

                                            this.InserirClientes(oCompany, clienteMkt, enderecoMkt, _orderIntegraCommerce);
                                        }*/
                                        DateTime diaAnterior = DateTime.Now.AddDays(-1);
                                        /*if (_orderIntegraCommerce.OrderStatus.Equals("APPROVED") && _orderIntegraCommerce.PurchasedDate.CompareTo(diaAnterior) == 1)
                                        {

                                        }
                                        else {
                                            pedidosARemover += order.Id + ",";
                                            pedidosARemoverML += order.IdOrder + ",";
                                            continue;
                                        }*/
                                        /*if (!_orderIntegraCommerce.OrderStatus.Equals("APPROVED") || _orderIntegraCommerce.PurchasedDate.Day >= diaAnterior && !_orderIntegraCommerce.PurchasedDate.Year.ToString().Equals(DateTime.Now.Year.ToString()))
                                        {
                                            pedidosARemover += order.Id + ",";
                                            pedidosARemoverML += order.IdOrder + ",";
                                            continue;
                                        }*/
                                    }


                                }
                            }
                        }
                        //this.log.WriteLogPedido(pedidosARemover);
                        //this.log.WriteLogPedido(pedidosARemoverML);
                    }
                }
                else
                {
                    this.log.WriteLogPedido("Não foi possível consultar OrderQueue IntegraCommerce" + responsePedido.Result.ReasonPhrase);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

            }
            catch (Exception e)
            {
                this.log.WriteLogPedido("Exception IniciarIntegracaoPedido " + e.Message);
                throw;
            }
        }

        private void InserirClientes(SAPbobsCOM.Company company, OrderIntegraCommerce pedido)
        {
            try
            {
                BusinessPartnersDAL bpDAL = new BusinessPartnersDAL();

                string errorMessage;

                bpDAL.InserirBusinessPartner(company, pedido, out errorMessage);
            }
            catch (Exception e)
            {
                this.log.WriteLogPedido("Exception inserirClientes " + e.Message);
                throw;
            }
        }

        private int InserirPedidoVenda(SAPbobsCOM.Company oCompany, OrderIntegraCommerce pedido, int idOrderQueue)
        {
            try
            {
                if (oCompany.Connected)
                {
                    OrdersDAL order = new OrdersDAL(oCompany);
                    string messageError = "";
                    int oOrderNum = 0;
                    Boolean inserir = true;

                    /*foreach (ItemVtex item in pedido.items)
                    {
                        if (item.refId == null && inserir)
                        {
                            this.log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, "", EnumStatusIntegracao.Erro, "Um ou mais item(s) do pedido está com o código de referência inválido.");
                            //throw new ArgumentException("Não foi possível criar o Pedido de Venda para o pedido "+pedidoVtex.orderId+" pois um ou mais item(s) do pedido está com o código de referência inválido.");
                            inserir = false;
                        }
                    }*/

                    if (inserir)
                    {
                        oOrderNum = order.InsertOrder(pedido, out messageError);

                        if (oOrderNum == 0)
                        {
                            Repositorio repositorio = new Repositorio();

                            //Pedido inserido no SAP, removendo pedido da fila de enventos(Feed), para não ser mais processado.

                            Task<HttpResponseMessage> response = repositorio.AtualizaFilaEnvetoPedido(idOrderQueue);

                            if (response.Result.IsSuccessStatusCode)
                            {
                                this.log.WriteLogPedido("Pedido " + pedido.IdOrder + " removido de OrderQueue");
                            }
                            else
                            {
                                this.log.WriteLogPedido("Não foi possível remover o pedido " + pedido.IdOrder + " de OrderQueue" + response.Result.ReasonPhrase);
                            }

                            //Alterar status para Processing
                            if (!pedido.OrderStatus.Equals("INVOICED"))
                            {
                                Task<HttpResponseMessage> responseIniciarManuseio = repositorio.AlterarStatusPedido(pedido.IdOrder);

                                if (responseIniciarManuseio.Result.IsSuccessStatusCode)
                                {
                                    this.log.WriteLogPedido("Alterado status do pedido "+pedido.IdOrder+" para Iniciar PROCESSING.");

                                }
                                else
                                {
                                    this.log.WriteLogPedido("Não foi possível alterar o status do pedido " + pedido.IdOrder + " para PROCESSING." + response.Result.ReasonPhrase);
                                }
                            }
                            
                        }
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                this.log.WriteLogPedido("Exception InserirPedidoVenda " + e.Message);
                throw;
            }
        }

        public void IniciarIntegracaoEstoque(SAPbobsCOM.Company oCompany)
        {
            string numItemTeste = string.Empty;
            try
            {
                Repositorio repositorio = new Repositorio();

                this.log.WriteLogEstoque("Inicio do Processo de Integração de Estoque");

                WarehouseDAL whsDAL = new WarehouseDAL();

                SAPbobsCOM.Recordset recordset = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                recordset = whsDAL.RecuperarSaldoEstoqueSAP(oCompany);

                

                if (recordset != null && recordset.RecordCount > 0)
                {
                    for (int i = 0; i < recordset.RecordCount; i++)
                    {

                        try
                        {
                            string _itemCode = recordset.Fields.Item("ItemCode").Value.ToString();
                            Int16 _onHand = System.Convert.ToInt16(recordset.Fields.Item("OnHand").Value.ToString());
                            string warehouseId = ConfigurationManager.AppSettings["warehouseId"];
                            numItemTeste = _itemCode;
                            Task<HttpResponseMessage> response = repositorio.BuscarItemPorSKU(_itemCode, oCompany);

                            if (response.Result.IsSuccessStatusCode)
                            {
                                Item item = new Item();

                                var jsonResponse = response.Result.Content.ReadAsStringAsync();

                                item = JsonConvert.DeserializeObject<Item>(jsonResponse.Result);

                                //Log.WriteLog("Item " + item.ManufacturerCode + " localizado.");

                                if (item.Status)
                                {
                                    Task<HttpResponseMessage> responseAtualizacaoEstoque = repositorio.AtualizarQuantidadeEstoque(_itemCode, _onHand);

                                    if (responseAtualizacaoEstoque.Result.IsSuccessStatusCode)
                                    {
                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, _itemCode, _itemCode, EnumStatusIntegracao.Sucesso, "Estoque atualizado com sucesso.");
                                        this.log.WriteLogEstoque("Quantidade de estoque do Produto " + _itemCode + " para o depósito " + warehouseId + " atualizada com sucesso.");
                                    }
                                    else {
                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, _itemCode, _itemCode, EnumStatusIntegracao.Erro, response.Result.ReasonPhrase);
                                        this.log.WriteLogEstoque("Não foi possível atualizar a quantidade de estoque para o produto " + _itemCode + ". Retorno API IntegraCommerce: " + response.Result.ReasonPhrase);
                                        
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        
                        recordset.MoveNext();

                    }
                    
                }

                if (recordset != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(recordset);
                }

            }
            catch (Exception e)
            {

                this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, "", "", EnumStatusIntegracao.Erro, e.Message+numItemTeste);
                this.log.WriteLogEstoque("Exception IniciarProcessoEstoque "+ e.Message);
                throw;
            }
        }

        public void RetornoNotaFiscal(SAPbobsCOM.Company oCompany)
        {
            try
            {
                if (oCompany.Connected)
                {
                    OrdersDAL orders = new OrdersDAL(oCompany);

                    SAPbobsCOM.Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    recordSet = orders.RecuperarNumeroNF();

                    if (recordSet != null && recordSet.RecordCount > 0)
                    {
                        for (int i = 0; i < recordSet.RecordCount; i++)
                        {
                            Repositorio repositorio = new Repositorio();
                            Invoice invoice = new Invoice();

                            invoice.InvoicedIssueDate = String.Concat(recordSet.Fields.Item("invoiceDate").Value.ToString());
                            //invoice.InvoicedIssueDate = String.Concat(recordSet.Fields.Item("invoiceDate").Value.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ","T"), "Z");
                            invoice.InvoicedNumber = recordSet.Fields.Item("invoiceNumber").Value.ToString();
                            invoice.InvoicedKey = recordSet.Fields.Item("nfeKey").Value.ToString();
                            invoice.OrderStatus = "INVOICED";
                            invoice.InvoicedLine = 0;
                            string externalId = recordSet.Fields.Item("externalId").Value.ToString();
                            string idOrderIntegraCommerce = recordSet.Fields.Item("idOrderIntegraCommerce").Value.ToString();
                            string idOrderIntegraCommerce2 = recordSet.Fields.Item("idOrderIntegraCommerce2").Value.ToString();
                            string docSAP = recordSet.Fields.Item("docSAP").Value.ToString();
                            string docNPV = recordSet.Fields.Item("docNPIntegraCommerce").Value.ToString();
                            
                            //invoice.invoiceValue = recordSet.Fields.Item("totalNF").Value.ToString().Replace(",","");
                            //invoice.courier = recordSet.Fields.Item("shippingMethod").Value.ToString();

                            int updatePedidoNum = 0;

                            if (!string.IsNullOrEmpty(idOrderIntegraCommerce))
                            {
                                invoice.IdOrder = idOrderIntegraCommerce;
                            }
                            else if (!string.IsNullOrEmpty(idOrderIntegraCommerce2))
                            {
                                invoice.IdOrder = idOrderIntegraCommerce2;
                            }

                            Task<HttpResponseMessage> responseOrder = repositorio.BuscarPedido(idOrderIntegraCommerce);

                            if (responseOrder.Result.IsSuccessStatusCode)
                            {
                                var jsonOrder = responseOrder.Result.Content.ReadAsStringAsync().Result;

                                var _order = JsonConvert.DeserializeObject<OrderIntegraCommerce>(jsonOrder);


                                //Para validar para enviar só se o pedido não estiver cancelado
                                if (!_order.OrderStatus.Equals("CANCELED"))
                                {
                                    
                                }

                                if (!string.IsNullOrEmpty(invoice.IdOrder))
                                {

                                    Task<HttpResponseMessage> response = repositorio.RetornoNotaFiscal(invoice);

                                    if (response.Result.IsSuccessStatusCode)
                                    {
                                        //Atualizando campo de usuário U_EnvioNFVTEX
                                        updatePedidoNum = orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId));

                                        if (updatePedidoNum == 0)
                                        {
                                            this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, docSAP, EnumStatusIntegracao.Sucesso, "Número NF " + invoice.InvoicedNumber + " enviado para IntegraCommerce com sucesso.");
                                            this.log.WriteLogRetornoNF("Número NF para o Pedido de Venda " + docSAP + " enviado para IntegraCommerce com sucesso.");
                                        }
                                        else
                                        {
                                            this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, docSAP, EnumStatusIntegracao.Erro, "Número NF " + invoice.InvoicedNumber + " retornado porém não foi possivél atualizar campo de usuário (U_EnvioNFIC) do Pedido de Venda");
                                            this.log.WriteLogRetornoNF("Falha ao atualizar Pedido de Venda " + docSAP);
                                        }

                                    }
                                    else
                                    {
                                        var responseNFJson = response.Result.Content.ReadAsStringAsync().Result;
                                        var responseBody = JsonConvert.DeserializeObject<RetNFResponse>(responseNFJson);

                                        var responseMessage = string.Empty;

                                        foreach (Error err in responseBody.Errors)
                                        {

                                            if (!String.IsNullOrEmpty(err.Message))
                                            {
                                                responseMessage = err.Message;
                                            }
                                            else
                                            {
                                                responseMessage = response.Result.ReasonPhrase;
                                            }
                                        }

                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, externalId, EnumStatusIntegracao.Erro, responseMessage);
                                        this.log.WriteLogRetornoNF("Falha ao retornar número da Nota Fiscal " + externalId + " para IntegraCommerce");

                                        //Atualizando campo de usuário U_EnvioNFVTEX
                                        updatePedidoNum = orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId));

                                        if (updatePedidoNum != 0)
                                        {
                                            //this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, docSAP, EnumStatusIntegracao.Erro, "Número NF " + invoice.InvoicedNumber + " retornado porém não foi possivél atualizar campo de usuário (U_EnvioNFIntegraC) do Pedido de Venda");
                                            this.log.WriteLogRetornoNF("Falha ao atualizar Pedido de Venda " + docSAP);
                                        }
                                    }
                                }
                                else
                                {
                                    this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, externalId, EnumStatusIntegracao.Erro, "Id do Pedido IntegraCommerce (NumAtCard e U_NumPedEXT) do Pedido de Venda " + docNPV + " em branco.");
                                    this.log.WriteLogRetornoNF("Falha ao retornar número da Nota Fiscal " + externalId + " para a Vtex - Id do Pedido IntegraCommerce (NumAtCard) do Pedido de Venda " + docNPV + " em branco.");

                                    //Atualizando campo de usuário U_EnvioNFVTEX
                                    updatePedidoNum = orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId));

                                    if (updatePedidoNum != 0)
                                    {
                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, invoice.IdOrder, docSAP, EnumStatusIntegracao.Erro, "Número NF " + invoice.InvoicedNumber + " retornado porém não foi possivél atualizar campo de usuário (U_EnvioNFIntegraC) do Pedido de Venda");
                                        this.log.WriteLogRetornoNF("Falha ao atualizar Pedido de Venda " + docSAP);
                                    }
                                }
                            }

                            recordSet.MoveNext();
                        }
                    }

                    if (recordSet != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);
                    }
                }
            }
            catch (Exception e)
            {
                this.log.WriteLogRetornoNF("Exception RetornoNotaFiscal "+e.Message);
                //throw;
            }
        }

        public void IniciarIntegracaoCancelamentoPedido(SAPbobsCOM.Company oCompany)
        {
            try
            {
                //var test = Convert.ToDouble(testIn);
                Repositorio repositorioCancelPedido = new Repositorio();
                OrderFiltered orders = new OrderFiltered();

                Task<HttpResponseMessage> responseOrderFiltered = repositorioCancelPedido.PedidosACancelar();

                if (responseOrderFiltered.Result.IsSuccessStatusCode)
                {
                    var jsonListOrderFiltered = responseOrderFiltered.Result.Content.ReadAsStringAsync().Result;

                    orders = JsonConvert.DeserializeObject<OrderFiltered>(jsonListOrderFiltered);

                    if (orders.list.Length > 0)
                    {
                        foreach (List item in orders.list)
                        {
                            if (item.currencyCode.Equals("BRL") && item.status.Equals("payment-pending"))
                            {
                                //string idFormaPagmt = pedido.paymentData.transactions.ElementAt<Transaction>(0).payments.ElementAt<Payment>(0).paymentSystem;

                                TimeSpan date = DateTime.Now - item.creationDate;

                                int qtdDias = date.Days;

                                if (qtdDias > System.Convert.ToInt32(ConfigurationManager.AppSettings["qtdDiasCancelemtno"]))
                                {
                                    //cancelar pedido com mais de 3 dias
                                    Task<HttpResponseMessage> responseCacelPedido = repositorioCancelPedido.CancelarPedido(item.orderId);

                                    if (responseCacelPedido.Result.IsSuccessStatusCode)
                                    {
                                        //pedido cancelado
                                        this.log.WriteLogPedido("Pedido " + item.orderId + " cancelado com sucesso." + responseCacelPedido.Result.ReasonPhrase);
                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cancel, item.orderId,"", EnumStatusIntegracao.Sucesso, "Pedido " + item.orderId + " cancelado com sucesso.");
                                    }
                                    else
                                    {
                                        this.log.WriteLogPedido("Não foi possível cancelar pedido " + item.orderId + "." + responseCacelPedido.Result.ReasonPhrase);
                                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cancel, item.orderId, "", EnumStatusIntegracao.Erro, "Não foi possível cancelar pedido " + item.orderId + "." + responseCacelPedido.Result.ReasonPhrase);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    log.WriteLogPedido("Nenhum Pedido pendente a ser cancelado.");
                }
            }
            catch (Exception e)
            {
                this.log.WriteLogPedido("Exception IntegracaoCancelamentoPedido " + e.Message);
                throw;
            }
        }

    }
}
