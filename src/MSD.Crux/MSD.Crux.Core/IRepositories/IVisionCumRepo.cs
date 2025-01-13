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
    public interface IVisionCumRepo
    {
        /// <summary>
        /// 새로운 품질검사 결과 추가
        /// </summary>
        /// <param name="visionCum">추가할 품질 검사 결과 정보를 가진 visionCum 객체</param>
        /// <returns>비동기 작업 완료</returns>
        Task<int> AddVisionCumAsync(VisionCum visionCum);
    }
}
