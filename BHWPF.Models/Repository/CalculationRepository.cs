using BHWPF.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BHWPF.Data.Repository
{
    public class CalculationRepository : ICalculationRepository
    {

        public List<CalculationTypes> GetCalculationTypes()
        {
            List<CalculationTypes> types = ((CalculationTypesEnum[])Enum.GetValues(typeof(CalculationTypesEnum))).Select(c => new CalculationTypes() { Id = (int)c, CalculationTypeName = c.ToString() }).ToList();

            return types;
        }

        public enum CalculationTypesEnum : int
        {
            CubicMeter=1,
            CubicFeet=2,
            Barrels=3
        }
    }
}
