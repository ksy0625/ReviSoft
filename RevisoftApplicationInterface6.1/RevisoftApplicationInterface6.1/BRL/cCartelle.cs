using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace RevisoftApplication.BRL
{

	public class Cartella : INotifyPropertyChanged
	{
		public Cartella(string codice, string titolo = null, int? esecutoreId = null, string esecutore = null)
		{
			GlobalPropertyChanged += HandleGlobalPropertyChanged;
			Figli = new List<Cartella>();
			Codice = codice ?? throw new ArgumentNullException();
			Titolo = titolo;
			isChecked = esecutoreId != null && esecutore != null;
			if (isChecked)
			{
				Esecutore = esecutore;
				EsecutoreId = esecutoreId;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };

		private static void OnGlobalPropertyChanged(string propertyName)
		{
			GlobalPropertyChanged(typeof(Cartella), new PropertyChangedEventArgs(propertyName));
		}

		private void HandleGlobalPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
		}

		public bool Add(Cartella discendente)
		{
			if (discendente == null || discendente.CodList == null || discendente.CodList.Length <= CodList.Length)
				return false;
			var mCodice = CodList;
			var dCodice = discendente.CodList;
			for (int i = 0; i < mCodice.Length; i++)
			{
				if (mCodice[i] != dCodice[i])
					return false;
			}
			var codFiglio = Codice + "." + dCodice[mCodice.Length];
			var figlio = Figli.Find(f => f.Codice == codFiglio);
			if (figlio == null)
			{
				if (dCodice.Length > mCodice.Length + 1)
					return false;
				discendente.Genitore = this;
				Figli.Add(discendente);
				return true;
			}
			return figlio.Add(discendente);
		}

		public Cartella Clone()
		{
			var clone = new Cartella(Codice, Titolo, EsecutoreId, Esecutore);
			Figli.ForEach(f => clone.Add(f.Clone()));
			return clone;
		}

		public Cartella GetCartelleByEsecutore(int idEsecutore)
		{
			var figliByEsecutore = new List<Cartella>();
			foreach (var figlio in Figli)
			{
				var figlioByEsecutore = figlio.GetCartelleByEsecutore(idEsecutore);
				if (figlioByEsecutore != null)
					figliByEsecutore.Add(figlioByEsecutore);
			}
			return figliByEsecutore.Count > 0 || EsecutoreId == idEsecutore ? new Cartella(Codice, Titolo, EsecutoreId, Esecutore) { Figli = figliByEsecutore } : null;
		}

		public Dictionary<string, bool> GetCodici()
		{
			var dic = new Dictionary<string, bool>();
			if (IsEnabled)
				dic[Codice] = isChecked;
			Figli.ForEach(f => dic = dic.Concat(f.GetCodici()).ToDictionary(x => x.Key, x => x.Value));
			return dic;
		}

		public static string EsecutoreSelected { get; set; }
		private static int? esecutoreIdSelected;
		public static int? EsecutoreIdSelected
		{
			get { return esecutoreIdSelected; }
			set
			{
				if (esecutoreIdSelected == value)
					return;
				esecutoreIdSelected = value;
				OnGlobalPropertyChanged("IsEnabled");
			}
		}

		public string Titolo { get; set; }

		public List<Cartella> Figli { get; private set; }

		public Cartella Genitore { get; set; }

		public string Codice { get; private set; }
		public string[] CodList { get { return Codice?.Split('.'); } }

		private bool isChecked;
		public bool IsChecked
		{
			get { return isChecked; }
			set { SetIsChecked(value, true, true); }
		}
		private void SetIsChecked(bool value, bool updateChildren, bool updateParent)
		{
			if (value == isChecked || EsecutoreIdSelected == null)
				return;
			isChecked = value;
			EsecutoreId = value ? EsecutoreIdSelected : null;
			Esecutore = value ? EsecutoreSelected : null;

			if (updateChildren)
				Figli.ForEach(f => f.SetIsChecked(isChecked, true, false));
			if (updateParent && Genitore != null)
				Genitore.SetIsCheckedGenitore();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAssegnato"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EsecutoreId"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Esecutore"));
		}
		private void SetIsCheckedGenitore()
		{
			var stato = Figli.All(f => f.IsChecked && f.EsecutoreId == EsecutoreIdSelected);
			SetIsChecked(stato, false, true);
		}

		public string Esecutore { get; private set; }
		public int? EsecutoreId { get; private set; }
		public bool IsAssegnato { get { return EsecutoreId != null; } }

		public bool IsEnabled
		{
			get
			{
				if (EsecutoreIdSelected == null)
					return false;
				return (EsecutoreId == null || EsecutoreId == EsecutoreIdSelected) && Figli.All(f => f.IsEnabled);
			}
		}

	}

	public class AreaCartella
	{
		public string Codice { get; set; }
		public string Titolo { get; set; }
	}

	public class cCartelle
	{

		public static List<AreaCartella> GetAree()
		{
			return new List<AreaCartella>
			{
			  //new AreaCartella { Codice = "1", Titolo = "Area 1" },
			  //new AreaCartella { Codice = "ISQC", Titolo = "Area ISQC" },
			  new AreaCartella { Codice = "2", Titolo = "Area 2 - Comprensione - Rischio - Pianificazione controllo bilancio" },
			  new AreaCartella { Codice = "3", Titolo = "Area 3 - Controllo del bilancio" },
			  new AreaCartella { Codice = "4", Titolo = "Area 4 - Verifiche per controllo contabile" },
			  new AreaCartella { Codice = "5", Titolo = "Area 5 - Verifiche per attivita di vigilanza" },
			  new AreaCartella { Codice = "6", Titolo = "Area 6 - Pianificazione controllo contabile" },
			  new AreaCartella { Codice = "7", Titolo = "Area 7 - Pianificazione attività di vigilanza" },
			  new AreaCartella { Codice = "9", Titolo = "Area 9 - Conclusioni" }
			};
		}

		public static List<Cartella> GetCartelleByCliente(string idCliente)
		{

			var query = $@"select * from 
	RemapTreeNodeCodici b left join
	(select RemapTreeNodeCodici.*,UTENTI.UTE_LOGIN, UTENTI.UTE_ID, UTE_NOME, UTE_COGNOME, UXC_REV_ID from RemapTreeNodeCodici
	left join CARTELLEXCLIENTE on RemapTreeNodeCodici.Codice = CARTELLEXCLIENTE.CXC_COD_ID
	inner join UTENTIXCLIENTE on CARTELLEXCLIENTE.CXC_UXC_ID = UTENTIXCLIENTE.UXC_ID and UTENTIXCLIENTE.UXC_CLI_ID =  {idCliente}

	
	left join UTENTI on UTENTIXCLIENTE.UXC_UTE_ID = UTENTI.UTE_ID and UTENTI.UTE_RUO_ID = {(int)App.RuoloDesc.Esecutore}
	) A
	 on b.codice = A.codice	
	 INNER JOIN TITOLI ON b.CODICE = TITOLI.TIT_CODICE 
	order by b.padded
";

			var dsCartelle = cDBManager.CaricaDataset(query);
			if (dsCartelle == null || dsCartelle.Tables.Count <= 0 || dsCartelle.Tables[0].Rows.Count <= 0)
				return null;
			var lista = new List<Cartella>();
			foreach (DataRow row in dsCartelle.Tables[0].Rows)
			{
				var codice = Convert.ToString(row["Codice"]);
				var titolo = Convert.ToString(row["TIT_TITOLO"]);
				int? idUtente = null;
				try { idUtente = Convert.ToInt32(row["UTE_ID"]); } catch { }
				string utente = null;
				try
				{
					var login = Convert.ToString(row["UTE_LOGIN"]);
					var nome = Convert.ToString(row["UTE_NOME"]);
					var cognome = Convert.ToString(row["UTE_COGNOME"]);
					utente = $"{login} - {nome} {cognome}";
				}
				catch { }
				lista.Add(new Cartella(codice, titolo, idUtente, utente));
			}
			lista = lista.OrderBy(cartella => cartella.CodList == null ? 0 : cartella.CodList.Length).ToList();
			var ret = new List<Cartella>();
			foreach (var cartella in lista)
			{
				if (cartella.CodList == null || cartella.CodList.Length == 0)
					continue;
				if (cartella.CodList.Length == 1)
				{
					ret.Add(cartella);
					continue;
				}
				foreach (var padre in ret)
				{
					if (!padre.Add(cartella))
						continue;
				}
			}
			return ret;
		}

		//		public static List<Cartella> GetCartelleByClienteEUtente(string idCliente)
		//		{
		//			var query = $@"
		//select RemapTreeNodeCodici.*,TITOLI.TIT_TITOLO, UTENTI.UTE_LOGIN, UTENTI.UTE_ID, UTE_NOME, UTE_COGNOME
		//from RemapTreeNodeCodici
		//left join CARTELLEXCLIENTE on RemapTreeNodeCodici.Codice = CARTELLEXCLIENTE.CXC_COD_ID
		//left join UTENTIXCLIENTE on CARTELLEXCLIENTE.CXC_UXC_ID = UTENTIXCLIENTE.UXC_ID and UTENTIXCLIENTE.UXC_CLI_ID = {idCliente}
		//left join UTENTI on UTENTIXCLIENTE.UXC_UTE_ID = UTENTI.UTE_ID and UTENTI.UTE_RUO_ID = {(int)App.RuoloDesc.Esecutore}
		//INNER JOIN TITOLI ON RemapTreeNodeCodici.CODICE = TITOLI.TIT_CODICE
		//order by RemapTreeNodeCodici.padded
		//";
		//			var dsCartelle = cDBManager.CaricaDataset(query);
		//			if (dsCartelle == null || dsCartelle.Tables.Count <= 0 || dsCartelle.Tables[0].Rows.Count <= 0)
		//				return null;
		//			var lista = new List<Cartella>();
		//			foreach (DataRow row in dsCartelle.Tables[0].Rows)
		//			{
		//				var codice = Convert.ToString(row["Codice"]);
		//				var titolo = Convert.ToString(row["TIT_TITOLO"]);
		//				int? idUtente = null;
		//				try { idUtente = Convert.ToInt32(row["UTE_ID"]); } catch { }
		//				string utente = null;
		//				try
		//				{
		//					var login = Convert.ToString(row["UTE_LOGIN"]);
		//					var nome = Convert.ToString(row["UTE_NOME"]);
		//					var cognome = Convert.ToString(row["UTE_COGNOME"]);
		//					utente = $"{login} - {nome} {cognome}";
		//				}
		//				catch { }
		//				lista.Add(new Cartella(codice, titolo, idUtente, utente));
		//			}
		//			lista = lista.OrderBy(cartella => cartella.CodList == null ? 0 : cartella.CodList.Length).ToList();
		//			var ret = new List<Cartella>();
		//			foreach (var cartella in lista)
		//			{
		//				if (cartella.CodList == null || cartella.CodList.Length == 0)
		//					continue;
		//				if (cartella.CodList.Length == 1)
		//				{
		//					ret.Add(cartella);
		//					continue;
		//				}
		//				foreach (var padre in ret)
		//				{
		//					if (!padre.Add(cartella))
		//						continue;
		//				}
		//			}
		//			return ret;
		//		}

		public static bool UpsertCartelleCliente(string idCliente, int? idEsecutore, Dictionary<string, bool> codici)
		{
			if (idCliente == null || idEsecutore == null || codici == null || codici.Keys.Count == 0)
				return false;
			var codiciDaRimuovere = string.Join(", ", codici.Keys.Select(k => $"'{k}'"));
			var codiciDaInserire = codici.Where(p => p.Value).Select(p => p.Key);
			using (var ts = new TransactionScope())
			{
				using (SqlConnection conn = cDBManager.GetNewConnection())
				{
					conn.Open();
					var query = $@"
	delete from CARTELLEXCLIENTE
	where CXC_COD_ID in ({codiciDaRimuovere})
	and CXC_UXC_ID in (select UXC_ID
		from UTENTIXCLIENTE
		where UXC_CLI_ID = '{idCliente}'
		and UXC_UTE_ID = {idEsecutore}) 
	";
					try
					{
						cDBManager.EseguiComando(query, conn);
					}
					catch (Exception ex)
					{
						conn.Close();
						App.GestioneLog(ex.Message);
						return false;
					}
					query = @"
	insert into CARTELLEXCLIENTE(CXC_COD_ID, CXC_UXC_ID)
	select '{0}', UXC_ID
	from UTENTIXCLIENTE
	where UXC_CLI_ID = '{1}'
	and UXC_UTE_ID = {2}
	";
					string queryesecutore;
					foreach (var codice in codiciDaInserire)
					{
						try
						{
							queryesecutore = string.Format(query, codice, idCliente, idEsecutore);
							cDBManager.EseguiComando(queryesecutore, conn);
						}
						catch (Exception ex)
						{
							conn.Close();
							App.GestioneLog(ex.Message);
							return false;
						}
					}
					conn.Close();
				}
				ts.Complete();
			}
			return true;
		}

		public static bool EsisteCartellaPerEsecutoreDiRevisore(int idCliente, int idRevisore, string codice)
		{
			int numcartelle = 0;
			try
			{
				string query = "select count(*) from CARTELLEXCLIENTE inner join UTENTIXCLIENTE on CXC_UXC_ID = UXC_ID ";
				query += $"where UXC_CLI_ID = {idCliente} and UXC_REV_ID = {idRevisore} and CXC_COD_ID = '{codice}'";
				object ret = cDBManager.EseguiComando(query);
				if (ret != null) numcartelle = Convert.ToInt32(ret);
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return numcartelle > 0 ? true : false;
		}

		public static bool EsisteCartellaPerEsecutore(int idCliente, int idEsecutore, string codice)
		{
			int numcartelle = 0;
			try
			{
				string query = "select count(*) from CARTELLEXCLIENTE inner join UTENTIXCLIENTE on CXC_UXC_ID = UXC_ID ";
				query += $"where UXC_CLI_ID = {idCliente} and UXC_UTE_ID = {idEsecutore} and CXC_COD_ID = '{codice}'";
				object ret = cDBManager.EseguiComando(query);
				if (ret != null) numcartelle = Convert.ToInt32(ret);
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return numcartelle > 0 ? true : false;
		}

		public static RiepilogoItem CartellaToRiepilogoItem(Cartella cartella)
		{
			if (cartella == null)
				return null;
			var item = new RiepilogoItem { Etichetta = $"{cartella.Codice} {cartella.Titolo}" };
			cartella.Figli.ForEach(f => item.Figli.Add(CartellaToRiepilogoItem(f)));
			return item;
		}

		public static void BloccaCartella(string codice, int idRevisore, string idCliente)
		{
			try
			{

				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("idCliente", idCliente);
				param.Add("idRevisore", idRevisore.ToString());
				param.Add("codice", codice);
				
				cDBManager.CaricaDatasetDaStoredProcedure("SP_InsertCartellaBloccata", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

		}

		public static void SbloccaCartella(string codice, int idRevisore, string idCliente)
		{
			try
			{
				string query = $"delete from CARTELLE_BLOCCATE where CBL_CXC_ID = (select CXC_ID from CARTELLEXCLIENTE INNER JOIN UTENTIXCLIENTE ON CXC_UXC_ID = UXC_ID and UXC_REV_ID = {idRevisore} ";
				query += $"where UXC_CLI_ID = {idCliente} and CXC_COD_ID = '{codice}')";
				cDBManager.EseguiComando(query);
			}
			catch(Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

		}

		public static bool IsCartellaBloccata(string codice, int idUtente, string idCliente, bool revisore)
		{
			object resp = null;
			try
			{
				string query = $"select CBL_ID from CARTELLE_BLOCCATE inner join CARTELLEXCLIENTE on  CBL_CXC_ID = CXC_ID INNER JOIN UTENTIXCLIENTE ON CXC_UXC_ID = UXC_ID and ";
				if (revisore)
					query += $" UXC_REV_ID = {idUtente} ";
				else
					query += $" UXC_UTE_ID = {idUtente} ";
				query += $"where UXC_CLI_ID = {idCliente} and CXC_COD_ID = '{codice}'";
				resp = cDBManager.EseguiComando(query); 				
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			if (resp != null)
				return true;
			else
				return false;
		}
	}

	public class RiepilogoItem
	{
		public string Etichetta { get; set; }
		public bool IsEsecutore { get; set; }
		public bool IsRevisore { get; set; }
		public bool IsArea { get; set; }
		public bool IsTeam { get; set; }
		public bool IsCliente { get; set; }
		public bool IsLeader { get; set; }
		public bool IsNonAssegnato { get; set; }
		public List<RiepilogoItem> Figli { get; private set; }
		public RiepilogoItem()
		{
			Figli = new List<RiepilogoItem>();
		}
	}
}
