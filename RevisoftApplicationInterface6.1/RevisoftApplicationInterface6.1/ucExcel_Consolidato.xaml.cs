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
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using RevisoftApplication;
using System.Data;

namespace UserControls
{
	public partial class ucExcel_Consolidato : UserControl
    {
        public int id;
        private DataTable dati=null;
        //private XmlDataProviderManager _x = null;
        //private string _ID = "";

		private bool _ReadOnly = false;

		public ucExcel_Consolidato()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            InitializeComponent();
        }
		
		public bool ReadOnly
		{
			set
			{
				_ReadOnly = value;
			}
		}

        public void LoadDataSource(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());


			ArrayList Al = new ArrayList();
	        dati = cBusinessObjects.GetData(id, typeof(Excel_Consolidato));
            bool trovato=false;

            foreach (DataRow dtrow in dati.Rows)
            {
			
				if (dtrow["name"] != null)
				{
					if (!Al.Contains(dtrow["name"].ToString()))
					{
						Al.Add(dtrow["name"].ToString());
                        trovato=true;
					}
				}
            }
			if(!trovato)
				{
                   DataRow dd =dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);
                   dd["name"] = "Sconosciuta";

					Al.Add("Sconosciuta");
				}
			

			if (Al.Count == 0)
			{
				Al.Add("Sconosciuta");
			}

			foreach (string item in Al)
			{
				TabItem ti = new TabItem();
				ti.Header = item;

				tabControl.Items.Add(ti);
			}

			TabItem tiout = new TabItem();
			tiout.Header = App.NewTabHeaderText;

			tabControl.Items.Add(tiout);
        }
		
        
        public int Save()
		{
		
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_Consolidato));

		}
        
        
        private void GenerateTotal()
        {
            getdatahere();

            double TOTrisultatonetto = 0.0;
            double TOTrisultatoanteimposte = 0.0;
            double TOTproduzionesenzacosti = 0.0;
            double TOTvaloreproduzione = 0.0;
            double TOTpatrimonionetto = 0.0;
            double TOTpassivo = 0.0;
            double TOTattivo = 0.0;

            foreach (DataRow dtrow in dati.Rows)
            {
				double risultatonetto = 0.0;
				double risultatoanteimposte = 0.0;
				double produzionesenzacosti = 0.0;
                double valoreproduzione = 0.0;
                double patrimonionetto = 0.0;
                double passivo = 0.0;
                double attivo = 0.0;

                double.TryParse(dtrow["risultatonetto"].ToString(), out risultatonetto);
				double.TryParse(dtrow["risultatoanteimposte"].ToString(), out risultatoanteimposte);
				double.TryParse(dtrow["produzionesenzacosti"].ToString(), out produzionesenzacosti);
                double.TryParse(dtrow["valoreproduzione"].ToString(), out valoreproduzione);
                double.TryParse(dtrow["patrimonionetto"].ToString(), out patrimonionetto);
                double.TryParse(dtrow["passivo"].ToString(), out passivo);
                double.TryParse(dtrow["attivo"].ToString(), out attivo);

                TOTrisultatonetto += risultatonetto;
                TOTrisultatoanteimposte += risultatoanteimposte;
                TOTproduzionesenzacosti += produzionesenzacosti;
                TOTvaloreproduzione += valoreproduzione;
                TOTpatrimonionetto += patrimonionetto;
                TOTpassivo += passivo;
                TOTattivo += attivo;
            }
               
            txtNettoTOT.Text = ConvertNumber(TOTrisultatonetto.ToString());
            txtAnteImposteTOT.Text = ConvertNumber(TOTrisultatoanteimposte.ToString());
            txtProduzionemenocostiTOT.Text = ConvertNumber(TOTproduzionesenzacosti.ToString());
            txtValoreProduzioneTOT.Text = ConvertNumber(TOTvaloreproduzione.ToString()); 
            txtPatrimonioNettoTOT.Text = ConvertNumber(TOTpatrimonionetto.ToString());
            txtPassivoTOT.Text = ConvertNumber(TOTpassivo.ToString());
            txtAttivoTOT.Text = ConvertNumber(TOTattivo.ToString());

            double CONSOLIDATOrisultatonetto = 0.0;
            double CONSOLIDATOrisultatoanteimposte = 0.0;
            double CONSOLIDATOproduzionesenzacosti = 0.0;
            double CONSOLIDATOvaloreproduzione = 0.0;
            double CONSOLIDATOpatrimonionetto = 0.0;
            double CONSOLIDATOpassivo = 0.0;
            double CONSOLIDATOattivo = 0.0;

            double.TryParse(txtNettoCONSOLIDATO.Text, out CONSOLIDATOrisultatonetto);
            double.TryParse(txtAnteImposteCONSOLIDATO.Text, out CONSOLIDATOrisultatoanteimposte);
            double.TryParse(txtProduzionemenocostiCONSOLIDATO.Text, out CONSOLIDATOproduzionesenzacosti);
            double.TryParse(txtValoreProduzioneCONSOLIDATO.Text, out CONSOLIDATOvaloreproduzione);
            double.TryParse(txtPatrimonioNettoCONSOLIDATO.Text, out CONSOLIDATOpatrimonionetto);
            double.TryParse(txtPassivoCONSOLIDATO.Text, out CONSOLIDATOpassivo);
            double.TryParse(txtAttivoCONSOLIDATO.Text, out CONSOLIDATOattivo);

            txtNettoCHECK.Text = ConvertNumber((TOTrisultatonetto - CONSOLIDATOrisultatonetto).ToString());
            txtAnteImposteCHECK.Text = ConvertNumber((TOTrisultatoanteimposte - CONSOLIDATOrisultatoanteimposte).ToString());
            txtProduzionemenocostiCHECK.Text = ConvertNumber((TOTproduzionesenzacosti - CONSOLIDATOproduzionesenzacosti).ToString());
            txtValoreProduzioneCHECK.Text = ConvertNumber((TOTvaloreproduzione - CONSOLIDATOvaloreproduzione).ToString());
            txtPatrimonioNettoCHECK.Text = ConvertNumber((TOTpatrimonionetto - CONSOLIDATOpatrimonionetto).ToString());
            txtPassivoCHECK.Text = ConvertNumber((TOTpassivo - CONSOLIDATOpassivo).ToString());
            txtAttivoCHECK.Text = ConvertNumber((TOTattivo - CONSOLIDATOattivo).ToString());


            double CONSOLIDATO2risultatonetto = 0.0;
            double CONSOLIDATO2risultatoanteimposte = 0.0;
            double CONSOLIDATO2produzionesenzacosti = 0.0;
            double CONSOLIDATO2valoreproduzione = 0.0;
            double CONSOLIDATO2patrimonionetto = 0.0;
            double CONSOLIDATO2passivo = 0.0;
            double CONSOLIDATO2attivo = 0.0;

            double.TryParse(txtNettoCONSOLIDATO2.Text, out CONSOLIDATO2risultatonetto);
            double.TryParse(txtAnteImposteCONSOLIDATO2.Text, out CONSOLIDATO2risultatoanteimposte);
            double.TryParse(txtProduzionemenocostiCONSOLIDATO2.Text, out CONSOLIDATO2produzionesenzacosti);
            double.TryParse(txtValoreProduzioneCONSOLIDATO2.Text, out CONSOLIDATO2valoreproduzione);
            double.TryParse(txtPatrimonioNettoCONSOLIDATO2.Text, out CONSOLIDATO2patrimonionetto);
            double.TryParse(txtPassivoCONSOLIDATO2.Text, out CONSOLIDATO2passivo);
            double.TryParse(txtAttivoCONSOLIDATO2.Text, out CONSOLIDATO2attivo);

            txtNettoCHECK2.Text = ConvertNumber((TOTrisultatonetto - CONSOLIDATOrisultatonetto - CONSOLIDATO2risultatonetto).ToString());
            txtAnteImposteCHECK2.Text = ConvertNumber((TOTrisultatoanteimposte - CONSOLIDATOrisultatoanteimposte - CONSOLIDATO2risultatoanteimposte).ToString());
            txtProduzionemenocostiCHECK2.Text = ConvertNumber((TOTproduzionesenzacosti - CONSOLIDATOproduzionesenzacosti - CONSOLIDATO2produzionesenzacosti).ToString());
            txtValoreProduzioneCHECK2.Text = ConvertNumber((TOTvaloreproduzione - CONSOLIDATOvaloreproduzione - CONSOLIDATO2valoreproduzione).ToString());
            txtPatrimonioNettoCHECK2.Text = ConvertNumber((TOTpatrimonionetto - CONSOLIDATOpatrimonionetto - CONSOLIDATO2patrimonionetto).ToString());
            txtPassivoCHECK2.Text = ConvertNumber((TOTpassivo - CONSOLIDATOpassivo - CONSOLIDATO2passivo).ToString());
            txtAttivoCHECK2.Text = ConvertNumber((TOTattivo - CONSOLIDATOattivo - CONSOLIDATO2attivo).ToString());

            foreach (DataRow dtrow in dati.Rows)
            {
                 dtrow["risultatonettoCHECK"] = ConvertNumberD(txtNettoCHECK.Text);
                 dtrow["risultatonettoCHECK"] = ConvertNumberD(txtNettoCHECK.Text);
                 dtrow["risultatoanteimposteCHECK"] = ConvertNumberD(txtAnteImposteCHECK.Text);
                 dtrow["produzionesenzacostiCHECK"] = ConvertNumberD(txtProduzionemenocostiCHECK.Text);
                 dtrow["valoreproduzioneCHECK"] = ConvertNumberD(txtValoreProduzioneCHECK.Text);
                 dtrow["patrimonionettoCHECK"] = ConvertNumberD(txtPatrimonioNettoCHECK.Text);
                 dtrow["passivoCHECK"] = ConvertNumberD(txtPassivoCHECK.Text);
                 dtrow["attivoCHECK"] = ConvertNumberD(txtAttivoCHECK.Text);
                 dtrow["risultatonettoCHECK2"] = ConvertNumberD(txtNettoCHECK2.Text);
                 dtrow["risultatoanteimposteCHECK2"]= ConvertNumberD(txtAnteImposteCHECK2.Text);
                 dtrow["produzionesenzacostiCHECK2"] = ConvertNumberD(txtProduzionemenocostiCHECK2.Text);
                 dtrow["valoreproduzioneCHECK2"] = ConvertNumberD(txtValoreProduzioneCHECK2.Text);
                 dtrow["patrimonionettoCHECK2"] = ConvertNumberD(txtPatrimonioNettoCHECK2.Text);
                 dtrow["passivoCHECK2"] = ConvertNumberD(txtPassivoCHECK2.Text);
                 dtrow["attivoCHECK2"] = ConvertNumberD(txtAttivoCHECK2.Text);
               //TOT
                 dtrow["risultatonettoTOT"] = ConvertNumberD(txtNettoTOT.Text);
                 dtrow["risultatoanteimposteTOT"] = ConvertNumberD(txtAnteImposteTOT.Text);
                 dtrow["produzionesenzacostiTOT"] = ConvertNumberD(txtProduzionemenocostiTOT.Text);
                 dtrow["valoreproduzioneTOT"] = ConvertNumberD(txtValoreProduzioneTOT.Text);
                 dtrow["patrimonionettoTOT"] = ConvertNumberD(txtPatrimonioNettoTOT.Text);
                 dtrow["passivoTOT"] = ConvertNumberD(txtPassivoTOT.Text);
                 dtrow["attivoTOT"] = ConvertNumberD(txtAttivoTOT.Text);


            }


     
        }

		private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
		{
			GenerateTotal();
		}

		private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			GenerateTotal();
		}
        private double ConvertNumberD(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);
            return dblValore;
        }

            private string ConvertNumber(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,#}", dblValore);
			}
		}

		private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            DataGrid grd = (DataGrid)sender;
            grd.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                string controlName = child.GetValue(Control.NameProperty) as string;

                if (controlName == name)
                {
                    return child as T;
                }

                else
                {
                    T result = FindVisualChildByName<T>(child, name);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        { 
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				e.Cancel = true;
				return;
			}
        }

		private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}
        
		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && (e.AddedItems[0]).GetType().Name == "TabItem")
			{
				if (((string)(((TabItem)(e.AddedItems[0])).Header)) == App.NewTabHeaderText)
				{
					if (_ReadOnly)
					{
						MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
						return;
					}

					var dialog = new wInputBox("Inserire Denominazione Nuovo Componente");
					dialog.ShowDialog();

					string newHeader = dialog.ResponseText;

					if (newHeader == "")
					{
						MessageBox.Show("Attenzione, Denominazione non valida");
						tabControl.SelectedIndex = 0;
						return;
					}

					foreach (TabItem item in tabControl.Items)
					{
						if (((string)(item.Header)) == newHeader)
						{
							MessageBox.Show("Attenzione, Denominazione già esistente");
							tabControl.SelectedIndex = 0;
							return;
						}
					}

					TabItem ti = new TabItem();
					ti.Header = newHeader;

					tabControl.Items.Insert(tabControl.Items.Count - 1, ti);
					tabControl.SelectedIndex = tabControl.Items.Count - 2;
                    bool trovatoheader=false;
                    foreach (DataRow dtrow in dati.Rows)
                     {
                        if(dtrow["name"].ToString()==newHeader)
                        {
                            trovatoheader=true;
                        }
                     }
                    
                    if(!trovatoheader)
                        {
                        DataRow dd=  dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);
                        dd["name"]= newHeader;

                        }

                  
                }
				else
				{
                    bool trovatoheader=false;
                    foreach (DataRow dtrow in dati.Rows)
                     {
                        if(dtrow["name"].ToString()==((string)(((TabItem)(e.AddedItems[0])).Header)))
                        {
                            trovatoheader=true;
                        }
                     }
                    
                    if(!trovatoheader)
                        {
                         
                          DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                          dd["name"] = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    }

                    GenerateTotal();
                }
            }			
		}

		private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
		{
            if ( tabControl.Items.Count <= 2 )
            {
                MessageBox.Show( "L'unica tabella presente non è cancellabile; usa il comando Cancella Contenuto.", "Attenzione" );
                return;
            }

			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
                string newHeader = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
               
              
                for (int i = dati.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati.Rows[i];
                    if (dtrow["name"].ToString() == newHeader)
                        dtrow.Delete();
                }
           
                this.dati.AcceptChanges();


                TabItem oldSelected = ((TabItem)(tabControl.SelectedItem));

				if (tabControl.Items.Count > 0)
				{
					tabControl.SelectedIndex = 0;
				}
				else
				{
					tabControl.SelectedIndex = -1;
				}

				tabControl.Items.Remove(oldSelected);
			}
		}

		private void btnRenameTable_Click(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

			var dialog = new wInputBox("Inserire Titolo della nuova Tabella");
            dialog.ResponseText = ( (TabItem)( tabControl.SelectedItem ) ).Header.ToString();
			dialog.ShowDialog();

			string newHeader = dialog.ResponseText;

			if (newHeader == "")
			{
				MessageBox.Show("Attenzione, Titolo non valido");
				tabControl.SelectedIndex = 0;
				return;
			}

			foreach (TabItem item in tabControl.Items)
			{
				if (((string)(item.Header)) == newHeader)
				{
					MessageBox.Show("Attenzione, Titolo già esistente");
					tabControl.SelectedIndex = 0;
					return;
				}
			}

			ChangeNameTab(newHeader, ((TabItem)(tabControl.SelectedItem)).Header.ToString());			
            
            ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
        }

        private void ChangeNameTab(string newname, string oldheader)
		{
        foreach (DataRow dtrow in dati.Rows)
        {
            if (dtrow["name"].ToString()==oldheader)
                dtrow["name"] = newname;
        }
        
		}

		private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var tabItem = e.Source as TabItem;

			if (tabItem == null)
				return;

			if (tabItem.Header.ToString() == App.NewTabHeaderText)
			{
				return;
			}

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
			}
		}


		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			if (_ReadOnly)
			{
				return;
			}

			var tabItemTarget = e.Source as TabItem;

			if (tabItemTarget.Header.ToString() == App.NewTabHeaderText)
			{
				return;
			}

			var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

			if (!tabItemTarget.Equals(tabItemSource))
			{
				string sourceHeader = tabItemSource.Header.ToString();
				string targetHeader = tabItemTarget.Header.ToString();

				ChangeNameTab(sourceHeader, sourceHeader);
				ChangeNameTab(targetHeader, targetHeader);

				tabItemTarget.Header = sourceHeader;
				tabItemSource.Header = targetHeader;
                foreach (DataRow dtrow in dati.Rows)
                {
                        if (dtrow["name"].ToString() == sourceHeader)
                        {
                              dtrow["name"] = targetHeader;
                        }
                }

                GenerateTotal();
            }
        }


        Hashtable valoreEA = new Hashtable();
        Hashtable SommeDaExcel = new Hashtable();

        private double GetValoreEA(string Cella)
        {
            double returnvalue = 0.0;

            if (SommeDaExcel.Contains(Cella))
            {
                foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
                {
                    double dblValore = 0.0;

                    if (valoreEA.Contains(ID))
                    {
                        double.TryParse(valoreEA[ID].ToString(), out dblValore);
                    }

                    returnvalue += dblValore;
                }
            }

            return returnvalue;
        }
        

        void getdatahere()
        {
          //  XmlNode nodehere = _x.Document.SelectSingleNode("/Dati/Dato[@ID" + _ID + "]/Valore[@tipo='Componenti'][@name='" + ((TabItem)(tabControl.SelectedItem)).Header.ToString() + "']");
            foreach (DataRow dtrow in dati.Rows)
            {
               
                //  if (dtrow["tipo"].ToString() =="Componenti"  && dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                {
                    if (dtrow["scope"].ToString() != "")
                    {
                        txtScope.SelectedIndex = Convert.ToInt32(dtrow["scope"].ToString());
                    }
                    else
                    {
                        txtScope.SelectedIndex = 0;
                    }
                    txtNetto.Text = dtrow["risultatonetto"].ToString();
                    txtAnteImposte.Text = dtrow["risultatoanteimposte"].ToString();
                    txtProduzionemenocosti.Text = dtrow["produzionesenzacosti"].ToString();
                    txtValoreProduzione.Text  = dtrow["valoreproduzione"].ToString();
                    txtPatrimonioNetto.Text = dtrow["patrimonionetto"].ToString();
                    txtPassivo.Text = dtrow["passivo"].ToString();
                    txtAttivo.Text = dtrow["attivo"].ToString();

                    if (dtrow["metodoconsolidamento"].ToString() != "")
                    {
                        txtMetodo.SelectedIndex = Convert.ToInt32(dtrow["metodoconsolidamento"].ToString());
                    }
                    else
                    {
                        txtMetodo.SelectedIndex = 0;
                    }
                    txtRevisore.Text = dtrow["revisore"].ToString();
                    txtSede.Text = dtrow["sede"].ToString();
                    txtCF.Text = dtrow["CF"].ToString();
                    txtDenominazione.Text = dtrow["denominazione"].ToString();
               }
            }

        
           foreach (DataRow nodehere in dati.Rows)
           {
                try
                {
                    string IDB_Padre_Consolidato = "321";
                    DataTable datiPadre_Consolidato = null;

                    datiPadre_Consolidato = cBusinessObjects.GetData(int.Parse(IDB_Padre_Consolidato), typeof(Excel_Consolidato));
                    foreach (DataRow dtrowP in datiPadre_Consolidato.Rows)
                    {
                     
                        //Calcolo valori attuali

                        if (dtrowP["EA"].ToString() != "")
                        {
                            valoreEA.Add(dtrowP["ID"].ToString(), dtrowP["EA"].ToString());
                        }
                        else
                        {
                            valoreEA.Add(dtrowP["ID"].ToString(), "0");
                        }
                    }

                  

                    SommeDaExcel.Add("Passivo", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20181142|20161143|2016114|2016998|11611|124|125|2018125|126|2016126|129|133|134|136|137|139|140|142|143|145|146|148|149|151|152|154|155|157|158|160|161|163|164|166|167|169|170|172|173|2016163|2016164|175");
                    SommeDaExcel.Add("Attivo", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|73|77|78|81|82|83|84|85|86|90|91|92|98|201655|201627|201638|201639|201677|201678|201651|201683");
                    SommeDaExcel.Add("ValoreProduzione", "189|190|191|192|194|195");
                    SommeDaExcel.Add("PatrimonioNetto", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20181142|20161143|2016114|2016998|11611");
                    SommeDaExcel.Add("Produzionemenocosti", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
                    SommeDaExcel.Add("AnteImposte", "247|248|249|251|252|253|2016249|20162491|20162492|20162493|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|189|190|191|192|194|195|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
                    SommeDaExcel.Add("Netto", "247|248|249|251|252|253|2016249|20162491|20162492|20162493|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|189|190|191|192|194|195|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243|267|268|217005|217006|2016267");

                    txtNettoCONSOLIDATO2.Text = ((nodehere["risultatonetto2"].ToString()!= "") ? nodehere["risultatonetto2"].ToString() : "0");
                    txtAnteImposteCONSOLIDATO2.Text = ((nodehere["risultatoanteimposte2"].ToString() != "") ? nodehere["risultatoanteimposte2"].ToString() : "0");
                    txtProduzionemenocostiCONSOLIDATO2.Text = ((nodehere["produzionesenzacosti2"].ToString() != "") ? nodehere["produzionesenzacosti2"].ToString() : "0");
                    txtValoreProduzioneCONSOLIDATO2.Text = ((nodehere["valoreproduzione2"].ToString() != "") ? nodehere["valoreproduzione2"].ToString() : "0");
                    txtPatrimonioNettoCONSOLIDATO2.Text = ((nodehere["patrimonionetto2"].ToString() != "") ? nodehere["patrimonionetto2"].ToString() : "0");
                    txtPassivoCONSOLIDATO2.Text = ((nodehere["passivo2"].ToString() != "") ? nodehere["passivo2"].ToString() : "0");
                    txtAttivoCONSOLIDATO2.Text = ((nodehere["attivo2"].ToString() != "") ? nodehere["attivo2"].ToString() : "0");

                    txtNettoCONSOLIDATO.Text = ((GetValoreEA("Netto") == 0) ? ((nodehere["risultatonetto"].ToString() != "") ? nodehere["risultatonetto"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("Netto").ToString()));
                    txtAnteImposteCONSOLIDATO.Text = ((GetValoreEA("AnteImposte") == 0) ? ((nodehere["risultatoanteimposte"].ToString() != "") ? nodehere["risultatoanteimposte"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("AnteImposte").ToString())); //nodehere.Attributes["risultatoanteimposte"].Value;
                    txtProduzionemenocostiCONSOLIDATO.Text = ((GetValoreEA("Produzionemenocosti") == 0) ? ((nodehere["produzionesenzacosti"].ToString() != "") ? nodehere["produzionesenzacosti"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("Produzionemenocosti").ToString())); //nodehere.Attributes["produzionesenzacosti"].Value;
                    txtValoreProduzioneCONSOLIDATO.Text = ((GetValoreEA("ValoreProduzione") == 0) ? ((nodehere["valoreproduzione"].ToString() != "") ? nodehere["valoreproduzione"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("ValoreProduzione").ToString())); //nodehere.Attributes["valoreproduzione"].Value;
                    txtPatrimonioNettoCONSOLIDATO.Text = ((GetValoreEA("PatrimonioNetto") == 0) ? ((nodehere["patrimonionetto"].ToString() != "") ? nodehere["patrimonionetto"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("PatrimonioNetto").ToString())); //nodehere.Attributes["patrimonionetto"].Value;
                    txtPassivoCONSOLIDATO.Text = ((GetValoreEA("Passivo") == 0) ? ((nodehere["passivo"].ToString() != "") ? nodehere["passivo"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("Passivo").ToString())); //nodehere.Attributes["passivo"].Value;
                    txtAttivoCONSOLIDATO.Text = ((GetValoreEA("Attivo") == 0) ? ((nodehere["attivo"].ToString() != "") ? nodehere["attivo"].ToString() : "0") : cBusinessObjects.ConvertInteger(GetValoreEA("Attivo").ToString())); //nodehere.Attributes["attivo"].Value;



                    nodehere["risultatonetto"] = ConvertNumberD(txtNettoCONSOLIDATO.Text);
                    nodehere["risultatoanteimposte"] = ConvertNumberD(txtAnteImposteCONSOLIDATO.Text);
                    nodehere["produzionesenzacosti"] = ConvertNumberD(txtProduzionemenocostiCONSOLIDATO.Text);
                    nodehere["valoreproduzione"] = ConvertNumberD(txtValoreProduzioneCONSOLIDATO.Text);
                    nodehere["patrimonionetto"] = ConvertNumberD(txtPatrimonioNettoCONSOLIDATO.Text);
                    nodehere["passivo"] = ConvertNumberD(txtPassivoCONSOLIDATO.Text);
                    nodehere["attivo"] = ConvertNumberD(txtAttivoCONSOLIDATO.Text);
                   
                }
                catch (Exception ex)
                {
                string log = ex.Message;
                }
           }
        }

        private void txtScope_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtScope.SelectedValue == null)
            {
                return;
            }

            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["scope"] = txtScope.SelectedIndex.ToString();
            }
        }

        private void txtNetto_LostFocus(object sender, RoutedEventArgs e)
        {
          
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["risultatonetto"] = ConvertNumberD(txtNetto.Text);
            }
            GenerateTotal();
        }

        private void txtAnteImposte_LostFocus(object sender, RoutedEventArgs e)
        {
           
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["risultatoanteimposte"] = ConvertNumberD(txtAnteImposte.Text);
            }
            GenerateTotal();
        }

        private void txtProduzionemenocosti_LostFocus(object sender, RoutedEventArgs e)
        {
          

            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["produzionesenzacosti"] = ConvertNumberD(txtProduzionemenocosti.Text);
            }
            GenerateTotal();
        }

        private void txtValoreProduzione_LostFocus(object sender, RoutedEventArgs e)
        {
        

            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["valoreproduzione"] = ConvertNumberD(txtValoreProduzione.Text);
            }
            GenerateTotal();
        }

        private void txtPatrimonioNetto_LostFocus(object sender, RoutedEventArgs e)
        {
         

            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["patrimonionetto"] = ConvertNumberD(txtPatrimonioNetto.Text);
            }
            GenerateTotal();
        }

        private void txtPassivo_LostFocus(object sender, RoutedEventArgs e)
        {
           
     
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["passivo"] = ConvertNumberD(txtPassivo.Text);
            }
            GenerateTotal();
        }

        private void txtAttivo_LostFocus(object sender, RoutedEventArgs e)
        {
         
              foreach (DataRow dtrow in dati.Rows)
                {
                if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                    dtrow["attivo"] = ConvertNumberD(txtAttivo.Text);
                }
              GenerateTotal();
        }

        private void txtMetodo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMetodo.SelectedValue == null)
            {
                return;
            }

            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["metodoconsolidamento"] = txtMetodo.SelectedIndex.ToString();
            }
        }

        private void txtRevisore_LostFocus(object sender, RoutedEventArgs e)
        {
           
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["revisore"] = txtRevisore.Text;
            }
        }

        private void txtSede_LostFocus(object sender, RoutedEventArgs e)
        {
          
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["sede"] = txtRevisore.Text;
            }
        }

        private void txtCF_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
            if (dtrow["name"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                dtrow["CF"] = txtCF.Text;
            }
        }

        private void txtDenominazione_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
            if(dtrow["name"].ToString()== ((TabItem)(tabControl.SelectedItem)).Header.ToString())
              dtrow["denominazione"] = txtDenominazione.Text;
            }
        }



        //CONSOLIDATO

        private void txtNettoCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
           
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["risultatonetto"] = ConvertNumberD(txtNettoCONSOLIDATO.Text);
                txtNettoCONSOLIDATO.Text = dtrow["risultatonetto"].ToString();
            }
            GenerateTotal();
        }

        private void txtAnteImposteCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["risultatoanteimposte"] = ConvertNumberD(txtAnteImposteCONSOLIDATO.Text);
                txtAnteImposteCONSOLIDATO.Text = dtrow["risultatoanteimposte"].ToString();
            }
            GenerateTotal();
        }

        private void txtProduzionemenocostiCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
   
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["produzionesenzacosti"] = ConvertNumberD(txtProduzionemenocostiCONSOLIDATO.Text);
                txtProduzionemenocostiCONSOLIDATO.Text = dtrow["produzionesenzacosti"].ToString();
            }
            GenerateTotal();
        }

        private void txtValoreProduzioneCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
 
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["valoreproduzione"] = ConvertNumberD(txtValoreProduzioneCONSOLIDATO.Text);
                txtValoreProduzioneCONSOLIDATO.Text = dtrow["valoreproduzione"].ToString();
            }
            GenerateTotal();
        }

        private void txtPatrimonioNettoCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
         
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["patrimonionetto"] = ConvertNumberD(txtPatrimonioNettoCONSOLIDATO.Text);
               txtPatrimonioNettoCONSOLIDATO.Text = dtrow["patrimonionetto"].ToString();
            }
            GenerateTotal();
        }

        private void txtPassivoCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
            dtrow["passivo"] = ConvertNumberD(txtPassivoCONSOLIDATO.Text);
            txtPassivoCONSOLIDATO.Text = dtrow["passivo"].ToString();
            }
            GenerateTotal();
        }

        private void txtAttivoCONSOLIDATO_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["attivo"] = ConvertNumberD(txtAttivoCONSOLIDATO.Text);
                txtAttivoCONSOLIDATO.Text = dtrow["attivo"].ToString();
            }
            GenerateTotal();
        }

        private void txtNettoCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txtNettoCONSOLIDATO2"] = ConvertNumberD(txtNettoCONSOLIDATO2.Text);
                txtNettoCONSOLIDATO2.Text = dtrow["txtNettoCONSOLIDATO2"].ToString();
            }
            GenerateTotal();
        }

        private void txtAnteImposteCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
               dtrow["risultatoanteimposte2"] = ConvertNumberD(txtAnteImposteCONSOLIDATO2.Text);
               txtAnteImposteCONSOLIDATO2.Text = dtrow["risultatoanteimposte2"].ToString();
            }
            GenerateTotal();
        }

        private void txtProduzionemenocostiCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dtrow in dati.Rows)
            {
            dtrow["produzionesenzacosti2"] = ConvertNumberD(txtProduzionemenocostiCONSOLIDATO2.Text);
            txtProduzionemenocostiCONSOLIDATO2.Text = dtrow["produzionesenzacosti2"].ToString();
            }
            GenerateTotal();
        }

        private void txtValoreProduzioneCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
           
            foreach (DataRow dtrow in dati.Rows)
            {
              dtrow["valoreproduzione2"] = ConvertNumberD(txtValoreProduzioneCONSOLIDATO2.Text);
              txtValoreProduzioneCONSOLIDATO2.Text = dtrow["valoreproduzione2"].ToString();
            }
            GenerateTotal();
        }

        private void txtPatrimonioNettoCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
            
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txtPatrimonioNettoCONSOLIDATO2"] = ConvertNumberD(txtPatrimonioNettoCONSOLIDATO2.Text);
                txtPatrimonioNettoCONSOLIDATO2.Text = dtrow["txtPatrimonioNettoCONSOLIDATO2"].ToString();
            }
            GenerateTotal();
        }

        private void txtPassivoCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
          
            foreach (DataRow dtrow in dati.Rows)
            {
            dtrow["passivo2"] = ConvertNumberD(txtPassivoCONSOLIDATO2.Text);
            txtPassivoCONSOLIDATO2.Text = dtrow["passivo2"].ToString();
            }
            GenerateTotal();
        }

        private void txtAttivoCONSOLIDATO2_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dtrow in dati.Rows)
            {
                    dtrow["attivo2"] = ConvertNumberD(txtAttivoCONSOLIDATO2.Text);
                    txtAttivoCONSOLIDATO2.Text = dtrow["attivo2"].ToString();
            }
            GenerateTotal();
        }
    }
}
