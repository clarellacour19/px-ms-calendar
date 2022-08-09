using System;
using System.Collections.Generic;
using System.Text;

namespace PG.ABBs.CalendarOrganizer.Core.DTO
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } 
        public object Data { get; set; }
    }
}
