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
	public partial class ucExcel_Compensazioni : UserControl
    {
        public int id;
        private DataTable dati = null;
        private int CurrentTabSelectedIndex = 0;


        private bool _ReadOnly = false;

        GenericTable gtCompensazioni = null;

        public ucExcel_Compensazioni()
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

            dati = cBusinessObjects.GetData(id, typeof(Excel_Compensazioni));



            ArrayList Al = new ArrayList();

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["periodo"] != null)
                {
                    if (!Al.Contains(dtrow["periodo"].ToString()))
                    {
                        Al.Add(dtrow["periodo"].ToString());
                    }
                }
            }
         


			if (Al.Count == 0)
			{
				Al.Add("Tributo XX");
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

            gtCompensazioni = new GenericTable( tblCompensazioni, _ReadOnly);

            gtCompensazioni.ColumnsAlias = new string[] { "Data", "Tributi Compensati", "Importi" };
            gtCompensazioni.ColumnsValues = new string[] { "name", "codice", "importoPagato" };
            gtCompensazioni.ColumnsWidth = new double[] { 1.0, 3.0, 1.0 };
            gtCompensazioni.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
            gtCompensazioni.ColumnsTypes = new string[] { "string", "string", "money" };
            gtCompensazioni.ColumnsAlignment = new string[] { "right", "left", "right" };
            gtCompensazioni.ColumnsReadOnly = new bool[] { false, false, false };
            gtCompensazioni.ConditionalReadonly = new bool[] { false, false, false };
            gtCompensazioni.ConditionalAttribute = "new";
            gtCompensazioni.ColumnsHasTotal = new bool[] { false, false, true };
            gtCompensazioni.AliasTotale = "Credito residuo (Credito esistente - Tot. importi)";
            gtCompensazioni.xml = false;
            gtCompensazioni.dati = dati;
            gtCompensazioni.ColumnAliasTotale = 1;

            gtCompensazioni.TotalHasBeenCalculated += GtCompensazioni_TotalHasBeenCalculated;
        }

        private void GtCompensazioni_TotalHasBeenCalculated(object sender, EventArgs e)
        {
            double a = 0.0;
            double b = 0.0;
            double c = 0.0;

            double.TryParse(gtCompensazioni.GetTotalValue("2"), out a);
            double.TryParse(txtCreditoEsistente.Text, out b);

            c = b - a;

            gtCompensazioni.SetTotalValue("2", cBusinessObjects.ConvertNumber(c.ToString()));
        }

        public int Save()
		{
           
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["periodo"].ToString() == ((TabItem)tabControl.Items[CurrentTabSelectedIndex]).Header.ToString())
                {
                    dtrow["txtCreditoEsistente"] = txtCreditoEsistente.Text;
                }
            }
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_Compensazioni));
        }

        private void AggiungiNodo(string Alias,  string Codice)
        {
           dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header,Alias, Codice,0);
           gtCompensazioni.GenerateTable();
        }

   //     private void GenerateTotal()
   //     {
   //         DeleteTotal();

			//double importoPagato = 0.0;
			//double importoCompensato = 0.0;

   //         if ( tabControl.SelectedItem == null )
   //         {
   //             return;
   //         }

   //         if (!_ReadOnly && _x.Document.SelectNodes("/Dati/Dato[@ID" + _ID + "]/Valore[@tipo=\"Compensazioni\"][@periodo=\"" + ((TabItem)(tabControl.SelectedItem)).Header + "\"]").Count <= 0)
   //         {
   //             AggiungiNodo("", _ID, "");
   //         }

			//foreach (XmlNode item in _x.Document.SelectNodes("/Dati/Dato[@ID" + _ID + "]/Valore[@tipo=\"Compensazioni\"][@periodo=\"" + ((TabItem)(tabControl.SelectedItem)).Header + "\"]"))
   //         {
			//	importoPagato += Convert.ToDouble(item.Attributes["importoPagato"].Value);
			//	importoCompensato += Convert.ToDouble(item.Attributes["importoCompensato"].Value);
   //         }

   //         double CreditoEsistente = 0.0;
   //         double.TryParse(txtCreditoEsistente.Text, out CreditoEsistente);

			//string xmlcs = "<Valore tipo=\"Compensazioni\" name=\"\" periodo=\"" + ((TabItem)(tabControl.SelectedItem)).Header + "\" codice=\"Credito residuo (Credito esistente - Tot. importi)\" importoPagato=\"" + (CreditoEsistente - importoPagato).ToString() + "\" importoCompensato=\"" + (CreditoEsistente - importoCompensato).ToString() + "\" bold=\"true\"/>";

   //         XmlDocument doccs = new XmlDocument();
   //         doccs.LoadXml(xmlcs);

   //         XmlNode tmpNodecs = doccs.SelectSingleNode("/Valore");

   //         XmlNode importedNodecs = _x.xdp.Document.ImportNode(tmpNodecs, true);

   //         _x.Document.SelectSingleNode("//Dati/Dato[@ID" + _ID + "]").AppendChild(importedNodecs);
			
   //     }

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            AggiungiNodo("",  "");
        }

        private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            gtCompensazioni.DeleteRow();          
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
        
		private void txtPeriodoDiRiferimento_LostFocus(object sender, RoutedEventArgs e)
		{
            if ( tabControl.SelectedItem == null )
            {
                return;
            }

            txtCreditoEsistente.Text = cBusinessObjects.ConvertNumber(txtCreditoEsistente.Text);
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["periodo"].ToString()==((TabItem)(tabControl.SelectedItem)).Header.ToString())
                {
                    dtrow["txtCreditoEsistente"] = txtCreditoEsistente.Text;
                }
            }
            
            gtCompensazioni.GenerateTable();
        }

      

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            string head = "";
            if (e.RemovedItems.Count > 0)
            {

                head = ((TabItem)tabControl.Items[CurrentTabSelectedIndex]).Header.ToString();
                foreach (DataRow dtrow in dati.Rows)
                {
                    if (dtrow["periodo"].ToString() == head)
                    {
                        dtrow["txtCreditoEsistente"] = txtCreditoEsistente.Text;
                    }
                }
                
            }



            if (e.AddedItems.Count > 0 && (e.AddedItems[0]).GetType().Name == "TabItem")
			{
                if (((string)(((TabItem)(e.AddedItems[0])).Header)) == App.NewTabHeaderText)
				{
					if (_ReadOnly)
					{
						MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
						return;
					}

					var dialog = new wInputBox("Inserire Nome del nuovo tributo");
					dialog.ShowDialog();

					string newHeader = dialog.ResponseText;

					if (newHeader == "")
					{
						MessageBox.Show("Attenzione, tributo non valido");
						tabControl.SelectedIndex = 0;
						return;
					}

					foreach (TabItem item in tabControl.Items)
					{
						if (((string)(item.Header)) == newHeader)
						{
							MessageBox.Show("Attenzione, tributo già esistente");
							tabControl.SelectedIndex = 0;
							return;
						}
					}

					TabItem ti = new TabItem();
					ti.Header = newHeader;

              
                    tabControl.Items.Insert(tabControl.Items.Count - 1, ti);
                    tabControl.SelectedIndex = tabControl.Items.Count - 2;

                    gtCompensazioni.filtercolumn = "periodo";
                    gtCompensazioni.filtervalue = newHeader;
                    gtCompensazioni.GenerateTable();

              

				}
				else
				{
                    head = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["periodo"].ToString() == head)
                        {
                            txtCreditoEsistente.Text = dtrow["txtCreditoEsistente"].ToString();
                        }
                    }
                    gtCompensazioni.filtercolumn = "periodo";
                    gtCompensazioni.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCompensazioni.GenerateTable();
             
				}

			}
            CurrentTabSelectedIndex = tabControl.SelectedIndex;
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

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

			if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				string newHeader = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
                for (int i = dati.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati.Rows[i];
                    if (dtrow["periodo"].ToString() == newHeader)
                        dtrow.Delete();
                }
 

                dati.AcceptChanges();
                

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
            if (!dialog.diagres)
            {
                return;
            }
            string newHeader =dialog.ResponseText;

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
            gtCompensazioni.filtercolumn = "periodo";
            gtCompensazioni.filtervalue = newHeader;
            gtCompensazioni.GenerateTable();

         
		}

		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["periodo"] != null)
                {
                    if (dtrow["periodo"].ToString() == oldheader)
                    {
                        dtrow["periodo"] = newname;
                    }
                }

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


                tabItemTarget.Header = sourceHeader;
                tabItemSource.Header = targetHeader;

                DataTable datiC = dati.Clone();
                foreach (TabItem tb in tabControl.Items)
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["periodo"] != null)
                        {
                            if (dtrow["periodo"].ToString() == tb.Header.ToString())
                            {
                                DataRow firstNewRow = datiC.NewRow();
                                firstNewRow.ItemArray = dtrow.ItemArray;
                                datiC.Rows.Add(firstNewRow);
                            }
                        }
                    }
                }
                dati = datiC;
                gtCompensazioni.filtercolumn = "periodo";
                gtCompensazioni.filtervalue = targetHeader;
                gtCompensazioni.GenerateTable();
                    
			}
		}
    }
}
