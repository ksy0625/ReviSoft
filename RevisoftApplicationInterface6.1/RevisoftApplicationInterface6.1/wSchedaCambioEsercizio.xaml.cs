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
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;
using UserControls;

namespace RevisoftApplication
{
  public partial class wSchedaCambioEsercizio : Window
  {
    public wSchedaCambioEsercizio()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      //interfaccia 
      ConfiguraMaschera();
    }

    public void ConfiguraMaschera()
    {
    }


    private void buttonSOSPESI_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();

      wSchedaAnafrafica shere = ((wSchedaAnafrafica)(this.Owner));

      int idCliente = shere.idRecord;
      if (IntermedioDa.Text == "")
      {
        MessageBox.Show("Inserire Valore Periodo Intermedio");
        return;
      }

      try
      {
        DateTime da = Convert.ToDateTime(IntermedioDa.Text.Trim());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaCambioEsercizio.buttonSOSPESI_Click1 exception");
        string log = ex.Message;
        MessageBox.Show("Inserire Valore Periodo Intermedio Valido");
        return;
      }

      if (IntermedioA.Text == "")
      {
        MessageBox.Show("Inserire Valore Periodo Intermedio");
        return;
      }

      try
      {
        DateTime da = Convert.ToDateTime(IntermedioA.Text.Trim());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaCambioEsercizio.buttonSOSPESI_Click2 exception");
        string log = ex.Message;
        MessageBox.Show("Inserire Valore Periodo Intermedio Valido");
        return;
      }

      if (txtEsercizioDal.Text == "")
      {
        MessageBox.Show("Inizio Esercizio mancante.");
        return;
      }

      if (txtEsercizioAl.Text == "")
      {
        MessageBox.Show("Fine Esercizio mancante");
        return;
      }

      if (rdbEsercizioAcavallo.IsChecked == true)
      {
        try
        {
          //calcola la durata ipotetica del periodo in esame
          TimeSpan ts = Convert.ToDateTime(txtEsercizioAl.Text.Trim() + "/2013").Subtract(Convert.ToDateTime(txtEsercizioDal.Text.Trim() + "/2012"));
          if (ts.Days != 364)
          {
            MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
            return;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaCambioEsercizio.buttonSOSPESI_Click3 exception");
          string log = ex.Message;
          MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
          return;
        }
      }

      shere.txtEsercizioAl.Text = txtEsercizioAl.Text;
      shere.txtEsercizioDal.Text = txtEsercizioDal.Text;

      shere.rdbEsercizioAcavallo.IsChecked = rdbEsercizioAcavallo.IsChecked;
      shere.rdbEsercizioSolare.IsChecked = rdbEsercizioSolare.IsChecked;

      Hashtable ht = new Hashtable();

      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRevisioneIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetBilancioIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetConclusioneIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);


      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRelazioneBIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRelazioneBCIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRelazioneVIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRelazioneVCIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      ht.Clear();
      ht.Add("Cliente", idCliente);
      ht.Add("Data", IntermedioDa.Text);
      ht.Add("Note", "Periodo intermedio per cambio esercizio dal " + IntermedioDa.Text + " al " + IntermedioA.Text);

      mf.SetRelazioneBVIntermedio(ht, idCliente, IntermedioDa.Text, IntermedioA.Text);

      base.Close();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
