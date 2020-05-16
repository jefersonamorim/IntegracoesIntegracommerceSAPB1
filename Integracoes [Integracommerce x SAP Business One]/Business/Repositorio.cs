using Integracommerce.Entity;
using Integracommerce.Service;
using Integracommerce.Util;
using Newtonsoft.Json;
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
    class Repositorio : BaseService
    {
        //Método responsável por buscar item por SKU
        public async Task<HttpResponseMessage> BuscarItemPorSKU(string _itemCode, SAPbobsCOM.Company oCompany)
        {
            Log log = new Log();
            try
            {
                //log.WriteLogEstoque("Buscando Item VTEX por ManufacturerCode - Código Item SAP: "+_itemCode);

                string uri = "api/Sku/" + _itemCode;

                //Log.WriteLog("URI: " + uri);

                HttpResponseMessage response = await BuildClient().GetAsync(uri);

                return response;
                
            }
            catch (HttpRequestException e)
            {
                log.WriteLogEstoque("Exception BuscarItemPorSKU "+e.InnerException.Message);
                //throw;
            }
            return null;
        }

        //Método responsável por Atualizar quantidade em estoque por produto
        public async Task<HttpResponseMessage> AtualizarQuantidadeEstoque(string _skuId, int _onHand) {
            Log log = new Log();
            try
            {
                log.WriteLogEstoque("Atualizando quantidade em estoque para o Item "+_skuId);

                UpdateInventory updateInventory = new UpdateInventory();
                List<Skus> listSku = new List<Skus>();
                Skus sku = new Skus();

                sku.IdSku = _skuId;
                sku.Quantity = _onHand;

                listSku.Add(sku);

                //updateInventory.SkuList = listSku;

                string jsonUpdateInventory = JsonUtil.ConvertToJsonString(listSku);

                string uri = "api/Stock";

                HttpResponseMessage response = await BuildClient().PutAsync(uri, new StringContent(jsonUpdateInventory, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogEstoque("Exception AtualizarQuantidadeEstoque " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por consultar o Feed de Eventos de Pedidos
        public async Task<HttpResponseMessage> ConsultarFilaDeEventos() {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Consultando OrderQueue");

                string _paramOrderQueue = "?Status=APPROVED";

                string _uriOrderQueue = "api/OrderQueue"+ _paramOrderQueue;

                HttpResponseMessage responseFeed = await BuildClientPedido().GetAsync(_uriOrderQueue);

                return responseFeed;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception ConsultarFilaDeEventos "+e.InnerException.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PedidosACancelar()
        {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Consultando Pedidos Pendentes");

                string _param = "?orderBy=creationDate,asc&per_page=30&f_status=payment-pending&f_paymentNames=Boleto Bancário";

                string uri = "api/oms/pvt/orders" + _param;

                HttpResponseMessage responseOrderFiltered = await BuildClientPedido().GetAsync(uri);

                return responseOrderFiltered;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception PedidosACancelar " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por remover Pedido da fila de Eventos.
        public async Task<HttpResponseMessage> AtualizaFilaEnvetoPedido(int idOrderQueue)
        {
            Log log = new Log();
            try
            {
                string postContent = "[{\"Id\":" + idOrderQueue + "}]";

                //Handle handle = new Handle();
                //handle.insertHandle(_handle);
                //var jsonUpdate = JsonConvert.SerializeObject(handle);

                string uri = "/api/OrderQueue";

                HttpResponseMessage response = await BuildClientPedido().PutAsync(uri, new StringContent(postContent, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception AtualizaFilaEnvetoPedido " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por recuperar Pedido Vtex
        public async Task<HttpResponseMessage> BuscarPedido(string orderId){
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Recuperando pedido "+orderId);

                string uriOrder = "api/Order/"+orderId;

                HttpResponseMessage responseOrder = await BuildClientPedido().GetAsync(uriOrder);

                return responseOrder;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception BuscarPedido " +e.InnerException.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> CancelarPedido(string orderId)
        {
            Log log = new Log();
            try
            {
                //log.WriteLogPedido("Buscando Pedido " + orderId);

                string uriOrder = "api/oms/pvt/orders/" + orderId + "/cancel";

                HttpResponseMessage responseOrder = await BuildClientPedido().PostAsync(uriOrder,null);

                return responseOrder;
            }
            catch (HttpRequestException e)
            {
                //log.WriteLogPedido("Exception dido " + e.InnerException.Message);
                throw e;
            }
        }

        public async Task<HttpResponseMessage> RetornoNotaFiscal(Invoice invoice) {
            Log log = new Log();
            try
            {
                log.WriteLogRetornoNF("Fazendo Post número NF");

                string uri = "api/Order";

                var jsonInvoice = JsonConvert.SerializeObject(invoice);

                HttpResponseMessage response = await BuildClient().PutAsync(uri, new StringContent(jsonInvoice, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogRetornoNF("Exception RetornoNotaFiscal "+e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por Iniciar Manuseio pedido
        public async Task<HttpResponseMessage> AlterarStatusPedido(string _orderId)
        {
            Log log = new Log();

            try
            {
                log.WriteLogPedido("Setando pedido para PROCESSING: "+_orderId);

                string uri = "/api/Order";

                string postContent = "{\"OrderStatus\":\"PROCESSING\",\"IdOrder\":\""+_orderId+"\"}";

                HttpResponseMessage response = await BuildClientPedido().PutAsync(uri, new StringContent(postContent, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception AtualizaFilaEnvetoPedido " + e.InnerException.Message);
                throw;
            }
        }
    }
}
