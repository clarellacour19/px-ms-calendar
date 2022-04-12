
using PG.ABBs.Webservices.DiaperSizerService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PG.ABBs.Webservices.DiaperSizerService.Repositories
{
    public interface IDiaperSizerRepository
    {
        Task<List<DiaperFitFinder>> FindAllDiaperFitByLocale(string locale);

        Task<DiaperFitFinder> GetDiaperFitFinderByLocaleAndValue(string locale, int value);

        Task<List<DiaperSize>> GetDiaperSizeByDiaperFitFinder(DiaperFitFinder diaperFitFinder);
    }
}
