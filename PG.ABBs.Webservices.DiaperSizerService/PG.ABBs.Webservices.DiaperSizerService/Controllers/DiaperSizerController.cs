using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PG.ABBs.Webservices.DiaperSizerService.Dto;
using PG.ABBs.Webservices.DiaperSizerService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PG.ABBs.Webservices.DiaperSizerService.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("service/[controller]")]
    [Produces("application/json")]
    [EnableCors("AllowSpecificOrigin")]
    [ApiController]
    public class DiaperSizerController : Controller
    {

        private readonly IDiaperSizerServices _diaperSizerServices;

        public DiaperSizerController(IDiaperSizerServices diaperSizerServices)
        {
            _diaperSizerServices = diaperSizerServices;
        }

        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        [Route("listBabyWeight")]
        public async Task<IActionResult> ListBabyWeight([FromBody] ListBabyWeightRequest parameters)
        {
            DiaperSizerResponseDto result = DiaperSizerResponseDto.GetOkResponse();
            try
            {
                result = await _diaperSizerServices.ListBabyWeight(parameters);
            } catch (Exception exc)
            {
                result = DiaperSizerResponseDto.GetUnexpectedErrorResponse();
                result.Exception = exc.Message;
            }
            return Json(result);
        }

        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        [Route("getBabyWeightDetails")]
        public async Task<IActionResult> GetBabyWeightDetails([FromBody] BabyWeightDetailsRequest parameters)
        {
            DiaperSizerResponseDto result = DiaperSizerResponseDto.GetOkResponse();
            try
            {
                result = await _diaperSizerServices.GetBabyWeightDetails(parameters);
            }
            catch (Exception exc)
            {
                result = DiaperSizerResponseDto.GetUnexpectedErrorResponse();
                result.Exception = exc.Message;
            }
            return Json(result);
        }
    }
}