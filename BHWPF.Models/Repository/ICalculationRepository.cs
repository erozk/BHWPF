using System.Collections.Generic;
using BHWPF.Data.Models;

namespace BHWPF.Data.Repository
{
    public interface ICalculationRepository
    {
        List<CalculationTypes> GetCalculationTypes();

    }
}