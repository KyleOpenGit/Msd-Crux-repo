// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories
{
    public interface ILotRepo
    {
        /// <summary>
        /// 생산이 끝난 Lot를 조회
        /// </summary>
        /// <returns>조회된 로트정보 또는 null</returns>
        Task<List<Lot?>> GetAllCompletedLotsAsync();

    }
}
