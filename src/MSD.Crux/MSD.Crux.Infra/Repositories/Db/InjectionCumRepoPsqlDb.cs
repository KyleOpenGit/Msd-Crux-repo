using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

public class InjectionCumRepoPsqlDb : IInjectionCumRepo
{
    private readonly IDbConnection _dbConnection;

    public InjectionCumRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<int> AddInjectionCumAsync(InjectionCum injectionCum)
    {
        const string query = @"INSERT INTO public.injection_cum (line_id, time, lot_id, shift, employee_number, total)
                    VALUES (@LineId, @Time, @LotId, @Shift, @EmployeeNumber, @Total);";
        return await _dbConnection.ExecuteAsync(query, injectionCum);
    }
}
