using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PG.ABBs.Webservices.DiaperSizerService.Context;
using PG.ABBs.Webservices.DiaperSizerService.Models;

namespace PG.ABBs.Webservices.DiaperSizerService.Repositories
{
    public class DiaperSizerRepository : IDiaperSizerRepository
    {
        private readonly DataContext _context;

        public DiaperSizerRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<List<DiaperFitFinder>> FindAllDiaperFitByLocale(string locale)
        {
            var diaperFitList = await _context.DiaperFitFinders
                .Include(diaperFit => diaperFit.MarketInfo)
                .Where(element => element.MarketInfo.Locale.ToLower() == locale.ToLower())
                .OrderBy(element => element.BabyWeightValue)
                .ToListAsync();
            return diaperFitList;
        }

        public async Task<DiaperFitFinder> GetDiaperFitFinderByLocaleAndValue(string locale, int value)
        {
            DiaperFitFinder diaperFitFinder = await _context.DiaperFitFinders
                .Include(diaperFit => diaperFit.MarketInfo)
                .Where(element => element.MarketInfo.Locale == locale.ToLower() && element.BabyWeightValue == value)
                .OrderBy(element => element.BabyWeightValue)
                .FirstOrDefaultAsync();
            return diaperFitFinder;
        }

        public async Task<List<DiaperSize>> GetDiaperSizeByDiaperFitFinder(DiaperFitFinder diaperFitFinder)
        {
            List<DiaperSize> result = new List<DiaperSize>();
            if (diaperFitFinder != null)
            {
                List<DiaperSizeAssociation> association = await _context.DiaperSizeAssociations
                    .Where(element => element.DiaperFitFinderID == diaperFitFinder.DiaperFitFinderID)
                    .Include(diaperSize => diaperSize.DiaperSize)
                    .ToListAsync();
                if(association != null && association.Count != 0)
                {
                    foreach(DiaperSizeAssociation dsa in association)
                    {
                        result.Add(dsa.DiaperSize);
                    }
                }
            }
            if (result != null) 
                result = result.OrderBy(element => element.Size).ToList();
            return result;
        }
    }
}
