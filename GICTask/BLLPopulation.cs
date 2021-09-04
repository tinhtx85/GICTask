using GICTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GICTask
{
    public class BLLPopulation
    {
        private readonly PopulationContext _context;
        public BLLPopulation(PopulationContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Innit DB From Excel File.
        /// </summary>
        /// <param name="lstActual">List Actual </param>
        /// <param name="lstEstimate"> List Estimate </param>
        /// <returns></returns>
        public async Task<string> InitPopulations(List<Actual> lstActual, List<Estimate> lstEstimate)
        {
            try
            {
                if (_context.tblActuals.Any())
                {
                    foreach (var entity in _context.tblActuals)
                        _context.tblActuals.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                if (_context.tblEstimates.Any())
                {
                    foreach (var entity in _context.tblEstimates)
                        _context.tblEstimates.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                _context.tblActuals.AddRange(lstActual);
                _context.tblEstimates.AddRange(lstEstimate);
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                return "DB Error:" + ex.ToString();
            }
        }

        /// <summary>
        /// Get Population From DB.
        /// </summary>
        /// <param name="stateId"> state ID</param>
        /// <returns>Population item </returns>
        public Population GetPopulation(int stateId)
        {
            var itemActual =  _context.tblActuals.FirstOrDefault(s => s.State == stateId);
            if (itemActual == null)
            {
                //return a sum of the value over all districts in the Estimates table
                double sumDistricts =  _context.tblEstimates.Where(s => s.State == stateId).Sum(s => s.EstimatesPopulation);
                Population itemPopulation = new Population
                {
                    State = stateId,
                    Populations = sumDistricts
                };
                return itemPopulation;
            }
            else
            {
                Population itemPopulation = new Population
                {
                    State = stateId,
                    Populations = itemActual.ActualPopulation
                };
                return itemPopulation;
            }
        }
    }
}
