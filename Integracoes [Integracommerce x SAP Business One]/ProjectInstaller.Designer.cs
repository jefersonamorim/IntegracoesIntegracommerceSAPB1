namespace Integracommerce
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Instalador = new System.ServiceProcess.ServiceProcessInstaller();
            this.IntegracommerceService = new System.ServiceProcess.ServiceInstaller();
            // 
            // Instalador
            // 
            this.Instalador.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.Instalador.Password = null;
            this.Instalador.Username = null;
            // 
            // IntegracommerceService
            // 
            this.IntegracommerceService.ServiceName = "IntegracoesIntegracommerce";
            this.IntegracommerceService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.Instalador,
            this.IntegracommerceService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller Instalador;
        private System.ServiceProcess.ServiceInstaller IntegracommerceService;
    }
}