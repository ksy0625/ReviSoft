using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography;

namespace RevisoftApplication.BRL
{
	public class UtenteGriglia : Utente
	{

		//public int? IdParentInTeam { get; set; }
		//public int? RuoloId { get; set; }
		//public int? IdInTeam { get; set; }
		public bool InTeam { get; set; }
		public bool ReadOnly { get; set; }
		public string TeamLeader { get; set; }

		public UtenteGriglia() { }
		public UtenteGriglia(DataRow row, bool inteam, bool readOnly):base(row)
		{				
			InTeam = inteam;
			ReadOnly = readOnly;
			TeamLeader = "";
		}
	}
	public class Utente
	{
		public int Id { get; set; }
		public short RuoId { get; set; }
		public string RuoDescr { get; set; }
		public string Login { get; set; }
		public string Psw { get; set; }
		public string Nome { get; set; }
		public string Cognome { get; set; }
		public string Descr { get; set; }
		public int UtePadre { get; set; }

		private const string REVISOFT_CRIPT_STRONG_KEY = "ambararabacicicocotrecivettesulcomochefacevanolamoreconlafigliadeldottore";
		public Utente() { }
		public Utente(DataRow row)
		{
			Initialize(row);
		}

		public Utente(DataRow row,bool decrypt)
		{
			Initialize(row);
			
			if (decrypt)
				DecryptPSW();
		}



		public void EncriptPSW()
		{
			string plainText = this.Psw;
			string PasswordHash = "P@@Sw0rd";
			string SaltKey = "S@LT&KEY";
			string VIKey = "@1B2c3D4e5F6g7H8";

			byte[] plainTextBytes = Encoding.Unicode.GetBytes(plainText);

			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

			byte[] cipherTextBytes;

			using (var memoryStream = new System.IO.MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			this.Psw =  Convert.ToBase64String(cipherTextBytes);
		}

		public void DecriptPSW()
		{
			string encryptedText = this.Psw;
			string PasswordHash = "P@@Sw0rd";
			string SaltKey = "S@LT&KEY";
			string VIKey = "@1B2c3D4e5F6g7H8";

			byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			var memoryStream = new System.IO.MemoryStream(cipherTextBytes);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];

			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			this.Psw = Encoding.Unicode.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}
		public void EncryptPSW()
		{
			byte[] keyBytes;
			keyBytes = Encoding.Unicode.GetBytes(REVISOFT_CRIPT_STRONG_KEY);
			Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(REVISOFT_CRIPT_STRONG_KEY, keyBytes);
			RijndaelManaged rijndaelCSP = new RijndaelManaged();
			rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
			rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
			ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();
			byte[] encrypted = Encoding.Unicode.GetBytes(this.Psw);
			byte[] cipherTextBytes;
			using (var memoryStream = new System.IO.MemoryStream())
			{
				using (var encryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					encryptStream.Write(encrypted, 0, encrypted.Length);
					encryptStream.FlushFinalBlock();
					rijndaelCSP.Clear();
					encryptStream.Close();
					cipherTextBytes = memoryStream.ToArray();
					encryptStream.Close();
				}
				memoryStream.Close();
			}

			this.Psw = Convert.ToBase64String(cipherTextBytes);
			//CryptoStream encryptStream =
			//  new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
			//encryptStream.Write(encrypted, 0, encrypted.Length);
			//encryptStream.FlushFinalBlock();
			//rijndaelCSP.Clear();
			//encryptStream.Close();
			//outputFileStream.Close();

			/*private static string Encrypt(string plainText)
        {
            string PasswordHash = "P@@Sw0rd";
            string SaltKey = "S@LT&KEY";
            string VIKey = "@1B2c3D4e5F6g7H8";

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }*/
		}

		public void DecryptPSW()
		{
			byte[] keyBytes = Encoding.Unicode.GetBytes(REVISOFT_CRIPT_STRONG_KEY);
			Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(REVISOFT_CRIPT_STRONG_KEY, keyBytes);
			RijndaelManaged rijndaelCSP = new RijndaelManaged();
			rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
			rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
			ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();
			byte[] cipherTextBytes = Convert.FromBase64String(this.Psw);
			var memoryStream = new System.IO.MemoryStream(cipherTextBytes);

			bool ok = true;
			using (CryptoStream decryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
			{
				byte[] inputFileData = new byte[(int)memoryStream.Length];
				try
				{
					decryptStream.Read(inputFileData, 0, (int)memoryStream.Length);
				}
				catch (Exception ee)
				{
					decryptStream.Dispose();
					App.GestioneLog(ee.Message);
				}
				if (ok)
				{
					this.Psw = Encoding.Unicode.GetString(inputFileData).Replace("\0","");
					decryptStream.Close();
				}
			}
			rijndaelCSP.Clear();
			memoryStream.Close();


			//	public static string Decrypt(string encryptedText)
			//{
			//	string PasswordHash = "P@@Sw0rd";
			//	string SaltKey = "S@LT&KEY";
			//	string VIKey = "@1B2c3D4e5F6g7H8";

			//	byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
			//	byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			//	var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

			//	var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			//	var memoryStream = new MemoryStream(cipherTextBytes);
			//	var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			//	byte[] plainTextBytes = new byte[cipherTextBytes.Length];

			//	int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			//	memoryStream.Close();
			//	cryptoStream.Close();
			//	return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
			//}

		}

		private void Initialize(DataRow row)
		{
			Id = Convert.ToInt32(row["UTE_ID"]);
			RuoId = Convert.ToInt16(row["UTE_RUO_ID"]);
			RuoDescr = Convert.ToString(row["RUO_DESCR"]);
			Login = Convert.ToString(row["UTE_LOGIN"]);
			Psw = Convert.ToString(row["UTE_PSW"]);
			Nome = Convert.ToString(row["UTE_NOME"]);
			Cognome = Convert.ToString(row["UTE_COGNOME"]);
			Descr = Convert.ToString(row["UTE_DESCR"]);
			UtePadre = Convert.ToInt16(row["UTE_UTE_ID"]);
		}
	}

	public class UtentexCartella:Utente
	{
		public int RevisoreID { get; set; }

		public UtentexCartella(DataRow row):base(row)
		{
			this.RevisoreID = Convert.ToInt32(row["UXC_REV_ID"]);
		}
	}

	public class cUtenti
	{
		public static Dictionary<int, Utente> GetUtenti()
		{
			Dictionary<int, Utente> utenti = null;
			string queryUtenti = "SELECT UTENTI.*,RUO_DESCR FROM ";
			queryUtenti += "UTENTI INNER JOIN RUOLI ON UTE_RUO_ID = RUOLI.RUO_ID ORDER BY UTE_LOGIN";
			//queryUtenti += string.Format("WHERE UTE_RUO_ID <> {0} ORDER BY UTE_LOGIN", (int)App.RuoloDesc.Administrator);

			DataSet dsUtenti = cDBManager.CaricaDataset(queryUtenti);
			if (dsUtenti != null && dsUtenti.Tables != null && dsUtenti.Tables[0].Rows != null && dsUtenti.Tables[0].Rows.Count > 0)
			{
				utenti = new Dictionary<int, Utente>(dsUtenti.Tables[0].Rows.Count);
				for (int i = 0; i < dsUtenti.Tables[0].Rows.Count; i++)
				{
					DataRow row = dsUtenti.Tables[0].Rows[i];
					utenti[i] = new Utente(row,true);
				}
			}

			return utenti;
		}

		//public static List<UtenteGriglia> GetUtentiGriglia_old(int ruoloId)
		//{
		//	List<UtenteGriglia> lista = null;
		//	string queryUtenti = "SELECT UTE_ID,UTE_LOGIN,UTE_NOME,UTE_COGNOME FROM UTENTI INNER JOIN RUOLIXUTENTI ON UTE_ID = RXU_UTE_ID ";
		//	queryUtenti += string.Format("WHERE RXU_RUO_ID = {0} ORDER BY UTE_LOGIN", ruoloId);
		//	DataSet dsUtenti = cDBManager.CaricaDataset(queryUtenti);
		//	if (dsUtenti != null && dsUtenti.Tables != null && dsUtenti.Tables[0].Rows != null && dsUtenti.Tables[0].Rows.Count > 0)
		//	{
		//		lista = new List<UtenteGriglia>();
		//		for (int i = 0; i < dsUtenti.Tables[0].Rows.Count; i++)
		//		{
		//			DataRow row = dsUtenti.Tables[0].Rows[i];
		//			lista.Add(new UtenteGriglia(row));
		//		}
		//	}
		//	return lista;
		//}

		public static List<Utente> GetUtentiPerTeamLeaderGriglia(int idTeamLeader)
		{
			List<Utente> lista = null;
			try
			{
				string query = string.Format("select * from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ute_id = {0} union ", idTeamLeader);
				query += string.Format("select* from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ute_id in (select ute_id from utenti where ute_ute_id = {0})", idTeamLeader);
				DataSet dsUtenti = cDBManager.CaricaDataset(query);

				if (dsUtenti != null && dsUtenti.Tables.Count > 0 && dsUtenti.Tables[0].Rows.Count > 0)
				{
					lista = new List<Utente>();
					foreach (DataRow row in dsUtenti.Tables[0].Rows)
					{
						lista.Add(new Utente(row));
					}
				}
			}
			catch(Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return lista;
		}

		/// <summary>
		/// restiusce l'elenco degli utenti suddividendoli nel modo seguente:
		/// se l'utente fa parte del team associato al  team leader passato nel parametro: InTeam = true e ReadOnly = false
		/// se l'utente fa parte di un altro team: InTeam = true e ReadOnly = true
		/// se l'utente non fa parte di alcun team: InTeam = false e ReadOnly = false
		/// </summary>
		/// <param name="idTeamLeader">UTE_ID per il team leader</param>
		/// <returns></returns>
		public static List<UtenteGriglia> GetUtentiGrigliaTeam(int idTeamLeader)
		{
			// utenti associati al team		

			Dictionary<int, UtenteGriglia> lista = GetTeamFromLeader(idTeamLeader);

			int admin = (int)App.RuoloDesc.Administrator;
			int alone = (int)App.RuoloDesc.StandAlone;
			int leader = (int)App.RuoloDesc.TeamLeader;
			int revioreAutonomo = (int)App.RuoloDesc.RevisoreAutonomo;
			//string queryForUtenti = string.Format("select UTENTI.*,ruo_descr from UTENTI inner join ruoli on UTE_RUO_ID = RUO_ID where ute_ruo_id not in ({0},{1},{2})",admin,alone,leader);
			string query = "select UTENTI.*,ruo_descr, LEADER.UTE_Nome + ' ' +LEADER.UTE_Cognome as leader_descr from UTENTI inner join ruoli on UTE_RUO_ID = RUO_ID ";
			query += $"left join utenti LEADER on utenti.ute_ute_id = leader.ute_id where utenti.ute_ruo_id not in ({admin},{alone},{leader},{revioreAutonomo})";
			
			DataSet ds = cDBManager.CaricaDataset(query);
			if (ds != null && ds.Tables.Count > 0)
			{
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					UtenteGriglia ute = new UtenteGriglia(row, false, false);
					if (lista.ContainsKey(ute.Id))
						continue;
					if (ute.UtePadre != -1)
					{
						ute.InTeam = true;
						ute.ReadOnly = true;
						ute.TeamLeader = Convert.ToString(row["leader_descr"]);
					}
					lista.Add(ute.Id, ute);
				}
			}

			return lista.Values.ToList();
		}

    public static List<Utente> GetUtentiByIdCliente(string idCliente)
    {
      var query = $@"
select u5.*, ruoli.RUO_DESCR
from (select u1.*
	from utenti u1
	where u1.ute_ruo_id = {(int)App.RuoloDesc.Reviewer}
	and	u1.ute_id in
		(select u2.ute_ute_id
		from utenti u2
		left join utentixcliente uxc1
		on u2.ute_id = uxc1.uxc_ute_id
		where u2.ute_ruo_id = {(int)App.RuoloDesc.Esecutore}
		and uxc1.uxc_cli_id = {idCliente})
	union
	select u3.*
	from utenti u3
	left join utentixcliente uxc2
	on u3.ute_id = uxc2.uxc_ute_id
	where u3.ute_ruo_id = {(int)App.RuoloDesc.Esecutore}
	and uxc2.uxc_cli_id = {idCliente}
	union
	select u4.*
	from utenti u4
	left join utentixcliente uxc3
	on u4.ute_id = uxc3.uxc_ute_id
	where u4.ute_ruo_id = {(int)App.RuoloDesc.TeamLeader}
	and uxc3.uxc_cli_id = {idCliente}) u5
left join ruoli 
on u5.ute_ruo_id = ruoli.ruo_id";
      DataSet ds = cDBManager.CaricaDataset(query);
      var lista = new List<Utente>();
      if (ds != null && ds.Tables.Count > 0)
      {
        foreach (DataRow row in ds.Tables[0].Rows)
          lista.Add(new Utente(row));
      }
      return lista;
    }

	 public static List<UtentexCartella> GetUtentiXCliente(string idCliente)
	 {
			List<UtentexCartella> lista = null;
			try
			{
				string query = $"select utenti.*,UXC_REV_ID, ruo_descr from utenti inner join UTENTIXCLIENTE on UXC_UTE_ID = UTE_ID inner join ruoli on ute_ruo_id = ruo_id where UXC_CLI_ID = {idCliente}";
				query += " union ";
				query += $"	select utenti.*, -1, ruo_descr from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ruo_id = {(int)App.RuoloDesc.Reviewer} and ute_id in (select UXC_REV_ID from utentixcliente where uxc_cli_id =  {idCliente} )";
				DataSet ds = cDBManager.CaricaDataset(query);
				lista = new List<UtentexCartella>();
				if (ds != null && ds.Tables.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
						lista.Add(new UtentexCartella(row));
				}
			}
			catch(Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return lista;
		}

		/// <summary>
		/// inserisce o aggiorna le informazioni per l'utente passato nell'argomento 
		/// sono gestite anche le associazioni con i clienti	 e le cartelle
		/// metodo utilizzato dal team leader per la gestione delle anagrafiche utenti
		/// </summary>
		/// <param name="utente">informazioni utente</param>
		/// <param name="listaClienti">lista clienti da associare all'utente se ruolo = revisore autonomo</param>
		/// /// <param name="listaDisassociati">lista clienti per i quali deve essere cancellata l'associazione con l'utente se ruolo = revisore autonomo</param>
		public static void UpsertUtente(Utente utente, string listaClienti, string listaDisassociati)
		{
			try
			{

				utente.EncryptPSW();
				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("UserId", utente.Id.ToString());
				param.Add("RoleId", utente.RuoId.ToString());
				param.Add("UserLogin", utente.Login);
				param.Add("UserPsw", utente.Psw);
				param.Add("UserName", utente.Nome);
				param.Add("UserSurname", utente.Cognome);
				param.Add("UserDescr", utente.Descr);
				param.Add("ClientiList", listaClienti);
				param.Add("ClientiListDisassociati", listaDisassociati);
				param.Add("IdRuoloRevAutonomo", ((int)App.RuoloDesc.RevisoreAutonomo).ToString());
				param.Add("IdRuoloTeamLeader", ((int)App.RuoloDesc.TeamLeader).ToString());
				param.Add("UserLogged", App.AppUtente.Login);

				cDBManager.CaricaDatasetDaStoredProcedure("SP_UpsertUser", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

		}

		public static void UpdateUtente(Utente utente)
		{
			try
			{
				utente.EncryptPSW();
				string query = $"update UTENTI set UTE_LOGIN = '{utente.Login}', UTE_PSW = '{utente.Psw}', UTE_NOME = '{utente.Nome}',UTE_COGNOME = '{utente.Cognome}',UTE_DESCR = '{utente.Descr}' where UTE_ID = {utente.Id} ";
				cDBManager.EseguiComando(query);
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
		}

		/// <summary>
		/// verifica se le credenziali dell'utente sono corrette, se lo sono restituisce tutte le 
		/// informazioni per l'utente
		/// </summary>
		/// <param name="ute">informazioni utente</param>
		/// <returns></returns>
		public static bool EseguiLogIn(ref Utente ute)
		{
			//string psw = 
			ute.EncryptPSW();
			string query = "SELECT UTENTI.*, RUO_DESCR FROM UTENTI INNER JOIN RUOLI ON UTE_RUO_ID = RUO_ID ";
			query += string.Format("WHERE UTE_LOGIN = '{0}' AND UTE_PSW = '{1}'", ute.Login,ute.Psw);
			DataSet ds = cDBManager.CaricaDataset(query);
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				ute.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["UTE_ID"]);
				ute.Nome = Convert.ToString(ds.Tables[0].Rows[0]["UTE_NOME"]);
				ute.Cognome = Convert.ToString(ds.Tables[0].Rows[0]["UTE_COGNOME"]);
				ute.RuoId = Convert.ToInt16(ds.Tables[0].Rows[0]["UTE_RUO_ID"]);
				ute.RuoDescr = Convert.ToString(ds.Tables[0].Rows[0]["RUO_DESCR"]);
				ute.Descr = Convert.ToString(ds.Tables[0].Rows[0]["UTE_DESCR"]);
				ute.UtePadre = Convert.ToInt32(ds.Tables[0].Rows[0]["UTE_UTE_ID"]);

				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// seleziona tutti gli utenti in ruolo team leader
		/// </summary>
		/// <returns></returns>
		public static Dictionary<int, Utente> GetUtentiTeamLeader()
		{				
			Dictionary<int, Utente> utenti = null;

			string queryUtenti = "SELECT UTE_ID, UTE_LOGIN + ' - ' + UTE_NOME + ' ' + ISNULL(UTE_COGNOME,'') AS UTE_LOGIN, UTE_NOME, UTE_COGNOME, UTE_DESCR, ";
			queryUtenti += string.Format("UTE_PSW, UTE_RUO_ID , RUO_DESCR, UTE_UTE_ID FROM UTENTI INNER JOIN RUOLI ON UTE_RUO_ID = RUO_ID WHERE UTE_RUO_ID = {0} ORDER BY UTE_LOGIN", (int)App.RuoloDesc.TeamLeader);

			DataSet dsUtenti = cDBManager.CaricaDataset(queryUtenti);
			if (dsUtenti != null && dsUtenti.Tables != null && dsUtenti.Tables[0].Rows != null && dsUtenti.Tables[0].Rows.Count > 0)
			{
				utenti = new Dictionary<int, Utente>(dsUtenti.Tables[0].Rows.Count);
				for (int i = 0; i < dsUtenti.Tables[0].Rows.Count; i++)
				{
					DataRow row = dsUtenti.Tables[0].Rows[i];
					utenti[i] = new Utente(row);
				}
			}

			return utenti;

		}

		/// <summary>
		/// restituisce l'utente associato allo user id passato nell'argomento
		/// </summary>
		/// <param name="login">UTE_LOGIN dell'utente</param>
		/// <returns></returns>
		public static int GetUtente(string login)
		{
			string query = string.Format("select * from utenti where ute_login = '{0}'", login);
			object ret = cDBManager.EseguiComando(query);
			if (ret != null)
				return (int)ret;
			else
				return -1;
		}

		/// <summary>
		/// elimina l'utente dal sistema, se all'utente o a suoi sottoposti sono associate cartelle
		/// di lavoro elimina l'associazione
		/// </summary>
		/// <param name="ute">informazioni utente</param>
		public static void EliminaUtente(Utente ute)
		{
			try
			{

				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("UserId", ute.Id.ToString());

				cDBManager.CaricaDatasetDaStoredProcedure("SP_DeleteUtente", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

		}

		/// <summary>
		/// restituisce true se all'utente o suoi sottoposti risultano associate delle cartelle di lavoro, false altrimenti
		/// </summary>
		/// <param name="ute">informazioni utente</param>
		/// <returns></returns>
		public static bool EsistonoCartelleAssociate(Utente ute)
		{
			string query = "select cxc_uxc_id from cartellexcliente where cxc_uxc_id in (select uxc_id from utentixcliente where uxc_ute_id in ";
			query += string.Format("(select ute_id from utenti where ute_ute_id = {0} union ", ute.Id);
			query += string.Format("select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = {0}) union select ute_id from utenti where ute_id = {0}))", ute.Id);

			DataSet ds = cDBManager.CaricaDataset(query);
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Dato l'id dell'utente team leader restituisce l'elenco degli utenti associati nel team
		/// </summary>
		/// <param name="teamLeaderID">UTE_ID del team leader</param>
		/// <returns>dictionary con chiave progressivo e valore oggetto utenteGriglia</returns>
		public static Dictionary<int,UtenteGriglia> GetTeamFromLeader(int teamLeaderID)
		{			   
			string query = string.Format("select utenti.*, RUO_DESCR from utenti inner join RUOLI on UTE_RUO_ID = RUO_ID where ute_ute_id = {0}", teamLeaderID);
			query += " union ";
			query += string.Format("select utenti.*, RUO_DESCR from utenti inner join RUOLI on UTE_RUO_ID = RUO_ID where ute_ute_id in (select ute_id from utenti where ute_ute_id = {0})", teamLeaderID);
			Dictionary<int, UtenteGriglia> utenti = new Dictionary<int, UtenteGriglia>();
			try
			{
				DataSet dsTeam = cDBManager.CaricaDataset(query);
				if (dsTeam != null && dsTeam.Tables.Count > 0 && dsTeam.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dsTeam.Tables[0].Rows)
						utenti.Add(Convert.ToInt32(row["UTE_ID"]), new UtenteGriglia(row,true,false));
				}
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

			return utenti;
		}

		public static List<int> GetFigli(Utente padre)
		{
			List<int> listaId = null;
			try
			{
				string query = string.Format("select ute_id from utenti where ute_ute_id = {0}", padre.Id);
				query += string.Format(" union select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = {0})", padre.Id);

				DataSet ds = cDBManager.CaricaDataset(query);
				if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
				{
					listaId = new List<int>();
					foreach (DataRow row in ds.Tables[0].Rows)
						listaId.Add(Convert.ToInt32(row["UTE_ID"]));
				}
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

			return listaId;
		}

		/// <summary>
		/// se non presente alcun legame per l'utente team leader leaderId crea i legami impostando
		/// padre = leaderId della lista utenti listUtentiId 
		/// se l legame è già presente lo aggiorna: 
		///  --lascia invariata la situazione per gli utenti presenti nel legame esistene e nella lista listUtentiId
		///  --inserisce il legame impostando padre = leaderId per i nuovi utenti presenti in listUtentiId
		///  --elimina il egame per gli utenti presenti nel legame precedente ma non più presenti nella lista listUtentiId
		///    prima di sciogliere il legame elimina tutte le eventuali assocazioni con cartelle di lavoro presenti fra l'utente ed eventuali sottoposti
		/// </summary>
		/// <param name="leaderId">UTE_ID dell'utente team leader</param>
		/// <param name="listUtentiId">lista degli UTE_ID, separata da virgola da gestire per il team </param>
		public static void UpsertTeam(int leaderId, string listUtentiId)
		{
			try
			{
				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("TeamLeaderId", leaderId.ToString());
				param.Add("UtentiListId", listUtentiId);

				cDBManager.CaricaDatasetDaStoredProcedure("SP_UpsertTeamAdministrator", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
		}

		/// <summary>
		/// crea l'associazione tra il team leader e il cliente
		/// se fosse presente una precedente associazione la elimina disassociando tutte le eventuali schede associate
		/// </summary>
		/// <param name="leaderId">id utente team leader</param>
		/// <param name="clienteId">id cliente</param>
		public static void UpsertClientiPerUtente(int leaderId, string listaClientiId, string listaClientiDisassociati)
		{
			//SP_UpsertClientexUtente
			try
			{
				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("TeamLeaderID", leaderId.ToString());
				param.Add("ClientiList", listaClientiId);
				param.Add("ClientiListDisassociati", listaClientiDisassociati);
				param.Add("RuoloEsecutore", ((int)App.RuoloDesc.Esecutore).ToString());

				cDBManager.CaricaDatasetDaStoredProcedure("SP_UpsertClientexUtente", param);
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
		}

		public static void AssociaRuoliUtenti(string listRevisori,string listEsecutori, string listNessuno, int idLeader)
		{
			try
			{
				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("TeamLeaderId", idLeader.ToString());
				param.Add("UtentiRevisoriList", listRevisori);
				param.Add("UtentiEsecutoriList", listEsecutori);
				param.Add("UtentiNonAssegnatiList", listNessuno);
				param.Add("RuoloID_Revisore", ((int)App.RuoloDesc.Reviewer).ToString());
				param.Add("RuoloID_Esecutore", ((int)App.RuoloDesc.Esecutore).ToString());
				param.Add("RuoloID_Nessuno", ((int)App.RuoloDesc.NessunRuolo).ToString());

				cDBManager.CaricaDatasetDaStoredProcedure("SP_AssociaRuoli_Utenti", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
		}

		public static List<Utente> GetRevisoriPerCliente(int teamLeaderID,int clienteId)
		{
			List<Utente> list = null;

			// elenco di tutti i revisori di teamleader
			//string query = string.Format("select * from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ute_id = {0}",teamLeaderID);
			//DataSet dsTutti = cDBManager.CaricaDataset(query);

			//elenco dei revisori associati al cliente
			//string queryAssociati = $"select * from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_id in (select distinct(ute_ute_id) from utenti left join UTENTIXCLIENTE on ute_id = UXC_UTE_ID where UXC_CLI_ID = {clienteId} ";
			//queryAssociati += $"and ute_id in (select ute_id from utenti where ute_ute_id in (select ute_id from utenti where ute_ute_id = {teamLeaderID} and ute_ruo_id = {(int)App.RuoloDesc.Reviewer})))";
			string queryAssociati = $"select * from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ute_id = {teamLeaderID} and ute_ruo_id =  {(int)App.RuoloDesc.Reviewer}";
			DataSet dsAssociati = cDBManager.CaricaDataset(queryAssociati); 
			
			if (dsAssociati != null && dsAssociati.Tables != null && dsAssociati.Tables.Count > 0)
			{
				list = new List<Utente>();
				foreach (DataRow row in dsAssociati.Tables[0].Rows)
				{
					list.Add(new Utente(row));
				}
			}

			return list;
		}

		public static List<UtenteGriglia> GetEsecutoriSelezionePerClienteRevisori(int teamLaderID, int clienteId, int revisoreID)
		{
			List<UtenteGriglia> list = null;
			bool assegnatoAlRevisore = false;
			
			int ese_id;
			DataRow rowEsecutore;

			// tutte le query si riferiscono al cliente selezionato
			//esecutori associati agli altri revisori
			string queryEsecutoriAltriRevisori = $"select UXC_UTE_ID from UTENTIXCLIENTE where uxc_rev_id <> -1 and uxc_cli_id = {clienteId} and UXC_REV_ID <> {revisoreID}";
			DataSet dsEsexAltriRev = cDBManager.CaricaDataset(queryEsecutoriAltriRevisori);

			// esecutori associati al cliente e al revisore selezionati 
			//string queryEsecutoriassegnatiClienti = $"select ute_id from UTENTIXCLIENTE left join utenti on UXC_UTE_ID = ute_id and ute_ruo_id = {(int)App.RuoloDesc.Esecutore} ";
			//queryEsecutoriassegnatiClienti += $" inner join ruoli on ute_ruo_id = ruo_id where uxc_rev_id = {revisoreID} and UXC_CLI_ID = {clienteId}";
			string queryEsecutoriAssegnatiRevisore = $"select UXC_UTE_ID from UTENTIXCLIENTE where uxc_rev_id = {revisoreID} and UXC_CLI_ID = {clienteId}";
			DataSet dsEsexRev = cDBManager.CaricaDataset(queryEsecutoriAssegnatiRevisore);
			
			// tutti gli esecutori del team
			string queryEsecutoriTuttiPerRevisori = $"select * from utenti inner join ruoli on ute_ruo_id = ruo_id where ute_ute_id = {teamLaderID} and ute_ruo_id = {(int)App.RuoloDesc.Esecutore}";
			DataSet dsEseTuttixRev = cDBManager.CaricaDataset(queryEsecutoriTuttiPerRevisori);
			if (dsEseTuttixRev != null && dsEseTuttixRev.Tables != null && dsEseTuttixRev.Tables.Count > 0)
			{
				list = new List<UtenteGriglia>();
				// si scorrono tutti gli esecutori del team
				foreach (DataRow row in dsEseTuttixRev.Tables[0].Rows)
				{
					ese_id = Convert.ToInt32(row["UTE_ID"]);

					// se l'esecutore è assegnato ad un altro revisore non deve essere visualizzato nella lista 	
					if (dsEsexAltriRev != null && dsEsexAltriRev.Tables != null && dsEsexAltriRev.Tables.Count > 0 && dsEsexAltriRev.Tables[0].Rows.Count > 0)
					{							
						rowEsecutore = dsEsexAltriRev.Tables[0].Select($"UXC_UTE_ID = {ese_id}").FirstOrDefault();
						if (rowEsecutore != null)
							continue;
					}

					assegnatoAlRevisore = false;
					if (dsEsexRev != null && dsEsexRev.Tables != null && dsEsexRev.Tables.Count > 0 && dsEsexRev.Tables[0].Rows.Count > 0)
					{
						rowEsecutore = dsEsexRev.Tables[0].Select($"UXC_UTE_ID = {ese_id}").FirstOrDefault();
						if (rowEsecutore != null)
							assegnatoAlRevisore = true;
					}

					list.Add(new UtenteGriglia(row, assegnatoAlRevisore, true));
				}
			}

			return list;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="listEsecutoriAssociati"></param>
		/// <param name="listaEsecuoriNonAssociati"></param>
		/// <param name="cliId"></param>
		/// <param name="teamLaderId"></param>
		/// <param name="revisoreId"></param>
		public static void AssociaUtentiCliente(string listEsecutoriAssociati, string listaEsecuoriNonAssociati, int cliId, int teamLaderId, int revisoreId)
		{
			try
			{
				Dictionary<string, string> param = new Dictionary<string, string>();
				param.Add("EsecutoriAssociati", listEsecutoriAssociati);
				param.Add("EsecutoriNonAssociati", listaEsecuoriNonAssociati);
				param.Add("ClienteId", cliId.ToString());
				param.Add("RevisoreId", revisoreId.ToString());
				param.Add("TeamLeaderId", teamLaderId.ToString());

				cDBManager.CaricaDatasetDaStoredProcedure("SP_AssociaUtenti_Cliente", param);

			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}

		}

		/// <summary>
		/// restituisce true se è presente l'associazione cliente team e se almeno un utente del team è esecutore
		/// </summary>
		/// <returns></returns>
		public static bool EsistAssociazioneEsecutoriEClienti()
		{
			bool abilita = false;
			try
			{
				List<UtenteGriglia> list =  GetUtentiGrigliaTeam(App.AppUtente.Id);
				foreach(UtenteGriglia ute in list)
				{
					if (ute.RuoId == (int)App.RuoloDesc.Esecutore)
					{
						abilita = true;
						break;
					}
				}

				string query = $"select * from UTENTIXCLIENTE where uxc_ute_id = {App.AppUtente.Id}";
				DataSet da = cDBManager.CaricaDataset(query);
				if (da != null && da.Tables.Count > 0 && da.Tables[0].Rows.Count > 0)
					abilita = abilita & true;
				else
					abilita = false;
			}
			catch (Exception ec)
			{
				App.GestioneLog(ec.Message);
			}
			return abilita;
		}

		/// <summary>
		/// restiruisce l'id utente del team leader associato al cliente
		/// </summary>
		/// <param name="clienteId">id del cliente</param>
		/// <returns>UTE_ID dell'utente team leader</returns>
		public static int GetLaderIdAssociatoAlCliente(string idCliente)
		{
			int ute_id = -1;
			try
			{
				string query = $"select uxc_ute_id from UTENTIXCLIENTE inner join utenti on uxc_ute_id = ute_id and UTE_RUO_ID = {(int)App.RuoloDesc.TeamLeader} where UXC_CLI_ID = '{idCliente}'";
				DataSet ds = cDBManager.CaricaDataset(query);
				if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
				{
					ute_id = Convert.ToInt32(ds.Tables[0].Rows[0]["uxc_ute_id"]);
				}
			}
			catch (Exception ex)
			{
				App.GestioneLog(ex.Message);
			}
			return ute_id;
		}
	}			

		
}
