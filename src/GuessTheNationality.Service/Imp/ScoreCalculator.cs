using GuessTheNationality.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessTheNationality.Service.Imp
{
    /// <summary>
    /// Implementation of score calculator in order to return the score
    /// </summary>
    public class ScoreCalculator : IScoreCalculator
    {
        /// <summary>
        /// return score after adding or subtracting in it
        /// </summary>
        /// <param name="tagValue"></param>
        /// <param name="pointName"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public int CalculateScore(string tagValue, string pointName, int score)
        {
            if (tagValue.Contains(pointName))
                score += 20;
            else
                score -= 5;
            return score;
        }
    }
}
