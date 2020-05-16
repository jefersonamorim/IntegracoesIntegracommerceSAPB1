using Integracommerce.Business;
using Integracommerce.DAL;
using Integracommerce.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Integracommerce
{
    public partial class Schedule : ServiceBase
    {
        private Timer timerEstoque = null;

        private Timer timerPedidos = null;

        private Timer timerRetNF = null;

        //private Timer timerCancelPedido = null;

        private string _path = System.Configuration.ConfigurationManager.AppSettings["Path"];

        private Boolean jobIntegracaoEstoque = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["jobIntegracaoEstoque"]);

        private Boolean jobIntegracaoPedido = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["jobIntegracaoPedido"]);

        private Boolean jobIntegracaoRetNF = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["jobIntegracaoRetornoNF"]);

        private Boolean jobIntegracaoCancelPedido = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["jobIntegracaoCancelPedido"]);

        private Log log;

        private SAPbobsCOM.Company oCompany;

        private string userApi = System.Configuration.ConfigurationManager.AppSettings["user"];

        private string passwordApi = System.Configuration.ConfigurationManager.AppSettings["password"];

        public Schedule()

        {
            this.log = new Log();
            this.oCompany = CommonConn.InitializeCompany();
            InitializeComponent();

            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["authorization"].Value = BaseCripto.Base64Encode(userApi + ":" + passwordApi);

            config.Save(ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        public void Teste()
        {
            //this.timerEstoque.Interval = TimeSpan.FromHours(Convert.ToDouble(intervaloExecucaoEstoque)).TotalMilliseconds;

            IntegracaoService integracaoService = new IntegracaoService();
            //integracaoService.IniciarIntegracaoCancelamentoPedido(oCompany);
            //integracaoService.IniciarIntegracaoEstoque(this.oCompany);
            integracaoService.RetornoNotaFiscal(this.oCompany);
            //integracaoService.IniciarIntegracaoPedido(this.oCompany);
            
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                if (jobIntegracaoPedido)
                {
                    this.timerPedidos = new Timer();

                    string intervaloExecucaoPedido = System.Configuration.ConfigurationManager.AppSettings["intervaloExecucaoPedido"];

                    this.timerPedidos.Interval = Convert.ToInt32(intervaloExecucaoPedido);

                    timerPedidos.Enabled = true;

                    this.timerPedidos.Elapsed += new System.Timers.ElapsedEventHandler(this.IntegracaoPedido);
                }

                if (jobIntegracaoEstoque)
                {
                    this.timerEstoque = new Timer();

                    string intervaloExecucaoEstoque = System.Configuration.ConfigurationManager.AppSettings["intervaloExecucaoEstoque"] + ",01";
                    //double teste = Convert.ToDouble(intervaloExecucaoEstoque);
                    //this.timerEstoque.Interval = Convert.ToDouble(intervaloExecucaoEstoque);
                    this.timerEstoque.Interval = TimeSpan.FromHours(Convert.ToDouble(intervaloExecucaoEstoque)).TotalMilliseconds;

                    timerEstoque.Enabled = true;

                    this.timerEstoque.Elapsed += new System.Timers.ElapsedEventHandler(this.IntegracaoEstoque);

                }

                if (jobIntegracaoRetNF)
                {
                    this.timerRetNF = new Timer();

                    string intervaloExecucaoRetNF = System.Configuration.ConfigurationManager.AppSettings["intervaloExecucaoRetNF"];

                    this.timerRetNF.Interval = Convert.ToInt32(intervaloExecucaoRetNF);

                    timerRetNF.Enabled = true;

                    this.timerRetNF.Elapsed += new System.Timers.ElapsedEventHandler(this.IntegracaoRetornoNF);

                }

                #region demais integracoes
                /*
                
                
                if (jobIntegracaoCancelPedido)
                {
                    this.timerCancelPedido = new Timer();

                    string intervaloExecucaoCancelPedido = ConfigurationManager.AppSettings["intervaloExecucaoCancelPedido"];

                    this.timerCancelPedido.Interval = Convert.ToInt32(intervaloExecucaoCancelPedido);

                    timerCancelPedido.Enabled = true;

                    this.timerCancelPedido.Elapsed += new System.Timers.ElapsedEventHandler(this.IntegracaoCancelPedido);
                }*/
                #endregion

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void IntegracaoPedido(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerPedidos.Enabled = false;
                timerPedidos.AutoReset = false;

                this.log.WriteLogPedido("#### INTEGRAÇÃO DE PEDIDOS INICIALIZADA");

                IntegracaoService integracaoService = new IntegracaoService();

                integracaoService.IniciarIntegracaoPedido(this.oCompany);

                timerPedidos.Enabled = true;

                //System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompany);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.log.WriteLogPedido("Exception IntegracaoPedido " + ex.Message);
            }
        }

        private void IntegracaoEstoque(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerEstoque.Enabled = false;
                timerEstoque.AutoReset = false;

                this.log.WriteLogEstoque("#### INTEGRAÇÃO DE ESTOQUE INICIALIZADA");

                IntegracaoService integracaoService = new IntegracaoService();

                integracaoService.IniciarIntegracaoEstoque(this.oCompany);

                timerEstoque.Enabled = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

            }
            catch (Exception ex)
            {
                this.log.WriteLogEstoque("Exception IntegracaoEstoque " + ex.Message);
                throw;
            }
        }

        private void IntegracaoRetornoNF(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerRetNF.Enabled = false;
                timerRetNF.AutoReset = false;

                this.log.WriteLogRetornoNF("#### INTEGRAÇÃO RETORNO NF INICIALIZADA");

                IntegracaoService integracaoService = new IntegracaoService();

                integracaoService.RetornoNotaFiscal(this.oCompany);

                timerRetNF.Enabled = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.log.WriteLogRetornoNF("Exception IntegracaoRetornoNF " + ex.Message);
                throw;
            }
        }

        #region demais integracoes 
        /*
        
        

        private void IntegracaoCancelPedido(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerCancelPedido.Enabled = false;
                timerCancelPedido.AutoReset = false;

                this.log.WriteLogPedido("#### INTEGRAÇÃO CANCELAMENTO DE PEDIDO INICIADA");

                IntegracaoService integracaoService = new IntegracaoService();

                integracaoService.IniciarIntegracaoCancelamentoPedido(this.oCompany);

                timerCancelPedido.Enabled = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.log.WriteLogPedido("Exception IntegracaoCancelamento " + ex.Message);
                throw;
            }
        }
        */
        #endregion

        protected override void OnStop()
        {
            /*timerPedidos.Stop();
            this.log.WriteLogPedido("#### INTEGRAÇÃO DE PEDIDOS ENCERRADA");

            timerEstoque.Stop();
            this.log.WriteLogEstoque("#### INTEGRAÇÃO DE ESTOQUE ENCERRADA");*/
        }

    }
}
