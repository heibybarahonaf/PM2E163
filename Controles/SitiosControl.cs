using SQLite;

namespace PM2E163.Controles
{
    public class SitiosControl
    {
        private SQLiteAsyncConnection conec;
    
    public SitiosControl()
    {

    }

    public async Task Init()
    {
        if (conec is not null) 
        {
            return;
        }
        SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite |
                                             SQLite.SQLiteOpenFlags.Create |
                                             SQLite.SQLiteOpenFlags.SharedCache; 


        conec = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBSitios.db3"), extensiones); 
        var creacion = await conec.CreateTableAsync<ModeloSQL.Sitios>();
    }

    //CREAR
    public async Task<int> StoreSitio(ModeloSQL.Sitios Sitios)
    {
        await Init();
        if (Sitios.id == 0)
        {
            return await conec.InsertAsync(Sitios);
        }
        else
        {
            return await conec.UpdateAsync(Sitios);
        }
    }

    public async Task<List<ModeloSQL.Sitios>> GetListSitios()
    {
        await Init();
        return await conec.Table<ModeloSQL.Sitios>().ToListAsync();
    }

    public async Task<ModeloSQL.Sitios> GetSitio(int pid)
    {
        await Init();
        return await conec.Table<ModeloSQL.Sitios>().Where(i => i.id == pid).FirstOrDefaultAsync();
    }

    public async Task<int> DeleteSitio(ModeloSQL.Sitios Sitios)
    {
        await Init();
        return await conec.DeleteAsync(Sitios);
    }
    }
}
