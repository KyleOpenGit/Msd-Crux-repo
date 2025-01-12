using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

public class LotRepoPsqlDb : ILotRepo
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// (생성자). Service Provider를 통해 DIC에 등록된  IDbConnection 객체를 주입해준다
    /// </summary>
    /// <param name="dbConnection">DI가 주입해주는 Npgsql로 만든 IDbConnection 객체 </param>
    public LotRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        const string query = "SELECT * FROM lot WHERE injection_end IS NOT NULL;";
        var result = await _dbConnection.QueryAsync<Lot?>(query);
        return result.ToList();
    }
}
