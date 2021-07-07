using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessTheNationality.Service.Interface
{
    /// <summary>
    /// Interface for score calculations
    /// </summary>
    public interface IScoreCalculator
    {
        int CalculateScore(string tagValue, string pointName, int score);
    }
}
