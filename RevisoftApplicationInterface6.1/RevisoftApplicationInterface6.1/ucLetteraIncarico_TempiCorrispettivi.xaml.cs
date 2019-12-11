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
	public partial class ucLetteraIncarico_TempiCorrispettivi : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datiVigilanza = null;

        public WindowWorkArea Owner;

      

		private bool _ReadOnly = false;

        public ucLetteraIncarico_TempiCorrispettivi()
        {
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
               new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch(Exception e)
            {
               
            }
         
         
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

         

            dati = cBusinessObjects.GetData(id, typeof(TempiCorrispettivi));
            if(dati.Rows.Count==0)
            {
              dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            }
            datiVigilanza = cBusinessObjects.GetData(id, typeof(TempiCorrispettiviVigilanza));

            foreach (DataRow dtrow in dati.Rows)
            {
                if (String.IsNullOrEmpty(dtrow["Incipit"].ToString()))
                    txtIncipit.Text = "";
                else
                    txtIncipit.Text = dtrow["Incipit"].ToString();
            }

            Binding b = new Binding();
            b.Source = dati;
		    dtgTempiCorrispettivi.SetBinding( ItemsControl.ItemsSourceProperty, b );

            if(datiVigilanza.Rows.Count == 0)
            {
                MasterFile mf = MasterFile.Create();
                Hashtable n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));

                if(n.Contains("OrganoDiRevisione") && n.Contains("RevisoreAutonomo") && n.Contains("OrganoDiControllo"))
                {
                    if(n["OrganoDiRevisione"].ToString() == "2" || n["OrganoDiRevisione"].ToString() == "3")
                    {
                        AggiungiNodoVigilanza(n["RevisoreAutonomo"].ToString(), "Revisore");
                    }
                    else
                    {
                        if (n["OrganoDiControllo"].ToString() == "2")
                        {
                            if (ID == "2016161")
                            {
                                MessageBox.Show("Composizione Organo diverso da quello presente nel Cliente.", "Attenzione");
                                this.Owner.ConsentiChiusuraFinestra = true;

                                this.Owner.Close();
                                return;
                            }
                            else
                            {
                                AggiungiNodoVigilanza(n["Presidente"].ToString(), "Sindaco Unico");
                            }
                        }

                        if (n["OrganoDiControllo"].ToString() == "1")
                        {
                            if (ID == "161")
                            {
                                MessageBox.Show("Composizione Organo diverso da quello presente nel Cliente.", "Attenzione");
                                this.Owner.ConsentiChiusuraFinestra = true;

                                this.Owner.Close();
                                return;
                            }
                            else
                            {
                                AggiungiNodoVigilanza(n["Presidente"].ToString(), "Presidente");
                                AggiungiNodoVigilanza(n["MembroEffettivo"].ToString(), "Membro Effettivo");
                                AggiungiNodoVigilanza(n["MembroEffettivo2"].ToString(), "Membro Effettivo");
                            }
                        }
                    }
                }                
            }            

            Binding c = new Binding();
            c.Source = datiVigilanza;
            dtgTempiCorrispettiviVigilanza.SetBinding(ItemsControl.ItemsSourceProperty, c);            

        }

		public int Save()
		{
            foreach (DataRow dtrow in this.dati.Rows)
            {
                dtrow["Incipit"] = txtIncipit.Text;
            }
            cBusinessObjects.SaveData(id, dati, typeof(TempiCorrispettivi));
            return cBusinessObjects.SaveData(id, datiVigilanza, typeof(TempiCorrispettiviVigilanza));
        }


        private void AggiungiNodoVigilanza(string Professionista, string Qualifica)
        {
            datiVigilanza.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione, Professionista, Qualifica);

        }

        private void AggiungiNodo(string Alias,  string Codice)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Codice);
            GenerateTotal();


                //_x.Save();

            //dtgCapitaleSociale.Items.Refresh();

        }

        private void DeleteTotal()
        {

            foreach (DataRow dtrow in this.dati.Rows)
            {
                if (dtrow["name"].ToString()== "Totale")
                {
                    dtrow.Delete();
                    break;
                }
            }
            this.dati.AcceptChanges();
        }

      

        private void GenerateTotal()
        {
            if (!_ReadOnly && dati.Rows.Count == 0)
            {
                return;
            }

            DeleteTotal();

			//double importoPagato = 0.0;
            double onorariototale = 0.0;
            int oretot = 0;
           
            foreach (DataRow dtrow in dati.Rows)
            {
         

                double ore = 0.0;
                double tariffaora = 0.0;
                double onorario= 0.0;

                double.TryParse(dtrow["ore"].ToString(), out ore );
                double.TryParse(dtrow["tariffaoraria"].ToString(), out tariffaora );

                onorario = ore * tariffaora;
                oretot += Convert.ToInt32(ore);
                dtrow["onorario"] =onorario;

                onorariototale += onorario;
            }

            DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "", "Totale");
            dd["ore"]= oretot;
            dd["onorario"]= onorariototale;

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

        private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTotal();
        }

        private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            GenerateTotal();

            //_x.Save();
        }

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {

            AggiungiNodo("", "");
            GenerateTotal();
        }

        private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				

				if (dtgTempiCorrispettivi.SelectedCells.Count >0)
				{
					
				}
				else
				{
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.dati.Rows)
                    {
                        if (k == dtgTempiCorrispettivi.Items.IndexOf(dtgTempiCorrispettivi.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }
                        k++;
                    }
                    this.dati.AcceptChanges();
                    GenerateTotal();

					//_x.Save();

					return;
				}
				catch (Exception ex)
				{
					string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
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

		private void txtIncipit_LostFocus(object sender, RoutedEventArgs e)
		{
            foreach (DataRow dtrow in this.dati.Rows)
            {
                dtrow["Incipit"] = txtIncipit.Text;
            }

           
		}

    }
}
