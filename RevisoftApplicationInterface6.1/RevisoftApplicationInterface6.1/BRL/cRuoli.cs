using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace RevisoftApplication.BRL
{
    
   public class Ruolo
   {
      public short Id { get; set; }
      public string Descr{ get; set; }

      public Ruolo(DataRow row)
      {
         Id = Convert.ToInt16(row["RUO_ID"]);
         Descr = Convert.ToString(row["RUO_DESCR"]);
      }
}
   public class cRuoli
   {
      // gestione tabella ruoli
      public cRuoli() { }

      public static Dictionary<int,Ruolo> GetRuoli()
      {
         Dictionary<int, Ruolo> ruoli = null;
         string queryUtenti = "SELECT * FROM RUOLI ORDER BY RUO_DESCR";
         DataSet dsRuoli= cDBManager.CaricaDataset(queryUtenti);
         if (dsRuoli != null && dsRuoli.Tables != null && dsRuoli.Tables[0].Rows != null && dsRuoli.Tables[0].Rows.Count > 0)
         {
            ruoli = new Dictionary<int, Ruolo>(dsRuoli.Tables[0].Rows.Count);
            for (int i = 0; i < dsRuoli.Tables[0].Rows.Count; i++)
            {
               DataRow row = dsRuoli.Tables[0].Rows[i];
               ruoli[i] = new Ruolo(row);
            }
         }

         return ruoli;
      }

      public static Dictionary<int, Ruolo> GetRuoliCombo()
      {
         Dictionary<int, Ruolo> ruoli = null;
         string queryUtenti = string.Format("SELECT * FROM RUOLI WHERE RUO_ID <> {0} AND RUO_ID <> {1} ORDER BY RUO_ID",(int)App.RuoloDesc.Administrator,(int)App.RuoloDesc.StandAlone);
         DataSet dsRuoli = cDBManager.CaricaDataset(queryUtenti);
         if (dsRuoli != null && dsRuoli.Tables != null && dsRuoli.Tables[0].Rows != null && dsRuoli.Tables[0].Rows.Count > 0)
         {
            ruoli = new Dictionary<int, Ruolo>(dsRuoli.Tables[0].Rows.Count);
            for (int i = 0; i < dsRuoli.Tables[0].Rows.Count; i++)
            {
               DataRow row = dsRuoli.Tables[0].Rows[i];
               ruoli[i] = new Ruolo(row);
            }
         }

         return ruoli;
      }
      public static Dictionary<int, Ruolo> GetRuoliPerAdministrator()
      {
         Dictionary<int, Ruolo> ruoli = null;
         string queryUtenti = string.Format("SELECT * FROM RUOLI WHERE RUO_ID in ({0},{1})", (int)App.RuoloDesc.NessunRuolo, (int)App.RuoloDesc.TeamLeader, (int)App.RuoloDesc.StandAlone);
         DataSet dsRuoli = cDBManager.CaricaDataset(queryUtenti);
         if (dsRuoli != null && dsRuoli.Tables != null && dsRuoli.Tables[0].Rows != null && dsRuoli.Tables[0].Rows.Count > 0)
         {
            ruoli = new Dictionary<int, Ruolo>(dsRuoli.Tables[0].Rows.Count);
            for (int i = 0; i < dsRuoli.Tables[0].Rows.Count; i++)
            {
               DataRow row = dsRuoli.Tables[0].Rows[i];
               ruoli[i] = new Ruolo(row);
            }
         }

         return ruoli;
      }

		public static List<Ruolo> GetRuoliPerTeamLeader()
		{
			int nessuno = (int)App.RuoloDesc.NessunRuolo;
			int revisore = (int)App.RuoloDesc.Reviewer;
			int esecutore = (int)App.RuoloDesc.Esecutore;
			List<Ruolo> ruoliList = new List<Ruolo>();
			string query = string.Format("SELECT RUO_DESCR,RUO_ID FROM RUOLI WHERE RUO_ID IN ({0},{1},{2})", nessuno, revisore, esecutore);
			DataSet dsRuoli = cDBManager.CaricaDataset(query);
			if (dsRuoli != null && dsRuoli.Tables != null && dsRuoli.Tables[0].Rows != null && dsRuoli.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dsRuoli.Tables[0].Rows)
					ruoliList.Add(new Ruolo(row));					
			}
			return ruoliList;
		}

		public static List<Ruolo> GetRuoliPerLeader()
		{
			int nessuno = (int)App.RuoloDesc.NessunRuolo;
			int revisore = (int)App.RuoloDesc.Reviewer;
			int esecutore = (int)App.RuoloDesc.Esecutore;
			List<Ruolo> ruoliList = new List<Ruolo>();
			string query = string.Format("SELECT RUO_DESCR,RUO_ID FROM RUOLI WHERE RUO_ID IN ({0},{1},{2})", nessuno, revisore, esecutore);
			DataSet dsRuoli = cDBManager.CaricaDataset(query);
			if (dsRuoli != null && dsRuoli.Tables != null && dsRuoli.Tables[0].Rows != null && dsRuoli.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dsRuoli.Tables[0].Rows)
					ruoliList.Add(new Ruolo(row));
			}
			return ruoliList;
		}
		public static string DescrRuolo(App.RuoloDesc ruolo)
		{
			return DescrRuolo((int)ruolo);
		}
		public static string DescrRuolo(int id)
		{
			switch(id)
			{
				case (int)App.RuoloDesc.Administrator:
					return "amministratore";
				case (int)App.RuoloDesc.Esecutore:
					return "esecutore";
				case (int)App.RuoloDesc.NessunRuolo:
					return "nessun ruolo";
				case (int)App.RuoloDesc.Reviewer:
					return "revisore";
				case (int)App.RuoloDesc.StandAlone:
					return "stand alone";
				case (int)App.RuoloDesc.TeamLeader:
					return "team leader";
				default:
					return "ruolo non definito";

			}
		}

		public static bool IsStandAlone()
		{
      //return true;
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      object res = null;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      try
			{
				string query = "select RUO_ID from ruoli where ruo_id = 100";
        res = cDBManager.EseguiComando(query);
			}
			catch(Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			if (res != null)
				return true;
			else
				return false;
		}
   }
}
