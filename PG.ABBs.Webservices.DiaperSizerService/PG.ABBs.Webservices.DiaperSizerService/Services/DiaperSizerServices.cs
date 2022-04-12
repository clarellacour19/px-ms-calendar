using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PG.ABBs.Webservices.DiaperSizerService.Dto;
using PG.ABBs.Webservices.DiaperSizerService.Models;
using PG.ABBs.Webservices.DiaperSizerService.Repositories;

namespace PG.ABBs.Webservices.DiaperSizerService.Services
{
    public class DiaperSizerServices : IDiaperSizerServices
    {
        private readonly IDiaperSizerRepository _diaperSizerRepository;
        public DiaperSizerServices(IDiaperSizerRepository diaperSizerRepository)
        {
            _diaperSizerRepository = diaperSizerRepository;
        }

        public async Task<DiaperSizerResponseDto> GetBabyWeightDetails(BabyWeightDetailsRequest parameters)
        {
            DiaperSizerResponseDto result = DiaperSizerResponseDto.GetOkResponse();
            DiaperFitFinder diaperFit = await _diaperSizerRepository.GetDiaperFitFinderByLocaleAndValue(parameters.Locale, parameters.Value);
            List<DiaperSize> diaperSizes = await _diaperSizerRepository.GetDiaperSizeByDiaperFitFinder(diaperFit);
            if(diaperSizes != null && diaperSizes.Count != 0)
            {
                BabyWeightDetails detail = new BabyWeightDetails
                {
                    WeightRange = diaperFit.WeightRange,
                    DiapersPerDay = diaperFit.AverageDiapersPerDay,
                    LastAroundMonths = diaperFit.LastAroundMonths,
                    DiaperSize = diaperSizes
                };
                result.ResultData = detail;
            }
            else
            {
                result = DiaperSizerResponseDto.GetNoResultsFoundResponse();
            }
            return result;
        }

        public async Task<DiaperSizerResponseDto> ListBabyWeight(ListBabyWeightRequest parameters)
        {
            DiaperSizerResponseDto result = DiaperSizerResponseDto.GetOkResponse();
            List<DiaperFitFinder> diaperFitFinders = await _diaperSizerRepository.FindAllDiaperFitByLocale(parameters.Locale);
            if(diaperFitFinders != null && diaperFitFinders.Count != 0)
            {
                List<DiaperSizerValue> diaperSizerValues = new List<DiaperSizerValue>();
                foreach(DiaperFitFinder diaperFit in diaperFitFinders)
                {
                    diaperSizerValues.Add(new DiaperSizerValue { Value = diaperFit.BabyWeightValue.ToString(), Description = diaperFit.BabyWeightDescription });
                }
                result.ResultData = diaperSizerValues;
            }
            else
            {
                result = DiaperSizerResponseDto.GetNoResultsFoundResponse();
            }
            return result;
        }
    }
}