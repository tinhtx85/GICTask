using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GICTask.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PopulationController : ControllerBase
    {
        private readonly PopulationContext _context;

        private readonly ILogger<PopulationController> _logger;
        public PopulationController(ILogger<PopulationController> logger, PopulationContext context)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(string state)
        {
            List<Population> lstPopulation = new List<Population>();
            List<int> statesNumbers = new List<int>();
            _logger.LogInformation("API endpoint called - " + Request.Path.ToString() + Request.QueryString);
            //Parse string and check state id available
            string lstState = ParseListState(state, ref statesNumbers);
            
            if(lstState != "success")
            {
                return NotFound(lstState);
            }
            else
            {
                foreach(int stateId in statesNumbers)
                {
                    var itemActual = _context.tblActuals.FirstOrDefault(s => s.State == stateId);
                    if(itemActual == null)
                    {
                        //return a sum of the value over all districts in the Estimates table
                        double sumDistricts = _context.tblEstimates.Where(s => s.State == stateId).Sum(s => s.EstimatesPopulation);
                        Population itemPopulation = new Population
                        {
                            State = stateId,
                            Populations = sumDistricts
                        };
                        lstPopulation.Add(itemPopulation);
                    }
                    else
                    {
                        Population itemPopulation = new Population
                        {
                            State= stateId,
                            Populations=itemActual.ActualPopulation
                        };
                        lstPopulation.Add(itemPopulation);
                    }    
                }    
            }
            
            return Ok(lstPopulation);
        }

        [HttpGet("deleteAllActuals")]
        public ActionResult DeleteAllActuals()
        {
            foreach (var entity in _context.tblActuals)
                _context.tblActuals.Remove(entity);
            _context.SaveChanges();
            return Ok("Success");
        }
        
        private string ParseListState(string states, ref List<int> statesNumbers)
        {
            try
            {
            statesNumbers = states.Split(',').Select(Int32.Parse).ToList();
            foreach(int state in statesNumbers)
            {
                if ((state < 1) || (state > 51))
                    return "state id must from 1 to 51";
            }
            }
            catch
            {
                return "List state invalid";
            }
            return "success";
        }
    }
}
