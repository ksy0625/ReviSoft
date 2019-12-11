using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RevisoftApplication.BRL
{

  public class NoteXRevisore
  {
    public int? NXR_ID { get; set; }
    public string NXR_NOTE { get; set; }
    public int? NXR_UTE_ID { get; set; }
    public string NXR_CLI_ID { get; set; }
    public string NXR_COD_ID { get; set; }

  }

  public class cNote
  {
    public static NoteXRevisore GetNote(int idRevisore, string idCliente, string codice)
    {
      var query = $"SELECT * FROM NOTEXREVISORE WHERE NXR_UTE_ID = {idRevisore} AND NXR_CLI_ID = '{idCliente}' AND NXR_COD_ID = '{codice}'";
      var dsNote = cDBManager.CaricaDataset(query);
      if (dsNote == null || dsNote.Tables.Count <= 0 || dsNote.Tables[0].Rows.Count <= 0 || dsNote.Tables[0].Rows[0] == null )
        return null;
      var row = dsNote.Tables[0].Rows[0];
      var ret = new NoteXRevisore();
      try { ret.NXR_ID = Convert.ToInt32(row["NXR_ID"]); } catch { }
      try { ret.NXR_UTE_ID = Convert.ToInt32(row["NXR_UTE_ID"]); } catch { }
      try { ret.NXR_NOTE = Convert.ToString(row["NXR_NOTE"]); } catch { }
      try { ret.NXR_CLI_ID = Convert.ToString(row["NXR_CLI_ID"]); } catch { }
      try { ret.NXR_COD_ID = Convert.ToString(row["NXR_COD_ID"]); } catch { }
      return ret;
    }

    public static bool UpsertNote(int idRevisore, string idCliente, string codice, string nota)
    {
      try
      {
        Dictionary<string, string> param = new Dictionary<string, string>
        {
          { "idRevisore", idRevisore.ToString() },
          { "idCliente", idCliente },
          { "codice", codice },
          { "nota", nota }
        };
        var dsNote = cDBManager.CaricaDatasetDaStoredProcedure("SP_UpsertNota", param);
        foreach (DataTable table in dsNote.Tables)
        {

          foreach (DataRow r in table.Rows)
          {
            var x = r;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        App.GestioneLog(ex.Message);
        return false;
      }
    }

  }
  

}
