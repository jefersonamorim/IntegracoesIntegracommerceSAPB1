using Integracommerce.Entity;
using Integracommerce.Util;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.DAL
{
    public class BusinessPartnersDAL
    {

        private SAPbobsCOM.Company oCompany;

        private Log log;

        internal BusinessPartnersDAL()
         {
            this.log = new Log();
            //this.oCompany = company;
        }

        public void InserirBusinessPartner(SAPbobsCOM.Company company, OrderIntegraCommerce pedido, out string messageError)
        {
            int addBPNumber = 0;

            string document = string.Empty;
            Boolean isCorporate = false;
            //Boolean marketPlace = false;

            if (!String.IsNullOrEmpty(pedido.CustomerPjCnpj))
            {
                document = pedido.CustomerPjCnpj;
                isCorporate = true;
            }
            else if (!String.IsNullOrEmpty(pedido.CustomerPfCpf))
            {
                document = pedido.CustomerPfCpf;
            }

            try
            {
                CountyDAL countyDAL = new CountyDAL();

                this.oCompany = company;

                int _groupCode = Convert.ToInt32(ConfigurationManager.AppSettings["GroupCode"]);
                int _splCode = Convert.ToInt32(ConfigurationManager.AppSettings["SlpCode"]);
                int _QoP = Convert.ToInt32(ConfigurationManager.AppSettings["QoP"]);
                int groupNum = Convert.ToInt32(ConfigurationManager.AppSettings["GroupNum"]);
                string indicadorIE = ConfigurationManager.AppSettings["IndicadorIE"];
                string indicadorOpConsumidor = ConfigurationManager.AppSettings["IndicadorOpConsumidor"];
                string gerente = ConfigurationManager.AppSettings["Gerente"];
                int priceList = Convert.ToInt32(ConfigurationManager.AppSettings["PriceList"]);
                string cardCodePrefix = ConfigurationManager.AppSettings["CardCodePrefix"];
                int categoriaCliente = Convert.ToInt32(ConfigurationManager.AppSettings["CategoriaCliente"]);
                
                this.log.WriteLogPedido("Inserindo Cliente " + cardCodePrefix + document);

                BusinessPartners oBusinessPartner = null;
                oBusinessPartner = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                BusinessPartners oBusinessPartnerUpdateTest = null;
                oBusinessPartnerUpdateTest = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                if (oBusinessPartnerUpdateTest.GetByKey(cardCodePrefix + document))
                {
                    oBusinessPartner = oBusinessPartnerUpdateTest;
                }

                //Setando campos padrões
                oBusinessPartner.CardCode = cardCodePrefix + document;

                if (isCorporate)
                {
                    oBusinessPartner.CardName = pedido.CustomerPjCorporatename;
                }
                else
                {
                    oBusinessPartner.CardName = pedido.CustomerPfName;
                }
                
                //oBusinessPartner.EmailAddress = cliente.email;
                
                oBusinessPartner.CardType = BoCardTypes.cCustomer;
                oBusinessPartner.GroupCode = _groupCode;
                oBusinessPartner.SalesPersonCode = _splCode;
                oBusinessPartner.PayTermsGrpCode = groupNum;
                oBusinessPartner.PriceListNum = priceList;
                //oBusinessPartner.CardForeignName = "Teste";

                //Setando campos de usuário
                oBusinessPartner.UserFields.Fields.Item("U_TX_IndIEDest").Value = indicadorIE;
                oBusinessPartner.UserFields.Fields.Item("U_TX_IndFinal").Value = indicadorOpConsumidor;
                oBusinessPartner.UserFields.Fields.Item("U_Gerente").Value = gerente;
                oBusinessPartner.UserFields.Fields.Item("U_CategoriaCliente").Value = gerente;


                //removendo o +55
                if (!String.IsNullOrEmpty(pedido.TelephoneMainNumber))
                {
                    if (pedido.TelephoneMainNumber.Length >= 9)
                    {
                        oBusinessPartner.Phone1 = pedido.TelephoneMainNumber.Substring(2);
                    }
                    else
                    {
                        //oBusinessPartner.Phone1 = cliente.homePhone.Substring(2);
                        oBusinessPartner.Phone1 = pedido.TelephoneMainNumber;
                    }
                   
                }
                else if (!String.IsNullOrEmpty(pedido.TelephoneBusinessNumber))
                {
                    if (pedido.TelephoneBusinessNumber.Length >= 9 )
                    {
                        //oBusinessPartner.Phone1 = cliente.homePhone.Substring(2);
                        oBusinessPartner.Phone1 = pedido.TelephoneBusinessNumber.Substring(2);
                    }
                    else
                    {
                        //oBusinessPartner.Phone1 = cliente.homePhone.Substring(2);
                        oBusinessPartner.Phone1 = pedido.TelephoneBusinessNumber;
                    }
                }
                if (!String.IsNullOrEmpty(pedido.TelephoneSecundaryNumber))
                {
                    oBusinessPartner.Cellular = pedido.TelephoneSecundaryNumber;
                }

                string codMunicipio = string.Empty;

                codMunicipio = countyDAL.RecuperarCodigoMunicipio(pedido.DeliveryAddressCity, this.oCompany);

                ///Inserindo endereços
                //COBRANÇA
                oBusinessPartner.Addresses.SetCurrentLine(0);
                oBusinessPartner.Addresses.AddressType = BoAddressType.bo_BillTo;
                oBusinessPartner.Addresses.AddressName = "COBRANCA";

                oBusinessPartner.Addresses.City = pedido.DeliveryAddressCity;

                if (!String.IsNullOrEmpty(pedido.DeliveryAddressAdditionalInfo) && pedido.DeliveryAddressAdditionalInfo.Length <= 100)
                {
                    oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressAdditionalInfo;
                }
                else if (!String.IsNullOrEmpty(pedido.DeliveryAddressReference) && pedido.DeliveryAddressReference.Length <= 100)
                {
                    oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressReference;
                }

                //oBusinessPartner.Addresses.Country = "1058";
                oBusinessPartner.Addresses.Block = pedido.DeliveryAddressNeighborhood;
                oBusinessPartner.Addresses.StreetNo = pedido.DeliveryAddressNumber;
                oBusinessPartner.Addresses.ZipCode = pedido.DeliveryAddressZipcode;
                oBusinessPartner.Addresses.State = pedido.DeliveryAddressState;
                oBusinessPartner.Addresses.Street = pedido.DeliveryAddressStreet;
                oBusinessPartner.Addresses.County = codMunicipio;
                //oBusinessPartner.Addresses.Country = "br";

                //FATURAMENTO
                oBusinessPartner.Addresses.SetCurrentLine(1);
                oBusinessPartner.Addresses.AddressType = BoAddressType.bo_ShipTo;
                oBusinessPartner.Addresses.AddressName = "FATURAMENTO";

                oBusinessPartner.Addresses.City = pedido.DeliveryAddressCity;

                if (!String.IsNullOrEmpty(pedido.DeliveryAddressAdditionalInfo) && pedido.DeliveryAddressAdditionalInfo.Length <= 100)
                {
                    oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressAdditionalInfo;
                }
                else if (!String.IsNullOrEmpty(pedido.DeliveryAddressReference) && pedido.DeliveryAddressReference.Length <= 100)
                {
                    oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressReference;
                }

                //oBusinessPartner.Addresses.Country = "1058";
                oBusinessPartner.Addresses.Block = pedido.DeliveryAddressNeighborhood;
                oBusinessPartner.Addresses.StreetNo = pedido.DeliveryAddressNumber;
                oBusinessPartner.Addresses.ZipCode = pedido.DeliveryAddressZipcode;
                oBusinessPartner.Addresses.State = pedido.DeliveryAddressState;
                oBusinessPartner.Addresses.Street = pedido.DeliveryAddressStreet;
                oBusinessPartner.Addresses.County = codMunicipio;
                //oBusinessPartner.Addresses.Country = "br";


                #region ENDEREÇO FOR
                /*
                for (int i = 0; i < 2; i++)
                {
                    if (i > 0)
                    {
                        oBusinessPartner.Addresses.SetCurrentLine(i);
                        oBusinessPartner.Addresses.AddressType = BoAddressType.bo_ShipTo;
                        oBusinessPartner.Addresses.AddressName = "FATURAMENTO";
                    }
                    else
                    {
                        oBusinessPartner.Addresses.SetCurrentLine(i);
                        oBusinessPartner.Addresses.AddressType = BoAddressType.bo_BillTo;
                        oBusinessPartner.Addresses.AddressName = "COBRANCA";

                        if (!oBusinessPartnerUpdateTest.GetByKey(cardCodePrefix + document))
                        {
                            oBusinessPartner.Addresses.Add();
                        }
                    }

                    oBusinessPartner.Addresses.City = pedido.DeliveryAddressCity;

                    if (!String.IsNullOrEmpty(pedido.DeliveryAddressAdditionalInfo) && pedido.DeliveryAddressAdditionalInfo.Length <= 100)
                    {
                        oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressAdditionalInfo;
                    }
                    else if (!String.IsNullOrEmpty(pedido.DeliveryAddressReference) && pedido.DeliveryAddressReference.Length <= 100)
                    {
                        oBusinessPartner.Addresses.BuildingFloorRoom = pedido.DeliveryAddressReference;
                    }
                      
                    //oBusinessPartner.Addresses.Country = "1058";
                    oBusinessPartner.Addresses.Block = pedido.DeliveryAddressNeighborhood;
                    oBusinessPartner.Addresses.StreetNo = pedido.DeliveryAddressNumber;
                    oBusinessPartner.Addresses.ZipCode = pedido.DeliveryAddressZipcode;
                    oBusinessPartner.Addresses.State = pedido.DeliveryAddressState;
                    oBusinessPartner.Addresses.Street = pedido.DeliveryAddressStreet;
                    oBusinessPartner.Addresses.County = codMunicipio;
                    //oBusinessPartner.Addresses.Country = "br";
                    
                }*/
                #endregion

                oBusinessPartner.BilltoDefault = "COBRANCA";
                oBusinessPartner.ShipToDefault = "FATURAMENTO";

                BusinessPartners oBusinessPartnerUpdate = null;
                oBusinessPartnerUpdate = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                if (oBusinessPartnerUpdate.GetByKey(cardCodePrefix + document))
                {
                    addBPNumber = oBusinessPartner.Update();

                    if (addBPNumber != 0)
                    {
                        messageError = oCompany.GetLastErrorDescription();
                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, cardCodePrefix+document, EnumStatusIntegracao.Erro, messageError);
                    }
                    else
                    {
                        messageError = "";
                        this.log.WriteLogTable(oCompany,EnumTipoIntegracao.Cliente,document, cardCodePrefix + document,EnumStatusIntegracao.Sucesso,"Cliente atualizado com sucesso.");

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartner);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartnerUpdate);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartnerUpdateTest);
                    }

                }
                else
                {
                    //Setando informações Fiscais
                    //oBusinessPartner.FiscalTaxID.SetCurrentLine(0);
                    if (isCorporate)
                    {
                        oBusinessPartner.FiscalTaxID.TaxId0 = document;
                    }
                    else {

                        oBusinessPartner.FiscalTaxID.TaxId4 = document;
                        oBusinessPartner.FiscalTaxID.TaxId1 = "Isento";
                    }
                    //oBusinessPartner.FiscalTaxID.Address = "FATURAMENTO";
                    //oBusinessPartner.FiscalTaxID.Add();

                    addBPNumber = oBusinessPartner.Add();

                    if (addBPNumber != 0)
                    {
                        messageError = oCompany.GetLastErrorDescription();
                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, "", EnumStatusIntegracao.Erro, messageError);
                    }
                    else
                    {
                        string CardCode = oCompany.GetNewObjectKey();
                        this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, CardCode, EnumStatusIntegracao.Sucesso, "Cliente inserido com sucesso.");
                        messageError = "";
                    }
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartner);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartnerUpdateTest);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartnerUpdate);
            }
            catch (Exception e)
            {
                this.log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, "", EnumStatusIntegracao.Erro, e.Message);
                this.log.WriteLogPedido("InserirBusinessPartner Exception: " + e.Message);
                throw;
            }

        }

    }
}
