using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using RevisoftApplication;
using System.Collections;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Data;

namespace UserControls
{

    public partial class ucRischioIndividuazione : UserControl
    {

        public int id;
        private DataTable datiordinario = new DataTable();
        private DataTable datiabbreviato = new DataTable();
      

        private int Offset = 260;
        private int OffsetNote = 270 + 1000;
        private int Minimo = 200;

        private string check = "./Images/icone/check2-24x24.png";
        private string uncheck = "./Images/icone/check1-24x24.png";

        private string up = "./Images/icone/navigate_up.png";
        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

        private XmlDataProviderManager _x;
        private string _ID = "-1";
        private string IDCompensiERisorse = "42";

        private bool _ReadOnly = false;
        private bool _StartingCalculation = true;

        GenericTable gtordinario = null;
        GenericTable gtabbreviato =null;
   
        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

        public ucRischioIndividuazione()
        {
            
            InitializeComponent();
        }

        public void Load(string ID, string IDCliente, string IDSessione)
        {
            string gacquisti = ((TextBlock)cBusinessObjects.uc_controls["txt3c"]).Text;
            string gmagazzino =  ((TextBlock)cBusinessObjects.uc_controls["txt4c"]).Text;
            string gtesoreria =  ((TextBlock)cBusinessObjects.uc_controls["txt5c"]).Text;
            string gvendite =  ((TextBlock)cBusinessObjects.uc_controls["txt2c"]).Text;
            string gpersonale =  ((TextBlock)cBusinessObjects.uc_controls["txt6c"]).Text;

            datiabbreviato.Columns.Add("col1", typeof(System.String));
            datiabbreviato.Columns.Add("col2", typeof(System.String));
            datiabbreviato.Columns.Add("col3", typeof(System.String));
            datiabbreviato.Columns.Add("col4", typeof(System.String));
            datiabbreviato.Columns.Add("txtfinder", typeof(System.String));
            datiabbreviato.Columns.Add("isnew", typeof(System.String));
            
            datiordinario.Columns.Add("col1", typeof(System.String));
            datiordinario.Columns.Add("col2", typeof(System.String));
            datiordinario.Columns.Add("col3", typeof(System.String));
            datiordinario.Columns.Add("col4", typeof(System.String));
            datiordinario.Columns.Add("txtfinder", typeof(System.String));
            datiordinario.Columns.Add("isnew", typeof(System.String));
        
            datiordinario.Rows.Add("3-Ord.4.1 Pian","Immobilizzazioni immateriali","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.2 Pian","Immobilizzazioni materiali","ACQUISTI",gacquisti,"","");
            datiordinario.Rows.Add("3-Ord.4.3 Pian","Immobilizzazioni finanziarie","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.4 Pian","Rimanenze di magazzino","MAGAZZINO",gmagazzino,"","");
            datiordinario.Rows.Add("3-Ord.4.5 Pian","Rimanenze - commesse/opere a lungo termine","MAGAZZINO",gmagazzino,"","");
            datiordinario.Rows.Add("3-Ord.4.6 Pian","Attività finanziarie non immobilizzate","TESORERIA",gtesoreria,"","");
            datiordinario.Rows.Add("3-Ord.4.7 Pian","Crediti commerciali (Clienti)","VENDITE",gvendite,"","");
            datiordinario.Rows.Add("3-Ord.4.8 Pian","Crediti e debiti infra Gruppo","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.9 Pian","Crediti tributari e per imposte anticipate","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.10 Pian","Crediti verso altri","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.11 Pian","Cassa e banche","TESORERIA",gtesoreria,"","");
            datiordinario.Rows.Add("3-Ord.4.12 Pian","Ratei e risconti (attivi e passivi)","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.13 Pian","Patrimonio netto","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.14 Pian","Fondi per rischi e oneri","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.15 Pian","T.F.R. (Trattamento Fine Rapporto)","PERSONALE DIPENDENTE",gpersonale,"","");
            datiordinario.Rows.Add("3-Ord.4.16 Pian","Mutui e finanziamenti non bancari","TESORERIA",gtesoreria,"","");
            datiordinario.Rows.Add("3-Ord.4.17 Pian","Debiti commerciali (Fornitori)","ACQUISTI",gacquisti,"","");
            datiordinario.Rows.Add("3-Ord.4.18 Pian","Debiti tributari","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.19 Pian","Debiti verso altri","Ciclo (procedura)non esistente","Procedure di validità","","");
            datiordinario.Rows.Add("3-Ord.4.21 Pian","Conto economico","Ciclo (procedura)non esistente","Procedure di validità","","");

            
            gtordinario = new GenericTable( tblordinario, _ReadOnly);


            gtordinario.ColumnsAlias = new string[] { "", "VOCI DI BILANCIO", "CICLO COLLEGATO", "RISCHIO DI INDIVIDUAZIONE PROPOSTO" };
            gtordinario.ColumnsValues = new string[] { "col1", "col2", "col3", "col4"};
            gtordinario.ColumnsWidth = new double[] { 9.0, 9.0, 9.0,9.0 };
            gtordinario.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0 };
            gtordinario.ColumnsTypes = new string[] { "string", "string", "string","string" };
            gtordinario.ColumnsAlignment = new string[] { "left","left", "center", "center" };
            gtordinario.ColumnsReadOnly = new bool[] { true, true,  true,  true  };
            gtordinario.ConditionalReadonly = new bool[] { false, false,  false,  false  };
            gtordinario.ConditionalAttribute = "new";
            gtordinario.ColumnsHasTotal = new bool[] { false, false, false, false };
            gtordinario.ColumnAliasTotale = 0;
            gtordinario.dati = datiordinario;
            gtordinario.xml = false;
            gtordinario.GenerateTable();
      
      
            datiabbreviato.Rows.Add("3-Abb.4.1 Pian","Immobilizzazioni Immateriali - Pianificazione","Nessuno","Procedure di validità","","");		
            datiabbreviato.Rows.Add("3-Abb.4.2 Pian","Immobilizzazioni materiali - Pianificazione","ACQUISTI",gacquisti,"","");
	        datiabbreviato.Rows.Add("3-Abb.4.3 Pian","Immobilizazioni Finanziarie - Pianificazione","Nessuno","Procedure di validità","","");			
            datiabbreviato.Rows.Add("3-Abb.4.10.1 Pian","Rimanenze magazzino - Pianificazione","MAGAZZINO",gmagazzino,"","");
            datiabbreviato.Rows.Add("3-Abb.4.10.11 Pian","Rimanenze commesse - Pianificazione","MAGAZZINO",gmagazzino,"","");
            datiabbreviato.Rows.Add("3-Abb.4.10.21 Pian","Immobiliz. Mat. In vendita - Pianificazione","","","","");					
            datiabbreviato.Rows.Add("3-Abb.4.20.1 Pian","Att. Fin. non immobil. - Pianificazione","TESORERIA",gtesoreria,"","");
            datiabbreviato.Rows.Add("3-Abb.4.20.11 Pian","Crediti comm. (clienti) - Pianificazione","VENDITE",gvendite,"","");
            datiabbreviato.Rows.Add("3-Abb.4.20.21 Pian","Crediti Infragruppo - Pianificazione","Nessuno","Procedure di validità","","");
            datiabbreviato.Rows.Add("3-Abb.4.20.31 Pian","Crediti tributari Pianifcazione","Nessuno","Procedure di validità","","");
            datiabbreviato.Rows.Add("3-Abb.4.20.41 Pian","Imposte anticipate - Pianificazione","Nessuno","Procedure di validità","","");
            datiabbreviato.Rows.Add("3-Abb.4.20.51 Pian","Crediti altri - Pianificazione","Nessuno","Procedure di validità","","");	
            datiabbreviato.Rows.Add("3-Abb.4.25.1 Pian","Cassa e banche - Pianificazione","TESORERIA",gtesoreria,"","");		
            datiabbreviato.Rows.Add("3-Abb.4.30.1 Pian","Ratei e risconti attivi - Pianificazione","Nessuno","Procedure di validità","","");		
            datiabbreviato.Rows.Add("3-Abb.4.40.1 Pian","Patrimonio netto - Pianificazione","Nessuno","Procedure di validità","","");
			
            datiabbreviato.Rows.Add("3-Abb.4.43.1 Pian","Fondi rischi - Pianificaizone","Nessuno","Procedure di validità","","");		
            datiabbreviato.Rows.Add("3-Abb.4.45.1 Pian","T.F.R. - Pianificazione","PERSONALE DIPENDENTE",gpersonale,"","");
			
            datiabbreviato.Rows.Add("3-Abb.4.50.1 Pian","Debiti verso banche - Pianificaizone","TESORERIA",gtesoreria,"","");
            datiabbreviato.Rows.Add("3-Abb.4.50.5 Pian","Finanz. non bancari - Pianificazione","TESORERIA",gtesoreria,"","");
            datiabbreviato.Rows.Add("3-Abb.4.50.11 Pian","Debiti comm. (Fornitori) - Pianificazione","ACQUISTI",gacquisti,"","");;
            datiabbreviato.Rows.Add("3-Abb.4.50.21 Pian","Debiti Infra - Pianificazione","Nessuno","Procedure di validità","","");
            datiabbreviato.Rows.Add("3-Abb.4.50.31 Pian","Debiti tributari - Pianificazione","Nessuno","Procedure di validità","","");
            datiabbreviato.Rows.Add("3-Abb.4.50.41 Pian","Debiti altri - Pianificazione","Nessuno","Procedure di validità","","");
			
            datiabbreviato.Rows.Add("3-Abb.4.55.1 Pian","Ratei e risconti passivi - Pianificazione","Nessuno","Procedure di validità","","");
			
            datiabbreviato.Rows.Add("3-Abb.4.60.1 Pian","Conto economico - Pianificazione","Nessuno","Procedure di validità","","");
            
            gtabbreviato = new GenericTable( tblabbreviato, _ReadOnly);


            gtabbreviato.ColumnsAlias = new string[] { "", "VOCI DI BILANCIO", "CICLO COLLEGATO", "RISCHIO DI INDIVIDUAZIONE PROPOSTO" };
            gtabbreviato.ColumnsValues = new string[] { "col1", "col2", "col3", "col4"};
            gtabbreviato.ColumnsWidth = new double[] { 9.0, 9.0, 9.0,9.0 };
            gtabbreviato.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0 };
            gtabbreviato.ColumnsTypes = new string[] { "string", "string", "string","string" };
            gtabbreviato.ColumnsAlignment = new string[] { "left","left", "center", "center" };
            gtabbreviato.ColumnsReadOnly = new bool[] { true, true,  true,  true  };
            gtabbreviato.ConditionalReadonly = new bool[] { false, false,  false,  false  };
            gtabbreviato.ConditionalAttribute = "new";
            gtabbreviato.ColumnsHasTotal = new bool[] { false, false, false, false };
            gtabbreviato.ColumnAliasTotale = 0;
            gtabbreviato.dati = datiabbreviato;
            gtabbreviato.xml = false;
            gtabbreviato.GenerateTable();
     
            
        }


      
    }
}
