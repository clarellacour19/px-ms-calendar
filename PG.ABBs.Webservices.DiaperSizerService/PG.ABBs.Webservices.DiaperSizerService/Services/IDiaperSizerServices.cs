using PG.ABBs.Webservices.DiaperSizerService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PG.ABBs.Webservices.DiaperSizerService.Services
{
    public interface IDiaperSizerServices
    {
        Task<DiaperSizerResponseDto> ListBabyWeight(ListBabyWeightRequest parameters);

        Task<DiaperSizerResponseDto> GetBabyWeightDetails(BabyWeightDetailsRequest parameters);
    }
}
