using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RevisoftApplication.BRL
{
	public class ClienteComparer : IEqualityComparer<Cliente>
	{
		public bool Equals(Cliente x, Cliente y)
		{
			if (string.Compare(x.ID, y.ID, true) == 0)
				return true;
			else
				return false;
		}

		public int GetHashCode(Cliente cli)
		{
			if (Object.ReferenceEquals(cli, null)) return -1;
			int hashCod = cli.ID.GetHashCode();
			return hashCod;
		}
	}

  public class Cliente
  {
    public string ID { get; set; }
    public string RagioneSociale { get; set; }
    public string CodiceFiscale { get; set; }
    public string PartitaIVA { get; set; }
    public string EsercizioAl { get; set; }
    public string EsercizioDal { get; set; }
    public string Esercizio { get; set; }
    public string Stato { get; set; }
    public string MembroEffettivo { get; set; }
    public string MembroEffettivo2 { get; set; }
    public string Note { get; set; }
    public string OrganoDiControllo { get; set; }
    public string OrganoDiRevisione { get; set; }
    public string Presidente { get; set; }
    public string RevisoreAutonomo { get; set; }
    public string SindacoSupplente { get; set; }
    public string SindacoSupplente2 { get; set; }

  }

	public class ClientePerGriglia
	{
		public string ID { get; set; }
		public string RagioneSociale { get; set; }
		public string CodiceFiscale { get; set; }
		public string PartitaIVA { get; set; }
		public string Stato { get; set; }
		public short AssociatoValue { get; set; }
		public bool AssociatoLeader { get; set; }
		public string Leader { get; set; }

		public ClientePerGriglia  (DataRow row)
		{
			ID = Convert.ToString(row["ID"]);
			RagioneSociale = Convert.ToString(row["RagioneSociale"]);
			CodiceFiscale = Convert.ToString(row["CodiceFiscale"]);
			PartitaIVA = Convert.ToString(row["PartitaIVA"]);
			short statoInt = Convert.ToInt16(row["Stato"]);
			switch(statoInt)
			{
				case (int)App.TipoAnagraficaStato.Bloccato:
					Stato = "Bloccato";
					break;
				case (int)App.TipoAnagraficaStato.Disponibile:
					Stato = "Disponibile";
					break;
				case (int)App.TipoAnagraficaStato.Esportato:
					Stato = "Esportato";
					break;
				case (int)App.TipoAnagraficaStato.InUso:
                    //	Stato = "In Uso";
                    Stato = "Disponibile";
                    break;
				case (int)App.TipoAnagraficaStato.Sconosciuto:
					Stato = "Sconosciuto";
					break;
			}			
			AssociatoValue = Convert.ToInt16(row["ASSEGNATO"]);
			AssociatoLeader = AssociatoValue > 0 ? true : false;
			Leader = Convert.ToString(row["Leader"]);
		}

	}
	public class cCliente
  {

//    public static List<Cliente> GetClientiByIdUtente(int idUtente)
//    {
//      var query = $@"
//select mf.cliente.* 
//from mf.cliente 
//left join UTENTIXCLIENTE on CONVERT(varchar(10), UTENTIXCLIENTE.UXC_CLI_ID) = mf.cliente.id
//where UTENTIXCLIENTE.UXC_UTE_ID = {idUtente}";
//      var dsClienti = cDBManager.CaricaDataset(query);
//      if (dsClienti == null || dsClienti.Tables.Count <= 0 || dsClienti.Tables[0].Rows.Count <= 0)
//        return null;
//      var lista = new List<Cliente>();
//      foreach (DataRow rowCli in dsClienti.Tables[0].Rows)
//        lista.Add(new Cliente
//        {
//          ID = Convert.ToString(rowCli["ID"]),
//          RagioneSociale = Convert.ToString(rowCli["RagioneSociale"]),
//          CodiceFiscale = Convert.ToString(rowCli["CodiceFiscale"]),
//          PartitaIVA = Convert.ToString(rowCli["PartitaIVA"]),
//          EsercizioAl = Convert.ToString(rowCli["EsercizioAl"]),
//          EsercizioDal = Convert.ToString(rowCli["EsercizioDal"]),
//          Esercizio = Convert.ToString(rowCli["Esercizio"]),
//          Stato = Convert.ToString(rowCli["Stato"]),
//          MembroEffettivo = Convert.ToString(rowCli["MembroEffettivo"]),
//          MembroEffettivo2 = Convert.ToString(rowCli["MembroEffettivo2"]),
//          Note = Convert.ToString(rowCli["Note"]),
//          OrganoDiControllo = Convert.ToString(rowCli["OrganoDiControllo"]),
//          OrganoDiRevisione = Convert.ToString(rowCli["OrganoDiRevisione"]),
//          Presidente = Convert.ToString(rowCli["Presidente"]),
//          RevisoreAutonomo = Convert.ToString(rowCli["RevisoreAutonomo"]),
//          SindacoSupplente = Convert.ToString(rowCli["SindacoSupplente"]),
//          SindacoSupplente2 = Convert.ToString(rowCli["SindacoSupplente2"])
//        });
//        return lista;
//    }

		//public static List<Cliente> GetClientiByUtenteERuolo(int idUtente)
      public static List<Cliente> GetClientiByIdUtente(int idUtente, App.RuoloDesc ruolo)
      {
			List<Cliente> lista = new List<Cliente>();
			try
			{
				string query = "select distinct (mf.cliente.id), mf.cliente.* from mf.cliente left join UTENTIXCLIENTE on ";
				query += "CONVERT(varchar(10), UTENTIXCLIENTE.UXC_CLI_ID) = mf.cliente.id	";
				if (ruolo == App.RuoloDesc.Reviewer)
				{
					query += $"	where UTENTIXCLIENTE.UXC_REV_ID = {idUtente}";
				}
				else
					query += $"	where UTENTIXCLIENTE.UXC_UTE_ID = {idUtente}";
				var dsClienti = cDBManager.CaricaDataset(query);
				if (dsClienti == null || dsClienti.Tables.Count <= 0 || dsClienti.Tables[0].Rows.Count <= 0)
					return null;

				foreach (DataRow rowCli in dsClienti.Tables[0].Rows)
					lista.Add(new Cliente
					{
						ID = Convert.ToString(rowCli["ID"]),
						RagioneSociale = Convert.ToString(rowCli["RagioneSociale"]),
						CodiceFiscale = Convert.ToString(rowCli["CodiceFiscale"]),
						PartitaIVA = Convert.ToString(rowCli["PartitaIVA"]),
						EsercizioAl = Convert.ToString(rowCli["EsercizioAl"]),
						EsercizioDal = Convert.ToString(rowCli["EsercizioDal"]),
						Esercizio = Convert.ToString(rowCli["Esercizio"]),
						Stato = Convert.ToString(rowCli["Stato"]),
						MembroEffettivo = Convert.ToString(rowCli["MembroEffettivo"]),
						MembroEffettivo2 = Convert.ToString(rowCli["MembroEffettivo2"]),
						Note = Convert.ToString(rowCli["Note"]),
						OrganoDiControllo = Convert.ToString(rowCli["OrganoDiControllo"]),
						OrganoDiRevisione = Convert.ToString(rowCli["OrganoDiRevisione"]),
						Presidente = Convert.ToString(rowCli["Presidente"]),
						RevisoreAutonomo = Convert.ToString(rowCli["RevisoreAutonomo"]),
						SindacoSupplente = Convert.ToString(rowCli["SindacoSupplente"]),
						SindacoSupplente2 = Convert.ToString(rowCli["SindacoSupplente2"])
					});

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return lista;
		}

		public static List<ClientePerGriglia> GetClientiPerTeam(int idTeamLeader)
		{
			List<ClientePerGriglia> clienti = new List<ClientePerGriglia>();

			try
			{
				string campi = " ID,RagioneSociale,CodiceFiscale,PartitaIVA,Stato";
				string query = $"select {campi}, 0 as ASSEGNATO,'' as Leader from mf.cliente where id not in (select uxc_cli_id from UTENTIXCLIENTE where (uxc_ute_id <> {idTeamLeader} and uxc_rev_id = -1) OR UXC_UTE_ID = {idTeamLeader})";
				query += " union ";
				query += $"select {campi}, 1 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader from mf.Cliente left join UTENTIXCLIENTE on UXC_CLI_ID = mf.Cliente.ID inner join UTENTI on uxc_ute_id = ute_id where uxc_ute_id = {idTeamLeader}";
				query += " union ";
				query += $"select {campi}, 2 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader from mf.Cliente inner join UTENTIXCLIENTE on UXC_CLI_ID = ID and uxc_ute_id <> {idTeamLeader} and uxc_rev_id = -1 inner join UTENTI on uxc_ute_id = ute_id";

				DataSet ds = cDBManager.CaricaDataset(query);
				if (ds != null && ds.Tables.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
						clienti.Add(new ClientePerGriglia(row));
				}
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

			return clienti;
		}

		public static List<ClientePerGriglia> GetClientiPerRevisoreAutonomo(int idRevAutonomo)
		{
			List<ClientePerGriglia> clienti = new List<ClientePerGriglia>();

			try
			{
				string campi = " ID,RagioneSociale,CodiceFiscale,PartitaIVA,Stato";
				string query = $"select {campi}, 0 as ASSEGNATO,'' as Leader from mf.cliente where id not in (select uxc_cli_id from UTENTIXCLIENTE where (uxc_ute_id <> {idRevAutonomo} and uxc_rev_id = -1) OR UXC_UTE_ID = {idRevAutonomo})";
				query += " union ";
				query += $"select {campi}, 0 as ASSEGNATO,'' as Leader from mf.Cliente inner join UTENTIXCLIENTE on UXC_CLI_ID = ID and uxc_ute_id <> {idRevAutonomo} and uxc_rev_id = -1 and uxc_rev_Auto = 1 where id not in (select uxc_cli_id from UTENTIXCLIENTE where uxc_ute_id = {idRevAutonomo})";
				query += " union ";
				query += $"select {campi}, 1 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader from mf.Cliente left join UTENTIXCLIENTE on UXC_CLI_ID = mf.Cliente.ID inner join UTENTI on uxc_ute_id = ute_id where uxc_ute_id = {idRevAutonomo}";
				query += " union ";
				query += $"select {campi}, 2 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader from mf.Cliente inner join UTENTIXCLIENTE on UXC_CLI_ID = ID and uxc_ute_id <> {idRevAutonomo} and uxc_rev_id = -1 inner join UTENTI on uxc_ute_id = ute_id and ute_ruo_id = {(int)App.RuoloDesc.TeamLeader}";
				query += " union ";
				query += $"select {campi}, 3 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader from mf.Cliente inner join UTENTIXCLIENTE on UXC_CLI_ID = ID and uxc_ute_id <> {idRevAutonomo} and uxc_rev_id = -1 inner join UTENTI on uxc_ute_id = ute_id and ute_ruo_id = {(int)App.RuoloDesc.RevisoreAutonomo}";
				query += "  order by ASSEGNATO,RagioneSociale";
				DataSet ds = cDBManager.CaricaDataset(query);
				if (ds != null && ds.Tables.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
						clienti.Add(new ClientePerGriglia(row));
				}
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

			return clienti;
		}

		public static List<ClientePerGriglia> GetClientiPerTeamRiepilogo(int idTeamLeader)
		{
			List<ClientePerGriglia> clienti = new List<ClientePerGriglia>();
			try
			{
				string campi = "ID,RagioneSociale,CodiceFiscale,PartitaIVA,Stato, 1 as ASSEGNATO,UTE_NOME + ' ' + UTE_COGNOME as Leader";
				string query = $"select {campi} from mf.Cliente left join UTENTIXCLIENTE on UXC_CLI_ID = mf.Cliente.ID inner join UTENTI on uxc_ute_id = ute_id where uxc_ute_id = {idTeamLeader}";
				DataSet ds = cDBManager.CaricaDataset(query);
				if (ds != null && ds.Tables.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
						clienti.Add(new ClientePerGriglia(row));
				}
			}
			catch(Exception)
			{

			}
			return clienti;
		}

		public static bool ExistCliente(int idCliente)
		{
			object ret = null;
			try
			{
				string query = $"select id from mf.Cliente where id = {idCliente}";
				ret = cDBManager.EseguiComando(query);
			}
			catch(Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			if (ret == null)
				return false;
			else
				return true;
		}
	}
}
